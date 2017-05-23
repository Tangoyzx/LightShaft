Shader "Unlit/TestShaft"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="true"}
		LOD 100
		Cull Off
		Blend One One

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
				float3 lightPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 vMinBound, vMaxBound;
			float4x4 viewToWorldMat;
			float4x4 worldToViewMat;
			float4x4 worldToLightMat;
			float4x4 lightToWorldMat;

			
			v2f vert (appdata v)
			{
				v2f o;
				float4 pos = vMinBound * v.vertex + vMaxBound * (1 - v.vertex);
				pos.w = 1;
				pos = mul(viewToWorldMat, pos);
				o.lightPos = mul(worldToLightMat, pos);
				o.vertex = mul(UNITY_MATRIX_VP, pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// fixed4 col = tex2D(_MainTex, i.uv);
				float offset = max(0, 1 - dot(i.lightPos.xy, i.lightPos.xy) * 0.01)/length(i.lightPos.xyz);
				return fixed4(offset, offset, offset, 1);
			}
			ENDCG
		}
	}
}
