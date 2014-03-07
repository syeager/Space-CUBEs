Shader "CUBEs/Diffuse_Vertex" 
{
	Properties 
	{
		_Color ("Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Attenu ( "Attenuation", Range (0.0, 4.0)) = 2.0
	}

	SubShader 
	{
		Pass
		{
			tags {"Lightmode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			// user defined variables
			uniform float4 _Color;
			uniform float4 _LightColor0;
			uniform float _Attenu;

			// base input structs
			struct vertIn 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertOut
			{
				float4 pos : SV_POSITION;	// object vertex position from gpu (SV_ because of directX 11)
				float4 clr : COLOR;
			};

			// per-vertex function
			vertOut Vert(vertIn v) 
			{
				vertOut o;

				float3 normalDir = normalize( mul( float4(v.normal, 0.0), _World2Object).xyz );
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseReflect = _Attenu * _LightColor0.xyz * max( 0.0, dot(normalDir, lightDir) );
				float3 lightFinal = diffuseReflect + UNITY_LIGHTMODEL_AMBIENT.xyz;

				o.clr = float4(lightFinal * _Color.rgb, 1.0);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			// per-fragment function
			float4 Frag(vertOut i) : COLOR
			{
				return i.clr;
			}

			ENDCG
		}
	} 

	// Fallback commented out during dev for easier error catching
	// FallBack "Diffuse"
}
