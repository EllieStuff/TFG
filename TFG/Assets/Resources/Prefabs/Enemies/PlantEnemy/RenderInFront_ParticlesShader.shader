Shader "Custom/RenderInFront_ParticlesShader"
{
    Properties{

        _Color("Main Color", Color) = (1,1,1,0.5)

        _MainTex("Base (RGB)", 2D) = "white" {}

    }

        SubShader{

            Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent+2000" }

            Lighting Off



            CGPROGRAM

            #pragma surface surf Lambert



            float4 _Color;

            sampler2D _MainTex;



            struct Input {

                float4 screenPos;

                float4 color : COLOR;

            };



            void surf(Input IN, inout SurfaceOutput o) {

                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                half4 c = tex2D(_MainTex, screenUV);

                c = c * _Color * IN.color;

                o.Albedo = c.rgb;

                o.Alpha = c.a;

            }

            ENDCG

    }
}
