// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"

float _NormalMapIntensity;
sampler2D _NormalMap;
float4 _NormalMap_ST;

sampler2D _EmissionMap;
float4 _EmissionMap_ST;

float4 _EmissionColor;
float _EmissionIntensity;

float _Glossiness;
float4 _SpecularColor;

float4 _RimColor;
float _RimAmount;

float _RimThreshold;

float4 _AmbientColor;

struct appdata
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
	float2 uvNormalMap: TEXCOORD2;
	float2 uvEmission: TEXCOORD3;
};

struct v2f
{
	float4 pos : SV_POSITION;
	float3 worldNormal : NORMAL;
	float3 viewDir : TEXCOORD0;
	float2 uv : TEXCOORD1;
	float2 uvNormalMap : TEXCOORD2;
	float2 uvEmission: TEXCOORD3;
	SHADOW_COORDS(4)

	float3 worldPos: TEXCOORD4;

#if defined(VERTEXLIGHT_ON)
	float3 vertexLightColor : TEXCOORD3;
#endif
};

void ComputeVertexLightColor(inout v2f i)
{
#if defined(VERTEXLIGHT_ON)
	i.vertexLightColor = Shade4PointLights(
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb,
		unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, i.worldPos, i.worldNormal
	);
#endif
}

UnityLight CreateLight(v2f i)
{
	UnityLight light;

#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
	light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
#else
	light.dir = _WorldSpaceLightPos0.xyz;
#endif

	UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
	light.color = _LightColor0.rgb * attenuation;
	light.ndotl = DotClamped(i.worldNormal, light.dir);
	return light;
}

UnityIndirect CreateIndirectLight(v2f i)
{
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

#if defined(VERTEXLIGHT_ON)
	indirectLight.diffuse = i.vertexLightColor;
#endif

#if defined(FORWARD_BASE_PASS)
	indirectLight.diffuse += max(0, ShadeSH9(float4(i.worldNormal, 1)));
#endif

	return indirectLight;
}


v2f vert(appdata v)
{
	v2f o;

	o.pos = UnityObjectToClipPos(v.vertex);
	o.worldPos = mul(unity_ObjectToWorld, v.vertex);

	o.worldNormal = UnityObjectToWorldNormal(v.normal);
	o.viewDir = WorldSpaceViewDir(v.vertex);

	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	o.uvNormalMap = TRANSFORM_TEX(v.uvNormalMap, _NormalMap);
	o.uvEmission = TRANSFORM_TEX(v.uvEmission, _EmissionMap);

	TRANSFER_SHADOW(o);

	ComputeVertexLightColor(o);
	return o;
}

float3 frag(v2f i) : SV_Target
{
	float3 normal = normalize(i.worldNormal); // World Normal Here
	float DirDot = dot(_WorldSpaceLightPos0, normal);

	float shadow = SHADOW_ATTENUATION(i);
	float lightIntensity = smoothstep(0, 0.01, DirDot * shadow);
	float4 light = lightIntensity * _LightColor0;

	float3 viewDir = normalize(i.viewDir);
	float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
	float DirDotH = dot(normal, halfVector);

	float specularIntensity = pow(DirDotH * lightIntensity, _Glossiness * _Glossiness);

	float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
	float4 specular = specularIntensitySmooth * _SpecularColor;

	float4 rimDot = 1 - dot(viewDir, normal);

	float rimIntensity = rimDot * pow(DirDot, _RimThreshold);
	rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);

	float4 rim = rimIntensity * _RimColor;

	float3 NMap = tex2D(_NormalMap, i.uvNormalMap).rgb;
	float NM = (NMap.r + NMap.g + NMap.b) / 3;
	NM *= _NormalMapIntensity;

	half dissolveValue = tex2D(_DissolveTexture, i.uv).r;
	clip(dissolveValue - _Strength);

	float4 c = tex2D(_MainTex, i.uv);
	fixed4 e = _DisOutline * step(dissolveValue - _Strength, _OutlineStrength);


	float3 specularTint;
	float oneMinusReflectivity = 0;

	//c.rgb = DiffuseAndSpecularFromMetallic(
	//	c.rgb, rim, specularTint, oneMinusReflectivity
	//);

	c = c * (_AmbientColor + specular + rim + light) + (tex2D(_EmissionMap, i.uvEmission) * _EmissionColor * _EmissionIntensity) * NM;

	return UNITY_BRDF_PBS(
		c, _AmbientColor,
		oneMinusReflectivity, _AmbientColor,
		i.worldNormal, viewDir,
		CreateLight(i), CreateIndirectLight(i)
	) + e;

	//return (c + e) * (_AmbientColor + light + specular + rim) /** NM + (tex2D(_EmissionMap, i.uvEmission) * _EmissionColor * _EmissionIntensity)*/;
}

#endif