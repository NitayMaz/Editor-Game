Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _ClickedColor("Clicked Outline Color", Color) = (1,0.5,0,1)
        _OutlineThickness("Outline Thickness (Pixels)", Float) = 1
        _ClickedThickness("Clicked Outline Thickness (Pixels)", Float) = 2
        _Clicked("Clicked", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            fixed4 _ClickedColor;
            float _OutlineThickness;
            float _ClickedThickness;
            float _Clicked;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float thicknessPixels = lerp(_OutlineThickness, _ClickedThickness, _Clicked);
                fixed4 outlineColor = lerp(_OutlineColor, _ClickedColor, _Clicked);

                // Distance from each edge
                float2 edgeDist = min(i.uv, 1.0 - i.uv);

                // Convert pixel thickness to UV space in both directions
                float2 thicknessUV = thicknessPixels * fwidth(i.uv);
                float2 outlineMask = step(edgeDist, thicknessUV);

                // If within thickness in either direction, draw outline
                float isOutline = max(outlineMask.x, outlineMask.y);

                fixed4 baseColor = i.color;
                float3 finalRGB = lerp(baseColor.rgb, outlineColor.rgb, isOutline);
                return fixed4(finalRGB, baseColor.a);
            }
            ENDCG
        }
    }
}