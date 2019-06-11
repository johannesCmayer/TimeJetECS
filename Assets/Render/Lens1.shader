Shader "Lens/1"
{
	Properties
	{
		_MainTex("Dummy Texture (Blit API)", 2D) = "black" {}
		
		_RenderTex0("Forward", 2D) = "black" {}
		_RenderTex1("Left", 2D) = "black" {}
		_RenderTex2("Right", 2D) = "black" {}
		_RenderTex3("Up", 2D) = "black" {}
		_RenderTex4("Down", 2D) = "black" {}
		_RenderTex5("Back", 2D) = "black" {}

		_Zoom("Zoom Factor", float) = 1
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

			float _Zoom;

			float4 red_1() { return float4(1, 0, 0, 1); }
			float4 green_1() { return float4(0, 1, 0, 1); }
			float4 blue_1() { return float4(0, 0, 1, 1); }
			float4 red_2() { return float4  (0.3, 0  , 0, 1); }
			float4 green_2() { return float4(0, 0.3, 0  , 1); }
			float4 blue_2() { return float4 (0  , 0, 0.3, 1); }

			float4 not_mapped() { return float4(0.5, 0.2, 0.5, 1); }

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			// LENSES_____________________START

			float3 fisheye(float2 uv)
			{				
				uv = uv * _Zoom;

				float u = uv.x;
				float v = uv.y;
				float2 r = sqrt(u*u + v * v);
				float theta = r;
				float s = sin(theta);

				float x = u / r * s;
				float y = v / r * s;
				float z = cos(theta);

				return normalize(float3(x, y, z));
			}

			float3 prism(float2 uv)
			{
				float coef = _Zoom * 8;
				return normalize(float3(sin(uv.x * coef), sin(uv.y * coef), cos(uv.x * coef)));
			}

			float3 octagonZoom(float2 uv)
			{
				uv = uv * _Zoom;
				return normalize(float3(uv.x, uv.y, 1 - abs(uv.x) - abs(uv.y)));
			}

			float3 stereo(float2 uv)
			{
				uv = uv * _Zoom;
				return normalize(float3(uv.x, uv.y, cos(uv.x)));
			}

			// LENSES__________________________________________END

			float2 CenteredUVToUV(float x, float y)
			{
				float offset = 0.5;
				float c_1 = 0.5;
				return float2(x * c_1 + offset, y * c_1 + offset);
			}

			float2 UVToCenteredUV(float2 uv)
			{
				return uv - 0.5;
			}

			float4 dirVecToCol(float3 dirVec)
			{
				float3 back = dirVec / abs(dirVec.z);
				float3 down = dirVec / abs(dirVec.y);
				float3 left = dirVec / abs(dirVec.x);

				bool debugColors = 0;
				
				float bounds = 1;

				float x = back.x;
				float y = back.y;
				if (dirVec.z >= 0 && 
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex0, CenteredUVToUV(x, y)) + (green_1() * debugColors);
				}
				if (dirVec.z < 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex5, CenteredUVToUV(-x, y)) + green_2() * debugColors;
				}

				x = left.z;
				y = left.y;
				if (dirVec.x >= 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex2, CenteredUVToUV(-x, y)) + blue_1() * debugColors;
				}
				if (dirVec.x < 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex1, CenteredUVToUV(x, y)) + blue_2() * debugColors;
				}

				x = down.x;
				y = down.z;
				if (dirVec.y >= 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex3, CenteredUVToUV(x, -y)) + red_1() * debugColors;
				}
				if (dirVec.y < 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex4, CenteredUVToUV(x, y)) + red_2() * debugColors;
				}

				return float4(1,0,0,1);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 centeredUV = UVToCenteredUV(i.uv);

				//float3 dirVec = stereo(centeredUV);
				float3 dirVec = fisheye(centeredUV);
				float4 col = dirVecToCol(dirVec);
				//float4 col = tex2D(_RenderTex0, i.uv);
				return col;
			}
			ENDCG
		}		
	}
}