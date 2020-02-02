Shader "Unlit/DeathZoneEffect"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Strength("intensity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZTest Always
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 screenPos : TEXCOORD0;
            };

            fixed4 _Color;
            float _Strength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                col.a = pow(distance(float2(0.5, 0.5), i.screenPos),2)*2 * _Strength;
                //col.rg = i.screenPos;
                return col;
            }
            ENDCG
        }
    }
}
