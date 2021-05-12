Shader "Custom/HollogramShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _Color("Color Tint", Color) = (1,1,1,1)
        _ScrollSpeed ("Scroll Speed", Float) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            float _ScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                float2 uvSample = float2(i.uv.x, i.uv.y + _Time.y * _ScrollSpeed);
                fixed4 col = tex2D(_MainTex, uvSample);
                
                return fixed4(_Color.rgb, col.r);
            }
            ENDCG
        }
    }
}
