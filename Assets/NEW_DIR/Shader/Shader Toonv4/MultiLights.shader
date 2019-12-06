Shader "SC/MultiLights"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Albedo", 2D) = "white" {}
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.1
	}

	Subshader
	{
		Pass
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
			#include "SC_Lighting.cginc"

			ENDCG
		}

		Pass
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
			#include "SC_Lighting.cginc"

			ENDCG
		}
	}
}
