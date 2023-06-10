Shader "X-Ray/FoliageShader"
{
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Parallax ("Height", Range (0.005, 0.08)) = 0.02
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _ParallaxMap ("Heightmap (A)", 2D) = "black" {}
    }

    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 500

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _ParallaxMap;
        fixed4 _Color;
        float _Parallax;

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half h = tex2D(_ParallaxMap, IN.uv_BumpMap).a;
            float2 offset = ParallaxOffset(h, _Parallax, IN.viewDir);
            IN.uv_MainTex += offset;
            IN.uv_BumpMap += offset;

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)).rgb;
        }
        ENDCG
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 500

        CGPROGRAM
        #pragma surface surf Lambert nodynlightmap

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Legacy Shaders/Bumped Diffuse"
}
