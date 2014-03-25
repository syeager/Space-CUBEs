Shader "CUBEs/Vertex Color Lerp" {
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,0)
		_Alpha ("Alpha", Range(0,1)) = 0
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }      
		
		Lighting Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers ps3 xbox360 flash
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex wfiVertCol
			#pragma fragment passThrough
			#include "UnityCG.cginc"

			float4 _Color;
			float _Alpha;

			struct VertOut
			{
				float4 position : POSITION;
				float4 color : COLOR;
			};

			struct VertIn
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			VertOut wfiVertCol(VertIn input)
			{
				VertOut output;
				output.position = mul(UNITY_MATRIX_MVP, input.vertex);
				output.color = lerp(input.color, _Color, _Alpha);
				return output;
			}

			struct FragOut
			{
				float4 color : COLOR;
			};

			FragOut passThrough(float4 color : COLOR)
			{
				FragOut output;
				output.color = color;
				return output;
			}
			ENDCG

		}
	}
}