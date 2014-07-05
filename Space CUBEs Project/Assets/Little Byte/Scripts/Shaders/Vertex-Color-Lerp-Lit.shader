Shader "CUBEs/Vertex Color Lerp Lit" {
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,0)
		_Mix ("Mix", Range(0,1)) = 0
		_Alpha ("Alpha", Range(0,1)) = 1
		_Attenu ( "Attenuation", Range (0.0, 4.0)) = 2.0
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "Lightmode" = "ForwardBase" }      
		
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
			float _Mix;
			float _Alpha;
			uniform float _Attenu;

			uniform float4 _LightColor0;

			struct VertOut
			{
				float4 position : POSITION;
				float4 color : COLOR;
			};

			struct VertIn
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;
			};

			VertOut wfiVertCol(VertIn input)
			{
				// light
				float3 normalDir = normalize( mul( float4(input.normal, 0.0), _World2Object).xyz );
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseReflect = _Attenu * _LightColor0.xyz * max( 0.0, dot(normalDir, lightDir) );
				float3 lightFinal = diffuseReflect + UNITY_LIGHTMODEL_AMBIENT.xyz;

				VertOut output;

				output.position = mul(UNITY_MATRIX_MVP, input.vertex);
				output.color = float4(lightFinal * lerp(input.color, _Color, _Mix), _Alpha);

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