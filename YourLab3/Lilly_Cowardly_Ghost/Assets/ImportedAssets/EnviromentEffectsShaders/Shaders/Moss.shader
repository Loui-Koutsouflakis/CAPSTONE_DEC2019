Shader "NanoRav/Moss" {
Properties {
	_Color ("Main Color", Color) = (0.75,0.75,0.75,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

	_MossAmount ("Moss Level", Range(0,1) ) = 0
    _MossColor ("Moss Color", Color) = (1.0,1.0,1.0,1.0)
    _MossDirection ("Moss Direction", Vector) = (0,1,0)

	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_MossMap ("Moss Heightmap", 2D) = "white" {}
	_MossTex ("Moss Texture", 2D) = "white" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	
	CGPROGRAM
	#pragma target 3.0
	#pragma surface surf BlinnPhong

		sampler2D _MainTex, _BumpMap, _MossMap, _MossTex;
		float4 _Color, _MossColor, _MossDirection;
		half _Shininess, _MossAmount;

		struct Input {
			float2 uv_MainTex;
			float2 uv_MossMap;
			float3 worldNormal;
		    INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 mossMap = tex2D(_MossMap, IN.uv_MossMap);
			fixed4 mossTex = tex2D(_MossTex, IN.uv_MainTex);

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			if(mossMap.r>0.5 && dot(WorldNormalVector(IN, o.Normal), _MossDirection.xyz)>=lerp(1,-1,_MossAmount)){
	        	o.Albedo = mossTex * _MossColor.rgb;
	        	o.Specular = 0.0;
	        	o.Gloss = 0.0;
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
