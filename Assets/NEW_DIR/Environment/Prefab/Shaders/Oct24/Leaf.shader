Shader "Capstone2019/Leaf"
{
    Properties
    {
		[Toggle] _UseVege("Vege Properties", Int) = 0
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_GradThresh("Gradiant Threshold", Range(0,1)) = 0.5 //_TranslucentGain

		_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0

		_BladeForward("Blade Forward Amount", Range(0, 0.1)) = 0
		_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2

		_BladeHeight("Blade Height", Float) = 0.1
		_BladeHeightRandom("Random Height Variation", Range(0, 0.1)) = 0 // Random Variation

		_BladeWidth("Blade Width", Float) = 0
		_BladeWidthRandom("Random Width Variation", Range(0, 0.03)) = 0 // Random Variation

		_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		_WindDir("Wind Direction", Vector) = (0.05, 0.05, 0, 0)
		_WindStrength("Wind Strength", Range(0.01, 1)) = 1

		_TessellationUniform("Density", Range(1, 64)) = 1

		//==
		[Toggle] _UseToon("Toon Properties", Int) = 0

		_Color("Color", Color) = (1,1,1,1)

		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)

		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32

		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716

		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
		//==
    }

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Autolight.cginc"
	#include "Lighting.cginc"
	#include "CustomTessellation.cginc"

	#define BLADE_SEGMENTS 3 // Reduce to 2 and adjust other values

	float _BendRotationRandom;

	float _BladeHeight;
	float _BladeHeightRandom;

	float _BladeWidth;
	float _BladeWidthRandom;

	float _BladeForward;
	float _BladeCurve;

	sampler2D _WindDistortionMap;
	float4 _WindDistortionMap_ST;

	float2 _WindDir;
	float _WindStrength;

	float4 _Color;

	float rand(float3 co) // Randomizer
	{
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
	}

	float3x3 AngleAxis3x3(float angle, float3 axis) // Create Rotation Matrix
	{
		float c, s;
		sincos(angle, s, c);

		float t = 1 - c;
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;

		return float3x3(
			t * x * x + c, t * x * y - s * z, t * x * z + s * y,
			t * x * y + s * z, t * y * y + c, t * y * z - s * x,
			t * x * z - s * y, t * y * z + s * x, t * z * z + c
			);
	}

	struct v2g // v2f equivilant
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		//unityShadowCoord4 _ShadowCoord : TEXCOORD1;
		float3 normal : NORMAL;
	};

	v2g OutputData(float3 pos, float2 uv, float3 normal)
	{
		v2g o;
		o.pos = UnityObjectToClipPos(pos);
		o.uv = uv;
		//o._ShadowCoord = ComputeScreenPos(o.pos);
		o.normal = UnityObjectToWorldNormal(normal);

		//#if UNITY_PASS_SHADOWCASTER // Take this off to enable grass blades to imit shadows on themselves
		//			o.pos = UnityApplyLinearShadowBias(o.pos);
		//#endif
		return o;
	}

	v2g GenVertData(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix) //Manually create the AppData here (g2f)
	{
		float3 tangentPoint = float3(width, forward, height);

		float3 tangentNormal = normalize(float3(0, -1, forward));
		float3 localNormal = mul(transformMatrix, tangentNormal);

		float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
		return OutputData(localPosition, uv, localNormal);
	}

	[maxvertexcount(BLADE_SEGMENTS * 3 + 1)]
	void geo(triangle vertexOutput IN[3], inout TriangleStream<v2g> triStream)
	{
		float3 pos = IN[0].vertex;
		float3 vNormal = IN[0].normal;
		float4 vTangent = IN[0].tangent;
		float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;

		float3x3 tangentToLocal = float3x3(
			vTangent.x, vBinormal.x, vNormal.x,
			vTangent.y, vBinormal.y, vNormal.y,
			vTangent.z, vBinormal.z, vNormal.z
			);

		float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
		float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

		float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindDir * _Time.y; // Wind applied Here
		float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength; // Put safety net there blades are still rendered if no wind
		float3 wind = normalize(float3(windSample.x, windSample.y, 0));

		float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

		float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);

		float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);

		float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
		float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;

		float forward = rand(pos.yyz) * _BladeForward;

		for (int i = 0; i < BLADE_SEGMENTS; i++)
		{
			float t = i / (float)BLADE_SEGMENTS;
			float segmentHeight = height * t;

			float segmentWidth;
			if (i == 0) { segmentWidth = 0; }
			else if (i == 1 || i == 2) { segmentWidth = width + 0.025; }

			float segmentForward = pow(t, _BladeCurve) * forward;
			float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;

			triStream.Append(GenVertData(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
			triStream.Append(GenVertData(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));
		}

		triStream.Append(GenVertData(pos, 0, height, forward, float2(0.5, 1), transformationMatrix));
	}
	ENDCG
	
	SubShader
	{
		Cull Off

		Pass
		{
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragC
			#pragma target 4.6
			#pragma geometry geo
			#pragma hull hull
			#pragma domain domain
			#pragma multi_compile_fwdbase

			float4 _TopColor;
			float4 _BottomColor;
			float _GradThresh;

			float4 fragC(v2g i, fixed facing : VFACE) : SV_Target
			{
				float3 normal = facing > 0 ? i.normal : -i.normal;

				//float shadow = SHADOW_ATTENUATION(i);
				float NdotL = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _GradThresh) /** shadow*/;

				float3 ambient = ShadeSH9(float4(normal, 1));
				float4 lightIntensity = NdotL * _LightColor0 + float4(ambient, 1);
				float4 col = lerp(_BottomColor, _TopColor * lightIntensity, i.uv.y);
				return col;
			}
			ENDCG
		}

		Pass // Toon
		{
			//ZWrite Off
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			CGPROGRAM
			#pragma vertex vertT
			#pragma fragment fragT
			#pragma multi_compile_fwdbase

			#include "Lighting.cginc"
			#include "AutoLight.cginc"


			float _Glossiness;
			float4 _SpecularColor;

			float4 _RimColor;
			float _RimAmount;

			float _RimThreshold;

			struct appdataT
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2fT
			{
				float4 pos : SV_POSITION;
				float3 viewDir : TEXCOORD0;
				float3 worldNormal : NORMAL;
			};

			v2fT vertT(appdataT v)
			{
				v2fT o;

				o.pos = UnityObjectToClipPos(v.vertex);
				float3 normal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = normal;
				o.viewDir = WorldSpaceViewDir(v.vertex);

				return o;
			}

			float4 _AmbientColor;

			float4 fragT(v2fT i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float lightIntensity = smoothstep(0, 0.01, NdotL);
				float4 light = lightIntensity * _LightColor0;

				float3 viewDir = normalize(i.viewDir);
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);

				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;

				float4 rimDot = 1 - dot(viewDir, normal);

				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);

				float4 rim = rimIntensity * _RimColor;
				return _Color * (_AmbientColor + light + specular + rim);
			}
			ENDCG
		}
    }
    FallBack "Capstone2019/ToonV3.31" // Create Basic Texture Fallback (with Cell shader)
	CustomEditor "VegeGUI"
}
