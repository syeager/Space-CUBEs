Shader "Custom/OverlayBlend"
{
    Properties 
    {
        _Color ("Blend Color", Color) = (0.5, 0.5, 0.5, 1.0)
    }   

    SubShader 
    {
        Tags 
        {
            "Queue" = "Overlay"
            "RenderType" = "Transparent" 
        }        

        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc" 

            struct appdata_custom 
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : POSITION;
            };

            fixed4 _Color;            

            v2f vert (appdata_custom v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }           

            fixed4 frag (v2f i) : COLOR
            {
				fixed4 diffuse = _Color;
				return diffuse;

                //fixed4 diffuse = tex2D(_MainTex, i.uv);
                //fixed luminance =  dot(diffuse, fixed4(0.2126, 0.7152, 0.0722, 0));
                //fixed oldAlpha = diffuse.a;

                //if (luminance < 0.5) 
                //    diffuse *= 2 * _Color;
               // else 
               //     diffuse = 1-2*(1-diffuse)*(1-_Color);

             //   diffuse.a  = oldAlpha * _Color.a;

             //   return diffuse;
            }

            ENDCG 
        }
    }   

    Fallback off 
}