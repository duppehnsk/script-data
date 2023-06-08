Shader "X-Ray/WaterShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Glossiness", Range(0, 1)) = 0.5
        _ReflectionTex ("Reflection Texture", 2D) = "white" {}
        _RefractionTex ("Refraction Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _ReflectionTex;
        sampler2D _RefractionTex;
        sampler2D _NormalMap;
        fixed4 _Color;
        half _Glossiness;
        half _WaveSpeed;

        struct Input {
            float2 uv_ReflectionTex;
            float2 uv_RefractionTex;
            float2 uv_NormalMap;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 reflTex = tex2D(_ReflectionTex, IN.uv_ReflectionTex);
            fixed4 refrTex = tex2D(_RefractionTex, IN.uv_RefractionTex);
            fixed3 normalMap = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            
            // Simulate water waves using time and UV coordinates
            float waveFactor = sin(_WaveSpeed * _Time.y + IN.uv_RefractionTex.x * 10);
            
            // Combine reflection and refraction colors
            fixed4 color = lerp(reflTex, refrTex, waveFactor);
            
            o.Albedo = color.rgb * _Color.rgb;
            o.Alpha = color.a * _Color.a;
            o.Specular = _Glossiness;
            o.Normal = normalMap;
        }
        ENDCG
    }
    FallBack "Diffuse"
}