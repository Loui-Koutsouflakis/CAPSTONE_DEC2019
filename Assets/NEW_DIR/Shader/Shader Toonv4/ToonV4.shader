Shader "Capstone2019/ToonV4"
{
	Properties
	{
		[Toggle] _UseOutline("Outline Properties", Int) = 0

		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(0, 10)) = .1
		//==
		[Toggle] _UseToon("Toon Properties", Int) = 0
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
				half dissolveValue = tex2D(_DissolveTexture, i.uv).r;
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

		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster

			#include "UnityCG.cginc"

				struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}

		Pass // Snow
		{
			Tags
			{
				"RenderType" = "Opaque"
				"Queue" = "Geometry+2"
			}
			Cull Off

			CGPROGRAM
			#pragma vertex vertS
			#pragma fragment fragS
			#pragma geometry geomS

			struct g2f // Appdata equivilant
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 col : COLOR;
			};

			struct v2g // v2f equivilant
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			sampler2D _SnowText;
			float4 _SnowText_ST;
			float _ObjYScale;

			v2g vertS(appdata_base v)
			{
				v2g o;
				o.vertex = v.vertex;
				o.uv = TRANSFORM_TEX(v.texcoord, _SnowText);
				o.normal = v.normal;
				return o;
			}

			float _Factor;

			[maxvertexcount(24)]
			void geomS(triangle v2g IN[3], inout TriangleStream<g2f> tristream)
			{
				if (_Factor > 0)
				{
					if (IN[0].vertex.y > _ObjYScale&& IN[1].vertex.y > _ObjYScale&& IN[2].vertex.y > _ObjYScale)
					{
						g2f o;
						for (int i = 0; i < 3; i++) // Side Trangles
						{
							o.pos = UnityObjectToClipPos(IN[i].vertex);
							o.uv = IN[i].uv;
							o.col = fixed4(0., 0., 0., 1.);
							tristream.Append(o);


							o.pos = UnityObjectToClipPos(IN[i].vertex + float4(0, 1, 0, 0) * _Factor);
							o.uv = IN[i].uv;
							o.col = fixed4(1., 1., 1., 1.);
							tristream.Append(o);

							int inext = (i + 1) % 3;

							o.pos = UnityObjectToClipPos(IN[inext].vertex);
							o.uv = IN[inext].uv;
							o.col = fixed4(0., 0., 0., 1.);
							tristream.Append(o);

							tristream.RestartStrip();

							o.pos = UnityObjectToClipPos(IN[i].vertex + float4(0, 1, 0, 0) * _Factor);
							o.uv = IN[i].uv;
							o.col = fixed4(1., 1., 1., 1.);
							tristream.Append(o);

							o.pos = UnityObjectToClipPos(IN[inext].vertex);
							o.uv = IN[inext].uv;
							o.col = fixed4(0., 0., 0., 1.);
							tristream.Append(o);

							o.pos = UnityObjectToClipPos(IN[inext].vertex + float4(0, 1, 0, 0) * _Factor);
							o.uv = IN[inext].uv;
							o.col = fixed4(1., 1., 1., 1.);
							tristream.Append(o);

							tristream.RestartStrip();
						}

						for (int i = 0; i < 3; i++) // Top Triangles
						{
							o.pos = UnityObjectToClipPos(IN[i].vertex + float4(0, 1, 0, 0) * _Factor);
							o.uv = IN[i].uv;
							o.col = fixed4(1., 1., 1., 1.);
							tristream.Append(o);
						}

						tristream.RestartStrip();
					}
				}
			}

			fixed4 fragS(g2f i) : SV_Target
			{
				fixed4 col = tex2D(_SnowText, i.uv) * i.col;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ToonGUIv5"
}