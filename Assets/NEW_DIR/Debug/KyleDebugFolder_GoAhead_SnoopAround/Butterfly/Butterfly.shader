Shader "Capstone2019/Butterfly"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", color) = (1,1,1,1)
		_AlphaCutoff("Alpha cutoff", Range(0, 1)) = 0
		_DisplacementAmount("Displacement Amount Up", float) = 1
		_DisplacementSpeed("Displacement Speed Up", float) = 1
		_DisplacementAmountY("Displacement Amount Up", float) = 1
		_DisplacementSpeedY("Displacement Speed Back", float) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100
			Cull off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 color : COLOR;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				float _AlphaCutoff;
				float _DisplacementAmount;
				float _DisplacementSpeed;
				float _DisplacementAmountY;
				float _DisplacementSpeedY;

				v2f vert(appdata v)
				{
					v2f o;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					float mask = 1 - sin(UNITY_PI * o.uv.x);
					v.vertex.z += sin(_Time.y * _DisplacementSpeed) * _DisplacementAmount * mask;
					v.vertex.y += sin(_Time.y * _DisplacementSpeedY) * _DisplacementAmountY * mask;
					o.vertex = UnityObjectToClipPos(v.vertex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				fixed4 _Color;

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv) * _Color;
					clip(col.a - _AlphaCutoff);
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}
