//base include for outline
#ifndef MK_TOON_OUTLINE_ONLY_BASE
	#define MK_TOON_OUTLINE_ONLY_BASE

	//Todo: needs to be optimized to not get in conflict with the original surfaceColor function
	//get surface color based on blendmode and color source
	#if MKTOON_TEXCLR
		void SurfaceColor(out fixed3 albedo, out fixed alpha, float2 uv)
		{
			#if _MK_MODE_CUTOUT || _MK_MODE_TRANSPARENT
				fixed4 c = tex2D(_MainTex, uv) * _OutlineColor;
				c.rgb = _OutlineColor;
				albedo = c.rgb;
				alpha = c.a;
			#else
				albedo = _OutlineColor.rgb;
				alpha = 1.0;
			#endif
		}
	#elif MKTOON_VERTCLR
		void SurfaceColor(out fixed3 albedo, out fixed alpha, fixed4 vrtColor)
		{
			#if _MK_MODE_CUTOUT || _MK_MODE_TRANSPARENT
				fixed4 c = vrtColor * _OutlineColor;
				albedo = c.rgb;
				alpha = c.a;
			#else
				albedo = vrtColor * _OutlineColor;
				alpha = 1.0;
			#endif
		}
	#endif

	/////////////////////////////////////////////////////////////////////////////////////////////
	// VERTEX SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	VertexOutputOutlineOnly outlinevert(VertexInputOutlineOnly v)
	{
		UNITY_SETUP_INSTANCE_ID(v);
		VertexOutputOutlineOnly o;
		UNITY_INITIALIZE_OUTPUT(VertexOutputOutlineOnly, o);
		UNITY_TRANSFER_INSTANCE_ID(v,o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		v.vertex.xyz += normalize(v.normal) * _OutlineSize;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.color = v.color;
		//texcoords
		#if MKTOON_TC
			o.uv = TRANSFORM_TEX(v.texcoord0, _MainTex);
		#elif _MKTOON_DISSOLVE
			o.uv = TRANSFORM_TEX(v.texcoord0, _DissolveMap);
		#endif
		UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////
	// FRAGMENT SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	fixed4 outlinefrag(VertexOutputOutlineOnly o) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(o);
		#if _MKTOON_DISSOLVE
			fixed3 sg =  tex2D (_DissolveMap, o.uv).r - _DissolveAmount;
			Clip0(sg);
		#endif

		fixed4 oColor;
		
		#if MKTOON_TEXCLR
			SurfaceColor(oColor.rgb, oColor.a, o.uv);
		#elif MKTOON_VERTCLR
			SurfaceColor(oColor.rgb, oColor.a, o.color);
		#endif

		#if _MK_MODE_CUTOUT
			//skip fragment in cutoff mode
			if(oColor.a < _Cutoff) discard;
		#endif

		#if _MK_DISSOLVE_RAMP
			//apply color for dissolving
			oColor.rgb = DissolveRamp(sg, _DissolveRamp, _DissolveColor, _DissolveRampSize, _DissolveAmount, o.uv, oColor);
		#endif

		UNITY_APPLY_FOG(o.fogCoord, oColor);
		return oColor;
	}
#endif