Shader "Custom/Dissolve"
{
    Properties
    {
		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Strength("Strength", Range(0, 1)) = 0.0
		_OutlineStrength("Outline Strength", Range(0, 0.1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
		Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _OutlineColor;
		float _OutlineStrength;
		sampler2D _DissolveTexture;
		half _Strength;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			half dissolveValue = tex2D(_DissolveTexture, IN.uv_MainTex).r;
			clip(dissolveValue - _Strength);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
			o.Emission = _OutlineColor * step(dissolveValue - _Strength, _OutlineStrength);
        }
        ENDCG
    }
    FallBack "Diffuse"
}


// _DissolveTexture("Dissolve Texture", 2D) = "white" {} 
// _Amount("Amount", Range(0, 1)) = 0
// 
// sampler2D _DissolveTexture;
// half _Amount;
// 
// half dissolve_value = tex2D(_DissolveTexture, IN.uv_MainTex).r;
// clip(dissolve_value - _Amount);
//
// 