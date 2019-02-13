Shader "NanoRav/Lava" {
Properties {
	_Color ("Main Color", Color) = (0.75,0.75,0.75,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

	_LavaAmount ("Lava Level", Range(0,1) ) = 0
    _LavaColor ("Lava Color", Color) = (1.0,1.0,1.0,1.0)

	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_LavaLight("Lava Light", Range(1,20)) = 7.5
	_LavaDirection("Lava Direction", Vector) = (0,0,0)

	_LavaMap ("Lava Heightmap", 2D) = "white" {}
	_LavaTex ("Lava Texture", 2D) = "white" {}
	_LavaBump ("Lava Normalmap", 2D) = "bump" {}
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

		sampler2D _BumpMap, _LavaTex, _LavaMap, _LavaBump;
		float4 _LavaColor, _LavaDirection;
		half _Shininess, _LavaAmount, _LavaLight;

		struct Input {
			float2 uv_MainTex;
			float2 uv_LavaMap;
		};

		void vert (inout appdata_full v) {
			float4 tex = tex2Dlod (_LavaMap, float4(v.texcoord.xy,0,0));
        	v.vertex.xyz += 0.02 * v.normal * clamp(tex.r-0.2,0,1);
      	}

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 lavaMap = tex2D(_LavaMap, IN.uv_LavaMap);
			fixed4 lavaTex = tex2D(_LavaTex, IN.uv_MainTex + (_LavaDirection*(_Time/20.0)));

			o.Normal = UnpackNormal(tex2D(_LavaBump, IN.uv_MainTex));
	        o.Albedo = lavaTex * _LavaColor;
	        o.Specular = 1.0;
	        o.Gloss = 1.0;
	        o.Emission = lavaTex/_LavaLight;
	        o.Alpha = clamp(lavaMap.r,0,_LavaAmount);
		}
	ENDCG
}
}
