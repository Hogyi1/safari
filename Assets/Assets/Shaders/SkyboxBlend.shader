Shader "Custom/SkyboxBlend"
{
    Properties
    {
        _CubemapDay ("Day Cubemap", CUBE) = "" {}
        _CubemapNight ("Night Cubemap", CUBE) = "" {}
        _Blend ("Blend", Range(0,1)) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Background"
            "RenderType"="Background"
            "IgnoreProjector"="True"
            "PreviewType"="Skybox"
        }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _CubemapDay;
            samplerCUBE _CubemapNight;
            float       _Blend;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                // We rely on Unity drawing a cube mesh for the skybox,
                // passing the vertex position as a direction vector.
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalize the direction from the cube’s vertex.
                float3 worldDir = normalize(i.texcoord);

                // Sample both cubemaps along that direction.
                fixed4 colDay   = texCUBE(_CubemapDay,   worldDir);
                fixed4 colNight = texCUBE(_CubemapNight, worldDir);

                // Linearly blend based on _Blend (0=day, 1=night).
                return lerp(colDay, colNight, _Blend);
            }
            ENDCG
        }
    }
    Fallback Off
}
