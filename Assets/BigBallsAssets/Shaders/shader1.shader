Shader "Unlit/LightningLine"
{
    Properties
    {
        _MainTex ("Sprite Sheet", 2D) = "white" {}
        _Columns ("Columns", Float) = 2
        _Rows ("Rows", Float) = 2
        _Speed ("Speed", Float) = 12.0
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
            float _Speed;

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
                // Номер кадра (от 0 до _Columns*_Rows-1)
                float totalFrames = _Columns * _Rows;
                float frame = fmod(floor(_Time.y * _Speed), totalFrames);

                // Координаты кадра в сетке: столбец и строка
                float col = fmod(frame, _Columns);
                float row = floor(frame / _Columns);   // снизу вверх

                // Можно инвертировать строку, если текстура упакована сверху вниз
                // row = _Rows - 1 - row;

                // Пересчитываем UV в границы ячейки
                float2 spriteUV = float2(
                    (i.uv.x + col) / _Columns,
                    (i.uv.y + row) / _Rows
                );

                fixed4 tex = tex2D(_MainTex, spriteUV);
                return tex * i.color;
            }
            ENDCG
        }
    }
}