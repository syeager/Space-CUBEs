Shader "Custom/Cursor_shdr" 
{
	Properties 
	{
		_MainColor ("Main Color", Color) = (0.0, 0.0, 0.0, 0.5)

	}

	SubShader
	{
		Tags 
		{ 
			"Queue" = "Overlay"
			"Queue" = "Transparent"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Always
			Color [_MainColor]
		}
	} 
	FallBack "Diffuse"
}
