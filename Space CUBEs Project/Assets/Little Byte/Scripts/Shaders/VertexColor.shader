Shader "Custom/Vertex Color" {
	Properties { }
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" "LightMode"="Always"}  
		
		Lighting Off 
		Fog { Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers ps3 xbox360 flash
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex wfiVertCol
			#pragma fragment passThrough
			#include "UnityCG.cginc"

			struct VertIn
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct VertOut
			{
				float4 position : POSITION;
				float4 color : COLOR;
			};			

			VertOut wfiVertCol(VertIn input, float3 normal : NORMAL)
			{
				VertOut output;
				output.position = mul(UNITY_MATRIX_MVP,input.vertex);
				output.color = input.color;
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
	FallBack "Diffuse"
}