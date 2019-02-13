Shader "NanoRav/Snow" {
Properties {
	_Color ("Main Color", Color) = (0.75,0.75,0.75,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

	_SnowAmount ("Snow Level", Range(0,1) ) = 0
	_SnowHeight ("Snow Height", Range(0,1) ) = 0.5
    _SnowColor ("Snow Color", Color) = (1.0,1.0,1.0,1.0)
    _SnowDirection ("Snow Direction", Vector) = (0,1,0)

	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_SnowMap ("Snow Heightmap", 2D) = "white" {}
	_SnowTex ("Snow Texture", 2D) = "white" {}
	_SnowBump ("Snow Normalmap", 2D) = "bump" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	
	CGPROGRAM
	#pragma target 3.0
	#pragma surface surf BlinnPhong

		sampler2D _MainTex, _BumpMap, _SnowMap, _SnowTex, _SnowBump;
		float4 _Color, _SnowColor, _SnowDirection;
		half _Shininess, _SnowAmount, _SnowHeight;

		struct Input {
			float2 uv_MainTex;
			float2 uv_SnowMap;
			float3 worldNormal;
		    INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 snowMap = tex2D(_SnowMap, IN.uv_SnowMap);
			fixed4 snowTex = tex2D(_SnowTex, IN.uv_MainTex);

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			if(snowMap.r>_SnowHeight && dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz)>=lerp(1,-1,_SnowAmount)){
	        	o.Albedo = snowTex * _SnowColor.rgb;
	        	o.Specular = 0.0;
	        	o.Gloss = 0.0;
	        	o.Normal = lerp(UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex)),UnpackNormal(tex2D(_SnowBump, IN.uv_MainTex)), _SnowAmount);
	        }else{
	        	o.Albedo = tex.rgb * _Color.rgb;
	        	o.Specular = _Shininess;
	        	o.Gloss = tex.a;
	        }

	        o.Alpha = 1;
		}
	ENDCG
}
}
