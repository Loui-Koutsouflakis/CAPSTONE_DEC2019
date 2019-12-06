Shader "Unlit/DrawTracks"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_FootprintTex("Footprint Texture", 2D) = "grey" {}
		_Coordinate("Coordinate", Vector) = (0,0,0,0)
		_Color("Draw Color", Color) = (1,0,0,0)
		_BrushSize("Brush Size", Range(0,5)) = 5 
		_BrushOpacity("Brush Opacity", Range(0,1)) = 1 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
			
			float _BrushSize;
			float _BrushOpacity;
			
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _FootprintTex;
            float4 _MainTex_ST;
            float4 _FootprintTex_ST;
            float4 _Coordinate, _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				float draw = pow(saturate(1 - distance(i.uv, _Coordinate.xy)), 1000 / _BrushSize); //= tex2D(_FootprintTex, (_Coordinate.xy + 0.5) * i.uv);
				fixed4 drawcol = _Color * (draw * _BrushOpacity);
				return saturate(col + drawcol);
            }
            ENDCG
        }
    }
}
