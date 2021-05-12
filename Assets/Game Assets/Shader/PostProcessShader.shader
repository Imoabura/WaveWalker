Shader "Custom/PostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _TransitionColor ("Transition Color", Color) = (1, 1, 1, 1)
        _TransitionLerpValue ("Transition Lerp Value", Range(0.0, 1.0)) = 0

        _BnWLerpValue ("BnW Lerp Value", Range(0.0, 1.0)) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 _TransitionColor;
            float _TransitionLerpValue;

            float _BnWLerpValue;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 greyscale = (col.r + col.g + col.b) / 3;

                col = lerp(col, greyscale, _BnWLerpValue);
                col = lerp(col, _TransitionColor, _TransitionLerpValue);

                return col;
            }
            ENDCG
        }
    }
}
