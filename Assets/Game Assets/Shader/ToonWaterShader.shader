Shader "Custom/Toon/Water"
{
    Properties
    {
        _WaterColorDeep ("Deep Water Color", Color) = (.086, .407, 1, .749)
        _WaterColorShallow ("Shallow Water Color", Color) = (.325, .807, .971, .725)
        _DepthMaxDistance("Depth Max Distance", Float) = 1

        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

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
                float4 vertex : SV_POSITION;
                float4 screenPosition : TEXCOORD2;
            };

            fixed4 _WaterColorDeep;
            fixed4 _WaterColorShallow;
            float _DepthMaxDistance;

            // Created by Camera DepthTextureMode
            // and Created by NormalsShader as global texture
            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalstexture;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {   
                // This code works the same as tex2Dproj
                // float existingDepth01 = tex2D(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition.xy / i.screenPosition.w)).r;
                
                // Grab Depth value from depth texture, convert it to linear as tex2Dproj returns non-linear values
                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01);

                // depth from depthTexture - depth from vertex
                float depthDifference = existingDepthLinear - i.screenPosition.w;

                // saturate: clamps value between 0 and 1
                // use a proportion of the max distance as lerp value between both colors
                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
                fixed4 waterColor = lerp(_WaterColorShallow, _WaterColorDeep, waterDepthDifference01);

                return waterColor;
            }
            ENDCG
        }
    }
}
