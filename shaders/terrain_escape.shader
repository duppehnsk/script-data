Shader "X-Ray/TerrainShader2.0" {
    Properties{
            _MainTex("Base (RGB)", 2D) = "white" {}
            _MaskTex("Mask (RGB)", 2D) = "red" {}
            _DetailedGrassTex("Detailed Grass (RGB)", 2D) = "white" {}
            _DetailedAsphaltTex("Detailed Asphalt (RGB)", 2D) = "white" {}
            _DetailedDirtTex("Detailed Dirt (RGB)", 2D) = "white" {}
            _BumpMapGrass("Nopmal Map Grass", 2D) = "bump" {}
            _BumpMapAsphalt("Nopmal Map Asphalt", 2D) = "bump" {}
            _BumpMapDirt("Nopmal Map Dirt", 2D) = "bump" {}
    }
        SubShader{
                Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
                LOD 200

                CGPROGRAM
                #pragma surface surf Lambert

                sampler2D _MainTex;
                sampler2D _MaskTex;
                sampler2D _DetailedGrassTex;
                sampler2D _DetailedAsphaltTex;
                sampler2D _DetailedDirtTex;
                sampler2D _BumpMapGrass;
                sampler2D _BumpMapAsphalt;
                sampler2D _BumpMapDirt;

                struct Input {
                        float2 uv_MainTex;
                        float2 uv_DetailedGrassTex;
                        float2 uv_DetailedAsphaltTex;
                        float2 uv_DetailedDirtTex;

                        // float2 uv_BumpMap;
                };
                float3 blend(float4 tex_red, float red, float4 tex_green, float green, float4 tex_blue, float blue) {
                        return (tex_red.rgb * red + tex_green.rgb * green + tex_blue.rgb * blue) / (red + green + blue) * 2;
                }
                void surf(Input IN, inout SurfaceOutput o) {
                        fixed4 mask_c = tex2D(_MaskTex, IN.uv_MainTex);

                        o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb *
                                                  blend(tex2D(_DetailedGrassTex,IN.uv_DetailedGrassTex),
                                                                mask_c.r,
                                                                tex2D(_DetailedAsphaltTex,IN.uv_DetailedAsphaltTex),
                                                                mask_c.g,
                                                                tex2D(_DetailedDirtTex,IN.uv_DetailedDirtTex),
                                                                mask_c.b);

                        o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;

                        o.Normal = UnpackNormal(tex2D(_BumpMapGrass, IN.uv_DetailedGrassTex)) +
                            UnpackNormal(tex2D(_BumpMapAsphalt, IN.uv_DetailedAsphaltTex)) +
                            UnpackNormal(tex2D(_BumpMapDirt, IN.uv_DetailedDirtTex));

                        // o.Normal = float3(1445.0, 1445.0, 1.0);
                }
                ENDCG
    }
        FallBack "Diffuse"
}
