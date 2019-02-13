Shader "NanoRav/Sand" {
Properties {
	_Color ("Main Color", Color) = (0.75,0.75,0.75,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

	_SandHeight ("Sand Height", Range(0,1)) = 0.5
    _SandColor ("Sand Color", Color) = (1.0,1.0,1.0,1.0)
    _SandDirection ("Sand Direction", Vector) = (0,1,0)

	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_SandMap ("Sand Heightmap", 2D) = "white" {}
	_SandTex ("Sand Texture", 2D) = "white" {}
	_SandBump ("Sand Bumpmap", 2D) = "bump" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	
	CGPROGRAM
	#pragma target 3.0
	#pragma surface surf BlinnPhong

		sampler2D _MainTex, _BumpMap, _SandMap, _SandTex, _SandBump;
		float4 _Color, _SandColor, _SandDirection;
		half _Shininess, _SandHeight;

		struct Input {
			float2 uv_MainTex;
			float2 uv_SandMap;
			float3 worldNormal;
		    INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 sandMap = tex2D(_SandMap, IN.uv_SandMap);
			fixed4 sandTex = tex2D(_SandTex, IN.uv_MainTex);

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			if(sandMap.r>_SandHeight){
	        	o.Albedo = sandTex * _SandColor.rgb;
	        	o.Specular = 0.0;
	        	o.Gloss = 0.0;
	        	o.Normal = lerp(UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex)),UnpackNormal(tex2D(_SandBump, IN.uv_MainTex)),0.75);
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
