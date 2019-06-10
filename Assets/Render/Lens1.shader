Shader "Lens/1"
{
	Properties
	{
		_MainTex("Albedo Texture", 2D) = "black" {}
		_RenderTex0("Albedo Texture", 2D) = "black" {}
		_RenderTex1("Albedo Texture", 2D) = "black" {}
		_RenderTex2("Albedo Texture", 2D) = "black" {}
		_RenderTex3("Albedo Texture", 2D) = "black" {}
		_RenderTex4("Albedo Texture", 2D) = "black" {}
		_RenderTex5("Albedo Texture", 2D) = "black" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }
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
			sampler2D _RenderTex0;
			sampler2D _RenderTex1;
			sampler2D _RenderTex2;
			sampler2D _RenderTex3;
			sampler2D _RenderTex4;
			sampler2D _RenderTex5;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float3 fisheye(float2 uv)
			{
				float u = uv.x;
				float v = uv.y;
				float2 r = sqrt(u*u + v * v);
				float theta = r;
				float s = sin(theta);
				return u / r * s, v / r * s, cos(theta);
			}

			int textureIdx(float3 dirVec)
			{
			}

			float2 cubeUV(float3 dirVec)
			{
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_RenderTex0, i.uv);
				return col;
			}
			ENDCG
		}		
	}
}