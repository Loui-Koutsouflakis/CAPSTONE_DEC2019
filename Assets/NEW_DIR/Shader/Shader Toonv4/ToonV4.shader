Shader "Capstone2019/ToonV4"
{
	Properties
	{
		[Toggle] _UseOutline("Outline Properties", Int) = 0

		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(0, 10)) = .1
		//==
		[Toggle] _UseToon("Toon Properties", Int) = 0
		_Alpha("Alpha", Range(0, 1)) = 0.5
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
		[Toggle] _UseNormalMap("Normal Map Properties", Int) = 0

		_NormalMap("Normal Map Texture", 2D) = "bump" {}
		_NormalMapIntensity("Normal Map Intensity", Range(0,2)) = 1.5
		
		//==
		[Toggle] _UseEmission("Emission Properties", Int) = 0

		_EmissionMap("Emission Map", 2D) = "white" {}
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0,1)
		_EmissionIntensity("Emission Intensity", Range(0,1)) = 0

		//==
		[Toggle] _UseSnow("Snow Properties", Int) = 0

		_SnowText("Texture", 2D) = "white" {}
		_Factor("Amount of Snow", Range(0, 0.5)) = 0
		_ObjYScale("YScale", float) = 0

		//==
		[Toggle] _UseDissolve("Dissolve Properties", Int) = 0

		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
		_Strength("Strength", Range(0, 1)) = 0.0
		_DisOutline("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineStrength("Outline Strength", Range(0, 0.1)) = 0
	}

	CGINCLUDE
		#include "UnityCG.cginc"

		struct appdataOL
		{
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2fOL
		{
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};

		uniform float _OutlineWidth;
		uniform float4 _OutlineColor;

		sampler2D _MainTex;
		float4 _MainTex_ST;
		//==
		sampler2D _DissolveTexture;
		half _Strength;
		fixed4 _DisOutline;
		float _OutlineStrength;
		//==
		
	ENDCG

	SubShader 
	{
		Pass //Outline
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" }

			ZWrite Off
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2fOL vert(appdataOL v)
			{
				v.pos.xyz += _OutlineWidth * normalize(v.pos.xyz);

				v2fOL o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			half4 frag(v2fOL i) : COLOR
			{
				half dissolveValue = tex2D(_DissolveTexture, i.uv).b;
				clip(dissolveValue - _Strength);
				fixed4 c = _OutlineColor;
				fixed4 e = _DisOutline * step(dissolveValue - _Strength, _OutlineStrength);
				return c * e;
			}
			ENDCG
		}

		Pass //Toon1
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile _ VERTEXLIGHT_ON

			#pragma vertex vert
			#pragma fragment frag

			#define FORWARD_BASE_PASS
			#include "Toon.cginc"

			ENDCG
		}

		Pass //Toon2
		{
			Tags
			{
				"LightMode" = "ForwardAdd"
			}

			Blend One One
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile_fwdadd

			#pragma vertex vert
			#pragma fragment frag
			#include "Toon.cginc"

			ENDCG
		}

		Pass // Shadow Reciever
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				SHADOW_COORDS(0)
				float2 uv : TEXCOORD01;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				TRANSFER_SHADOW(o)
				return o;
			}

			float _Alpha;

			float4 frag(v2f i) : SV_Target
			{
				float shadow = SHADOW_ATTENUATION(i);

				half dissolveValue = tex2D(_DissolveTexture, i.uv).r;
				clip(dissolveValue - _Strength);

				fixed4 e = _DisOutline * step(dissolveValue - _Strength, _OutlineStrength);
				fixed4 s = float4(0, 0, 0, (1 - shadow) * _Alpha);

				return s + e;
			}
			ENDCG
		}

		Pass // Shadow Caster
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex verts
			#pragma fragment frags
			#pragma multi_compile_shadowcaster

			struct appdatas
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2fs
			{
				float2 uv : TEXCOORD0;
				V2F_SHADOW_CASTER;
			};

			v2fs verts(appdata_base /*appdatas*/ v)
			{
				v2fs o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.vertex, _MainTex);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frags(v2fs i) : SV_Target
			{
				half dissolveValue = tex2D(_DissolveTexture, i.uv).r;
				clip(dissolveValue - _Strength);

				fixed4 e = _DisOutline * step(dissolveValue - _Strength, _OutlineStrength);
				SHADOW_CASTER_FRAGMENT(i)
				return e;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ToonGUIv6"
}