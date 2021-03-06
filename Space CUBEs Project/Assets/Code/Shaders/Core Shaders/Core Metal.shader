// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-311-OUT,spec-3-OUT,gloss-167-OUT,normal-5-RGB,amspl-217-RGB;n:type:ShaderForge.SFN_Color,id:2,x:33454,y:32580,ptlb:Diffuse Clr,ptin:_DiffuseClr,glob:False,c1:0.3,c2:0.3,c3:0.3,c4:1;n:type:ShaderForge.SFN_Slider,id:3,x:33482,y:32779,ptlb:Spec Slider,ptin:_SpecSlider,min:0,cur:0.2393173,max:1;n:type:ShaderForge.SFN_Slider,id:4,x:33476,y:32863,ptlb:Gloss Slider,ptin:_GlossSlider,min:0,cur:0.2649583,max:1;n:type:ShaderForge.SFN_Tex2d,id:5,x:33587,y:32962,ptlb:Normal Tex,ptin:_NormalTex,tex:777f7c809dffa814789d0fd9b5e190dc,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Fresnel,id:167,x:33103,y:32783|EXP-299-OUT;n:type:ShaderForge.SFN_Cubemap,id:217,x:33031,y:33059,ptlb:Spec Cubemap,ptin:_SpecCubemap,cube:476dfa2c635d92443aac5535c8e6e6c5,pvfc:0;n:type:ShaderForge.SFN_OneMinus,id:299,x:33295,y:32829|IN-4-OUT;n:type:ShaderForge.SFN_Multiply,id:311,x:33140,y:32538|A-367-OUT,B-2-RGB;n:type:ShaderForge.SFN_Tex2d,id:312,x:33759,y:32274,ptlb:Diffuse Tex,ptin:_DiffuseTex,tex:777f7c809dffa814789d0fd9b5e190dc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:367,x:33427,y:32329|VAL-312-RGB,EXP-368-OUT;n:type:ShaderForge.SFN_Vector1,id:368,x:33641,y:32469,v1:0.25;proporder:2-3-4-5-217-312;pass:END;sub:END;*/

Shader "CUBEs/Core Metal" {
    Properties {
        _DiffuseClr ("Diffuse Clr", Color) = (0.3,0.3,0.3,1)
        _SpecSlider ("Spec Slider", Range(0, 1)) = 0.2393173
        _GlossSlider ("Gloss Slider", Range(0, 1)) = 0.2649583
        _NormalTex ("Normal Tex", 2D) = "black" {}
        _SpecCubemap ("Spec Cubemap", Cube) = "_Skybox" {}
        _DiffuseTex ("Diffuse Tex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _DiffuseClr;
            uniform float _SpecSlider;
            uniform float _GlossSlider;
            uniform sampler2D _NormalTex; uniform float4 _NormalTex_ST;
            uniform samplerCUBE _SpecCubemap;
            uniform sampler2D _DiffuseTex; uniform float4 _DiffuseTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_611 = i.uv0;
                float3 normalLocal = tex2D(_NormalTex,TRANSFORM_TEX(node_611.rg, _NormalTex)).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
///////// Gloss:
                float gloss = pow(1.0-max(0,dot(normalDirection, viewDirection)),(1.0 - _GlossSlider));
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_SpecSlider,_SpecSlider,_SpecSlider);
                float3 specularAmb = texCUBE(_SpecCubemap,viewReflectDirection).rgb * specularColor;
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor + specularAmb;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * (pow(tex2D(_DiffuseTex,TRANSFORM_TEX(node_611.rg, _DiffuseTex)).rgb,0.25)*_DiffuseClr.rgb);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _DiffuseClr;
            uniform float _SpecSlider;
            uniform float _GlossSlider;
            uniform sampler2D _NormalTex; uniform float4 _NormalTex_ST;
            uniform sampler2D _DiffuseTex; uniform float4 _DiffuseTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_612 = i.uv0;
                float3 normalLocal = tex2D(_NormalTex,TRANSFORM_TEX(node_612.rg, _NormalTex)).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = pow(1.0-max(0,dot(normalDirection, viewDirection)),(1.0 - _GlossSlider));
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_SpecSlider,_SpecSlider,_SpecSlider);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * (pow(tex2D(_DiffuseTex,TRANSFORM_TEX(node_612.rg, _DiffuseTex)).rgb,0.25)*_DiffuseClr.rgb);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
