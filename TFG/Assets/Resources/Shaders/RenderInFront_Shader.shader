Shader "Custom/RenderInFront_Shader"
{
        Properties
        {
            Color_c4b6043ac1de492fb8bbd1482b723534("Color", Color) = (1, 0.02545873, 0, 0)
            Color_adc979e474cd4c52803b73073550b88e("Color 1", Color) = (1, 0.6121189, 0, 0)
            Vector1_f4346caac0c24533843067b1caa8641a("Blend", Range(1, 50)) = 1
            Vector1_f4346caac0c24533843067b1caa8641a_1("Blend (1)", Range(-50, 0.99)) = 0.94
            Vector1_9ab9734743834cf3842959c683cd7be1("Fade Heigth", Float) = 1
            [NoScaleOffset]Texture2D_93021b81bc604a779a4dd2f83b95f849("Normal Map", 2D) = "white" {}
            Vector1_ac9ee0e7cc6f41dda544b0e89518d65a("Normal Strength", Float) = 0
            Vector1_6ef3f7747728449dae573951cca43c42("Ambient Occlusion", Float) = 0
            Vector1_59d4fb46a8c44151b3b0ea55014c4b35("Smoothness", Float) = 0
            [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
            [HideInInspector]_QueueControl("_QueueControl", Float) = -1
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        }
            SubShader
        {
            Tags
            {
                "RenderPipeline" = "UniversalPipeline"
                "RenderType" = "Transparent"
                "UniversalMaterialType" = "Lit"
                "Queue" = "Transparent+2000"
                "ShaderGraphTargetId" = "UniversalLitSubTarget"
            }
            Pass
            {
                Name "Universal Forward"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }

            // Render State
            Cull Back
            Blend One Zero
            ZTest Off
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_SHADOW_COORD
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            #define _FOG_FRAGMENT 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                 float3 positionOS : POSITION;
                 float3 normalOS : NORMAL;
                 float4 tangentOS : TANGENT;
                 float4 uv0 : TEXCOORD0;
                 float4 uv1 : TEXCOORD1;
                 float4 uv2 : TEXCOORD2;
                #if UNITY_ANY_INSTANCING_ENABLED
                 uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
            struct Varyings
            {
                 float4 positionCS : SV_POSITION;
                 float3 positionWS;
                 float3 normalWS;
                 float4 tangentWS;
                 float4 texCoord0;
                 float3 viewDirectionWS;
                #if defined(LIGHTMAP_ON)
                 float2 staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                 float2 dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                 float3 sh;
                #endif
                 float4 fogFactorAndVertexLight;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                 float4 shadowCoord;
                #endif
                #if UNITY_ANY_INSTANCING_ENABLED
                 uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            struct SurfaceDescriptionInputs
            {
                 float3 TangentSpaceNormal;
                 float3 WorldSpacePosition;
                 float4 uv0;
            };
            struct VertexDescriptionInputs
            {
                 float3 ObjectSpaceNormal;
                 float3 ObjectSpaceTangent;
                 float3 ObjectSpacePosition;
            };
            struct PackedVaryings
            {
                 float4 positionCS : SV_POSITION;
                 float3 interp0 : INTERP0;
                 float3 interp1 : INTERP1;
                 float4 interp2 : INTERP2;
                 float4 interp3 : INTERP3;
                 float3 interp4 : INTERP4;
                 float2 interp5 : INTERP5;
                 float2 interp6 : INTERP6;
                 float3 interp7 : INTERP7;
                 float4 interp8 : INTERP8;
                 float4 interp9 : INTERP9;
                #if UNITY_ANY_INSTANCING_ENABLED
                 uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.interp0.xyz = input.positionWS;
                output.interp1.xyz = input.normalWS;
                output.interp2.xyzw = input.tangentWS;
                output.interp3.xyzw = input.texCoord0;
                output.interp4.xyz = input.viewDirectionWS;
                #if defined(LIGHTMAP_ON)
                output.interp5.xy = input.staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                output.interp6.xy = input.dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.interp7.xyz = input.sh;
                #endif
                output.interp8.xyzw = input.fogFactorAndVertexLight;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                output.interp9.xyzw = input.shadowCoord;
                #endif
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp0.xyz;
                output.normalWS = input.interp1.xyz;
                output.tangentWS = input.interp2.xyzw;
                output.texCoord0 = input.interp3.xyzw;
                output.viewDirectionWS = input.interp4.xyz;
                #if defined(LIGHTMAP_ON)
                output.staticLightmapUV = input.interp5.xy;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                output.dynamicLightmapUV = input.interp6.xy;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.interp7.xyz;
                #endif
                output.fogFactorAndVertexLight = input.interp8.xyzw;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                output.shadowCoord = input.interp9.xyzw;
                #endif
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Color_c4b6043ac1de492fb8bbd1482b723534;
            float4 Color_adc979e474cd4c52803b73073550b88e;
            float Vector1_f4346caac0c24533843067b1caa8641a;
            float Vector1_f4346caac0c24533843067b1caa8641a_1;
            float Vector1_9ab9734743834cf3842959c683cd7be1;
            float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
            float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
            float Vector1_6ef3f7747728449dae573951cca43c42;
            float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
            CBUFFER_END

                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                // Graph Includes
                // GraphIncludes: <None>

                // -- Property used by ScenePickingPass
                #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
                #endif

                // -- Properties used by SceneSelectionPass
                #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
                #endif

                // Graph Functions

                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }

                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }

                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }

                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }

                void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A + B;
                }

                void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
                {
                    Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
                }

                // Custom interpolators pre vertex
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };

                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }

                // Custom interpolators, pre surface
                #ifdef FEATURES_GRAPH_VERTEX
                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                {
                return output;
                }
                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                #endif

                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float3 NormalTS;
                    float3 Emission;
                    float Metallic;
                    float Smoothness;
                    float Occlusion;
                };

                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_271638d18c864c50adc2de9ba4cf9707_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_c4b6043ac1de492fb8bbd1482b723534) : Color_c4b6043ac1de492fb8bbd1482b723534;
                    float _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                    float _Property_910d825da2eb46b0a1754b9e69252646_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a;
                    float _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2;
                    Unity_Subtract_float(_Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Property_910d825da2eb46b0a1754b9e69252646_Out_0, _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2);
                    float _Add_bca707404b3243949fe8953b161e1601_Out_2;
                    Unity_Add_float(-1, _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Add_bca707404b3243949fe8953b161e1601_Out_2);
                    float _Split_4c5a12ca469049ee8d25c52df86199d7_R_1 = IN.WorldSpacePosition[0];
                    float _Split_4c5a12ca469049ee8d25c52df86199d7_G_2 = IN.WorldSpacePosition[1];
                    float _Split_4c5a12ca469049ee8d25c52df86199d7_B_3 = IN.WorldSpacePosition[2];
                    float _Split_4c5a12ca469049ee8d25c52df86199d7_A_4 = 0;
                    float _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3;
                    Unity_Smoothstep_float(_Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2, _Add_bca707404b3243949fe8953b161e1601_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3);
                    float4 _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2;
                    Unity_Multiply_float4_float4(_Property_271638d18c864c50adc2de9ba4cf9707_Out_0, (_Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3.xxxx), _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2);
                    float4 _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2;
                    Unity_Multiply_float4_float4(_Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2, float4(2, 2, 2, 2), _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2);
                    float4 _Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_adc979e474cd4c52803b73073550b88e) : Color_adc979e474cd4c52803b73073550b88e;
                    float _Property_429fa9faf52b49c891efaa416fe2c903_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                    float _Property_73479999c2b346edb73436833d483856_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a_1;
                    float _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2;
                    Unity_Subtract_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, _Property_73479999c2b346edb73436833d483856_Out_0, _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2);
                    float _Add_554d292e79d145489ad3cdc7084a70ca_Out_2;
                    Unity_Add_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, -1, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2);
                    float _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3;
                    Unity_Smoothstep_float(_Subtract_72abdf79c34d4aacadfe832896a30461_Out_2, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3);
                    float4 _Multiply_f02e57049b254438874c61113b259afa_Out_2;
                    Unity_Multiply_float4_float4(_Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0, (_Smoothstep_919b4e994d44484590da2d34a073773c_Out_3.xxxx), _Multiply_f02e57049b254438874c61113b259afa_Out_2);
                    float4 _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2;
                    Unity_Add_float4(_Multiply_9d41234165c1425c8005aa668f597e6c_Out_2, _Multiply_f02e57049b254438874c61113b259afa_Out_2, _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2);
                    UnityTexture2D _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                    float4 _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.tex, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.samplerstate, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.GetTransformedUV(IN.uv0.xy));
                    _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0);
                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_R_4 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.r;
                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_G_5 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.g;
                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_B_6 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.b;
                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_A_7 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.a;
                    float _Property_4660fad584cc4501bb4130e8448804fe_Out_0 = Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                    float3 _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                    Unity_NormalStrength_float((_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.xyz), _Property_4660fad584cc4501bb4130e8448804fe_Out_0, _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2);
                    float _Property_5ce20b43b0c240c2b9bd1f8080a107c0_Out_0 = Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                    float _Property_14ce586b6d704c89910791a1a3dac060_Out_0 = Vector1_6ef3f7747728449dae573951cca43c42;
                    surface.BaseColor = (_Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2.xyz);
                    surface.NormalTS = _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                    surface.Emission = float3(0, 0, 0);
                    surface.Metallic = 0;
                    surface.Smoothness = _Property_5ce20b43b0c240c2b9bd1f8080a107c0_Out_0;
                    surface.Occlusion = _Property_14ce586b6d704c89910791a1a3dac060_Out_0;
                    return surface;
                }

                // --------------------------------------------------
                // Build Graph Inputs
                #ifdef HAVE_VFX_MODIFICATION
                #define VFX_SRP_ATTRIBUTES Attributes
                #define VFX_SRP_VARYINGS Varyings
                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                #endif
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                    output.ObjectSpaceNormal = input.normalOS;
                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                    output.ObjectSpacePosition = input.positionOS;

                    return output;
                }
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif





                    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                    output.WorldSpacePosition = input.positionWS;
                    output.uv0 = input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                        return output;
                }

                // --------------------------------------------------
                // Main

                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

                // --------------------------------------------------
                // Visual Effect Vertex Invocations
                #ifdef HAVE_VFX_MODIFICATION
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                #endif

                ENDHLSL
                }
                Pass
                {
                    Name "GBuffer"
                    Tags
                    {
                        "LightMode" = "UniversalGBuffer"
                    }

                    // Render State
                    Cull Back
                    Blend One Zero
                    ZTest Off
                    ZWrite On

                    // Debug
                    // <None>

                    // --------------------------------------------------
                    // Pass

                    HLSLPROGRAM

                    // Pragmas
                    #pragma target 4.5
                    #pragma exclude_renderers gles gles3 glcore
                    #pragma multi_compile_instancing
                    #pragma multi_compile_fog
                    #pragma instancing_options renderinglayer
                    #pragma multi_compile _ DOTS_INSTANCING_ON
                    #pragma vertex vert
                    #pragma fragment frag

                    // DotsInstancingOptions: <None>
                    // HybridV1InjectedBuiltinProperties: <None>

                    // Keywords
                    #pragma multi_compile _ LIGHTMAP_ON
                    #pragma multi_compile _ DYNAMICLIGHTMAP_ON
                    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                    #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
                    #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
                    #pragma multi_compile_fragment _ _SHADOWS_SOFT
                    #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                    #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
                    #pragma multi_compile _ SHADOWS_SHADOWMASK
                    #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                    #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
                    #pragma multi_compile_fragment _ _LIGHT_LAYERS
                    #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
                    #pragma multi_compile_fragment _ DEBUG_DISPLAY
                    // GraphKeywords: <None>

                    // Defines

                    #define _NORMALMAP 1
                    #define _NORMAL_DROPOFF_TS 1
                    #define ATTRIBUTES_NEED_NORMAL
                    #define ATTRIBUTES_NEED_TANGENT
                    #define ATTRIBUTES_NEED_TEXCOORD0
                    #define ATTRIBUTES_NEED_TEXCOORD1
                    #define ATTRIBUTES_NEED_TEXCOORD2
                    #define VARYINGS_NEED_POSITION_WS
                    #define VARYINGS_NEED_NORMAL_WS
                    #define VARYINGS_NEED_TANGENT_WS
                    #define VARYINGS_NEED_TEXCOORD0
                    #define VARYINGS_NEED_VIEWDIRECTION_WS
                    #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                    #define VARYINGS_NEED_SHADOW_COORD
                    #define FEATURES_GRAPH_VERTEX
                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                    #define SHADERPASS SHADERPASS_GBUFFER
                    #define _FOG_FRAGMENT 1
                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                    // custom interpolator pre-include
                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                    // Includes
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                    // --------------------------------------------------
                    // Structs and Packing

                    // custom interpolators pre packing
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                    struct Attributes
                    {
                         float3 positionOS : POSITION;
                         float3 normalOS : NORMAL;
                         float4 tangentOS : TANGENT;
                         float4 uv0 : TEXCOORD0;
                         float4 uv1 : TEXCOORD1;
                         float4 uv2 : TEXCOORD2;
                        #if UNITY_ANY_INSTANCING_ENABLED
                         uint instanceID : INSTANCEID_SEMANTIC;
                        #endif
                    };
                    struct Varyings
                    {
                         float4 positionCS : SV_POSITION;
                         float3 positionWS;
                         float3 normalWS;
                         float4 tangentWS;
                         float4 texCoord0;
                         float3 viewDirectionWS;
                        #if defined(LIGHTMAP_ON)
                         float2 staticLightmapUV;
                        #endif
                        #if defined(DYNAMICLIGHTMAP_ON)
                         float2 dynamicLightmapUV;
                        #endif
                        #if !defined(LIGHTMAP_ON)
                         float3 sh;
                        #endif
                         float4 fogFactorAndVertexLight;
                        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                         float4 shadowCoord;
                        #endif
                        #if UNITY_ANY_INSTANCING_ENABLED
                         uint instanceID : CUSTOM_INSTANCE_ID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                        #endif
                    };
                    struct SurfaceDescriptionInputs
                    {
                         float3 TangentSpaceNormal;
                         float3 WorldSpacePosition;
                         float4 uv0;
                    };
                    struct VertexDescriptionInputs
                    {
                         float3 ObjectSpaceNormal;
                         float3 ObjectSpaceTangent;
                         float3 ObjectSpacePosition;
                    };
                    struct PackedVaryings
                    {
                         float4 positionCS : SV_POSITION;
                         float3 interp0 : INTERP0;
                         float3 interp1 : INTERP1;
                         float4 interp2 : INTERP2;
                         float4 interp3 : INTERP3;
                         float3 interp4 : INTERP4;
                         float2 interp5 : INTERP5;
                         float2 interp6 : INTERP6;
                         float3 interp7 : INTERP7;
                         float4 interp8 : INTERP8;
                         float4 interp9 : INTERP9;
                        #if UNITY_ANY_INSTANCING_ENABLED
                         uint instanceID : CUSTOM_INSTANCE_ID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                        #endif
                    };

                    PackedVaryings PackVaryings(Varyings input)
                    {
                        PackedVaryings output;
                        ZERO_INITIALIZE(PackedVaryings, output);
                        output.positionCS = input.positionCS;
                        output.interp0.xyz = input.positionWS;
                        output.interp1.xyz = input.normalWS;
                        output.interp2.xyzw = input.tangentWS;
                        output.interp3.xyzw = input.texCoord0;
                        output.interp4.xyz = input.viewDirectionWS;
                        #if defined(LIGHTMAP_ON)
                        output.interp5.xy = input.staticLightmapUV;
                        #endif
                        #if defined(DYNAMICLIGHTMAP_ON)
                        output.interp6.xy = input.dynamicLightmapUV;
                        #endif
                        #if !defined(LIGHTMAP_ON)
                        output.interp7.xyz = input.sh;
                        #endif
                        output.interp8.xyzw = input.fogFactorAndVertexLight;
                        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                        output.interp9.xyzw = input.shadowCoord;
                        #endif
                        #if UNITY_ANY_INSTANCING_ENABLED
                        output.instanceID = input.instanceID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        output.cullFace = input.cullFace;
                        #endif
                        return output;
                    }

                    Varyings UnpackVaryings(PackedVaryings input)
                    {
                        Varyings output;
                        output.positionCS = input.positionCS;
                        output.positionWS = input.interp0.xyz;
                        output.normalWS = input.interp1.xyz;
                        output.tangentWS = input.interp2.xyzw;
                        output.texCoord0 = input.interp3.xyzw;
                        output.viewDirectionWS = input.interp4.xyz;
                        #if defined(LIGHTMAP_ON)
                        output.staticLightmapUV = input.interp5.xy;
                        #endif
                        #if defined(DYNAMICLIGHTMAP_ON)
                        output.dynamicLightmapUV = input.interp6.xy;
                        #endif
                        #if !defined(LIGHTMAP_ON)
                        output.sh = input.interp7.xyz;
                        #endif
                        output.fogFactorAndVertexLight = input.interp8.xyzw;
                        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                        output.shadowCoord = input.interp9.xyzw;
                        #endif
                        #if UNITY_ANY_INSTANCING_ENABLED
                        output.instanceID = input.instanceID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        output.cullFace = input.cullFace;
                        #endif
                        return output;
                    }


                    // --------------------------------------------------
                    // Graph

                    // Graph Properties
                    CBUFFER_START(UnityPerMaterial)
                    float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                    float4 Color_adc979e474cd4c52803b73073550b88e;
                    float Vector1_f4346caac0c24533843067b1caa8641a;
                    float Vector1_f4346caac0c24533843067b1caa8641a_1;
                    float Vector1_9ab9734743834cf3842959c683cd7be1;
                    float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                    float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                    float Vector1_6ef3f7747728449dae573951cca43c42;
                    float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                    CBUFFER_END

                        // Object and Global properties
                        SAMPLER(SamplerState_Linear_Repeat);
                        TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                        SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                        // Graph Includes
                        // GraphIncludes: <None>

                        // -- Property used by ScenePickingPass
                        #ifdef SCENEPICKINGPASS
                        float4 _SelectionID;
                        #endif

                        // -- Properties used by SceneSelectionPass
                        #ifdef SCENESELECTIONPASS
                        int _ObjectId;
                        int _PassValue;
                        #endif

                        // Graph Functions

                        void Unity_Subtract_float(float A, float B, out float Out)
                        {
                            Out = A - B;
                        }

                        void Unity_Add_float(float A, float B, out float Out)
                        {
                            Out = A + B;
                        }

                        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                        {
                            Out = smoothstep(Edge1, Edge2, In);
                        }

                        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                        {
                            Out = A * B;
                        }

                        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                        {
                            Out = A + B;
                        }

                        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
                        {
                            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
                        }

                        // Custom interpolators pre vertex
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                        // Graph Vertex
                        struct VertexDescription
                        {
                            float3 Position;
                            float3 Normal;
                            float3 Tangent;
                        };

                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                        {
                            VertexDescription description = (VertexDescription)0;
                            description.Position = IN.ObjectSpacePosition;
                            description.Normal = IN.ObjectSpaceNormal;
                            description.Tangent = IN.ObjectSpaceTangent;
                            return description;
                        }

                        // Custom interpolators, pre surface
                        #ifdef FEATURES_GRAPH_VERTEX
                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                        {
                        return output;
                        }
                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                        #endif

                        // Graph Pixel
                        struct SurfaceDescription
                        {
                            float3 BaseColor;
                            float3 NormalTS;
                            float3 Emission;
                            float Metallic;
                            float Smoothness;
                            float Occlusion;
                        };

                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                        {
                            SurfaceDescription surface = (SurfaceDescription)0;
                            float4 _Property_271638d18c864c50adc2de9ba4cf9707_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_c4b6043ac1de492fb8bbd1482b723534) : Color_c4b6043ac1de492fb8bbd1482b723534;
                            float _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                            float _Property_910d825da2eb46b0a1754b9e69252646_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a;
                            float _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2;
                            Unity_Subtract_float(_Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Property_910d825da2eb46b0a1754b9e69252646_Out_0, _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2);
                            float _Add_bca707404b3243949fe8953b161e1601_Out_2;
                            Unity_Add_float(-1, _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Add_bca707404b3243949fe8953b161e1601_Out_2);
                            float _Split_4c5a12ca469049ee8d25c52df86199d7_R_1 = IN.WorldSpacePosition[0];
                            float _Split_4c5a12ca469049ee8d25c52df86199d7_G_2 = IN.WorldSpacePosition[1];
                            float _Split_4c5a12ca469049ee8d25c52df86199d7_B_3 = IN.WorldSpacePosition[2];
                            float _Split_4c5a12ca469049ee8d25c52df86199d7_A_4 = 0;
                            float _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3;
                            Unity_Smoothstep_float(_Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2, _Add_bca707404b3243949fe8953b161e1601_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3);
                            float4 _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2;
                            Unity_Multiply_float4_float4(_Property_271638d18c864c50adc2de9ba4cf9707_Out_0, (_Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3.xxxx), _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2);
                            float4 _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2;
                            Unity_Multiply_float4_float4(_Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2, float4(2, 2, 2, 2), _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2);
                            float4 _Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_adc979e474cd4c52803b73073550b88e) : Color_adc979e474cd4c52803b73073550b88e;
                            float _Property_429fa9faf52b49c891efaa416fe2c903_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                            float _Property_73479999c2b346edb73436833d483856_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a_1;
                            float _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2;
                            Unity_Subtract_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, _Property_73479999c2b346edb73436833d483856_Out_0, _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2);
                            float _Add_554d292e79d145489ad3cdc7084a70ca_Out_2;
                            Unity_Add_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, -1, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2);
                            float _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3;
                            Unity_Smoothstep_float(_Subtract_72abdf79c34d4aacadfe832896a30461_Out_2, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3);
                            float4 _Multiply_f02e57049b254438874c61113b259afa_Out_2;
                            Unity_Multiply_float4_float4(_Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0, (_Smoothstep_919b4e994d44484590da2d34a073773c_Out_3.xxxx), _Multiply_f02e57049b254438874c61113b259afa_Out_2);
                            float4 _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2;
                            Unity_Add_float4(_Multiply_9d41234165c1425c8005aa668f597e6c_Out_2, _Multiply_f02e57049b254438874c61113b259afa_Out_2, _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2);
                            UnityTexture2D _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                            float4 _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.tex, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.samplerstate, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.GetTransformedUV(IN.uv0.xy));
                            _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0);
                            float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_R_4 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.r;
                            float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_G_5 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.g;
                            float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_B_6 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.b;
                            float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_A_7 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.a;
                            float _Property_4660fad584cc4501bb4130e8448804fe_Out_0 = Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                            float3 _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                            Unity_NormalStrength_float((_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.xyz), _Property_4660fad584cc4501bb4130e8448804fe_Out_0, _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2);
                            float _Property_5ce20b43b0c240c2b9bd1f8080a107c0_Out_0 = Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                            float _Property_14ce586b6d704c89910791a1a3dac060_Out_0 = Vector1_6ef3f7747728449dae573951cca43c42;
                            surface.BaseColor = (_Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2.xyz);
                            surface.NormalTS = _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                            surface.Emission = float3(0, 0, 0);
                            surface.Metallic = 0;
                            surface.Smoothness = _Property_5ce20b43b0c240c2b9bd1f8080a107c0_Out_0;
                            surface.Occlusion = _Property_14ce586b6d704c89910791a1a3dac060_Out_0;
                            return surface;
                        }

                        // --------------------------------------------------
                        // Build Graph Inputs
                        #ifdef HAVE_VFX_MODIFICATION
                        #define VFX_SRP_ATTRIBUTES Attributes
                        #define VFX_SRP_VARYINGS Varyings
                        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                        #endif
                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                        {
                            VertexDescriptionInputs output;
                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                            output.ObjectSpaceNormal = input.normalOS;
                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                            output.ObjectSpacePosition = input.positionOS;

                            return output;
                        }
                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                        {
                            SurfaceDescriptionInputs output;
                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                        #ifdef HAVE_VFX_MODIFICATION
                            // FragInputs from VFX come from two places: Interpolator or CBuffer.
                            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                        #endif





                            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                            output.WorldSpacePosition = input.positionWS;
                            output.uv0 = input.texCoord0;
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                        #else
                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                        #endif
                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                return output;
                        }

                        // --------------------------------------------------
                        // Main

                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"

                        // --------------------------------------------------
                        // Visual Effect Vertex Invocations
                        #ifdef HAVE_VFX_MODIFICATION
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                        #endif

                        ENDHLSL
                        }
                        Pass
                        {
                            Name "ShadowCaster"
                            Tags
                            {
                                "LightMode" = "ShadowCaster"
                            }

                            // Render State
                            Cull Back
                            ZTest Off
                            ZWrite On
                            ColorMask 0

                            // Debug
                            // <None>

                            // --------------------------------------------------
                            // Pass

                            HLSLPROGRAM

                            // Pragmas
                            #pragma target 4.5
                            #pragma exclude_renderers gles gles3 glcore
                            #pragma multi_compile_instancing
                            #pragma multi_compile _ DOTS_INSTANCING_ON
                            #pragma vertex vert
                            #pragma fragment frag

                            // DotsInstancingOptions: <None>
                            // HybridV1InjectedBuiltinProperties: <None>

                            // Keywords
                            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
                            // GraphKeywords: <None>

                            // Defines

                            #define _NORMALMAP 1
                            #define _NORMAL_DROPOFF_TS 1
                            #define ATTRIBUTES_NEED_NORMAL
                            #define ATTRIBUTES_NEED_TANGENT
                            #define VARYINGS_NEED_NORMAL_WS
                            #define FEATURES_GRAPH_VERTEX
                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                            #define SHADERPASS SHADERPASS_SHADOWCASTER
                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                            // custom interpolator pre-include
                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                            // Includes
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                            // --------------------------------------------------
                            // Structs and Packing

                            // custom interpolators pre packing
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                            struct Attributes
                            {
                                 float3 positionOS : POSITION;
                                 float3 normalOS : NORMAL;
                                 float4 tangentOS : TANGENT;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                 uint instanceID : INSTANCEID_SEMANTIC;
                                #endif
                            };
                            struct Varyings
                            {
                                 float4 positionCS : SV_POSITION;
                                 float3 normalWS;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                #endif
                            };
                            struct SurfaceDescriptionInputs
                            {
                            };
                            struct VertexDescriptionInputs
                            {
                                 float3 ObjectSpaceNormal;
                                 float3 ObjectSpaceTangent;
                                 float3 ObjectSpacePosition;
                            };
                            struct PackedVaryings
                            {
                                 float4 positionCS : SV_POSITION;
                                 float3 interp0 : INTERP0;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                #endif
                            };

                            PackedVaryings PackVaryings(Varyings input)
                            {
                                PackedVaryings output;
                                ZERO_INITIALIZE(PackedVaryings, output);
                                output.positionCS = input.positionCS;
                                output.interp0.xyz = input.normalWS;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                output.instanceID = input.instanceID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                output.cullFace = input.cullFace;
                                #endif
                                return output;
                            }

                            Varyings UnpackVaryings(PackedVaryings input)
                            {
                                Varyings output;
                                output.positionCS = input.positionCS;
                                output.normalWS = input.interp0.xyz;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                output.instanceID = input.instanceID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                output.cullFace = input.cullFace;
                                #endif
                                return output;
                            }


                            // --------------------------------------------------
                            // Graph

                            // Graph Properties
                            CBUFFER_START(UnityPerMaterial)
                            float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                            float4 Color_adc979e474cd4c52803b73073550b88e;
                            float Vector1_f4346caac0c24533843067b1caa8641a;
                            float Vector1_f4346caac0c24533843067b1caa8641a_1;
                            float Vector1_9ab9734743834cf3842959c683cd7be1;
                            float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                            float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                            float Vector1_6ef3f7747728449dae573951cca43c42;
                            float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                            CBUFFER_END

                                // Object and Global properties
                                SAMPLER(SamplerState_Linear_Repeat);
                                TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                // Graph Includes
                                // GraphIncludes: <None>

                                // -- Property used by ScenePickingPass
                                #ifdef SCENEPICKINGPASS
                                float4 _SelectionID;
                                #endif

                                // -- Properties used by SceneSelectionPass
                                #ifdef SCENESELECTIONPASS
                                int _ObjectId;
                                int _PassValue;
                                #endif

                                // Graph Functions
                                // GraphFunctions: <None>

                                // Custom interpolators pre vertex
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                // Graph Vertex
                                struct VertexDescription
                                {
                                    float3 Position;
                                    float3 Normal;
                                    float3 Tangent;
                                };

                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                {
                                    VertexDescription description = (VertexDescription)0;
                                    description.Position = IN.ObjectSpacePosition;
                                    description.Normal = IN.ObjectSpaceNormal;
                                    description.Tangent = IN.ObjectSpaceTangent;
                                    return description;
                                }

                                // Custom interpolators, pre surface
                                #ifdef FEATURES_GRAPH_VERTEX
                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                {
                                return output;
                                }
                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                #endif

                                // Graph Pixel
                                struct SurfaceDescription
                                {
                                };

                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                {
                                    SurfaceDescription surface = (SurfaceDescription)0;
                                    return surface;
                                }

                                // --------------------------------------------------
                                // Build Graph Inputs
                                #ifdef HAVE_VFX_MODIFICATION
                                #define VFX_SRP_ATTRIBUTES Attributes
                                #define VFX_SRP_VARYINGS Varyings
                                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                #endif
                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                {
                                    VertexDescriptionInputs output;
                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                    output.ObjectSpaceNormal = input.normalOS;
                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                    output.ObjectSpacePosition = input.positionOS;

                                    return output;
                                }
                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                {
                                    SurfaceDescriptionInputs output;
                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                #ifdef HAVE_VFX_MODIFICATION
                                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                #endif







                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                #else
                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                #endif
                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                        return output;
                                }

                                // --------------------------------------------------
                                // Main

                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

                                // --------------------------------------------------
                                // Visual Effect Vertex Invocations
                                #ifdef HAVE_VFX_MODIFICATION
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                #endif

                                ENDHLSL
                                }
                                Pass
                                {
                                    Name "DepthOnly"
                                    Tags
                                    {
                                        "LightMode" = "DepthOnly"
                                    }

                                    // Render State
                                    Cull Back
                                    ZTest Off
                                    ZWrite On
                                    ColorMask 0

                                    // Debug
                                    // <None>

                                    // --------------------------------------------------
                                    // Pass

                                    HLSLPROGRAM

                                    // Pragmas
                                    #pragma target 4.5
                                    #pragma exclude_renderers gles gles3 glcore
                                    #pragma multi_compile_instancing
                                    #pragma multi_compile _ DOTS_INSTANCING_ON
                                    #pragma vertex vert
                                    #pragma fragment frag

                                    // DotsInstancingOptions: <None>
                                    // HybridV1InjectedBuiltinProperties: <None>

                                    // Keywords
                                    // PassKeywords: <None>
                                    // GraphKeywords: <None>

                                    // Defines

                                    #define _NORMALMAP 1
                                    #define _NORMAL_DROPOFF_TS 1
                                    #define ATTRIBUTES_NEED_NORMAL
                                    #define ATTRIBUTES_NEED_TANGENT
                                    #define FEATURES_GRAPH_VERTEX
                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                    #define SHADERPASS SHADERPASS_DEPTHONLY
                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                    // custom interpolator pre-include
                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                    // Includes
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                    // --------------------------------------------------
                                    // Structs and Packing

                                    // custom interpolators pre packing
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                    struct Attributes
                                    {
                                         float3 positionOS : POSITION;
                                         float3 normalOS : NORMAL;
                                         float4 tangentOS : TANGENT;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                         uint instanceID : INSTANCEID_SEMANTIC;
                                        #endif
                                    };
                                    struct Varyings
                                    {
                                         float4 positionCS : SV_POSITION;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                        #endif
                                    };
                                    struct SurfaceDescriptionInputs
                                    {
                                    };
                                    struct VertexDescriptionInputs
                                    {
                                         float3 ObjectSpaceNormal;
                                         float3 ObjectSpaceTangent;
                                         float3 ObjectSpacePosition;
                                    };
                                    struct PackedVaryings
                                    {
                                         float4 positionCS : SV_POSITION;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                        #endif
                                    };

                                    PackedVaryings PackVaryings(Varyings input)
                                    {
                                        PackedVaryings output;
                                        ZERO_INITIALIZE(PackedVaryings, output);
                                        output.positionCS = input.positionCS;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        output.cullFace = input.cullFace;
                                        #endif
                                        return output;
                                    }

                                    Varyings UnpackVaryings(PackedVaryings input)
                                    {
                                        Varyings output;
                                        output.positionCS = input.positionCS;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        output.cullFace = input.cullFace;
                                        #endif
                                        return output;
                                    }


                                    // --------------------------------------------------
                                    // Graph

                                    // Graph Properties
                                    CBUFFER_START(UnityPerMaterial)
                                    float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                    float4 Color_adc979e474cd4c52803b73073550b88e;
                                    float Vector1_f4346caac0c24533843067b1caa8641a;
                                    float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                    float Vector1_9ab9734743834cf3842959c683cd7be1;
                                    float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                    float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                    float Vector1_6ef3f7747728449dae573951cca43c42;
                                    float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                    CBUFFER_END

                                        // Object and Global properties
                                        SAMPLER(SamplerState_Linear_Repeat);
                                        TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                        SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                        // Graph Includes
                                        // GraphIncludes: <None>

                                        // -- Property used by ScenePickingPass
                                        #ifdef SCENEPICKINGPASS
                                        float4 _SelectionID;
                                        #endif

                                        // -- Properties used by SceneSelectionPass
                                        #ifdef SCENESELECTIONPASS
                                        int _ObjectId;
                                        int _PassValue;
                                        #endif

                                        // Graph Functions
                                        // GraphFunctions: <None>

                                        // Custom interpolators pre vertex
                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                        // Graph Vertex
                                        struct VertexDescription
                                        {
                                            float3 Position;
                                            float3 Normal;
                                            float3 Tangent;
                                        };

                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                        {
                                            VertexDescription description = (VertexDescription)0;
                                            description.Position = IN.ObjectSpacePosition;
                                            description.Normal = IN.ObjectSpaceNormal;
                                            description.Tangent = IN.ObjectSpaceTangent;
                                            return description;
                                        }

                                        // Custom interpolators, pre surface
                                        #ifdef FEATURES_GRAPH_VERTEX
                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                        {
                                        return output;
                                        }
                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                        #endif

                                        // Graph Pixel
                                        struct SurfaceDescription
                                        {
                                        };

                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                        {
                                            SurfaceDescription surface = (SurfaceDescription)0;
                                            return surface;
                                        }

                                        // --------------------------------------------------
                                        // Build Graph Inputs
                                        #ifdef HAVE_VFX_MODIFICATION
                                        #define VFX_SRP_ATTRIBUTES Attributes
                                        #define VFX_SRP_VARYINGS Varyings
                                        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                        #endif
                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                        {
                                            VertexDescriptionInputs output;
                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                            output.ObjectSpaceNormal = input.normalOS;
                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                            output.ObjectSpacePosition = input.positionOS;

                                            return output;
                                        }
                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                        {
                                            SurfaceDescriptionInputs output;
                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                        #ifdef HAVE_VFX_MODIFICATION
                                            // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                        #endif







                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                        #else
                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                        #endif
                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                return output;
                                        }

                                        // --------------------------------------------------
                                        // Main

                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

                                        // --------------------------------------------------
                                        // Visual Effect Vertex Invocations
                                        #ifdef HAVE_VFX_MODIFICATION
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                        #endif

                                        ENDHLSL
                                        }
                                        Pass
                                        {
                                            Name "DepthNormals"
                                            Tags
                                            {
                                                "LightMode" = "DepthNormals"
                                            }

                                            // Render State
                                            Cull Back
                                            ZTest Off
                                            ZWrite On

                                            // Debug
                                            // <None>

                                            // --------------------------------------------------
                                            // Pass

                                            HLSLPROGRAM

                                            // Pragmas
                                            #pragma target 4.5
                                            #pragma exclude_renderers gles gles3 glcore
                                            #pragma multi_compile_instancing
                                            #pragma multi_compile _ DOTS_INSTANCING_ON
                                            #pragma vertex vert
                                            #pragma fragment frag

                                            // DotsInstancingOptions: <None>
                                            // HybridV1InjectedBuiltinProperties: <None>

                                            // Keywords
                                            // PassKeywords: <None>
                                            // GraphKeywords: <None>

                                            // Defines

                                            #define _NORMALMAP 1
                                            #define _NORMAL_DROPOFF_TS 1
                                            #define ATTRIBUTES_NEED_NORMAL
                                            #define ATTRIBUTES_NEED_TANGENT
                                            #define ATTRIBUTES_NEED_TEXCOORD0
                                            #define ATTRIBUTES_NEED_TEXCOORD1
                                            #define VARYINGS_NEED_NORMAL_WS
                                            #define VARYINGS_NEED_TANGENT_WS
                                            #define VARYINGS_NEED_TEXCOORD0
                                            #define FEATURES_GRAPH_VERTEX
                                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                            #define SHADERPASS SHADERPASS_DEPTHNORMALS
                                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                            // custom interpolator pre-include
                                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                            // Includes
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                            // --------------------------------------------------
                                            // Structs and Packing

                                            // custom interpolators pre packing
                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                            struct Attributes
                                            {
                                                 float3 positionOS : POSITION;
                                                 float3 normalOS : NORMAL;
                                                 float4 tangentOS : TANGENT;
                                                 float4 uv0 : TEXCOORD0;
                                                 float4 uv1 : TEXCOORD1;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                 uint instanceID : INSTANCEID_SEMANTIC;
                                                #endif
                                            };
                                            struct Varyings
                                            {
                                                 float4 positionCS : SV_POSITION;
                                                 float3 normalWS;
                                                 float4 tangentWS;
                                                 float4 texCoord0;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                #endif
                                            };
                                            struct SurfaceDescriptionInputs
                                            {
                                                 float3 TangentSpaceNormal;
                                                 float4 uv0;
                                            };
                                            struct VertexDescriptionInputs
                                            {
                                                 float3 ObjectSpaceNormal;
                                                 float3 ObjectSpaceTangent;
                                                 float3 ObjectSpacePosition;
                                            };
                                            struct PackedVaryings
                                            {
                                                 float4 positionCS : SV_POSITION;
                                                 float3 interp0 : INTERP0;
                                                 float4 interp1 : INTERP1;
                                                 float4 interp2 : INTERP2;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                #endif
                                            };

                                            PackedVaryings PackVaryings(Varyings input)
                                            {
                                                PackedVaryings output;
                                                ZERO_INITIALIZE(PackedVaryings, output);
                                                output.positionCS = input.positionCS;
                                                output.interp0.xyz = input.normalWS;
                                                output.interp1.xyzw = input.tangentWS;
                                                output.interp2.xyzw = input.texCoord0;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                output.instanceID = input.instanceID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                output.cullFace = input.cullFace;
                                                #endif
                                                return output;
                                            }

                                            Varyings UnpackVaryings(PackedVaryings input)
                                            {
                                                Varyings output;
                                                output.positionCS = input.positionCS;
                                                output.normalWS = input.interp0.xyz;
                                                output.tangentWS = input.interp1.xyzw;
                                                output.texCoord0 = input.interp2.xyzw;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                output.instanceID = input.instanceID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                output.cullFace = input.cullFace;
                                                #endif
                                                return output;
                                            }


                                            // --------------------------------------------------
                                            // Graph

                                            // Graph Properties
                                            CBUFFER_START(UnityPerMaterial)
                                            float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                            float4 Color_adc979e474cd4c52803b73073550b88e;
                                            float Vector1_f4346caac0c24533843067b1caa8641a;
                                            float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                            float Vector1_9ab9734743834cf3842959c683cd7be1;
                                            float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                            float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                            float Vector1_6ef3f7747728449dae573951cca43c42;
                                            float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                            CBUFFER_END

                                                // Object and Global properties
                                                SAMPLER(SamplerState_Linear_Repeat);
                                                TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                // Graph Includes
                                                // GraphIncludes: <None>

                                                // -- Property used by ScenePickingPass
                                                #ifdef SCENEPICKINGPASS
                                                float4 _SelectionID;
                                                #endif

                                                // -- Properties used by SceneSelectionPass
                                                #ifdef SCENESELECTIONPASS
                                                int _ObjectId;
                                                int _PassValue;
                                                #endif

                                                // Graph Functions

                                                void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
                                                {
                                                    Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
                                                }

                                                // Custom interpolators pre vertex
                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                // Graph Vertex
                                                struct VertexDescription
                                                {
                                                    float3 Position;
                                                    float3 Normal;
                                                    float3 Tangent;
                                                };

                                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                {
                                                    VertexDescription description = (VertexDescription)0;
                                                    description.Position = IN.ObjectSpacePosition;
                                                    description.Normal = IN.ObjectSpaceNormal;
                                                    description.Tangent = IN.ObjectSpaceTangent;
                                                    return description;
                                                }

                                                // Custom interpolators, pre surface
                                                #ifdef FEATURES_GRAPH_VERTEX
                                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                {
                                                return output;
                                                }
                                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                #endif

                                                // Graph Pixel
                                                struct SurfaceDescription
                                                {
                                                    float3 NormalTS;
                                                };

                                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                {
                                                    SurfaceDescription surface = (SurfaceDescription)0;
                                                    UnityTexture2D _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                    float4 _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.tex, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.samplerstate, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.GetTransformedUV(IN.uv0.xy));
                                                    _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0);
                                                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_R_4 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.r;
                                                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_G_5 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.g;
                                                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_B_6 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.b;
                                                    float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_A_7 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.a;
                                                    float _Property_4660fad584cc4501bb4130e8448804fe_Out_0 = Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                    float3 _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                                                    Unity_NormalStrength_float((_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.xyz), _Property_4660fad584cc4501bb4130e8448804fe_Out_0, _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2);
                                                    surface.NormalTS = _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                                                    return surface;
                                                }

                                                // --------------------------------------------------
                                                // Build Graph Inputs
                                                #ifdef HAVE_VFX_MODIFICATION
                                                #define VFX_SRP_ATTRIBUTES Attributes
                                                #define VFX_SRP_VARYINGS Varyings
                                                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                #endif
                                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                {
                                                    VertexDescriptionInputs output;
                                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                    output.ObjectSpaceNormal = input.normalOS;
                                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                    output.ObjectSpacePosition = input.positionOS;

                                                    return output;
                                                }
                                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                {
                                                    SurfaceDescriptionInputs output;
                                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                #ifdef HAVE_VFX_MODIFICATION
                                                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                #endif





                                                    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                                                    output.uv0 = input.texCoord0;
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                #else
                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                #endif
                                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                        return output;
                                                }

                                                // --------------------------------------------------
                                                // Main

                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                                                // --------------------------------------------------
                                                // Visual Effect Vertex Invocations
                                                #ifdef HAVE_VFX_MODIFICATION
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                #endif

                                                ENDHLSL
                                                }
                                                Pass
                                                {
                                                    Name "Meta"
                                                    Tags
                                                    {
                                                        "LightMode" = "Meta"
                                                    }

                                                    // Render State
                                                    Cull Off

                                                    // Debug
                                                    // <None>

                                                    // --------------------------------------------------
                                                    // Pass

                                                    HLSLPROGRAM

                                                    // Pragmas
                                                    #pragma target 4.5
                                                    #pragma exclude_renderers gles gles3 glcore
                                                    #pragma vertex vert
                                                    #pragma fragment frag

                                                    // DotsInstancingOptions: <None>
                                                    // HybridV1InjectedBuiltinProperties: <None>

                                                    // Keywords
                                                    #pragma shader_feature _ EDITOR_VISUALIZATION
                                                    // GraphKeywords: <None>

                                                    // Defines

                                                    #define _NORMALMAP 1
                                                    #define _NORMAL_DROPOFF_TS 1
                                                    #define ATTRIBUTES_NEED_NORMAL
                                                    #define ATTRIBUTES_NEED_TANGENT
                                                    #define ATTRIBUTES_NEED_TEXCOORD0
                                                    #define ATTRIBUTES_NEED_TEXCOORD1
                                                    #define ATTRIBUTES_NEED_TEXCOORD2
                                                    #define VARYINGS_NEED_POSITION_WS
                                                    #define VARYINGS_NEED_TEXCOORD0
                                                    #define VARYINGS_NEED_TEXCOORD1
                                                    #define VARYINGS_NEED_TEXCOORD2
                                                    #define FEATURES_GRAPH_VERTEX
                                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                    #define SHADERPASS SHADERPASS_META
                                                    #define _FOG_FRAGMENT 1
                                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                    // custom interpolator pre-include
                                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                    // Includes
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                    // --------------------------------------------------
                                                    // Structs and Packing

                                                    // custom interpolators pre packing
                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                    struct Attributes
                                                    {
                                                         float3 positionOS : POSITION;
                                                         float3 normalOS : NORMAL;
                                                         float4 tangentOS : TANGENT;
                                                         float4 uv0 : TEXCOORD0;
                                                         float4 uv1 : TEXCOORD1;
                                                         float4 uv2 : TEXCOORD2;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                         uint instanceID : INSTANCEID_SEMANTIC;
                                                        #endif
                                                    };
                                                    struct Varyings
                                                    {
                                                         float4 positionCS : SV_POSITION;
                                                         float3 positionWS;
                                                         float4 texCoord0;
                                                         float4 texCoord1;
                                                         float4 texCoord2;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                        #endif
                                                    };
                                                    struct SurfaceDescriptionInputs
                                                    {
                                                         float3 WorldSpacePosition;
                                                    };
                                                    struct VertexDescriptionInputs
                                                    {
                                                         float3 ObjectSpaceNormal;
                                                         float3 ObjectSpaceTangent;
                                                         float3 ObjectSpacePosition;
                                                    };
                                                    struct PackedVaryings
                                                    {
                                                         float4 positionCS : SV_POSITION;
                                                         float3 interp0 : INTERP0;
                                                         float4 interp1 : INTERP1;
                                                         float4 interp2 : INTERP2;
                                                         float4 interp3 : INTERP3;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                        #endif
                                                    };

                                                    PackedVaryings PackVaryings(Varyings input)
                                                    {
                                                        PackedVaryings output;
                                                        ZERO_INITIALIZE(PackedVaryings, output);
                                                        output.positionCS = input.positionCS;
                                                        output.interp0.xyz = input.positionWS;
                                                        output.interp1.xyzw = input.texCoord0;
                                                        output.interp2.xyzw = input.texCoord1;
                                                        output.interp3.xyzw = input.texCoord2;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                        output.instanceID = input.instanceID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                        output.cullFace = input.cullFace;
                                                        #endif
                                                        return output;
                                                    }

                                                    Varyings UnpackVaryings(PackedVaryings input)
                                                    {
                                                        Varyings output;
                                                        output.positionCS = input.positionCS;
                                                        output.positionWS = input.interp0.xyz;
                                                        output.texCoord0 = input.interp1.xyzw;
                                                        output.texCoord1 = input.interp2.xyzw;
                                                        output.texCoord2 = input.interp3.xyzw;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                        output.instanceID = input.instanceID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                        output.cullFace = input.cullFace;
                                                        #endif
                                                        return output;
                                                    }


                                                    // --------------------------------------------------
                                                    // Graph

                                                    // Graph Properties
                                                    CBUFFER_START(UnityPerMaterial)
                                                    float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                    float4 Color_adc979e474cd4c52803b73073550b88e;
                                                    float Vector1_f4346caac0c24533843067b1caa8641a;
                                                    float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                    float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                    float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                    float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                    float Vector1_6ef3f7747728449dae573951cca43c42;
                                                    float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                    CBUFFER_END

                                                        // Object and Global properties
                                                        SAMPLER(SamplerState_Linear_Repeat);
                                                        TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                        SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                        // Graph Includes
                                                        // GraphIncludes: <None>

                                                        // -- Property used by ScenePickingPass
                                                        #ifdef SCENEPICKINGPASS
                                                        float4 _SelectionID;
                                                        #endif

                                                        // -- Properties used by SceneSelectionPass
                                                        #ifdef SCENESELECTIONPASS
                                                        int _ObjectId;
                                                        int _PassValue;
                                                        #endif

                                                        // Graph Functions

                                                        void Unity_Subtract_float(float A, float B, out float Out)
                                                        {
                                                            Out = A - B;
                                                        }

                                                        void Unity_Add_float(float A, float B, out float Out)
                                                        {
                                                            Out = A + B;
                                                        }

                                                        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                                                        {
                                                            Out = smoothstep(Edge1, Edge2, In);
                                                        }

                                                        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                        {
                                                            Out = A * B;
                                                        }

                                                        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                                                        {
                                                            Out = A + B;
                                                        }

                                                        // Custom interpolators pre vertex
                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                        // Graph Vertex
                                                        struct VertexDescription
                                                        {
                                                            float3 Position;
                                                            float3 Normal;
                                                            float3 Tangent;
                                                        };

                                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                        {
                                                            VertexDescription description = (VertexDescription)0;
                                                            description.Position = IN.ObjectSpacePosition;
                                                            description.Normal = IN.ObjectSpaceNormal;
                                                            description.Tangent = IN.ObjectSpaceTangent;
                                                            return description;
                                                        }

                                                        // Custom interpolators, pre surface
                                                        #ifdef FEATURES_GRAPH_VERTEX
                                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                        {
                                                        return output;
                                                        }
                                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                        #endif

                                                        // Graph Pixel
                                                        struct SurfaceDescription
                                                        {
                                                            float3 BaseColor;
                                                            float3 Emission;
                                                        };

                                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                        {
                                                            SurfaceDescription surface = (SurfaceDescription)0;
                                                            float4 _Property_271638d18c864c50adc2de9ba4cf9707_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_c4b6043ac1de492fb8bbd1482b723534) : Color_c4b6043ac1de492fb8bbd1482b723534;
                                                            float _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                            float _Property_910d825da2eb46b0a1754b9e69252646_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a;
                                                            float _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2;
                                                            Unity_Subtract_float(_Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Property_910d825da2eb46b0a1754b9e69252646_Out_0, _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2);
                                                            float _Add_bca707404b3243949fe8953b161e1601_Out_2;
                                                            Unity_Add_float(-1, _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Add_bca707404b3243949fe8953b161e1601_Out_2);
                                                            float _Split_4c5a12ca469049ee8d25c52df86199d7_R_1 = IN.WorldSpacePosition[0];
                                                            float _Split_4c5a12ca469049ee8d25c52df86199d7_G_2 = IN.WorldSpacePosition[1];
                                                            float _Split_4c5a12ca469049ee8d25c52df86199d7_B_3 = IN.WorldSpacePosition[2];
                                                            float _Split_4c5a12ca469049ee8d25c52df86199d7_A_4 = 0;
                                                            float _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3;
                                                            Unity_Smoothstep_float(_Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2, _Add_bca707404b3243949fe8953b161e1601_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3);
                                                            float4 _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2;
                                                            Unity_Multiply_float4_float4(_Property_271638d18c864c50adc2de9ba4cf9707_Out_0, (_Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3.xxxx), _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2);
                                                            float4 _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2;
                                                            Unity_Multiply_float4_float4(_Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2, float4(2, 2, 2, 2), _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2);
                                                            float4 _Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_adc979e474cd4c52803b73073550b88e) : Color_adc979e474cd4c52803b73073550b88e;
                                                            float _Property_429fa9faf52b49c891efaa416fe2c903_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                            float _Property_73479999c2b346edb73436833d483856_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                            float _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2;
                                                            Unity_Subtract_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, _Property_73479999c2b346edb73436833d483856_Out_0, _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2);
                                                            float _Add_554d292e79d145489ad3cdc7084a70ca_Out_2;
                                                            Unity_Add_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, -1, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2);
                                                            float _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3;
                                                            Unity_Smoothstep_float(_Subtract_72abdf79c34d4aacadfe832896a30461_Out_2, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3);
                                                            float4 _Multiply_f02e57049b254438874c61113b259afa_Out_2;
                                                            Unity_Multiply_float4_float4(_Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0, (_Smoothstep_919b4e994d44484590da2d34a073773c_Out_3.xxxx), _Multiply_f02e57049b254438874c61113b259afa_Out_2);
                                                            float4 _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2;
                                                            Unity_Add_float4(_Multiply_9d41234165c1425c8005aa668f597e6c_Out_2, _Multiply_f02e57049b254438874c61113b259afa_Out_2, _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2);
                                                            surface.BaseColor = (_Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2.xyz);
                                                            surface.Emission = float3(0, 0, 0);
                                                            return surface;
                                                        }

                                                        // --------------------------------------------------
                                                        // Build Graph Inputs
                                                        #ifdef HAVE_VFX_MODIFICATION
                                                        #define VFX_SRP_ATTRIBUTES Attributes
                                                        #define VFX_SRP_VARYINGS Varyings
                                                        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                        #endif
                                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                        {
                                                            VertexDescriptionInputs output;
                                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                            output.ObjectSpaceNormal = input.normalOS;
                                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                            output.ObjectSpacePosition = input.positionOS;

                                                            return output;
                                                        }
                                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                        {
                                                            SurfaceDescriptionInputs output;
                                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                        #ifdef HAVE_VFX_MODIFICATION
                                                            // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                        #endif







                                                            output.WorldSpacePosition = input.positionWS;
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                        #else
                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                        #endif
                                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                return output;
                                                        }

                                                        // --------------------------------------------------
                                                        // Main

                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

                                                        // --------------------------------------------------
                                                        // Visual Effect Vertex Invocations
                                                        #ifdef HAVE_VFX_MODIFICATION
                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                        #endif

                                                        ENDHLSL
                                                        }
                                                        Pass
                                                        {
                                                            Name "SceneSelectionPass"
                                                            Tags
                                                            {
                                                                "LightMode" = "SceneSelectionPass"
                                                            }

                                                            // Render State
                                                            Cull Off

                                                            // Debug
                                                            // <None>

                                                            // --------------------------------------------------
                                                            // Pass

                                                            HLSLPROGRAM

                                                            // Pragmas
                                                            #pragma target 4.5
                                                            #pragma exclude_renderers gles gles3 glcore
                                                            #pragma vertex vert
                                                            #pragma fragment frag

                                                            // DotsInstancingOptions: <None>
                                                            // HybridV1InjectedBuiltinProperties: <None>

                                                            // Keywords
                                                            // PassKeywords: <None>
                                                            // GraphKeywords: <None>

                                                            // Defines

                                                            #define _NORMALMAP 1
                                                            #define _NORMAL_DROPOFF_TS 1
                                                            #define ATTRIBUTES_NEED_NORMAL
                                                            #define ATTRIBUTES_NEED_TANGENT
                                                            #define FEATURES_GRAPH_VERTEX
                                                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                            #define SHADERPASS SHADERPASS_DEPTHONLY
                                                            #define SCENESELECTIONPASS 1
                                                            #define ALPHA_CLIP_THRESHOLD 1
                                                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                            // custom interpolator pre-include
                                                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                            // Includes
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                            // --------------------------------------------------
                                                            // Structs and Packing

                                                            // custom interpolators pre packing
                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                            struct Attributes
                                                            {
                                                                 float3 positionOS : POSITION;
                                                                 float3 normalOS : NORMAL;
                                                                 float4 tangentOS : TANGENT;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                 uint instanceID : INSTANCEID_SEMANTIC;
                                                                #endif
                                                            };
                                                            struct Varyings
                                                            {
                                                                 float4 positionCS : SV_POSITION;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                #endif
                                                            };
                                                            struct SurfaceDescriptionInputs
                                                            {
                                                            };
                                                            struct VertexDescriptionInputs
                                                            {
                                                                 float3 ObjectSpaceNormal;
                                                                 float3 ObjectSpaceTangent;
                                                                 float3 ObjectSpacePosition;
                                                            };
                                                            struct PackedVaryings
                                                            {
                                                                 float4 positionCS : SV_POSITION;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                #endif
                                                            };

                                                            PackedVaryings PackVaryings(Varyings input)
                                                            {
                                                                PackedVaryings output;
                                                                ZERO_INITIALIZE(PackedVaryings, output);
                                                                output.positionCS = input.positionCS;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                output.instanceID = input.instanceID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                output.cullFace = input.cullFace;
                                                                #endif
                                                                return output;
                                                            }

                                                            Varyings UnpackVaryings(PackedVaryings input)
                                                            {
                                                                Varyings output;
                                                                output.positionCS = input.positionCS;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                output.instanceID = input.instanceID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                output.cullFace = input.cullFace;
                                                                #endif
                                                                return output;
                                                            }


                                                            // --------------------------------------------------
                                                            // Graph

                                                            // Graph Properties
                                                            CBUFFER_START(UnityPerMaterial)
                                                            float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                            float4 Color_adc979e474cd4c52803b73073550b88e;
                                                            float Vector1_f4346caac0c24533843067b1caa8641a;
                                                            float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                            float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                            float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                            float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                            float Vector1_6ef3f7747728449dae573951cca43c42;
                                                            float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                            CBUFFER_END

                                                                // Object and Global properties
                                                                SAMPLER(SamplerState_Linear_Repeat);
                                                                TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                // Graph Includes
                                                                // GraphIncludes: <None>

                                                                // -- Property used by ScenePickingPass
                                                                #ifdef SCENEPICKINGPASS
                                                                float4 _SelectionID;
                                                                #endif

                                                                // -- Properties used by SceneSelectionPass
                                                                #ifdef SCENESELECTIONPASS
                                                                int _ObjectId;
                                                                int _PassValue;
                                                                #endif

                                                                // Graph Functions
                                                                // GraphFunctions: <None>

                                                                // Custom interpolators pre vertex
                                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                // Graph Vertex
                                                                struct VertexDescription
                                                                {
                                                                    float3 Position;
                                                                    float3 Normal;
                                                                    float3 Tangent;
                                                                };

                                                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                {
                                                                    VertexDescription description = (VertexDescription)0;
                                                                    description.Position = IN.ObjectSpacePosition;
                                                                    description.Normal = IN.ObjectSpaceNormal;
                                                                    description.Tangent = IN.ObjectSpaceTangent;
                                                                    return description;
                                                                }

                                                                // Custom interpolators, pre surface
                                                                #ifdef FEATURES_GRAPH_VERTEX
                                                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                {
                                                                return output;
                                                                }
                                                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                #endif

                                                                // Graph Pixel
                                                                struct SurfaceDescription
                                                                {
                                                                };

                                                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                {
                                                                    SurfaceDescription surface = (SurfaceDescription)0;
                                                                    return surface;
                                                                }

                                                                // --------------------------------------------------
                                                                // Build Graph Inputs
                                                                #ifdef HAVE_VFX_MODIFICATION
                                                                #define VFX_SRP_ATTRIBUTES Attributes
                                                                #define VFX_SRP_VARYINGS Varyings
                                                                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                #endif
                                                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                {
                                                                    VertexDescriptionInputs output;
                                                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                    output.ObjectSpaceNormal = input.normalOS;
                                                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                    output.ObjectSpacePosition = input.positionOS;

                                                                    return output;
                                                                }
                                                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                {
                                                                    SurfaceDescriptionInputs output;
                                                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                #ifdef HAVE_VFX_MODIFICATION
                                                                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                #endif







                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                #else
                                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                #endif
                                                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                        return output;
                                                                }

                                                                // --------------------------------------------------
                                                                // Main

                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                                                // --------------------------------------------------
                                                                // Visual Effect Vertex Invocations
                                                                #ifdef HAVE_VFX_MODIFICATION
                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                #endif

                                                                ENDHLSL
                                                                }
                                                                Pass
                                                                {
                                                                    Name "ScenePickingPass"
                                                                    Tags
                                                                    {
                                                                        "LightMode" = "Picking"
                                                                    }

                                                                    // Render State
                                                                    Cull Back

                                                                    // Debug
                                                                    // <None>

                                                                    // --------------------------------------------------
                                                                    // Pass

                                                                    HLSLPROGRAM

                                                                    // Pragmas
                                                                    #pragma target 4.5
                                                                    #pragma exclude_renderers gles gles3 glcore
                                                                    #pragma vertex vert
                                                                    #pragma fragment frag

                                                                    // DotsInstancingOptions: <None>
                                                                    // HybridV1InjectedBuiltinProperties: <None>

                                                                    // Keywords
                                                                    // PassKeywords: <None>
                                                                    // GraphKeywords: <None>

                                                                    // Defines

                                                                    #define _NORMALMAP 1
                                                                    #define _NORMAL_DROPOFF_TS 1
                                                                    #define ATTRIBUTES_NEED_NORMAL
                                                                    #define ATTRIBUTES_NEED_TANGENT
                                                                    #define FEATURES_GRAPH_VERTEX
                                                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                    #define SHADERPASS SHADERPASS_DEPTHONLY
                                                                    #define SCENEPICKINGPASS 1
                                                                    #define ALPHA_CLIP_THRESHOLD 1
                                                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                    // custom interpolator pre-include
                                                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                    // Includes
                                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                    // --------------------------------------------------
                                                                    // Structs and Packing

                                                                    // custom interpolators pre packing
                                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                    struct Attributes
                                                                    {
                                                                         float3 positionOS : POSITION;
                                                                         float3 normalOS : NORMAL;
                                                                         float4 tangentOS : TANGENT;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                         uint instanceID : INSTANCEID_SEMANTIC;
                                                                        #endif
                                                                    };
                                                                    struct Varyings
                                                                    {
                                                                         float4 positionCS : SV_POSITION;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                        #endif
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                        #endif
                                                                    };
                                                                    struct SurfaceDescriptionInputs
                                                                    {
                                                                    };
                                                                    struct VertexDescriptionInputs
                                                                    {
                                                                         float3 ObjectSpaceNormal;
                                                                         float3 ObjectSpaceTangent;
                                                                         float3 ObjectSpacePosition;
                                                                    };
                                                                    struct PackedVaryings
                                                                    {
                                                                         float4 positionCS : SV_POSITION;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                        #endif
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                        #endif
                                                                    };

                                                                    PackedVaryings PackVaryings(Varyings input)
                                                                    {
                                                                        PackedVaryings output;
                                                                        ZERO_INITIALIZE(PackedVaryings, output);
                                                                        output.positionCS = input.positionCS;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        output.instanceID = input.instanceID;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                        #endif
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        output.cullFace = input.cullFace;
                                                                        #endif
                                                                        return output;
                                                                    }

                                                                    Varyings UnpackVaryings(PackedVaryings input)
                                                                    {
                                                                        Varyings output;
                                                                        output.positionCS = input.positionCS;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        output.instanceID = input.instanceID;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                        #endif
                                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                        #endif
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        output.cullFace = input.cullFace;
                                                                        #endif
                                                                        return output;
                                                                    }


                                                                    // --------------------------------------------------
                                                                    // Graph

                                                                    // Graph Properties
                                                                    CBUFFER_START(UnityPerMaterial)
                                                                    float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                    float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                    float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                    float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                    float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                    float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                    float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                    float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                    float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                    CBUFFER_END

                                                                        // Object and Global properties
                                                                        SAMPLER(SamplerState_Linear_Repeat);
                                                                        TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                        SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                        // Graph Includes
                                                                        // GraphIncludes: <None>

                                                                        // -- Property used by ScenePickingPass
                                                                        #ifdef SCENEPICKINGPASS
                                                                        float4 _SelectionID;
                                                                        #endif

                                                                        // -- Properties used by SceneSelectionPass
                                                                        #ifdef SCENESELECTIONPASS
                                                                        int _ObjectId;
                                                                        int _PassValue;
                                                                        #endif

                                                                        // Graph Functions
                                                                        // GraphFunctions: <None>

                                                                        // Custom interpolators pre vertex
                                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                        // Graph Vertex
                                                                        struct VertexDescription
                                                                        {
                                                                            float3 Position;
                                                                            float3 Normal;
                                                                            float3 Tangent;
                                                                        };

                                                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                        {
                                                                            VertexDescription description = (VertexDescription)0;
                                                                            description.Position = IN.ObjectSpacePosition;
                                                                            description.Normal = IN.ObjectSpaceNormal;
                                                                            description.Tangent = IN.ObjectSpaceTangent;
                                                                            return description;
                                                                        }

                                                                        // Custom interpolators, pre surface
                                                                        #ifdef FEATURES_GRAPH_VERTEX
                                                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                        {
                                                                        return output;
                                                                        }
                                                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                        #endif

                                                                        // Graph Pixel
                                                                        struct SurfaceDescription
                                                                        {
                                                                        };

                                                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                        {
                                                                            SurfaceDescription surface = (SurfaceDescription)0;
                                                                            return surface;
                                                                        }

                                                                        // --------------------------------------------------
                                                                        // Build Graph Inputs
                                                                        #ifdef HAVE_VFX_MODIFICATION
                                                                        #define VFX_SRP_ATTRIBUTES Attributes
                                                                        #define VFX_SRP_VARYINGS Varyings
                                                                        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                        #endif
                                                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                        {
                                                                            VertexDescriptionInputs output;
                                                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                            output.ObjectSpaceNormal = input.normalOS;
                                                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                            output.ObjectSpacePosition = input.positionOS;

                                                                            return output;
                                                                        }
                                                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                        {
                                                                            SurfaceDescriptionInputs output;
                                                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                        #ifdef HAVE_VFX_MODIFICATION
                                                                            // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                        #endif







                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                        #else
                                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                        #endif
                                                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                return output;
                                                                        }

                                                                        // --------------------------------------------------
                                                                        // Main

                                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                                                        // --------------------------------------------------
                                                                        // Visual Effect Vertex Invocations
                                                                        #ifdef HAVE_VFX_MODIFICATION
                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                        #endif

                                                                        ENDHLSL
                                                                        }
                                                                        Pass
                                                                        {
                                                                            // Name: <None>
                                                                            Tags
                                                                            {
                                                                                "LightMode" = "Universal2D"
                                                                            }

                                                                            // Render State
                                                                            Cull Back
                                                                            Blend One Zero
                                                                            ZTest Off
                                                                            ZWrite On

                                                                            // Debug
                                                                            // <None>

                                                                            // --------------------------------------------------
                                                                            // Pass

                                                                            HLSLPROGRAM

                                                                            // Pragmas
                                                                            #pragma target 4.5
                                                                            #pragma exclude_renderers gles gles3 glcore
                                                                            #pragma vertex vert
                                                                            #pragma fragment frag

                                                                            // DotsInstancingOptions: <None>
                                                                            // HybridV1InjectedBuiltinProperties: <None>

                                                                            // Keywords
                                                                            // PassKeywords: <None>
                                                                            // GraphKeywords: <None>

                                                                            // Defines

                                                                            #define _NORMALMAP 1
                                                                            #define _NORMAL_DROPOFF_TS 1
                                                                            #define ATTRIBUTES_NEED_NORMAL
                                                                            #define ATTRIBUTES_NEED_TANGENT
                                                                            #define VARYINGS_NEED_POSITION_WS
                                                                            #define FEATURES_GRAPH_VERTEX
                                                                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                            #define SHADERPASS SHADERPASS_2D
                                                                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                            // custom interpolator pre-include
                                                                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                            // Includes
                                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                            // --------------------------------------------------
                                                                            // Structs and Packing

                                                                            // custom interpolators pre packing
                                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                            struct Attributes
                                                                            {
                                                                                 float3 positionOS : POSITION;
                                                                                 float3 normalOS : NORMAL;
                                                                                 float4 tangentOS : TANGENT;
                                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                                 uint instanceID : INSTANCEID_SEMANTIC;
                                                                                #endif
                                                                            };
                                                                            struct Varyings
                                                                            {
                                                                                 float4 positionCS : SV_POSITION;
                                                                                 float3 positionWS;
                                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                #endif
                                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                #endif
                                                                            };
                                                                            struct SurfaceDescriptionInputs
                                                                            {
                                                                                 float3 WorldSpacePosition;
                                                                            };
                                                                            struct VertexDescriptionInputs
                                                                            {
                                                                                 float3 ObjectSpaceNormal;
                                                                                 float3 ObjectSpaceTangent;
                                                                                 float3 ObjectSpacePosition;
                                                                            };
                                                                            struct PackedVaryings
                                                                            {
                                                                                 float4 positionCS : SV_POSITION;
                                                                                 float3 interp0 : INTERP0;
                                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                #endif
                                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                #endif
                                                                            };

                                                                            PackedVaryings PackVaryings(Varyings input)
                                                                            {
                                                                                PackedVaryings output;
                                                                                ZERO_INITIALIZE(PackedVaryings, output);
                                                                                output.positionCS = input.positionCS;
                                                                                output.interp0.xyz = input.positionWS;
                                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                                output.instanceID = input.instanceID;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                #endif
                                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                output.cullFace = input.cullFace;
                                                                                #endif
                                                                                return output;
                                                                            }

                                                                            Varyings UnpackVaryings(PackedVaryings input)
                                                                            {
                                                                                Varyings output;
                                                                                output.positionCS = input.positionCS;
                                                                                output.positionWS = input.interp0.xyz;
                                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                                output.instanceID = input.instanceID;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                #endif
                                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                #endif
                                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                output.cullFace = input.cullFace;
                                                                                #endif
                                                                                return output;
                                                                            }


                                                                            // --------------------------------------------------
                                                                            // Graph

                                                                            // Graph Properties
                                                                            CBUFFER_START(UnityPerMaterial)
                                                                            float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                            float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                            float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                            float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                            float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                            float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                            float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                            float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                            float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                            CBUFFER_END

                                                                                // Object and Global properties
                                                                                SAMPLER(SamplerState_Linear_Repeat);
                                                                                TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                // Graph Includes
                                                                                // GraphIncludes: <None>

                                                                                // -- Property used by ScenePickingPass
                                                                                #ifdef SCENEPICKINGPASS
                                                                                float4 _SelectionID;
                                                                                #endif

                                                                                // -- Properties used by SceneSelectionPass
                                                                                #ifdef SCENESELECTIONPASS
                                                                                int _ObjectId;
                                                                                int _PassValue;
                                                                                #endif

                                                                                // Graph Functions

                                                                                void Unity_Subtract_float(float A, float B, out float Out)
                                                                                {
                                                                                    Out = A - B;
                                                                                }

                                                                                void Unity_Add_float(float A, float B, out float Out)
                                                                                {
                                                                                    Out = A + B;
                                                                                }

                                                                                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                                                                                {
                                                                                    Out = smoothstep(Edge1, Edge2, In);
                                                                                }

                                                                                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                                                {
                                                                                    Out = A * B;
                                                                                }

                                                                                void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                                                                                {
                                                                                    Out = A + B;
                                                                                }

                                                                                // Custom interpolators pre vertex
                                                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                // Graph Vertex
                                                                                struct VertexDescription
                                                                                {
                                                                                    float3 Position;
                                                                                    float3 Normal;
                                                                                    float3 Tangent;
                                                                                };

                                                                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                {
                                                                                    VertexDescription description = (VertexDescription)0;
                                                                                    description.Position = IN.ObjectSpacePosition;
                                                                                    description.Normal = IN.ObjectSpaceNormal;
                                                                                    description.Tangent = IN.ObjectSpaceTangent;
                                                                                    return description;
                                                                                }

                                                                                // Custom interpolators, pre surface
                                                                                #ifdef FEATURES_GRAPH_VERTEX
                                                                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                {
                                                                                return output;
                                                                                }
                                                                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                #endif

                                                                                // Graph Pixel
                                                                                struct SurfaceDescription
                                                                                {
                                                                                    float3 BaseColor;
                                                                                };

                                                                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                {
                                                                                    SurfaceDescription surface = (SurfaceDescription)0;
                                                                                    float4 _Property_271638d18c864c50adc2de9ba4cf9707_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_c4b6043ac1de492fb8bbd1482b723534) : Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                    float _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                    float _Property_910d825da2eb46b0a1754b9e69252646_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                    float _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2;
                                                                                    Unity_Subtract_float(_Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Property_910d825da2eb46b0a1754b9e69252646_Out_0, _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2);
                                                                                    float _Add_bca707404b3243949fe8953b161e1601_Out_2;
                                                                                    Unity_Add_float(-1, _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Add_bca707404b3243949fe8953b161e1601_Out_2);
                                                                                    float _Split_4c5a12ca469049ee8d25c52df86199d7_R_1 = IN.WorldSpacePosition[0];
                                                                                    float _Split_4c5a12ca469049ee8d25c52df86199d7_G_2 = IN.WorldSpacePosition[1];
                                                                                    float _Split_4c5a12ca469049ee8d25c52df86199d7_B_3 = IN.WorldSpacePosition[2];
                                                                                    float _Split_4c5a12ca469049ee8d25c52df86199d7_A_4 = 0;
                                                                                    float _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3;
                                                                                    Unity_Smoothstep_float(_Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2, _Add_bca707404b3243949fe8953b161e1601_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3);
                                                                                    float4 _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2;
                                                                                    Unity_Multiply_float4_float4(_Property_271638d18c864c50adc2de9ba4cf9707_Out_0, (_Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3.xxxx), _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2);
                                                                                    float4 _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2;
                                                                                    Unity_Multiply_float4_float4(_Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2, float4(2, 2, 2, 2), _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2);
                                                                                    float4 _Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_adc979e474cd4c52803b73073550b88e) : Color_adc979e474cd4c52803b73073550b88e;
                                                                                    float _Property_429fa9faf52b49c891efaa416fe2c903_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                    float _Property_73479999c2b346edb73436833d483856_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                    float _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2;
                                                                                    Unity_Subtract_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, _Property_73479999c2b346edb73436833d483856_Out_0, _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2);
                                                                                    float _Add_554d292e79d145489ad3cdc7084a70ca_Out_2;
                                                                                    Unity_Add_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, -1, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2);
                                                                                    float _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3;
                                                                                    Unity_Smoothstep_float(_Subtract_72abdf79c34d4aacadfe832896a30461_Out_2, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3);
                                                                                    float4 _Multiply_f02e57049b254438874c61113b259afa_Out_2;
                                                                                    Unity_Multiply_float4_float4(_Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0, (_Smoothstep_919b4e994d44484590da2d34a073773c_Out_3.xxxx), _Multiply_f02e57049b254438874c61113b259afa_Out_2);
                                                                                    float4 _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2;
                                                                                    Unity_Add_float4(_Multiply_9d41234165c1425c8005aa668f597e6c_Out_2, _Multiply_f02e57049b254438874c61113b259afa_Out_2, _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2);
                                                                                    surface.BaseColor = (_Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2.xyz);
                                                                                    return surface;
                                                                                }

                                                                                // --------------------------------------------------
                                                                                // Build Graph Inputs
                                                                                #ifdef HAVE_VFX_MODIFICATION
                                                                                #define VFX_SRP_ATTRIBUTES Attributes
                                                                                #define VFX_SRP_VARYINGS Varyings
                                                                                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                #endif
                                                                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                {
                                                                                    VertexDescriptionInputs output;
                                                                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                    output.ObjectSpaceNormal = input.normalOS;
                                                                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                    output.ObjectSpacePosition = input.positionOS;

                                                                                    return output;
                                                                                }
                                                                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                {
                                                                                    SurfaceDescriptionInputs output;
                                                                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                #ifdef HAVE_VFX_MODIFICATION
                                                                                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                #endif







                                                                                    output.WorldSpacePosition = input.positionWS;
                                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                #else
                                                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                #endif
                                                                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                        return output;
                                                                                }

                                                                                // --------------------------------------------------
                                                                                // Main

                                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

                                                                                // --------------------------------------------------
                                                                                // Visual Effect Vertex Invocations
                                                                                #ifdef HAVE_VFX_MODIFICATION
                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                #endif

                                                                                ENDHLSL
                                                                                }
        }
        SubShader
                                                                            {
                                                                                Tags
                                                                                {
                                                                                    "RenderPipeline" = "UniversalPipeline"
                                                                                    "RenderType" = "Transparent"
                                                                                    "UniversalMaterialType" = "Lit"
                                                                                    "Queue" = "Transparent+2000"
                                                                                    "ShaderGraphTargetId" = "UniversalLitSubTarget"
                                                                                }
                                                                                Pass
                                                                                {
                                                                                    Name "Universal Forward"
                                                                                    Tags
                                                                                    {
                                                                                        "LightMode" = "UniversalForward"
                                                                                    }

                                                                                // Render State
                                                                                Cull Back
                                                                                Blend One Zero
                                                                                ZTest Off
            ZWrite On

                                                                                // Debug
                                                                                // <None>

                                                                                // --------------------------------------------------
                                                                                // Pass

                                                                                HLSLPROGRAM

                                                                                // Pragmas
                                                                                #pragma target 2.0
                                                                                #pragma only_renderers gles gles3 glcore d3d11
                                                                                #pragma multi_compile_instancing
                                                                                #pragma multi_compile_fog
                                                                                #pragma instancing_options renderinglayer
                                                                                #pragma vertex vert
                                                                                #pragma fragment frag

                                                                                // DotsInstancingOptions: <None>
                                                                                // HybridV1InjectedBuiltinProperties: <None>

                                                                                // Keywords
                                                                                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                                                                                #pragma multi_compile _ LIGHTMAP_ON
                                                                                #pragma multi_compile _ DYNAMICLIGHTMAP_ON
                                                                                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                                                                                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                                                                                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                                                                                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                                                                                #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
                                                                                #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
                                                                                #pragma multi_compile_fragment _ _SHADOWS_SOFT
                                                                                #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                                                                                #pragma multi_compile _ SHADOWS_SHADOWMASK
                                                                                #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                                                                                #pragma multi_compile_fragment _ _LIGHT_LAYERS
                                                                                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                                                                                #pragma multi_compile_fragment _ _LIGHT_COOKIES
                                                                                #pragma multi_compile _ _CLUSTERED_RENDERING
                                                                                // GraphKeywords: <None>

                                                                                // Defines

                                                                                #define _NORMALMAP 1
                                                                                #define _NORMAL_DROPOFF_TS 1
                                                                                #define ATTRIBUTES_NEED_NORMAL
                                                                                #define ATTRIBUTES_NEED_TANGENT
                                                                                #define ATTRIBUTES_NEED_TEXCOORD0
                                                                                #define ATTRIBUTES_NEED_TEXCOORD1
                                                                                #define ATTRIBUTES_NEED_TEXCOORD2
                                                                                #define VARYINGS_NEED_POSITION_WS
                                                                                #define VARYINGS_NEED_NORMAL_WS
                                                                                #define VARYINGS_NEED_TANGENT_WS
                                                                                #define VARYINGS_NEED_TEXCOORD0
                                                                                #define VARYINGS_NEED_VIEWDIRECTION_WS
                                                                                #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                                                                                #define VARYINGS_NEED_SHADOW_COORD
                                                                                #define FEATURES_GRAPH_VERTEX
                                                                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                #define SHADERPASS SHADERPASS_FORWARD
                                                                                #define _FOG_FRAGMENT 1
                                                                                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                // custom interpolator pre-include
                                                                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                // Includes
                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
                                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                // --------------------------------------------------
                                                                                // Structs and Packing

                                                                                // custom interpolators pre packing
                                                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                struct Attributes
                                                                                {
                                                                                        float3 positionOS : POSITION;
                                                                                        float3 normalOS : NORMAL;
                                                                                        float4 tangentOS : TANGENT;
                                                                                        float4 uv0 : TEXCOORD0;
                                                                                        float4 uv1 : TEXCOORD1;
                                                                                        float4 uv2 : TEXCOORD2;
                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                        uint instanceID : INSTANCEID_SEMANTIC;
                                                                                    #endif
                                                                                };
                                                                                struct Varyings
                                                                                {
                                                                                        float4 positionCS : SV_POSITION;
                                                                                        float3 positionWS;
                                                                                        float3 normalWS;
                                                                                        float4 tangentWS;
                                                                                        float4 texCoord0;
                                                                                        float3 viewDirectionWS;
                                                                                    #if defined(LIGHTMAP_ON)
                                                                                        float2 staticLightmapUV;
                                                                                    #endif
                                                                                    #if defined(DYNAMICLIGHTMAP_ON)
                                                                                        float2 dynamicLightmapUV;
                                                                                    #endif
                                                                                    #if !defined(LIGHTMAP_ON)
                                                                                        float3 sh;
                                                                                    #endif
                                                                                        float4 fogFactorAndVertexLight;
                                                                                    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                                                                                        float4 shadowCoord;
                                                                                    #endif
                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                    #endif
                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                    #endif
                                                                                };
                                                                                struct SurfaceDescriptionInputs
                                                                                {
                                                                                        float3 TangentSpaceNormal;
                                                                                        float3 WorldSpacePosition;
                                                                                        float4 uv0;
                                                                                };
                                                                                struct VertexDescriptionInputs
                                                                                {
                                                                                        float3 ObjectSpaceNormal;
                                                                                        float3 ObjectSpaceTangent;
                                                                                        float3 ObjectSpacePosition;
                                                                                };
                                                                                struct PackedVaryings
                                                                                {
                                                                                        float4 positionCS : SV_POSITION;
                                                                                        float3 interp0 : INTERP0;
                                                                                        float3 interp1 : INTERP1;
                                                                                        float4 interp2 : INTERP2;
                                                                                        float4 interp3 : INTERP3;
                                                                                        float3 interp4 : INTERP4;
                                                                                        float2 interp5 : INTERP5;
                                                                                        float2 interp6 : INTERP6;
                                                                                        float3 interp7 : INTERP7;
                                                                                        float4 interp8 : INTERP8;
                                                                                        float4 interp9 : INTERP9;
                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                    #endif
                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                    #endif
                                                                                };

                                                                                PackedVaryings PackVaryings(Varyings input)
                                                                                {
                                                                                    PackedVaryings output;
                                                                                    ZERO_INITIALIZE(PackedVaryings, output);
                                                                                    output.positionCS = input.positionCS;
                                                                                    output.interp0.xyz = input.positionWS;
                                                                                    output.interp1.xyz = input.normalWS;
                                                                                    output.interp2.xyzw = input.tangentWS;
                                                                                    output.interp3.xyzw = input.texCoord0;
                                                                                    output.interp4.xyz = input.viewDirectionWS;
                                                                                    #if defined(LIGHTMAP_ON)
                                                                                    output.interp5.xy = input.staticLightmapUV;
                                                                                    #endif
                                                                                    #if defined(DYNAMICLIGHTMAP_ON)
                                                                                    output.interp6.xy = input.dynamicLightmapUV;
                                                                                    #endif
                                                                                    #if !defined(LIGHTMAP_ON)
                                                                                    output.interp7.xyz = input.sh;
                                                                                    #endif
                                                                                    output.interp8.xyzw = input.fogFactorAndVertexLight;
                                                                                    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                                                                                    output.interp9.xyzw = input.shadowCoord;
                                                                                    #endif
                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                    output.instanceID = input.instanceID;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                    #endif
                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                    output.cullFace = input.cullFace;
                                                                                    #endif
                                                                                    return output;
                                                                                }

                                                                                Varyings UnpackVaryings(PackedVaryings input)
                                                                                {
                                                                                    Varyings output;
                                                                                    output.positionCS = input.positionCS;
                                                                                    output.positionWS = input.interp0.xyz;
                                                                                    output.normalWS = input.interp1.xyz;
                                                                                    output.tangentWS = input.interp2.xyzw;
                                                                                    output.texCoord0 = input.interp3.xyzw;
                                                                                    output.viewDirectionWS = input.interp4.xyz;
                                                                                    #if defined(LIGHTMAP_ON)
                                                                                    output.staticLightmapUV = input.interp5.xy;
                                                                                    #endif
                                                                                    #if defined(DYNAMICLIGHTMAP_ON)
                                                                                    output.dynamicLightmapUV = input.interp6.xy;
                                                                                    #endif
                                                                                    #if !defined(LIGHTMAP_ON)
                                                                                    output.sh = input.interp7.xyz;
                                                                                    #endif
                                                                                    output.fogFactorAndVertexLight = input.interp8.xyzw;
                                                                                    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                                                                                    output.shadowCoord = input.interp9.xyzw;
                                                                                    #endif
                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                    output.instanceID = input.instanceID;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                    #endif
                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                    #endif
                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                    output.cullFace = input.cullFace;
                                                                                    #endif
                                                                                    return output;
                                                                                }


                                                                                // --------------------------------------------------
                                                                                // Graph

                                                                                // Graph Properties
                                                                                CBUFFER_START(UnityPerMaterial)
                                                                                float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                CBUFFER_END

                                                                                    // Object and Global properties
                                                                                    SAMPLER(SamplerState_Linear_Repeat);
                                                                                    TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                    SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                    // Graph Includes
                                                                                    // GraphIncludes: <None>

                                                                                    // -- Property used by ScenePickingPass
                                                                                    #ifdef SCENEPICKINGPASS
                                                                                    float4 _SelectionID;
                                                                                    #endif

                                                                                    // -- Properties used by SceneSelectionPass
                                                                                    #ifdef SCENESELECTIONPASS
                                                                                    int _ObjectId;
                                                                                    int _PassValue;
                                                                                    #endif

                                                                                    // Graph Functions

                                                                                    void Unity_Subtract_float(float A, float B, out float Out)
                                                                                    {
                                                                                        Out = A - B;
                                                                                    }

                                                                                    void Unity_Add_float(float A, float B, out float Out)
                                                                                    {
                                                                                        Out = A + B;
                                                                                    }

                                                                                    void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                                                                                    {
                                                                                        Out = smoothstep(Edge1, Edge2, In);
                                                                                    }

                                                                                    void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                                                    {
                                                                                        Out = A * B;
                                                                                    }

                                                                                    void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                                                                                    {
                                                                                        Out = A + B;
                                                                                    }

                                                                                    void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
                                                                                    {
                                                                                        Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
                                                                                    }

                                                                                    // Custom interpolators pre vertex
                                                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                    // Graph Vertex
                                                                                    struct VertexDescription
                                                                                    {
                                                                                        float3 Position;
                                                                                        float3 Normal;
                                                                                        float3 Tangent;
                                                                                    };

                                                                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                    {
                                                                                        VertexDescription description = (VertexDescription)0;
                                                                                        description.Position = IN.ObjectSpacePosition;
                                                                                        description.Normal = IN.ObjectSpaceNormal;
                                                                                        description.Tangent = IN.ObjectSpaceTangent;
                                                                                        return description;
                                                                                    }

                                                                                    // Custom interpolators, pre surface
                                                                                    #ifdef FEATURES_GRAPH_VERTEX
                                                                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                    {
                                                                                    return output;
                                                                                    }
                                                                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                    #endif

                                                                                    // Graph Pixel
                                                                                    struct SurfaceDescription
                                                                                    {
                                                                                        float3 BaseColor;
                                                                                        float3 NormalTS;
                                                                                        float3 Emission;
                                                                                        float Metallic;
                                                                                        float Smoothness;
                                                                                        float Occlusion;
                                                                                    };

                                                                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                    {
                                                                                        SurfaceDescription surface = (SurfaceDescription)0;
                                                                                        float4 _Property_271638d18c864c50adc2de9ba4cf9707_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_c4b6043ac1de492fb8bbd1482b723534) : Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                        float _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                        float _Property_910d825da2eb46b0a1754b9e69252646_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                        float _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2;
                                                                                        Unity_Subtract_float(_Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Property_910d825da2eb46b0a1754b9e69252646_Out_0, _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2);
                                                                                        float _Add_bca707404b3243949fe8953b161e1601_Out_2;
                                                                                        Unity_Add_float(-1, _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Add_bca707404b3243949fe8953b161e1601_Out_2);
                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_R_1 = IN.WorldSpacePosition[0];
                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_G_2 = IN.WorldSpacePosition[1];
                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_B_3 = IN.WorldSpacePosition[2];
                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_A_4 = 0;
                                                                                        float _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3;
                                                                                        Unity_Smoothstep_float(_Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2, _Add_bca707404b3243949fe8953b161e1601_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3);
                                                                                        float4 _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2;
                                                                                        Unity_Multiply_float4_float4(_Property_271638d18c864c50adc2de9ba4cf9707_Out_0, (_Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3.xxxx), _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2);
                                                                                        float4 _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2;
                                                                                        Unity_Multiply_float4_float4(_Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2, float4(2, 2, 2, 2), _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2);
                                                                                        float4 _Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_adc979e474cd4c52803b73073550b88e) : Color_adc979e474cd4c52803b73073550b88e;
                                                                                        float _Property_429fa9faf52b49c891efaa416fe2c903_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                        float _Property_73479999c2b346edb73436833d483856_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                        float _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2;
                                                                                        Unity_Subtract_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, _Property_73479999c2b346edb73436833d483856_Out_0, _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2);
                                                                                        float _Add_554d292e79d145489ad3cdc7084a70ca_Out_2;
                                                                                        Unity_Add_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, -1, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2);
                                                                                        float _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3;
                                                                                        Unity_Smoothstep_float(_Subtract_72abdf79c34d4aacadfe832896a30461_Out_2, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3);
                                                                                        float4 _Multiply_f02e57049b254438874c61113b259afa_Out_2;
                                                                                        Unity_Multiply_float4_float4(_Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0, (_Smoothstep_919b4e994d44484590da2d34a073773c_Out_3.xxxx), _Multiply_f02e57049b254438874c61113b259afa_Out_2);
                                                                                        float4 _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2;
                                                                                        Unity_Add_float4(_Multiply_9d41234165c1425c8005aa668f597e6c_Out_2, _Multiply_f02e57049b254438874c61113b259afa_Out_2, _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2);
                                                                                        UnityTexture2D _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                        float4 _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.tex, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.samplerstate, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.GetTransformedUV(IN.uv0.xy));
                                                                                        _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0);
                                                                                        float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_R_4 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.r;
                                                                                        float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_G_5 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.g;
                                                                                        float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_B_6 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.b;
                                                                                        float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_A_7 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.a;
                                                                                        float _Property_4660fad584cc4501bb4130e8448804fe_Out_0 = Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                        float3 _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                                                                                        Unity_NormalStrength_float((_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.xyz), _Property_4660fad584cc4501bb4130e8448804fe_Out_0, _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2);
                                                                                        float _Property_5ce20b43b0c240c2b9bd1f8080a107c0_Out_0 = Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                        float _Property_14ce586b6d704c89910791a1a3dac060_Out_0 = Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                        surface.BaseColor = (_Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2.xyz);
                                                                                        surface.NormalTS = _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                                                                                        surface.Emission = float3(0, 0, 0);
                                                                                        surface.Metallic = 0;
                                                                                        surface.Smoothness = _Property_5ce20b43b0c240c2b9bd1f8080a107c0_Out_0;
                                                                                        surface.Occlusion = _Property_14ce586b6d704c89910791a1a3dac060_Out_0;
                                                                                        return surface;
                                                                                    }

                                                                                    // --------------------------------------------------
                                                                                    // Build Graph Inputs
                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                    #define VFX_SRP_ATTRIBUTES Attributes
                                                                                    #define VFX_SRP_VARYINGS Varyings
                                                                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                    #endif
                                                                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                    {
                                                                                        VertexDescriptionInputs output;
                                                                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                        output.ObjectSpaceNormal = input.normalOS;
                                                                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                        output.ObjectSpacePosition = input.positionOS;

                                                                                        return output;
                                                                                    }
                                                                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                    {
                                                                                        SurfaceDescriptionInputs output;
                                                                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                        // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                    #endif





                                                                                        output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                                                                                        output.WorldSpacePosition = input.positionWS;
                                                                                        output.uv0 = input.texCoord0;
                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                    #else
                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                    #endif
                                                                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                            return output;
                                                                                    }

                                                                                    // --------------------------------------------------
                                                                                    // Main

                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

                                                                                    // --------------------------------------------------
                                                                                    // Visual Effect Vertex Invocations
                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                    #endif

                                                                                    ENDHLSL
                                                                                    }
                                                                                    Pass
                                                                                    {
                                                                                        Name "ShadowCaster"
                                                                                        Tags
                                                                                        {
                                                                                            "LightMode" = "ShadowCaster"
                                                                                        }

                                                                                        // Render State
                                                                                        Cull Back
                                                                                        ZTest Off
                                                                                        ZWrite On
                                                                                        ColorMask 0

                                                                                        // Debug
                                                                                        // <None>

                                                                                        // --------------------------------------------------
                                                                                        // Pass

                                                                                        HLSLPROGRAM

                                                                                        // Pragmas
                                                                                        #pragma target 2.0
                                                                                        #pragma only_renderers gles gles3 glcore d3d11
                                                                                        #pragma multi_compile_instancing
                                                                                        #pragma vertex vert
                                                                                        #pragma fragment frag

                                                                                        // DotsInstancingOptions: <None>
                                                                                        // HybridV1InjectedBuiltinProperties: <None>

                                                                                        // Keywords
                                                                                        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
                                                                                        // GraphKeywords: <None>

                                                                                        // Defines

                                                                                        #define _NORMALMAP 1
                                                                                        #define _NORMAL_DROPOFF_TS 1
                                                                                        #define ATTRIBUTES_NEED_NORMAL
                                                                                        #define ATTRIBUTES_NEED_TANGENT
                                                                                        #define VARYINGS_NEED_NORMAL_WS
                                                                                        #define FEATURES_GRAPH_VERTEX
                                                                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                        #define SHADERPASS SHADERPASS_SHADOWCASTER
                                                                                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                        // custom interpolator pre-include
                                                                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                        // Includes
                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                        // --------------------------------------------------
                                                                                        // Structs and Packing

                                                                                        // custom interpolators pre packing
                                                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                        struct Attributes
                                                                                        {
                                                                                                float3 positionOS : POSITION;
                                                                                                float3 normalOS : NORMAL;
                                                                                                float4 tangentOS : TANGENT;
                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                uint instanceID : INSTANCEID_SEMANTIC;
                                                                                            #endif
                                                                                        };
                                                                                        struct Varyings
                                                                                        {
                                                                                                float4 positionCS : SV_POSITION;
                                                                                                float3 normalWS;
                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                            #endif
                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                            #endif
                                                                                        };
                                                                                        struct SurfaceDescriptionInputs
                                                                                        {
                                                                                        };
                                                                                        struct VertexDescriptionInputs
                                                                                        {
                                                                                                float3 ObjectSpaceNormal;
                                                                                                float3 ObjectSpaceTangent;
                                                                                                float3 ObjectSpacePosition;
                                                                                        };
                                                                                        struct PackedVaryings
                                                                                        {
                                                                                                float4 positionCS : SV_POSITION;
                                                                                                float3 interp0 : INTERP0;
                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                            #endif
                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                            #endif
                                                                                        };

                                                                                        PackedVaryings PackVaryings(Varyings input)
                                                                                        {
                                                                                            PackedVaryings output;
                                                                                            ZERO_INITIALIZE(PackedVaryings, output);
                                                                                            output.positionCS = input.positionCS;
                                                                                            output.interp0.xyz = input.normalWS;
                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                            output.instanceID = input.instanceID;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                            #endif
                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                            output.cullFace = input.cullFace;
                                                                                            #endif
                                                                                            return output;
                                                                                        }

                                                                                        Varyings UnpackVaryings(PackedVaryings input)
                                                                                        {
                                                                                            Varyings output;
                                                                                            output.positionCS = input.positionCS;
                                                                                            output.normalWS = input.interp0.xyz;
                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                            output.instanceID = input.instanceID;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                            #endif
                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                            #endif
                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                            output.cullFace = input.cullFace;
                                                                                            #endif
                                                                                            return output;
                                                                                        }


                                                                                        // --------------------------------------------------
                                                                                        // Graph

                                                                                        // Graph Properties
                                                                                        CBUFFER_START(UnityPerMaterial)
                                                                                        float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                        float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                        float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                        float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                        float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                        float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                        float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                        CBUFFER_END

                                                                                            // Object and Global properties
                                                                                            SAMPLER(SamplerState_Linear_Repeat);
                                                                                            TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                            SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                            // Graph Includes
                                                                                            // GraphIncludes: <None>

                                                                                            // -- Property used by ScenePickingPass
                                                                                            #ifdef SCENEPICKINGPASS
                                                                                            float4 _SelectionID;
                                                                                            #endif

                                                                                            // -- Properties used by SceneSelectionPass
                                                                                            #ifdef SCENESELECTIONPASS
                                                                                            int _ObjectId;
                                                                                            int _PassValue;
                                                                                            #endif

                                                                                            // Graph Functions
                                                                                            // GraphFunctions: <None>

                                                                                            // Custom interpolators pre vertex
                                                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                            // Graph Vertex
                                                                                            struct VertexDescription
                                                                                            {
                                                                                                float3 Position;
                                                                                                float3 Normal;
                                                                                                float3 Tangent;
                                                                                            };

                                                                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                            {
                                                                                                VertexDescription description = (VertexDescription)0;
                                                                                                description.Position = IN.ObjectSpacePosition;
                                                                                                description.Normal = IN.ObjectSpaceNormal;
                                                                                                description.Tangent = IN.ObjectSpaceTangent;
                                                                                                return description;
                                                                                            }

                                                                                            // Custom interpolators, pre surface
                                                                                            #ifdef FEATURES_GRAPH_VERTEX
                                                                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                            {
                                                                                            return output;
                                                                                            }
                                                                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                            #endif

                                                                                            // Graph Pixel
                                                                                            struct SurfaceDescription
                                                                                            {
                                                                                            };

                                                                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                            {
                                                                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                                                                return surface;
                                                                                            }

                                                                                            // --------------------------------------------------
                                                                                            // Build Graph Inputs
                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                            #define VFX_SRP_ATTRIBUTES Attributes
                                                                                            #define VFX_SRP_VARYINGS Varyings
                                                                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                            #endif
                                                                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                            {
                                                                                                VertexDescriptionInputs output;
                                                                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                                output.ObjectSpaceNormal = input.normalOS;
                                                                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                                output.ObjectSpacePosition = input.positionOS;

                                                                                                return output;
                                                                                            }
                                                                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                            {
                                                                                                SurfaceDescriptionInputs output;
                                                                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                            #endif







                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                            #else
                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                            #endif
                                                                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                                    return output;
                                                                                            }

                                                                                            // --------------------------------------------------
                                                                                            // Main

                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

                                                                                            // --------------------------------------------------
                                                                                            // Visual Effect Vertex Invocations
                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                            #endif

                                                                                            ENDHLSL
                                                                                            }
                                                                                            Pass
                                                                                            {
                                                                                                Name "DepthOnly"
                                                                                                Tags
                                                                                                {
                                                                                                    "LightMode" = "DepthOnly"
                                                                                                }

                                                                                                // Render State
                                                                                                Cull Back
                                                                                                ZTest Off
                                                                                                ZWrite On
                                                                                                ColorMask 0

                                                                                                // Debug
                                                                                                // <None>

                                                                                                // --------------------------------------------------
                                                                                                // Pass

                                                                                                HLSLPROGRAM

                                                                                                // Pragmas
                                                                                                #pragma target 2.0
                                                                                                #pragma only_renderers gles gles3 glcore d3d11
                                                                                                #pragma multi_compile_instancing
                                                                                                #pragma vertex vert
                                                                                                #pragma fragment frag

                                                                                                // DotsInstancingOptions: <None>
                                                                                                // HybridV1InjectedBuiltinProperties: <None>

                                                                                                // Keywords
                                                                                                // PassKeywords: <None>
                                                                                                // GraphKeywords: <None>

                                                                                                // Defines

                                                                                                #define _NORMALMAP 1
                                                                                                #define _NORMAL_DROPOFF_TS 1
                                                                                                #define ATTRIBUTES_NEED_NORMAL
                                                                                                #define ATTRIBUTES_NEED_TANGENT
                                                                                                #define FEATURES_GRAPH_VERTEX
                                                                                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                                #define SHADERPASS SHADERPASS_DEPTHONLY
                                                                                                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                                // custom interpolator pre-include
                                                                                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                                // Includes
                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                                // --------------------------------------------------
                                                                                                // Structs and Packing

                                                                                                // custom interpolators pre packing
                                                                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                                struct Attributes
                                                                                                {
                                                                                                        float3 positionOS : POSITION;
                                                                                                        float3 normalOS : NORMAL;
                                                                                                        float4 tangentOS : TANGENT;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                        uint instanceID : INSTANCEID_SEMANTIC;
                                                                                                    #endif
                                                                                                };
                                                                                                struct Varyings
                                                                                                {
                                                                                                        float4 positionCS : SV_POSITION;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                    #endif
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                    #endif
                                                                                                };
                                                                                                struct SurfaceDescriptionInputs
                                                                                                {
                                                                                                };
                                                                                                struct VertexDescriptionInputs
                                                                                                {
                                                                                                        float3 ObjectSpaceNormal;
                                                                                                        float3 ObjectSpaceTangent;
                                                                                                        float3 ObjectSpacePosition;
                                                                                                };
                                                                                                struct PackedVaryings
                                                                                                {
                                                                                                        float4 positionCS : SV_POSITION;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                    #endif
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                    #endif
                                                                                                };

                                                                                                PackedVaryings PackVaryings(Varyings input)
                                                                                                {
                                                                                                    PackedVaryings output;
                                                                                                    ZERO_INITIALIZE(PackedVaryings, output);
                                                                                                    output.positionCS = input.positionCS;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    output.instanceID = input.instanceID;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                    #endif
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                    output.cullFace = input.cullFace;
                                                                                                    #endif
                                                                                                    return output;
                                                                                                }

                                                                                                Varyings UnpackVaryings(PackedVaryings input)
                                                                                                {
                                                                                                    Varyings output;
                                                                                                    output.positionCS = input.positionCS;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    output.instanceID = input.instanceID;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                    #endif
                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                    #endif
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                    output.cullFace = input.cullFace;
                                                                                                    #endif
                                                                                                    return output;
                                                                                                }


                                                                                                // --------------------------------------------------
                                                                                                // Graph

                                                                                                // Graph Properties
                                                                                                CBUFFER_START(UnityPerMaterial)
                                                                                                float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                                float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                                float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                                float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                                float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                                CBUFFER_END

                                                                                                    // Object and Global properties
                                                                                                    SAMPLER(SamplerState_Linear_Repeat);
                                                                                                    TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                                    SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                                    // Graph Includes
                                                                                                    // GraphIncludes: <None>

                                                                                                    // -- Property used by ScenePickingPass
                                                                                                    #ifdef SCENEPICKINGPASS
                                                                                                    float4 _SelectionID;
                                                                                                    #endif

                                                                                                    // -- Properties used by SceneSelectionPass
                                                                                                    #ifdef SCENESELECTIONPASS
                                                                                                    int _ObjectId;
                                                                                                    int _PassValue;
                                                                                                    #endif

                                                                                                    // Graph Functions
                                                                                                    // GraphFunctions: <None>

                                                                                                    // Custom interpolators pre vertex
                                                                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                                    // Graph Vertex
                                                                                                    struct VertexDescription
                                                                                                    {
                                                                                                        float3 Position;
                                                                                                        float3 Normal;
                                                                                                        float3 Tangent;
                                                                                                    };

                                                                                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                                    {
                                                                                                        VertexDescription description = (VertexDescription)0;
                                                                                                        description.Position = IN.ObjectSpacePosition;
                                                                                                        description.Normal = IN.ObjectSpaceNormal;
                                                                                                        description.Tangent = IN.ObjectSpaceTangent;
                                                                                                        return description;
                                                                                                    }

                                                                                                    // Custom interpolators, pre surface
                                                                                                    #ifdef FEATURES_GRAPH_VERTEX
                                                                                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                                    {
                                                                                                    return output;
                                                                                                    }
                                                                                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                                    #endif

                                                                                                    // Graph Pixel
                                                                                                    struct SurfaceDescription
                                                                                                    {
                                                                                                    };

                                                                                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                                    {
                                                                                                        SurfaceDescription surface = (SurfaceDescription)0;
                                                                                                        return surface;
                                                                                                    }

                                                                                                    // --------------------------------------------------
                                                                                                    // Build Graph Inputs
                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                    #define VFX_SRP_ATTRIBUTES Attributes
                                                                                                    #define VFX_SRP_VARYINGS Varyings
                                                                                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                                    #endif
                                                                                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                                    {
                                                                                                        VertexDescriptionInputs output;
                                                                                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                                        output.ObjectSpaceNormal = input.normalOS;
                                                                                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                                        output.ObjectSpacePosition = input.positionOS;

                                                                                                        return output;
                                                                                                    }
                                                                                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                                    {
                                                                                                        SurfaceDescriptionInputs output;
                                                                                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                        // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                                    #endif







                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                    #else
                                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                                    #endif
                                                                                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                                            return output;
                                                                                                    }

                                                                                                    // --------------------------------------------------
                                                                                                    // Main

                                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

                                                                                                    // --------------------------------------------------
                                                                                                    // Visual Effect Vertex Invocations
                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                                    #endif

                                                                                                    ENDHLSL
                                                                                                    }
                                                                                                    Pass
                                                                                                    {
                                                                                                        Name "DepthNormals"
                                                                                                        Tags
                                                                                                        {
                                                                                                            "LightMode" = "DepthNormals"
                                                                                                        }

                                                                                                        // Render State
                                                                                                        Cull Back
                                                                                                        ZTest Off
                                                                                                        ZWrite On

                                                                                                        // Debug
                                                                                                        // <None>

                                                                                                        // --------------------------------------------------
                                                                                                        // Pass

                                                                                                        HLSLPROGRAM

                                                                                                        // Pragmas
                                                                                                        #pragma target 2.0
                                                                                                        #pragma only_renderers gles gles3 glcore d3d11
                                                                                                        #pragma multi_compile_instancing
                                                                                                        #pragma vertex vert
                                                                                                        #pragma fragment frag

                                                                                                        // DotsInstancingOptions: <None>
                                                                                                        // HybridV1InjectedBuiltinProperties: <None>

                                                                                                        // Keywords
                                                                                                        // PassKeywords: <None>
                                                                                                        // GraphKeywords: <None>

                                                                                                        // Defines

                                                                                                        #define _NORMALMAP 1
                                                                                                        #define _NORMAL_DROPOFF_TS 1
                                                                                                        #define ATTRIBUTES_NEED_NORMAL
                                                                                                        #define ATTRIBUTES_NEED_TANGENT
                                                                                                        #define ATTRIBUTES_NEED_TEXCOORD0
                                                                                                        #define ATTRIBUTES_NEED_TEXCOORD1
                                                                                                        #define VARYINGS_NEED_NORMAL_WS
                                                                                                        #define VARYINGS_NEED_TANGENT_WS
                                                                                                        #define VARYINGS_NEED_TEXCOORD0
                                                                                                        #define FEATURES_GRAPH_VERTEX
                                                                                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                                        #define SHADERPASS SHADERPASS_DEPTHNORMALS
                                                                                                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                                        // custom interpolator pre-include
                                                                                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                                        // Includes
                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                                        // --------------------------------------------------
                                                                                                        // Structs and Packing

                                                                                                        // custom interpolators pre packing
                                                                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                                        struct Attributes
                                                                                                        {
                                                                                                                float3 positionOS : POSITION;
                                                                                                                float3 normalOS : NORMAL;
                                                                                                                float4 tangentOS : TANGENT;
                                                                                                                float4 uv0 : TEXCOORD0;
                                                                                                                float4 uv1 : TEXCOORD1;
                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                uint instanceID : INSTANCEID_SEMANTIC;
                                                                                                            #endif
                                                                                                        };
                                                                                                        struct Varyings
                                                                                                        {
                                                                                                                float4 positionCS : SV_POSITION;
                                                                                                                float3 normalWS;
                                                                                                                float4 tangentWS;
                                                                                                                float4 texCoord0;
                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                            #endif
                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                            #endif
                                                                                                        };
                                                                                                        struct SurfaceDescriptionInputs
                                                                                                        {
                                                                                                                float3 TangentSpaceNormal;
                                                                                                                float4 uv0;
                                                                                                        };
                                                                                                        struct VertexDescriptionInputs
                                                                                                        {
                                                                                                                float3 ObjectSpaceNormal;
                                                                                                                float3 ObjectSpaceTangent;
                                                                                                                float3 ObjectSpacePosition;
                                                                                                        };
                                                                                                        struct PackedVaryings
                                                                                                        {
                                                                                                                float4 positionCS : SV_POSITION;
                                                                                                                float3 interp0 : INTERP0;
                                                                                                                float4 interp1 : INTERP1;
                                                                                                                float4 interp2 : INTERP2;
                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                            #endif
                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                            #endif
                                                                                                        };

                                                                                                        PackedVaryings PackVaryings(Varyings input)
                                                                                                        {
                                                                                                            PackedVaryings output;
                                                                                                            ZERO_INITIALIZE(PackedVaryings, output);
                                                                                                            output.positionCS = input.positionCS;
                                                                                                            output.interp0.xyz = input.normalWS;
                                                                                                            output.interp1.xyzw = input.tangentWS;
                                                                                                            output.interp2.xyzw = input.texCoord0;
                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                            output.instanceID = input.instanceID;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                            #endif
                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                            output.cullFace = input.cullFace;
                                                                                                            #endif
                                                                                                            return output;
                                                                                                        }

                                                                                                        Varyings UnpackVaryings(PackedVaryings input)
                                                                                                        {
                                                                                                            Varyings output;
                                                                                                            output.positionCS = input.positionCS;
                                                                                                            output.normalWS = input.interp0.xyz;
                                                                                                            output.tangentWS = input.interp1.xyzw;
                                                                                                            output.texCoord0 = input.interp2.xyzw;
                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                            output.instanceID = input.instanceID;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                            #endif
                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                            #endif
                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                            output.cullFace = input.cullFace;
                                                                                                            #endif
                                                                                                            return output;
                                                                                                        }


                                                                                                        // --------------------------------------------------
                                                                                                        // Graph

                                                                                                        // Graph Properties
                                                                                                        CBUFFER_START(UnityPerMaterial)
                                                                                                        float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                        float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                        float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                        float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                                        float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                                        float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                                        float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                                        CBUFFER_END

                                                                                                            // Object and Global properties
                                                                                                            SAMPLER(SamplerState_Linear_Repeat);
                                                                                                            TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                                            SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                                            // Graph Includes
                                                                                                            // GraphIncludes: <None>

                                                                                                            // -- Property used by ScenePickingPass
                                                                                                            #ifdef SCENEPICKINGPASS
                                                                                                            float4 _SelectionID;
                                                                                                            #endif

                                                                                                            // -- Properties used by SceneSelectionPass
                                                                                                            #ifdef SCENESELECTIONPASS
                                                                                                            int _ObjectId;
                                                                                                            int _PassValue;
                                                                                                            #endif

                                                                                                            // Graph Functions

                                                                                                            void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
                                                                                                            {
                                                                                                                Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
                                                                                                            }

                                                                                                            // Custom interpolators pre vertex
                                                                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                                            // Graph Vertex
                                                                                                            struct VertexDescription
                                                                                                            {
                                                                                                                float3 Position;
                                                                                                                float3 Normal;
                                                                                                                float3 Tangent;
                                                                                                            };

                                                                                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                                            {
                                                                                                                VertexDescription description = (VertexDescription)0;
                                                                                                                description.Position = IN.ObjectSpacePosition;
                                                                                                                description.Normal = IN.ObjectSpaceNormal;
                                                                                                                description.Tangent = IN.ObjectSpaceTangent;
                                                                                                                return description;
                                                                                                            }

                                                                                                            // Custom interpolators, pre surface
                                                                                                            #ifdef FEATURES_GRAPH_VERTEX
                                                                                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                                            {
                                                                                                            return output;
                                                                                                            }
                                                                                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                                            #endif

                                                                                                            // Graph Pixel
                                                                                                            struct SurfaceDescription
                                                                                                            {
                                                                                                                float3 NormalTS;
                                                                                                            };

                                                                                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                                            {
                                                                                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                                                                                UnityTexture2D _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                                                float4 _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.tex, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.samplerstate, _Property_a485729710fc4e1fad2c4ec60b5dae70_Out_0.GetTransformedUV(IN.uv0.xy));
                                                                                                                _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0);
                                                                                                                float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_R_4 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.r;
                                                                                                                float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_G_5 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.g;
                                                                                                                float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_B_6 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.b;
                                                                                                                float _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_A_7 = _SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.a;
                                                                                                                float _Property_4660fad584cc4501bb4130e8448804fe_Out_0 = Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                                                float3 _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                                                                                                                Unity_NormalStrength_float((_SampleTexture2D_8b063cee54c444dfa27adfad8de21f3a_RGBA_0.xyz), _Property_4660fad584cc4501bb4130e8448804fe_Out_0, _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2);
                                                                                                                surface.NormalTS = _NormalStrength_a34c74354020474681f37ad6fea42350_Out_2;
                                                                                                                return surface;
                                                                                                            }

                                                                                                            // --------------------------------------------------
                                                                                                            // Build Graph Inputs
                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                            #define VFX_SRP_ATTRIBUTES Attributes
                                                                                                            #define VFX_SRP_VARYINGS Varyings
                                                                                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                                            #endif
                                                                                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                                            {
                                                                                                                VertexDescriptionInputs output;
                                                                                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                                                output.ObjectSpaceNormal = input.normalOS;
                                                                                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                                                output.ObjectSpacePosition = input.positionOS;

                                                                                                                return output;
                                                                                                            }
                                                                                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                                            {
                                                                                                                SurfaceDescriptionInputs output;
                                                                                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                                            #endif





                                                                                                                output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                                                                                                                output.uv0 = input.texCoord0;
                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                            #else
                                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                                            #endif
                                                                                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                                                    return output;
                                                                                                            }

                                                                                                            // --------------------------------------------------
                                                                                                            // Main

                                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                                                                                                            // --------------------------------------------------
                                                                                                            // Visual Effect Vertex Invocations
                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                                            #endif

                                                                                                            ENDHLSL
                                                                                                            }
                                                                                                            Pass
                                                                                                            {
                                                                                                                Name "Meta"
                                                                                                                Tags
                                                                                                                {
                                                                                                                    "LightMode" = "Meta"
                                                                                                                }

                                                                                                                // Render State
                                                                                                                Cull Off

                                                                                                                // Debug
                                                                                                                // <None>

                                                                                                                // --------------------------------------------------
                                                                                                                // Pass

                                                                                                                HLSLPROGRAM

                                                                                                                // Pragmas
                                                                                                                #pragma target 2.0
                                                                                                                #pragma only_renderers gles gles3 glcore d3d11
                                                                                                                #pragma vertex vert
                                                                                                                #pragma fragment frag

                                                                                                                // DotsInstancingOptions: <None>
                                                                                                                // HybridV1InjectedBuiltinProperties: <None>

                                                                                                                // Keywords
                                                                                                                #pragma shader_feature _ EDITOR_VISUALIZATION
                                                                                                                // GraphKeywords: <None>

                                                                                                                // Defines

                                                                                                                #define _NORMALMAP 1
                                                                                                                #define _NORMAL_DROPOFF_TS 1
                                                                                                                #define ATTRIBUTES_NEED_NORMAL
                                                                                                                #define ATTRIBUTES_NEED_TANGENT
                                                                                                                #define ATTRIBUTES_NEED_TEXCOORD0
                                                                                                                #define ATTRIBUTES_NEED_TEXCOORD1
                                                                                                                #define ATTRIBUTES_NEED_TEXCOORD2
                                                                                                                #define VARYINGS_NEED_POSITION_WS
                                                                                                                #define VARYINGS_NEED_TEXCOORD0
                                                                                                                #define VARYINGS_NEED_TEXCOORD1
                                                                                                                #define VARYINGS_NEED_TEXCOORD2
                                                                                                                #define FEATURES_GRAPH_VERTEX
                                                                                                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                                                #define SHADERPASS SHADERPASS_META
                                                                                                                #define _FOG_FRAGMENT 1
                                                                                                                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                                                // custom interpolator pre-include
                                                                                                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                                                // Includes
                                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
                                                                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                                                // --------------------------------------------------
                                                                                                                // Structs and Packing

                                                                                                                // custom interpolators pre packing
                                                                                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                                                struct Attributes
                                                                                                                {
                                                                                                                        float3 positionOS : POSITION;
                                                                                                                        float3 normalOS : NORMAL;
                                                                                                                        float4 tangentOS : TANGENT;
                                                                                                                        float4 uv0 : TEXCOORD0;
                                                                                                                        float4 uv1 : TEXCOORD1;
                                                                                                                        float4 uv2 : TEXCOORD2;
                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                        uint instanceID : INSTANCEID_SEMANTIC;
                                                                                                                    #endif
                                                                                                                };
                                                                                                                struct Varyings
                                                                                                                {
                                                                                                                        float4 positionCS : SV_POSITION;
                                                                                                                        float3 positionWS;
                                                                                                                        float4 texCoord0;
                                                                                                                        float4 texCoord1;
                                                                                                                        float4 texCoord2;
                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                    #endif
                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                    #endif
                                                                                                                };
                                                                                                                struct SurfaceDescriptionInputs
                                                                                                                {
                                                                                                                        float3 WorldSpacePosition;
                                                                                                                };
                                                                                                                struct VertexDescriptionInputs
                                                                                                                {
                                                                                                                        float3 ObjectSpaceNormal;
                                                                                                                        float3 ObjectSpaceTangent;
                                                                                                                        float3 ObjectSpacePosition;
                                                                                                                };
                                                                                                                struct PackedVaryings
                                                                                                                {
                                                                                                                        float4 positionCS : SV_POSITION;
                                                                                                                        float3 interp0 : INTERP0;
                                                                                                                        float4 interp1 : INTERP1;
                                                                                                                        float4 interp2 : INTERP2;
                                                                                                                        float4 interp3 : INTERP3;
                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                    #endif
                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                    #endif
                                                                                                                };

                                                                                                                PackedVaryings PackVaryings(Varyings input)
                                                                                                                {
                                                                                                                    PackedVaryings output;
                                                                                                                    ZERO_INITIALIZE(PackedVaryings, output);
                                                                                                                    output.positionCS = input.positionCS;
                                                                                                                    output.interp0.xyz = input.positionWS;
                                                                                                                    output.interp1.xyzw = input.texCoord0;
                                                                                                                    output.interp2.xyzw = input.texCoord1;
                                                                                                                    output.interp3.xyzw = input.texCoord2;
                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                    output.instanceID = input.instanceID;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                    #endif
                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                    output.cullFace = input.cullFace;
                                                                                                                    #endif
                                                                                                                    return output;
                                                                                                                }

                                                                                                                Varyings UnpackVaryings(PackedVaryings input)
                                                                                                                {
                                                                                                                    Varyings output;
                                                                                                                    output.positionCS = input.positionCS;
                                                                                                                    output.positionWS = input.interp0.xyz;
                                                                                                                    output.texCoord0 = input.interp1.xyzw;
                                                                                                                    output.texCoord1 = input.interp2.xyzw;
                                                                                                                    output.texCoord2 = input.interp3.xyzw;
                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                    output.instanceID = input.instanceID;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                    #endif
                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                    #endif
                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                    output.cullFace = input.cullFace;
                                                                                                                    #endif
                                                                                                                    return output;
                                                                                                                }


                                                                                                                // --------------------------------------------------
                                                                                                                // Graph

                                                                                                                // Graph Properties
                                                                                                                CBUFFER_START(UnityPerMaterial)
                                                                                                                float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                                float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                                                float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                                float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                                float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                                                float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                                                float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                                                float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                                                CBUFFER_END

                                                                                                                    // Object and Global properties
                                                                                                                    SAMPLER(SamplerState_Linear_Repeat);
                                                                                                                    TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                                                    SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                                                    // Graph Includes
                                                                                                                    // GraphIncludes: <None>

                                                                                                                    // -- Property used by ScenePickingPass
                                                                                                                    #ifdef SCENEPICKINGPASS
                                                                                                                    float4 _SelectionID;
                                                                                                                    #endif

                                                                                                                    // -- Properties used by SceneSelectionPass
                                                                                                                    #ifdef SCENESELECTIONPASS
                                                                                                                    int _ObjectId;
                                                                                                                    int _PassValue;
                                                                                                                    #endif

                                                                                                                    // Graph Functions

                                                                                                                    void Unity_Subtract_float(float A, float B, out float Out)
                                                                                                                    {
                                                                                                                        Out = A - B;
                                                                                                                    }

                                                                                                                    void Unity_Add_float(float A, float B, out float Out)
                                                                                                                    {
                                                                                                                        Out = A + B;
                                                                                                                    }

                                                                                                                    void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                                                                                                                    {
                                                                                                                        Out = smoothstep(Edge1, Edge2, In);
                                                                                                                    }

                                                                                                                    void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                                                                                    {
                                                                                                                        Out = A * B;
                                                                                                                    }

                                                                                                                    void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                                                                                                                    {
                                                                                                                        Out = A + B;
                                                                                                                    }

                                                                                                                    // Custom interpolators pre vertex
                                                                                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                                                    // Graph Vertex
                                                                                                                    struct VertexDescription
                                                                                                                    {
                                                                                                                        float3 Position;
                                                                                                                        float3 Normal;
                                                                                                                        float3 Tangent;
                                                                                                                    };

                                                                                                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                                                    {
                                                                                                                        VertexDescription description = (VertexDescription)0;
                                                                                                                        description.Position = IN.ObjectSpacePosition;
                                                                                                                        description.Normal = IN.ObjectSpaceNormal;
                                                                                                                        description.Tangent = IN.ObjectSpaceTangent;
                                                                                                                        return description;
                                                                                                                    }

                                                                                                                    // Custom interpolators, pre surface
                                                                                                                    #ifdef FEATURES_GRAPH_VERTEX
                                                                                                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                                                    {
                                                                                                                    return output;
                                                                                                                    }
                                                                                                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                                                    #endif

                                                                                                                    // Graph Pixel
                                                                                                                    struct SurfaceDescription
                                                                                                                    {
                                                                                                                        float3 BaseColor;
                                                                                                                        float3 Emission;
                                                                                                                    };

                                                                                                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                                                    {
                                                                                                                        SurfaceDescription surface = (SurfaceDescription)0;
                                                                                                                        float4 _Property_271638d18c864c50adc2de9ba4cf9707_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_c4b6043ac1de492fb8bbd1482b723534) : Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                                        float _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                        float _Property_910d825da2eb46b0a1754b9e69252646_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                                        float _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2;
                                                                                                                        Unity_Subtract_float(_Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Property_910d825da2eb46b0a1754b9e69252646_Out_0, _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2);
                                                                                                                        float _Add_bca707404b3243949fe8953b161e1601_Out_2;
                                                                                                                        Unity_Add_float(-1, _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Add_bca707404b3243949fe8953b161e1601_Out_2);
                                                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_R_1 = IN.WorldSpacePosition[0];
                                                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_G_2 = IN.WorldSpacePosition[1];
                                                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_B_3 = IN.WorldSpacePosition[2];
                                                                                                                        float _Split_4c5a12ca469049ee8d25c52df86199d7_A_4 = 0;
                                                                                                                        float _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3;
                                                                                                                        Unity_Smoothstep_float(_Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2, _Add_bca707404b3243949fe8953b161e1601_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3);
                                                                                                                        float4 _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2;
                                                                                                                        Unity_Multiply_float4_float4(_Property_271638d18c864c50adc2de9ba4cf9707_Out_0, (_Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3.xxxx), _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2);
                                                                                                                        float4 _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2;
                                                                                                                        Unity_Multiply_float4_float4(_Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2, float4(2, 2, 2, 2), _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2);
                                                                                                                        float4 _Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_adc979e474cd4c52803b73073550b88e) : Color_adc979e474cd4c52803b73073550b88e;
                                                                                                                        float _Property_429fa9faf52b49c891efaa416fe2c903_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                        float _Property_73479999c2b346edb73436833d483856_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                                        float _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2;
                                                                                                                        Unity_Subtract_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, _Property_73479999c2b346edb73436833d483856_Out_0, _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2);
                                                                                                                        float _Add_554d292e79d145489ad3cdc7084a70ca_Out_2;
                                                                                                                        Unity_Add_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, -1, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2);
                                                                                                                        float _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3;
                                                                                                                        Unity_Smoothstep_float(_Subtract_72abdf79c34d4aacadfe832896a30461_Out_2, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3);
                                                                                                                        float4 _Multiply_f02e57049b254438874c61113b259afa_Out_2;
                                                                                                                        Unity_Multiply_float4_float4(_Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0, (_Smoothstep_919b4e994d44484590da2d34a073773c_Out_3.xxxx), _Multiply_f02e57049b254438874c61113b259afa_Out_2);
                                                                                                                        float4 _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2;
                                                                                                                        Unity_Add_float4(_Multiply_9d41234165c1425c8005aa668f597e6c_Out_2, _Multiply_f02e57049b254438874c61113b259afa_Out_2, _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2);
                                                                                                                        surface.BaseColor = (_Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2.xyz);
                                                                                                                        surface.Emission = float3(0, 0, 0);
                                                                                                                        return surface;
                                                                                                                    }

                                                                                                                    // --------------------------------------------------
                                                                                                                    // Build Graph Inputs
                                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                                    #define VFX_SRP_ATTRIBUTES Attributes
                                                                                                                    #define VFX_SRP_VARYINGS Varyings
                                                                                                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                                                    #endif
                                                                                                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                                                    {
                                                                                                                        VertexDescriptionInputs output;
                                                                                                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                                                        output.ObjectSpaceNormal = input.normalOS;
                                                                                                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                                                        output.ObjectSpacePosition = input.positionOS;

                                                                                                                        return output;
                                                                                                                    }
                                                                                                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                                                    {
                                                                                                                        SurfaceDescriptionInputs output;
                                                                                                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                                        // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                                                    #endif







                                                                                                                        output.WorldSpacePosition = input.positionWS;
                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                                    #else
                                                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                                                    #endif
                                                                                                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                                                            return output;
                                                                                                                    }

                                                                                                                    // --------------------------------------------------
                                                                                                                    // Main

                                                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

                                                                                                                    // --------------------------------------------------
                                                                                                                    // Visual Effect Vertex Invocations
                                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                                                    #endif

                                                                                                                    ENDHLSL
                                                                                                                    }
                                                                                                                    Pass
                                                                                                                    {
                                                                                                                        Name "SceneSelectionPass"
                                                                                                                        Tags
                                                                                                                        {
                                                                                                                            "LightMode" = "SceneSelectionPass"
                                                                                                                        }

                                                                                                                        // Render State
                                                                                                                        Cull Off

                                                                                                                        // Debug
                                                                                                                        // <None>

                                                                                                                        // --------------------------------------------------
                                                                                                                        // Pass

                                                                                                                        HLSLPROGRAM

                                                                                                                        // Pragmas
                                                                                                                        #pragma target 2.0
                                                                                                                        #pragma only_renderers gles gles3 glcore d3d11
                                                                                                                        #pragma multi_compile_instancing
                                                                                                                        #pragma vertex vert
                                                                                                                        #pragma fragment frag

                                                                                                                        // DotsInstancingOptions: <None>
                                                                                                                        // HybridV1InjectedBuiltinProperties: <None>

                                                                                                                        // Keywords
                                                                                                                        // PassKeywords: <None>
                                                                                                                        // GraphKeywords: <None>

                                                                                                                        // Defines

                                                                                                                        #define _NORMALMAP 1
                                                                                                                        #define _NORMAL_DROPOFF_TS 1
                                                                                                                        #define ATTRIBUTES_NEED_NORMAL
                                                                                                                        #define ATTRIBUTES_NEED_TANGENT
                                                                                                                        #define FEATURES_GRAPH_VERTEX
                                                                                                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                                                        #define SHADERPASS SHADERPASS_DEPTHONLY
                                                                                                                        #define SCENESELECTIONPASS 1
                                                                                                                        #define ALPHA_CLIP_THRESHOLD 1
                                                                                                                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                                                        // custom interpolator pre-include
                                                                                                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                                                        // Includes
                                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                                                        // --------------------------------------------------
                                                                                                                        // Structs and Packing

                                                                                                                        // custom interpolators pre packing
                                                                                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                                                        struct Attributes
                                                                                                                        {
                                                                                                                                float3 positionOS : POSITION;
                                                                                                                                float3 normalOS : NORMAL;
                                                                                                                                float4 tangentOS : TANGENT;
                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                uint instanceID : INSTANCEID_SEMANTIC;
                                                                                                                            #endif
                                                                                                                        };
                                                                                                                        struct Varyings
                                                                                                                        {
                                                                                                                                float4 positionCS : SV_POSITION;
                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                            #endif
                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                            #endif
                                                                                                                        };
                                                                                                                        struct SurfaceDescriptionInputs
                                                                                                                        {
                                                                                                                        };
                                                                                                                        struct VertexDescriptionInputs
                                                                                                                        {
                                                                                                                                float3 ObjectSpaceNormal;
                                                                                                                                float3 ObjectSpaceTangent;
                                                                                                                                float3 ObjectSpacePosition;
                                                                                                                        };
                                                                                                                        struct PackedVaryings
                                                                                                                        {
                                                                                                                                float4 positionCS : SV_POSITION;
                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                            #endif
                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                            #endif
                                                                                                                        };

                                                                                                                        PackedVaryings PackVaryings(Varyings input)
                                                                                                                        {
                                                                                                                            PackedVaryings output;
                                                                                                                            ZERO_INITIALIZE(PackedVaryings, output);
                                                                                                                            output.positionCS = input.positionCS;
                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                            output.instanceID = input.instanceID;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                            #endif
                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                            output.cullFace = input.cullFace;
                                                                                                                            #endif
                                                                                                                            return output;
                                                                                                                        }

                                                                                                                        Varyings UnpackVaryings(PackedVaryings input)
                                                                                                                        {
                                                                                                                            Varyings output;
                                                                                                                            output.positionCS = input.positionCS;
                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                            output.instanceID = input.instanceID;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                            #endif
                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                            #endif
                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                            output.cullFace = input.cullFace;
                                                                                                                            #endif
                                                                                                                            return output;
                                                                                                                        }


                                                                                                                        // --------------------------------------------------
                                                                                                                        // Graph

                                                                                                                        // Graph Properties
                                                                                                                        CBUFFER_START(UnityPerMaterial)
                                                                                                                        float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                                        float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                                        float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                        float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                                                        float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                                                        float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                                                        float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                                                        CBUFFER_END

                                                                                                                            // Object and Global properties
                                                                                                                            SAMPLER(SamplerState_Linear_Repeat);
                                                                                                                            TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                                                            SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                                                            // Graph Includes
                                                                                                                            // GraphIncludes: <None>

                                                                                                                            // -- Property used by ScenePickingPass
                                                                                                                            #ifdef SCENEPICKINGPASS
                                                                                                                            float4 _SelectionID;
                                                                                                                            #endif

                                                                                                                            // -- Properties used by SceneSelectionPass
                                                                                                                            #ifdef SCENESELECTIONPASS
                                                                                                                            int _ObjectId;
                                                                                                                            int _PassValue;
                                                                                                                            #endif

                                                                                                                            // Graph Functions
                                                                                                                            // GraphFunctions: <None>

                                                                                                                            // Custom interpolators pre vertex
                                                                                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                                                            // Graph Vertex
                                                                                                                            struct VertexDescription
                                                                                                                            {
                                                                                                                                float3 Position;
                                                                                                                                float3 Normal;
                                                                                                                                float3 Tangent;
                                                                                                                            };

                                                                                                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                                                            {
                                                                                                                                VertexDescription description = (VertexDescription)0;
                                                                                                                                description.Position = IN.ObjectSpacePosition;
                                                                                                                                description.Normal = IN.ObjectSpaceNormal;
                                                                                                                                description.Tangent = IN.ObjectSpaceTangent;
                                                                                                                                return description;
                                                                                                                            }

                                                                                                                            // Custom interpolators, pre surface
                                                                                                                            #ifdef FEATURES_GRAPH_VERTEX
                                                                                                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                                                            {
                                                                                                                            return output;
                                                                                                                            }
                                                                                                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                                                            #endif

                                                                                                                            // Graph Pixel
                                                                                                                            struct SurfaceDescription
                                                                                                                            {
                                                                                                                            };

                                                                                                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                                                            {
                                                                                                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                                                                                                return surface;
                                                                                                                            }

                                                                                                                            // --------------------------------------------------
                                                                                                                            // Build Graph Inputs
                                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                                            #define VFX_SRP_ATTRIBUTES Attributes
                                                                                                                            #define VFX_SRP_VARYINGS Varyings
                                                                                                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                                                            #endif
                                                                                                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                                                            {
                                                                                                                                VertexDescriptionInputs output;
                                                                                                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                                                                output.ObjectSpaceNormal = input.normalOS;
                                                                                                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                                                                output.ObjectSpacePosition = input.positionOS;

                                                                                                                                return output;
                                                                                                                            }
                                                                                                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                                                            {
                                                                                                                                SurfaceDescriptionInputs output;
                                                                                                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                                                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                                                            #endif







                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                                            #else
                                                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                                                            #endif
                                                                                                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                                                                    return output;
                                                                                                                            }

                                                                                                                            // --------------------------------------------------
                                                                                                                            // Main

                                                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                                                                                                            // --------------------------------------------------
                                                                                                                            // Visual Effect Vertex Invocations
                                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                                                            #endif

                                                                                                                            ENDHLSL
                                                                                                                            }
                                                                                                                            Pass
                                                                                                                            {
                                                                                                                                Name "ScenePickingPass"
                                                                                                                                Tags
                                                                                                                                {
                                                                                                                                    "LightMode" = "Picking"
                                                                                                                                }

                                                                                                                                // Render State
                                                                                                                                Cull Back

                                                                                                                                // Debug
                                                                                                                                // <None>

                                                                                                                                // --------------------------------------------------
                                                                                                                                // Pass

                                                                                                                                HLSLPROGRAM

                                                                                                                                // Pragmas
                                                                                                                                #pragma target 2.0
                                                                                                                                #pragma only_renderers gles gles3 glcore d3d11
                                                                                                                                #pragma multi_compile_instancing
                                                                                                                                #pragma vertex vert
                                                                                                                                #pragma fragment frag

                                                                                                                                // DotsInstancingOptions: <None>
                                                                                                                                // HybridV1InjectedBuiltinProperties: <None>

                                                                                                                                // Keywords
                                                                                                                                // PassKeywords: <None>
                                                                                                                                // GraphKeywords: <None>

                                                                                                                                // Defines

                                                                                                                                #define _NORMALMAP 1
                                                                                                                                #define _NORMAL_DROPOFF_TS 1
                                                                                                                                #define ATTRIBUTES_NEED_NORMAL
                                                                                                                                #define ATTRIBUTES_NEED_TANGENT
                                                                                                                                #define FEATURES_GRAPH_VERTEX
                                                                                                                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                                                                #define SHADERPASS SHADERPASS_DEPTHONLY
                                                                                                                                #define SCENEPICKINGPASS 1
                                                                                                                                #define ALPHA_CLIP_THRESHOLD 1
                                                                                                                                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                                                                // custom interpolator pre-include
                                                                                                                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                                                                // Includes
                                                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                                                                // --------------------------------------------------
                                                                                                                                // Structs and Packing

                                                                                                                                // custom interpolators pre packing
                                                                                                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                                                                struct Attributes
                                                                                                                                {
                                                                                                                                        float3 positionOS : POSITION;
                                                                                                                                        float3 normalOS : NORMAL;
                                                                                                                                        float4 tangentOS : TANGENT;
                                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                        uint instanceID : INSTANCEID_SEMANTIC;
                                                                                                                                    #endif
                                                                                                                                };
                                                                                                                                struct Varyings
                                                                                                                                {
                                                                                                                                        float4 positionCS : SV_POSITION;
                                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                                    #endif
                                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                                    #endif
                                                                                                                                };
                                                                                                                                struct SurfaceDescriptionInputs
                                                                                                                                {
                                                                                                                                };
                                                                                                                                struct VertexDescriptionInputs
                                                                                                                                {
                                                                                                                                        float3 ObjectSpaceNormal;
                                                                                                                                        float3 ObjectSpaceTangent;
                                                                                                                                        float3 ObjectSpacePosition;
                                                                                                                                };
                                                                                                                                struct PackedVaryings
                                                                                                                                {
                                                                                                                                        float4 positionCS : SV_POSITION;
                                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                                    #endif
                                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                                    #endif
                                                                                                                                };

                                                                                                                                PackedVaryings PackVaryings(Varyings input)
                                                                                                                                {
                                                                                                                                    PackedVaryings output;
                                                                                                                                    ZERO_INITIALIZE(PackedVaryings, output);
                                                                                                                                    output.positionCS = input.positionCS;
                                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                    output.instanceID = input.instanceID;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                                    #endif
                                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                    output.cullFace = input.cullFace;
                                                                                                                                    #endif
                                                                                                                                    return output;
                                                                                                                                }

                                                                                                                                Varyings UnpackVaryings(PackedVaryings input)
                                                                                                                                {
                                                                                                                                    Varyings output;
                                                                                                                                    output.positionCS = input.positionCS;
                                                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                    output.instanceID = input.instanceID;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                                    #endif
                                                                                                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                                    #endif
                                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                    output.cullFace = input.cullFace;
                                                                                                                                    #endif
                                                                                                                                    return output;
                                                                                                                                }


                                                                                                                                // --------------------------------------------------
                                                                                                                                // Graph

                                                                                                                                // Graph Properties
                                                                                                                                CBUFFER_START(UnityPerMaterial)
                                                                                                                                float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                                                float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                                                                float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                                                float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                                                float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                                float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                                                                float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                                                                float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                                                                float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                                                                CBUFFER_END

                                                                                                                                    // Object and Global properties
                                                                                                                                    SAMPLER(SamplerState_Linear_Repeat);
                                                                                                                                    TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                                                                    SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                                                                    // Graph Includes
                                                                                                                                    // GraphIncludes: <None>

                                                                                                                                    // -- Property used by ScenePickingPass
                                                                                                                                    #ifdef SCENEPICKINGPASS
                                                                                                                                    float4 _SelectionID;
                                                                                                                                    #endif

                                                                                                                                    // -- Properties used by SceneSelectionPass
                                                                                                                                    #ifdef SCENESELECTIONPASS
                                                                                                                                    int _ObjectId;
                                                                                                                                    int _PassValue;
                                                                                                                                    #endif

                                                                                                                                    // Graph Functions
                                                                                                                                    // GraphFunctions: <None>

                                                                                                                                    // Custom interpolators pre vertex
                                                                                                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                                                                    // Graph Vertex
                                                                                                                                    struct VertexDescription
                                                                                                                                    {
                                                                                                                                        float3 Position;
                                                                                                                                        float3 Normal;
                                                                                                                                        float3 Tangent;
                                                                                                                                    };

                                                                                                                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                                                                    {
                                                                                                                                        VertexDescription description = (VertexDescription)0;
                                                                                                                                        description.Position = IN.ObjectSpacePosition;
                                                                                                                                        description.Normal = IN.ObjectSpaceNormal;
                                                                                                                                        description.Tangent = IN.ObjectSpaceTangent;
                                                                                                                                        return description;
                                                                                                                                    }

                                                                                                                                    // Custom interpolators, pre surface
                                                                                                                                    #ifdef FEATURES_GRAPH_VERTEX
                                                                                                                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                                                                    {
                                                                                                                                    return output;
                                                                                                                                    }
                                                                                                                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                                                                    #endif

                                                                                                                                    // Graph Pixel
                                                                                                                                    struct SurfaceDescription
                                                                                                                                    {
                                                                                                                                    };

                                                                                                                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                                                                    {
                                                                                                                                        SurfaceDescription surface = (SurfaceDescription)0;
                                                                                                                                        return surface;
                                                                                                                                    }

                                                                                                                                    // --------------------------------------------------
                                                                                                                                    // Build Graph Inputs
                                                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                                                    #define VFX_SRP_ATTRIBUTES Attributes
                                                                                                                                    #define VFX_SRP_VARYINGS Varyings
                                                                                                                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                                                                    #endif
                                                                                                                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                                                                    {
                                                                                                                                        VertexDescriptionInputs output;
                                                                                                                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                                                                        output.ObjectSpaceNormal = input.normalOS;
                                                                                                                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                                                                        output.ObjectSpacePosition = input.positionOS;

                                                                                                                                        return output;
                                                                                                                                    }
                                                                                                                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                                                                    {
                                                                                                                                        SurfaceDescriptionInputs output;
                                                                                                                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                                                        // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                                                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                                                                    #endif







                                                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                                                    #else
                                                                                                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                                                                    #endif
                                                                                                                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                                                                            return output;
                                                                                                                                    }

                                                                                                                                    // --------------------------------------------------
                                                                                                                                    // Main

                                                                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                                                                                                                    // --------------------------------------------------
                                                                                                                                    // Visual Effect Vertex Invocations
                                                                                                                                    #ifdef HAVE_VFX_MODIFICATION
                                                                                                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                                                                    #endif

                                                                                                                                    ENDHLSL
                                                                                                                                    }
                                                                                                                                    Pass
                                                                                                                                    {
                                                                                                                                        // Name: <None>
                                                                                                                                        Tags
                                                                                                                                        {
                                                                                                                                            "LightMode" = "Universal2D"
                                                                                                                                        }

                                                                                                                                        // Render State
                                                                                                                                        Cull Back
                                                                                                                                        Blend One Zero
                                                                                                                                        ZTest Off
                                                                                                                                        ZWrite On

                                                                                                                                        // Debug
                                                                                                                                        // <None>

                                                                                                                                        // --------------------------------------------------
                                                                                                                                        // Pass

                                                                                                                                        HLSLPROGRAM

                                                                                                                                        // Pragmas
                                                                                                                                        #pragma target 2.0
                                                                                                                                        #pragma only_renderers gles gles3 glcore d3d11
                                                                                                                                        #pragma multi_compile_instancing
                                                                                                                                        #pragma vertex vert
                                                                                                                                        #pragma fragment frag

                                                                                                                                        // DotsInstancingOptions: <None>
                                                                                                                                        // HybridV1InjectedBuiltinProperties: <None>

                                                                                                                                        // Keywords
                                                                                                                                        // PassKeywords: <None>
                                                                                                                                        // GraphKeywords: <None>

                                                                                                                                        // Defines

                                                                                                                                        #define _NORMALMAP 1
                                                                                                                                        #define _NORMAL_DROPOFF_TS 1
                                                                                                                                        #define ATTRIBUTES_NEED_NORMAL
                                                                                                                                        #define ATTRIBUTES_NEED_TANGENT
                                                                                                                                        #define VARYINGS_NEED_POSITION_WS
                                                                                                                                        #define FEATURES_GRAPH_VERTEX
                                                                                                                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                                                                                                        #define SHADERPASS SHADERPASS_2D
                                                                                                                                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                                                                                                        // custom interpolator pre-include
                                                                                                                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                                                                                                        // Includes
                                                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                                                                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                                                                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                                                                                                        // --------------------------------------------------
                                                                                                                                        // Structs and Packing

                                                                                                                                        // custom interpolators pre packing
                                                                                                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                                                                                                        struct Attributes
                                                                                                                                        {
                                                                                                                                                float3 positionOS : POSITION;
                                                                                                                                                float3 normalOS : NORMAL;
                                                                                                                                                float4 tangentOS : TANGENT;
                                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                                uint instanceID : INSTANCEID_SEMANTIC;
                                                                                                                                            #endif
                                                                                                                                        };
                                                                                                                                        struct Varyings
                                                                                                                                        {
                                                                                                                                                float4 positionCS : SV_POSITION;
                                                                                                                                                float3 positionWS;
                                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                                            #endif
                                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                                            #endif
                                                                                                                                        };
                                                                                                                                        struct SurfaceDescriptionInputs
                                                                                                                                        {
                                                                                                                                                float3 WorldSpacePosition;
                                                                                                                                        };
                                                                                                                                        struct VertexDescriptionInputs
                                                                                                                                        {
                                                                                                                                                float3 ObjectSpaceNormal;
                                                                                                                                                float3 ObjectSpaceTangent;
                                                                                                                                                float3 ObjectSpacePosition;
                                                                                                                                        };
                                                                                                                                        struct PackedVaryings
                                                                                                                                        {
                                                                                                                                                float4 positionCS : SV_POSITION;
                                                                                                                                                float3 interp0 : INTERP0;
                                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                                uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                                                                                            #endif
                                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                                                            #endif
                                                                                                                                        };

                                                                                                                                        PackedVaryings PackVaryings(Varyings input)
                                                                                                                                        {
                                                                                                                                            PackedVaryings output;
                                                                                                                                            ZERO_INITIALIZE(PackedVaryings, output);
                                                                                                                                            output.positionCS = input.positionCS;
                                                                                                                                            output.interp0.xyz = input.positionWS;
                                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                            output.instanceID = input.instanceID;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                                            #endif
                                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                            output.cullFace = input.cullFace;
                                                                                                                                            #endif
                                                                                                                                            return output;
                                                                                                                                        }

                                                                                                                                        Varyings UnpackVaryings(PackedVaryings input)
                                                                                                                                        {
                                                                                                                                            Varyings output;
                                                                                                                                            output.positionCS = input.positionCS;
                                                                                                                                            output.positionWS = input.interp0.xyz;
                                                                                                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                                                            output.instanceID = input.instanceID;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                                                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                                                                                            #endif
                                                                                                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                                                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                                                                                            #endif
                                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                            output.cullFace = input.cullFace;
                                                                                                                                            #endif
                                                                                                                                            return output;
                                                                                                                                        }


                                                                                                                                        // --------------------------------------------------
                                                                                                                                        // Graph

                                                                                                                                        // Graph Properties
                                                                                                                                        CBUFFER_START(UnityPerMaterial)
                                                                                                                                        float4 Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                                                        float4 Color_adc979e474cd4c52803b73073550b88e;
                                                                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                                                        float Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                                                        float Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                                        float4 Texture2D_93021b81bc604a779a4dd2f83b95f849_TexelSize;
                                                                                                                                        float Vector1_ac9ee0e7cc6f41dda544b0e89518d65a;
                                                                                                                                        float Vector1_6ef3f7747728449dae573951cca43c42;
                                                                                                                                        float Vector1_59d4fb46a8c44151b3b0ea55014c4b35;
                                                                                                                                        CBUFFER_END

                                                                                                                                            // Object and Global properties
                                                                                                                                            SAMPLER(SamplerState_Linear_Repeat);
                                                                                                                                            TEXTURE2D(Texture2D_93021b81bc604a779a4dd2f83b95f849);
                                                                                                                                            SAMPLER(samplerTexture2D_93021b81bc604a779a4dd2f83b95f849);

                                                                                                                                            // Graph Includes
                                                                                                                                            // GraphIncludes: <None>

                                                                                                                                            // -- Property used by ScenePickingPass
                                                                                                                                            #ifdef SCENEPICKINGPASS
                                                                                                                                            float4 _SelectionID;
                                                                                                                                            #endif

                                                                                                                                            // -- Properties used by SceneSelectionPass
                                                                                                                                            #ifdef SCENESELECTIONPASS
                                                                                                                                            int _ObjectId;
                                                                                                                                            int _PassValue;
                                                                                                                                            #endif

                                                                                                                                            // Graph Functions

                                                                                                                                            void Unity_Subtract_float(float A, float B, out float Out)
                                                                                                                                            {
                                                                                                                                                Out = A - B;
                                                                                                                                            }

                                                                                                                                            void Unity_Add_float(float A, float B, out float Out)
                                                                                                                                            {
                                                                                                                                                Out = A + B;
                                                                                                                                            }

                                                                                                                                            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                                                                                                                                            {
                                                                                                                                                Out = smoothstep(Edge1, Edge2, In);
                                                                                                                                            }

                                                                                                                                            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                                                                                                            {
                                                                                                                                                Out = A * B;
                                                                                                                                            }

                                                                                                                                            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                                                                                                                                            {
                                                                                                                                                Out = A + B;
                                                                                                                                            }

                                                                                                                                            // Custom interpolators pre vertex
                                                                                                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                                                                                                            // Graph Vertex
                                                                                                                                            struct VertexDescription
                                                                                                                                            {
                                                                                                                                                float3 Position;
                                                                                                                                                float3 Normal;
                                                                                                                                                float3 Tangent;
                                                                                                                                            };

                                                                                                                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                                                                                                            {
                                                                                                                                                VertexDescription description = (VertexDescription)0;
                                                                                                                                                description.Position = IN.ObjectSpacePosition;
                                                                                                                                                description.Normal = IN.ObjectSpaceNormal;
                                                                                                                                                description.Tangent = IN.ObjectSpaceTangent;
                                                                                                                                                return description;
                                                                                                                                            }

                                                                                                                                            // Custom interpolators, pre surface
                                                                                                                                            #ifdef FEATURES_GRAPH_VERTEX
                                                                                                                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                                                                                                            {
                                                                                                                                            return output;
                                                                                                                                            }
                                                                                                                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                                                                                                            #endif

                                                                                                                                            // Graph Pixel
                                                                                                                                            struct SurfaceDescription
                                                                                                                                            {
                                                                                                                                                float3 BaseColor;
                                                                                                                                            };

                                                                                                                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                                                                            {
                                                                                                                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                                                                                                                float4 _Property_271638d18c864c50adc2de9ba4cf9707_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_c4b6043ac1de492fb8bbd1482b723534) : Color_c4b6043ac1de492fb8bbd1482b723534;
                                                                                                                                                float _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                                                float _Property_910d825da2eb46b0a1754b9e69252646_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a;
                                                                                                                                                float _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2;
                                                                                                                                                Unity_Subtract_float(_Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Property_910d825da2eb46b0a1754b9e69252646_Out_0, _Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2);
                                                                                                                                                float _Add_bca707404b3243949fe8953b161e1601_Out_2;
                                                                                                                                                Unity_Add_float(-1, _Property_c351a276a6cb4677b2d251b9f28a2271_Out_0, _Add_bca707404b3243949fe8953b161e1601_Out_2);
                                                                                                                                                float _Split_4c5a12ca469049ee8d25c52df86199d7_R_1 = IN.WorldSpacePosition[0];
                                                                                                                                                float _Split_4c5a12ca469049ee8d25c52df86199d7_G_2 = IN.WorldSpacePosition[1];
                                                                                                                                                float _Split_4c5a12ca469049ee8d25c52df86199d7_B_3 = IN.WorldSpacePosition[2];
                                                                                                                                                float _Split_4c5a12ca469049ee8d25c52df86199d7_A_4 = 0;
                                                                                                                                                float _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3;
                                                                                                                                                Unity_Smoothstep_float(_Subtract_d8e4c958168b4552be426928b2dda1f8_Out_2, _Add_bca707404b3243949fe8953b161e1601_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3);
                                                                                                                                                float4 _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2;
                                                                                                                                                Unity_Multiply_float4_float4(_Property_271638d18c864c50adc2de9ba4cf9707_Out_0, (_Smoothstep_f28099e6ff0141a39858497e181b7c80_Out_3.xxxx), _Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2);
                                                                                                                                                float4 _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2;
                                                                                                                                                Unity_Multiply_float4_float4(_Multiply_a792759f0e5b48c9acce5c2adca1b7af_Out_2, float4(2, 2, 2, 2), _Multiply_9d41234165c1425c8005aa668f597e6c_Out_2);
                                                                                                                                                float4 _Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_adc979e474cd4c52803b73073550b88e) : Color_adc979e474cd4c52803b73073550b88e;
                                                                                                                                                float _Property_429fa9faf52b49c891efaa416fe2c903_Out_0 = Vector1_9ab9734743834cf3842959c683cd7be1;
                                                                                                                                                float _Property_73479999c2b346edb73436833d483856_Out_0 = Vector1_f4346caac0c24533843067b1caa8641a_1;
                                                                                                                                                float _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2;
                                                                                                                                                Unity_Subtract_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, _Property_73479999c2b346edb73436833d483856_Out_0, _Subtract_72abdf79c34d4aacadfe832896a30461_Out_2);
                                                                                                                                                float _Add_554d292e79d145489ad3cdc7084a70ca_Out_2;
                                                                                                                                                Unity_Add_float(_Property_429fa9faf52b49c891efaa416fe2c903_Out_0, -1, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2);
                                                                                                                                                float _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3;
                                                                                                                                                Unity_Smoothstep_float(_Subtract_72abdf79c34d4aacadfe832896a30461_Out_2, _Add_554d292e79d145489ad3cdc7084a70ca_Out_2, _Split_4c5a12ca469049ee8d25c52df86199d7_G_2, _Smoothstep_919b4e994d44484590da2d34a073773c_Out_3);
                                                                                                                                                float4 _Multiply_f02e57049b254438874c61113b259afa_Out_2;
                                                                                                                                                Unity_Multiply_float4_float4(_Property_f5bcafcb06c44a489b4d842ed470bbeb_Out_0, (_Smoothstep_919b4e994d44484590da2d34a073773c_Out_3.xxxx), _Multiply_f02e57049b254438874c61113b259afa_Out_2);
                                                                                                                                                float4 _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2;
                                                                                                                                                Unity_Add_float4(_Multiply_9d41234165c1425c8005aa668f597e6c_Out_2, _Multiply_f02e57049b254438874c61113b259afa_Out_2, _Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2);
                                                                                                                                                surface.BaseColor = (_Add_32440e02d5e443a6b5a5c7a7cb675ceb_Out_2.xyz);
                                                                                                                                                return surface;
                                                                                                                                            }

                                                                                                                                            // --------------------------------------------------
                                                                                                                                            // Build Graph Inputs
                                                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                                                            #define VFX_SRP_ATTRIBUTES Attributes
                                                                                                                                            #define VFX_SRP_VARYINGS Varyings
                                                                                                                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                                                                                                            #endif
                                                                                                                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                                                                                                            {
                                                                                                                                                VertexDescriptionInputs output;
                                                                                                                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                                                                                                output.ObjectSpaceNormal = input.normalOS;
                                                                                                                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                                                                                                output.ObjectSpacePosition = input.positionOS;

                                                                                                                                                return output;
                                                                                                                                            }
                                                                                                                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                                                                                                            {
                                                                                                                                                SurfaceDescriptionInputs output;
                                                                                                                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                                                                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                                                                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                                                                                                            #endif







                                                                                                                                                output.WorldSpacePosition = input.positionWS;
                                                                                                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                                                            #else
                                                                                                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                                                                                                            #endif
                                                                                                                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                                                                                                    return output;
                                                                                                                                            }

                                                                                                                                            // --------------------------------------------------
                                                                                                                                            // Main

                                                                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                                                                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

                                                                                                                                            // --------------------------------------------------
                                                                                                                                            // Visual Effect Vertex Invocations
                                                                                                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                                                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                                                                                                            #endif

                                                                                                                                            ENDHLSL
                                                                                                                                            }
                                                                            }
                                                                                CustomEditorForRenderPipeline "UnityEditor.ShaderGraphLitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
                                                                                                                                                CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                                                                                                                                FallBack "Hidden/Shader Graph/FallbackError"

}
