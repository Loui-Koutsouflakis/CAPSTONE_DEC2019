Shader "Capstone2019/ToonV3.1"
{

	Properties
	{
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(0, 1)) = .1

		//==
		_Ramp("Toon Ramp", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
		_RotationSpeed("Rot Speed", float) = 0
		//==

		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)

		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32

		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716

		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
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
	uniform float4 _Color;

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
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG

		Pass
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

			//#include "UnityCG.cginc"
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
			float _RotationSpeed;

			struct appdataT
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float4 uvRamp : TEXCOORD1;
				float3 normal : NORMAL;
			};

			struct v2fT
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvRamp : TEXCOORD3;
				float3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD1;
				float4 diff : COLOR0;
				SHADOW_COORDS(2)
			};

			v2fT vertT(appdataT v)
			{
				v2fT o;

				o.pos = UnityObjectToClipPos(v.vertex);
				float3 normal = UnityObjectToWorldNormal(v.normal);

				float NDotL = dot(normal, _WorldSpaceLightPos0.xyz);
				o.diff = NDotL * _LightColor0;

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvRamp = TRANSFORM_TEX(v.uvRamp, _Ramp);

				o.worldNormal = normal;
				o.viewDir = WorldSpaceViewDir(v.vertex);

				TRANSFER_SHADOW(o)

				/*float sinX = sin(-NDotL + o.uvRamp.x);
				float cosX = cos(-NDotL + o.uvRamp.y);
				float sinY = sin(-NDotL + o.uvRamp.x);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
				o.uvRamp.xy = mul(v.uvRamp, rotationMatrix);*/

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

				return tex2D(_MainTex, i.uv) * tex2D(_Ramp, i.uvRamp) * (_AmbientColor + light + specular + rim)/* * i.diff*/;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

		//Pass
		//{
		//	Tags{ "LightMode" = "ShadowCaster" }
		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	#pragma multi_compile_shadowcaster

		//	#include "UnityCG.cginc"

		//		struct appdata
		//	{
		//		float4 vertex : POSITION;
		//		float2 uv : TEXCOORD0;
		//		float4 normal : NORMAL;
		//	};

		//	struct v2f
		//	{
		//		V2F_SHADOW_CASTER;
		//	};

		//	sampler2D _MainTex;
		//	float4 _MainTex_ST;

		//	v2f vert(appdata v)
		//	{
		//		v2f o;
		//		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
		//		return o;
		//	}

		//	fixed4 frag(v2f i) : SV_Target
		//	{
		//		SHADOW_CASTER_FRAGMENT(i)
		//	}
		//	ENDCG
		//}
	}
	Fallback "Diffuse"
}