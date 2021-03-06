// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:0,limd:1,uamb:False,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:33338,y:32442|diff-219-OUT,spec-226-OUT,gloss-225-OUT,normal-215-OUT,emission-224-OUT,transm-29-OUT,lwrap-29-OUT,alpha-227-OUT,refract-14-OUT;n:type:ShaderForge.SFN_Slider,id:13,x:34085,y:32752,ptlb:Refraction Intensity,ptin:_RefractionIntensity,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Multiply,id:14,x:33677,y:32724|A-16-OUT,B-220-OUT;n:type:ShaderForge.SFN_ComponentMask,id:16,x:33847,y:32651,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-25-RGB;n:type:ShaderForge.SFN_Tex2d,id:25,x:34085,y:32566,ptlb:Refraction Bump,ptin:_RefractionBump,tex:348d3407b81faa342b5ffba6b8d977c7,ntxv:2,isnm:False|UVIN-27-OUT;n:type:ShaderForge.SFN_TexCoord,id:26,x:34443,y:32505,uv:0;n:type:ShaderForge.SFN_Multiply,id:27,x:34272,y:32566|A-26-UVOUT,B-28-OUT;n:type:ShaderForge.SFN_Vector1,id:28,x:34443,y:32662,v1:1;n:type:ShaderForge.SFN_Vector1,id:29,x:33677,y:32577,v1:1;n:type:ShaderForge.SFN_Lerp,id:215,x:33847,y:32524|A-216-OUT,B-25-RGB,T-13-OUT;n:type:ShaderForge.SFN_Vector3,id:216,x:34085,y:32445,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Fresnel,id:217,x:33847,y:32284;n:type:ShaderForge.SFN_ConstantLerp,id:219,x:33677,y:32284,a:0.02,b:0.2|IN-217-OUT;n:type:ShaderForge.SFN_Multiply,id:220,x:33847,y:32803|A-13-OUT,B-221-OUT;n:type:ShaderForge.SFN_Vector1,id:221,x:34085,y:32831,v1:0.2;n:type:ShaderForge.SFN_Cubemap,id:222,x:34267,y:32352,ptlb:Emit Cubemap,ptin:_EmitCubemap,cube:1c5b6b0015b3622489503c11a6c00db5,pvfc:0;n:type:ShaderForge.SFN_Fresnel,id:223,x:34267,y:32192;n:type:ShaderForge.SFN_Divide,id:224,x:34097,y:32278|A-223-OUT,B-222-RGB;n:type:ShaderForge.SFN_Slider,id:225,x:33795,y:32454,ptlb:Gloss Slider,ptin:_GlossSlider,min:0,cur:0.6153846,max:1;n:type:ShaderForge.SFN_Slider,id:226,x:33623,y:32155,ptlb:Spec Slider,ptin:_SpecSlider,min:0,cur:5,max:5;n:type:ShaderForge.SFN_Fresnel,id:227,x:33947,y:32943|EXP-228-OUT;n:type:ShaderForge.SFN_Slider,id:228,x:34121,y:32980,ptlb:Alpha Fresnel Slider,ptin:_AlphaFresnelSlider,min:0,cur:2.469774,max:5;proporder:13-25-222-225-226-228;pass:END;sub:END;*/

Shader "CUBEs/Core Glass Refract" {
    Properties {
        _RefractionIntensity ("Refraction Intensity", Range(0, 1)) = 0.5
        _RefractionBump ("Refraction Bump", 2D) = "black" {}
        _EmitCubemap ("Emit Cubemap", Cube) = "_Skybox" {}
        _GlossSlider ("Gloss Slider", Range(0, 1)) = 0.6153846
        _SpecSlider ("Spec Slider", Range(0, 5)) = 5
        _AlphaFresnelSlider ("Alpha Fresnel Slider", Range(0, 5)) = 2.469774
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _RefractionBump; uniform float4 _RefractionBump_ST;
            uniform samplerCUBE _EmitCubemap;
            uniform float _GlossSlider;
            uniform float _SpecSlider;
            uniform float _AlphaFresnelSlider;
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
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 node_27 = (i.uv0.rg*1.0);
                float4 node_25 = tex2D(_RefractionBump,TRANSFORM_TEX(node_27, _RefractionBump));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_25.rgb.rg*(_RefractionIntensity*0.2));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalLocal = lerp(float3(0,0,1),node_25.rgb,_RefractionIntensity);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_29 = 1.0;
                float3 w = float3(node_29,node_29,node_29)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_29,node_29,node_29);
                float3 diffuse = (forwardLight+backLight) * attenColor;
////// Emissive:
                float3 emissive = ((1.0-max(0,dot(normalDirection, viewDirection)))/texCUBE(_EmitCubemap,viewReflectDirection).rgb);
///////// Gloss:
                float gloss = _GlossSlider;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_SpecSlider,_SpecSlider,_SpecSlider);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_219 = lerp(0.02,0.2,(1.0-max(0,dot(normalDirection, viewDirection))));
                finalColor += diffuseLight * float3(node_219,node_219,node_219);
                finalColor += specular;
                finalColor += emissive;
/// Final Color:
                return fixed4(lerp(sceneColor.rgb, finalColor,pow(1.0-max(0,dot(normalDirection, viewDirection)),_AlphaFresnelSlider)),1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _RefractionBump; uniform float4 _RefractionBump_ST;
            uniform samplerCUBE _EmitCubemap;
            uniform float _GlossSlider;
            uniform float _SpecSlider;
            uniform float _AlphaFresnelSlider;
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
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 node_27 = (i.uv0.rg*1.0);
                float4 node_25 = tex2D(_RefractionBump,TRANSFORM_TEX(node_27, _RefractionBump));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_25.rgb.rg*(_RefractionIntensity*0.2));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalLocal = lerp(float3(0,0,1),node_25.rgb,_RefractionIntensity);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_29 = 1.0;
                float3 w = float3(node_29,node_29,node_29)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_29,node_29,node_29);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = _GlossSlider;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_SpecSlider,_SpecSlider,_SpecSlider);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_219 = lerp(0.02,0.2,(1.0-max(0,dot(normalDirection, viewDirection))));
                finalColor += diffuseLight * float3(node_219,node_219,node_219);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * pow(1.0-max(0,dot(normalDirection, viewDirection)),_AlphaFresnelSlider),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
