// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Billboard" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _VerticalBillboarding ("Vertical Restraints", Range(0, 1)) = 1 
    }
    SubShader {
        // Need to disable batching because of the vertex animation
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "DisableBatching"="True"}
        Pass { 
            Tags { "LightMode"="ForwardBase" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _VerticalBillboarding;
            struct a2v {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            v2f vert (a2v v) {
                v2f o;
                // 锚点
                float3 center = float3(0, 0, 0);
                // 模型空间的观察方向
                float3 viewer = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos, 1));
                // 模型空间的法线方向
                float3 normalDir = viewer - center;
                //计算3个正交矢量。首先，我们根据观察位置和锚点计算目标法线方向，  
                //并根据_VerticalBillboarding 属性来控制垂直方向上的约束度。
                //如果 _VerticalBillboarding 等于 1, 意味着法线方向固定为视角方向  
                //如果 _VerticalBillboarding 等于 0, 意味着向上方向固定为（0,1,0）  
                normalDir.y =normalDir.y * _VerticalBillboarding;
                normalDir = normalize(normalDir);
                //我们得到了粗略的向上方向。为了防止法线方向和向上方向平行  
                //我们对法线方向的y分量进行判断，以得到合适的向上方向。然后，根据法线方向  
                //和粗略的向上方向得到向右方向，并对结果进行归一化。但由于此时向上的方向还是不  
                //准确的，我们又根据准确的法线方向和向右方向得到最后的向上方向  
                float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
                float3 rightDir = normalize(cross(upDir, normalDir));
                upDir = normalize(cross(normalDir, rightDir));
                //我们根据原始的位置相对于锚点的偏移量以及3个正交基矢量，以计算得到新的顶点位置。  
                float3 centerOffs = v.vertex.xyz - center;
                float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;
                //最后，把模型空间的顶点位置变换到裁剪空间中  
                o.pos = UnityObjectToClipPos(float4(localPos, 1));
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target {
                fixed4 c = tex2D (_MainTex, i.uv);
                c.rgb *= _Color.rgb;
                return c;
            }
            ENDCG
        }
    } 
    FallBack "Transparent/VertexLit"
}