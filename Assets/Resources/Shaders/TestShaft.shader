Shader "Unlit/TestShaft"
{
	Properties
	{
		_ShadowTex("Texture", 2D) = "white" {}
		_MaskTex("Texture", 2D) = "white" {}
		_NoiseTex("Texture", 2D) = "white" {}
		_LightZBufferParams("_LightZBufferParams", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent"}
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 lightPos : TEXCOORD1;
				float4 lightPPos : TEXCOORD2;
			};

			sampler2D _ShadowTex;
			sampler2D _MaskTex;
			sampler2D _NoiseTex;
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
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float area = 0.8;
				float2 lightUV = i.lightPPos.xy / -i.lightPPos.w;

				lightUV = lightUV * 0.5 + 0.5;

				fixed3 maskColor = tex2D(_MaskTex, lightUV).rgb;
				fixed3 noiseColor = tex2D(_NoiseTex, lightUV + _Time.xx * 3).rgb;

				float sampleDepthZ = 1 - tex2D(_ShadowTex, lightUV).r;
				float sampleDepth = 1.0 / (_LightZBufferParams.z * sampleDepthZ + _LightZBufferParams.w);

				float inShadow = step(i.lightPos.z, sampleDepth);

				float centerFalloff = step(0, lightUV.x) * step(lightUV.x, 1) * step(0, lightUV.y) * step(lightUV.y, 1);

				float atten = 1.0 / dot(i.lightPos.xyz, i.lightPos.xyz);

				fixed3 c = atten * inShadow * centerFalloff * maskColor * noiseColor * noiseColor * 1;
				return fixed4(c, 1);
			}
			ENDCG
		}
	}
}
