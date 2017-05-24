Shader "Unlit/TestShaft"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowTex("Texture", 2D) = "white" {}
		_LightZBufferParams("_LightZBufferParams", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"}
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
				float4 lightPPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _ShadowTex;
			float4 _MainTex_ST;
			float4 vMinBound, vMaxBound;

			float4 _LightZBufferParams;

			float4x4 viewToWorldMat;
			float4x4 worldToViewMat;
			float4x4 worldToLightMat;
			float4x4 lightToWorldMat;
			float4x4 lightProjectionMat;

			
			v2f vert (appdata v)
			{
				v2f o;
				float4 pos = vMinBound * v.vertex + vMaxBound * (1 - v.vertex);
				pos.w = 1;
				pos = mul(viewToWorldMat, pos);
				o.lightPos = mul(worldToLightMat, pos);
				o.lightPPos = mul(lightProjectionMat, o.lightPos);
				o.vertex = mul(UNITY_MATRIX_VP, pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 lightUV = i.lightPPos.xy / -i.lightPPos.w * 0.5 + 0.5;
				float sampleDepthZ = 1 - tex2D(_ShadowTex, lightUV).r;
				float sampleDepth = 1.0 / (_LightZBufferParams.z * sampleDepthZ + _LightZBufferParams.w);

				float b = step(i.lightPos.z, sampleDepth);
				float xx = abs(i.lightPos.z - sampleDepth);

				float mark = step(dot(i.lightPos.xy, i.lightPos.xy), 5);
				float atten = 1.0 / dot(i.lightPos.xyz, i.lightPos.xyz);

				// return fixed4(b, 0, 0, 1);
				// return fixed4(i.lightPos.z-1, 0, 0, 1);
				// return fixed4(sampleDepth-1.5, 0, 0, sampleDepth-1.5);
				return fixed4(b*mark*atten, 0, 0, 1);
				// return fixed4(0.1 * mark, 0.1 * mark, 0.1 * mark, 1);
			}
			ENDCG
		}
	}
}
