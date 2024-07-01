Shader "X-Ray/WaterShader"
{
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NormalMaps ("Normal Maps", 2D) = "" {}
        _AtlasSize ("Atlas Size", Vector) = (1, 1, 0, 0)
        _DistortionStrength ("Distortion Strength", Range(0.0, 1.0)) = 0.1
        _Speed ("Speed", Range(0.0, 10.0)) = 1.0
        _Scale ("Scale", Range(0.0, 10.0)) = 1.0
        _Alpha ("Alpha", Range(0.0, 1.0)) = 1.0
    }

    //Blend SrcAlpha OneMinusSrcAlpha

    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _NormalMaps;
        float4 _AtlasSize;
        float _DistortionStrength;
        float _Speed;
        float _Scale;
        float _Alpha;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            float2 uv = IN.uv_MainTex * _Scale;
            float2 offset = float2(
                sin(_Time.y * _Speed + uv.x) * _DistortionStrength,
                cos(_Time.y * _Speed + uv.y) * _DistortionStrength
            );

            float4 texColor = tex2D(_MainTex, IN.uv_MainTex + offset);

            float2 atlasUV = float2(IN.uv_MainTex.x * _AtlasSize.x + offset.x, IN.uv_MainTex.y * _AtlasSize.y + offset.y);
            float3 normal = UnpackNormal(tex2D(_NormalMaps, atlasUV));

            o.Normal = normal;
            o.Albedo = texColor.rgb;
            o.Alpha = texColor.a * _Alpha;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
