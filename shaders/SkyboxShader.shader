Shader "X-Ray/SkyboxShader"
{
    Properties {
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
        _Exposure ("Exposure", Range(0.1, 10)) = 1.0
        _Rotation ("Rotation", Range(-180, 180)) = 0.0
        _MainTex ("Cubemap", Cube) = "white" {}
        _BlendTex ("Cubemap2", Cube) = "white" {}
        _BlendAmount ("Blend Amount", Range(0, 1)) = 0.0
    }
 
    SubShader {
        Tags { "RenderType"="Background" }
        Cull Off
        Fog { Mode Off }
        Lighting Off
 
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata {
                float4 vertex : POSITION;
            };
 
            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };
 
            float4x4 _CameraRotation;
            float4 _Tint;
            float _Exposure;
            float _Rotation;
            samplerCUBE _MainTex;
            samplerCUBE _BlendTex;
            float _BlendAmount;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldDir = mul((float3x3)_CameraRotation, v.vertex.xyz);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldDir = normalize(i.worldDir);
                float3 skyboxColor = texCUBE(_MainTex, worldDir).rgb;
                float3 blendColor = texCUBE(_BlendTex, worldDir).rgb;
                float3 finalColor = lerp(skyboxColor, blendColor, _BlendAmount);
                finalColor *= _Tint.rgb * _Exposure;
 
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}