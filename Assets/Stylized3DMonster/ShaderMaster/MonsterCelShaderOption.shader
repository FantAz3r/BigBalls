Shader "PVFX/PVFX_URP_Low_CelShader1.1_Detail"
{
    Properties
    {
        [Header(Base Map)]
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)

        [Header(Detail Map)]
        _DetailTex ("Detail Texture", 2D) = "gray" {}
        _DetailColor ("Detail Color", Color) = (1,1,1,1)
        _DetailStrength ("Detail Strength", Range(0,2)) = 1
        _DetailTiling ("Detail Tiling (XY) / Offset (ZW)", Vector) = (1,1,0,0)
        [KeywordEnum(Multiply, Overlay, Add)] _DetailMode ("Detail Blend Mode", Float) = 0

        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.005

        [Header(Rim Lighting)]
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.5,10)) = 3
        _RimIntensity ("Rim Intensity", Range(0,3)) = 1

        [Header(Metallic and Emission)]
        _MetallicStrength ("Metallic Strength", Range(0,1)) = 0
        _MetallicColor ("Metallic Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)

        [Header(Cel Shading)]
        _ShadeThreshold ("Shade Threshold", Range(0,1)) = 0
        _ShadeIntensity ("Shade Intensity", Range(0,1)) = 0
        _ShadeColor ("Shade Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        // ------------------------------------------------------------
        // PASS 1: OUTLINE
        // ------------------------------------------------------------
        Pass
        {
            Name "OUTLINE"
            Cull Front
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float4 pos = v.vertex;
                pos.xyz += norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // ------------------------------------------------------------
        // PASS 2: BASE LIGHTING & EFFECTS
        // ------------------------------------------------------------
        Pass
        {
            Name "BASE"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _DETAILMODE_MULTIPLY _DETAILMODE_OVERLAY _DETAILMODE_ADD
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            sampler2D _DetailTex;
            float4 _DetailTex_ST;
            fixed4 _DetailColor;
            float _DetailStrength;
            float4 _DetailTiling;

            fixed4 _RimColor;
            float _RimPower;
            float _RimIntensity;

            float _MetallicStrength;
            fixed4 _MetallicColor;
            fixed4 _EmissionColor;

            float _ShadeThreshold;
            float _ShadeIntensity;
            fixed4 _ShadeColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float2 uvDetail : TEXCOORD3;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Detail UV: собственный tiling/offset, масштабируется _DetailTiling
                o.uvDetail = TRANSFORM_TEX(v.uv, _DetailTex) * _DetailTiling.xy + _DetailTiling.zw;
                return o;
            }

            // Функция наложения detail-карты
            fixed3 ApplyDetail(fixed3 baseCol, fixed3 detailCol)
            {
                fixed3 d = detailCol * _DetailColor.rgb;
                // "нейтральное" значение = 0.5 (серая карта не меняет цвет)
                fixed3 dSigned = d * 2.0 - 1.0;

                #if defined(_DETAILMODE_OVERLAY)
                    // Overlay: контрастнее, хорошо для трещин
                    fixed3 overlay = lerp(
                        2.0 * baseCol * d,
                        1.0 - 2.0 * (1.0 - baseCol) * (1.0 - d),
                        step(0.5, baseCol)
                    );
                    return lerp(baseCol, overlay, _DetailStrength);
                #elif defined(_DETAILMODE_ADD)
                    return baseCol + dSigned * _DetailStrength;
                #else
                    // Multiply (по умолчанию)
                    return baseCol * lerp(1.0, d, _DetailStrength);
                #endif
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = saturate(dot(normal, lightDir));

                // Cel Shading Logic
                float shadeMask = step(NdotL, _ShadeThreshold);
                float litMask = 1.0 - shadeMask;

                fixed3 litCol = baseCol.rgb;
                fixed3 shadeCol = baseCol.rgb * _ShadeColor.rgb * (1.0 - _ShadeIntensity);
                fixed3 lightingCol = (litCol * litMask) + (shadeCol * shadeMask);

                // === DETAIL MAP (трещины) ===
                fixed3 detailSample = tex2D(_DetailTex, i.uvDetail).rgb;
                lightingCol = ApplyDetail(lightingCol, detailSample);
                lightingCol = saturate(lightingCol);

                // Rim Light
                float rim = pow(1.0 - saturate(dot(viewDir, normal)), _RimPower) * _RimIntensity;
                fixed3 rimCol = _RimColor.rgb * rim;

                // Specular
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(saturate(dot(normal, halfDir)), 32) * _MetallicStrength;
                fixed3 specCol = _MetallicColor.rgb * spec;

                fixed3 finalCol = lightingCol + rimCol + specCol + _EmissionColor.rgb;

                return fixed4(finalCol, 1.0);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}