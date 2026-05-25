Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Blur Size", Range(0,0.01)) = 0
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _Color;
            float _Size;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 col = tex2D(_MainTex, uv) * 0.2;

                col += tex2D(_MainTex, uv + float2(_Size,0)) * 0.2;
                col += tex2D(_MainTex, uv - float2(_Size,0)) * 0.2;
                col += tex2D(_MainTex, uv + float2(0,_Size)) * 0.2;
                col += tex2D(_MainTex, uv - float2(0,_Size)) * 0.2;

                col *= _Color;

                return col;
            }
            ENDCG
        }
    }
}