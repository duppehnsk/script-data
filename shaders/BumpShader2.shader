Shader "X-Ray/BumpShader2" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Parallax ("Height", Range (0.01, 0.08)) = 0.02
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _ParallaxMap ("Heightmap (A)", 2D) = "black" {}
        _Wetness ("Wetness", Range (0, 1)) = 0
        _WetnessMap ("Wetness Map", 2D) = "white" {}
        _WetnessOffset ("Wetness Offset", Range (0, 1)) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 500

        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _ParallaxMap;
        sampler2D _WetnessMap;
        fixed4 _Color;
        float _Parallax;
        float _Wetness;
        float _WetnessOffset;

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_WetnessMap;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half h = tex2D(_ParallaxMap, IN.uv_BumpMap).w;
            float2 offset = ParallaxOffset(h, _Parallax, IN.viewDir);
            IN.uv_MainTex += offset;
            IN.uv_BumpMap += offset;

            float2 wetnessOffset = float2(0, -_WetnessOffset);

            // Apply wetness based on the wetness map with offset
            float wetness = tex2D(_WetnessMap, IN.uv_WetnessMap + wetnessOffset).r;
            wetness = saturate(wetness); // Clamp wetness value between 0 and 1
            float wetnessIntensity = lerp(1.0, 1.5, wetness * _Wetness);
            
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            
            // Preserve original color when wetness is zero
            c.rgb *= wetnessIntensity + (1.0 - _Wetness);
            
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * 2.0 - 1.0;
        }
        ENDCG
    }

    FallBack "Legacy Shaders/Bumped Diffuse"
}