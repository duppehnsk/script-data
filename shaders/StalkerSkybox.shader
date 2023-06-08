Shader "X-Ray/StalkerSkybox"
{
    Properties
    {
        _Cube1 ("Skybox Cubemap 1", Cube) = "" {}
        _Cube2 ("Skybox Cubemap 2", Cube) = "" {}
        _Blend ("Blend", Range(0.0, 1.0)) = 0.0
        _Rotation ("Rotation", Range(0.0, 1.0)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Cube1;
            samplerCUBE _Cube2;
            float _Blend;
            float _Rotation;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldDirection : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldDirection = normalize(UnityObjectToWorldNormal(v.vertex));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldDir = normalize(i.worldDirection);

                // Rotate the world direction
                float angle = _Rotation * 360.0;
                float s = sin(angle * 0.0174533);
                float c = cos(angle * 0.0174533);
                float3x3 rotationMatrix = float3x3(c, 0, s, 0, 1, 0, -s, 0, c);
                worldDir = mul(rotationMatrix, worldDir);

                // Blend the two cubemaps
                fixed4 color1 = texCUBE(_Cube1, worldDir);
                fixed4 color2 = texCUBE(_Cube2, worldDir);
                fixed4 finalColor = lerp(color1, color2, _Blend);

                return finalColor;
            }
            ENDCG
        }
    }
}