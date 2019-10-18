Shader "Capstone2019/ToonV3.2"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(0, 1)) = .1

		_Ramp("Toon Ramp", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}

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

		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalMapIntensity("Normal Map Intensity", Range(0,2)) = 1.5
		
		//==

		_EmissionMap("Emission Map", 2D) = "white" {}
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0,1)
		_EmissionIntensity("Emission Color", Range(0,1)) = 0
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
	};

	struct v2f
	{
		float4 pos : POSITION;
	};

	uniform float _OutlineWidth;
	uniform float4 _OutlineColor;
	uniform sampler2D _MainTex;

	ENDCG

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" }

		Pass //Outline
		{
			ZWrite Off
			Cull Back
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v)
			{
				appdata original = v;
				v.vertex.xyz += _OutlineWidth * normalize(v.vertex.xyz);

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;

			}

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}

			ENDCG
		}

		Tags{ "Queue" = "Geometry"}

		CGPROGRAM
		#pragma surface surf Lambert

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_NormalMap;
		};

		sampler2D _NormalMap;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG

		Pass // Toon
		{
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

			float4 _MainTex_ST;

			sampler2D _Ramp;
			float4 _Ramp_ST;

			float _Glossiness;
			float4 _SpecularColor;

			float4 _RimColor;
			float _RimAmount;

			float _RimThreshold;

			sampler2D _NormalMap;
			float4 _NormalMap_ST;

			float _NormalMapIntensity;

			sampler2D _EmissionMap;
			float4 _EmissionMap_ST;

			float4 _EmissionColor;
			float _EmissionIntensity;

			struct appdataT
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float4 uvRamp : TEXCOORD1;
				float2 uvNormalMap: TEXCOORD2;
				float2 uvEmission: TEXCOORD3;
				float3 normal : NORMAL;
			};

			struct v2fT
			{
				float4 pos : SV_POSITION;
				float3 viewDir : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float2 uvRamp : TEXCOORD2;
				float2 uvNormalMap : TEXCOORD3;
				float2 uvEmission: TEXCOORD4;
				float3 worldNormal : NORMAL;
				SHADOW_COORDS(5)
			};

			v2fT vertT(appdataT v)
			{
				v2fT o;

				o.pos = UnityObjectToClipPos(v.vertex);
				float3 normal = UnityObjectToWorldNormal(v.normal);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvRamp = TRANSFORM_TEX(v.uvRamp, _Ramp);
				o.uvNormalMap = TRANSFORM_TEX(v.uvNormalMap, _NormalMap);
				o.uvEmission = TRANSFORM_TEX(v.uvEmission, _EmissionMap);

				o.worldNormal = normal;
				o.viewDir = WorldSpaceViewDir(v.vertex);

				TRANSFER_SHADOW(o)
				return o;
			}

			float4 _AmbientColor;

			float4 fragT(v2fT i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float shadow = SHADOW_ATTENUATION(i);

				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
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

				float4 NMap = tex2D(_NormalMap, i.uvNormalMap);
				float NM = (NMap.r + NMap.g + NMap.b) / 3;
				NM *= _NormalMapIntensity;

				return tex2D(_MainTex, i.uv) * tex2D(_Ramp, i.uvRamp) * (_AmbientColor + light + specular + rim) * NM + (tex2D(_EmissionMap, i.uvEmission) * _EmissionColor * _EmissionIntensity);
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}