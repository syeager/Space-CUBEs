Shader "CUBEs/SurfaceOverlay" 
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
