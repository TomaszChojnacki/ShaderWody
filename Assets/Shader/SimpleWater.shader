Shader "Custom/BezierWaterWaves"
{
    Properties
    {
        // Tekstura wody
        _MainTex ("Texture", 2D) = "white" {}

        // Szybkoœæ fal
        _WaveSpeed ("Wave Speed", Float) = 0.1

        // Si³a ka¿dego z 4 p³atów Beziera (mo¿na je ³¹czyæ, ¿eby uzyskaæ ró¿ne kszta³ty fal)
        _WaveStrength1 ("Wave Strength 1", Float) = 0.05
        _WaveStrength2 ("Wave Strength 2", Float) = 0.05
        _WaveStrength3 ("Wave Strength 3", Float) = 0.05
        _WaveStrength4 ("Wave Strength 4", Float) = 0.05

        // Kolor i przezroczystoœæ wody
        _Color ("Water Color", Color) = (0, 0.5, 1, 0.5)
    }

    SubShader
    {
        // Kolejka renderowania i styl przezroczysty
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // Parametry blendowania i renderowania przezroczystoœci
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back
        Lighting Off

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            // Deklaracja w³aœciwoœci shaderowych
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveSpeed;
            float _WaveStrength1;
            float _WaveStrength2;
            float _WaveStrength3;
            float _WaveStrength4;
            fixed4 _Color;

            // Dane wejœciowe z mesha
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Dane przekazywane z vertex do fragment
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Funkcja licz¹ca wartoœæ pojedynczego p³ata Bezierowego
            float Bezier(float t, float p0, float p1, float p2, float p3)
            {
                float u = 1.0 - t;
                float tt = t * t;
                float uu = u * u;
                float uuu = uu * u;
                float ttt = tt * t;

                return uuu * p0 + 3.0 * uu * t * p1 + 3.0 * u * tt * p2 + ttt * p3;
            }

            // Vertex shader – modyfikuje wysokoœæ wierzcho³ków
            v2f vert (appdata v)
            {
                v2f o;

                float t = _Time.y * _WaveSpeed; // Czas u¿ywany do animacji

                float x = v.vertex.x;
                float z = v.vertex.z;

                // Normalizacja na zakres 0-1 i uzyskanie "ruchu"
                float bx = abs(sin(x + t));
                float bz = abs(sin(z - t));

                // Obliczenie 4 niezale¿nych p³atów Bezierowych
                float wave1 = Bezier(bx, 0, _WaveStrength1, -_WaveStrength1, 0);
                float wave2 = Bezier(bz, 0, _WaveStrength2, _WaveStrength2, 0);
                float wave3 = Bezier(bx, 0, -_WaveStrength3, _WaveStrength3, 0);
                float wave4 = Bezier(bz, 0, _WaveStrength4, -_WaveStrength4, 0);

                // Sumujemy fale i przesuwamy wysokoœæ
                float totalWave = wave1 + wave2 + wave3 + wave4;
                v.vertex.y += totalWave;

                // Przekszta³camy do przestrzeni ekranu i przekazujemy dalej
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Fragment shader – nak³ada teksturê + kolor
            fixed4 frag (v2f i) : SV_Target
            {
                // Pobierz kolor tekstury
                fixed4 tex = tex2D(_MainTex, i.uv);

                // Zastosuj kolor wody jako tint (mieszanie koloru + tekstury)
                return tex * _Color;
            }
            ENDCG
        }
    }
}
