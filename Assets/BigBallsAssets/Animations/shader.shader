Shader "Unlit/LightningLine"
{
    Properties
    {
        _MainTex ("Sprite Sheet", 2D) = "white" {}
        _Columns ("Columns", Float) = 2
        _Rows ("Rows", Float) = 2

        [Header(Animation Settings)]
        _AnimSpeed ("Animation Speed", Float) = 12.0

        [Header(Movement Settings)]
        _ScrollSpeed ("Scroll Speed (X/Y)", Vector) = (1, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Columns;
            float _Rows;
            float _AnimSpeed;
            float4 _ScrollSpeed; // x - скорость по горизонтали, y - по вертикали

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // --- 1. РАСЧЕТ СМЕЩЕНИЯ (SCROLLING) ---
                // Смещаем UV координаты во времени, чтобы текстура "ехала"
                float2 scrolledUV = i.uv + _Time.y * _ScrollSpeed.xy;

                // --- 2. РАСЧЕТ АНИМАЦИИ КАДРОВ ---
                float totalFrames = _Columns * _Rows;
                float cycleDuration = totalFrames / _AnimSpeed;
                float timeInCycle = fmod(_Time.y, cycleDuration);
                float frame = floor(timeInCycle * _AnimSpeed);

                float col = fmod(frame, _Columns);
                float row = floor(frame / _Columns);

                // --- 3. СБОРКА ИТОГОВЫХ UV ---
                // Мы берем "едущие" UV и накладываем на них сетку спрайтшита
                float2 spriteUV = float2(
                    (scrolledUV.x + col) / _Columns,
                    (scrolledUV.y + row) / _Rows
                );

                fixed4 tex = tex2D(_MainTex, spriteUV);
                return tex * i.color;
            }
            ENDCG
        }
    }
}
