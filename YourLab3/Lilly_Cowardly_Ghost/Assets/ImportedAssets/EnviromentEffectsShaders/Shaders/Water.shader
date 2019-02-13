Shader "NanoRav/Water" {
Properties {
	_Color ("Main Color", Color) = (0.75,0.75,0.75,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

	_WaterAmount ("Water Level", Range(0,1) ) = 0
    _WaterColor ("Water Color", Color) = (1.0,1.0,1.0,1.0)

	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_WaterDirection("Water Direction", Vector) = (0,0,0)

	_WaterMap ("Water Heightmap", 2D) = "white" {}
	_WaterTex ("Water Texture", 2D) = "white" {}
	_WaterBump ("Water Normalmap", 2D) = "bump" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	
	CGPROGRAM
	#pragma target 3.0
	#pragma surface surf BlinnPhong

		sampler2D _MainTex, _BumpMap;
		float4 _Color;
		half _Shininess;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	        o.Albedo = tex.rgb * _Color.rgb;
	        o.Specular = _Shininess;
	        o.Gloss = tex.a;
	        o.Alpha = 1;
		}
	ENDCG

	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
	#pragma target 3.0
	#pragma surface surf BlinnPhong vertex:vert
	#pragma exclude_renderers gles
	#pragma glsl

		sampler2D _BumpMap, _WaterTex, _WaterMap, _WaterBump;
		float4 _WaterColor, _WaterDirection;
		half _Shininess, _WaterAmount;

		struct Input {
			float2 uv_MainTex;
			float2 uv_WaterMap;
		};

		void vert (inout appdata_full v) {
			float4 tex = tex2Dlod (_WaterMap, float4(v.texcoord.xy,0,0));
        	v.vertex.xyz += 0.02 * v.normal * clamp(tex.r-0.2,0,1);
      	}

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 waterMap = tex2D(_WaterMap, IN.uv_WaterMap);
			fixed4 waterTex = tex2D(_WaterTex, IN.uv_MainTex + (_WaterDirection*(_Time/20.0)));

			o.Normal = UnpackNormal(tex2D(_WaterBump, IN.uv_MainTex));
	        o.Albedo = waterTex * _WaterColor;
	        o.Specular = 1.0;
	        o.Gloss = waterTex.a;
	        o.Alpha = clamp(waterMap.r,0,_WaterAmount);
		}
	ENDCG
}
}
