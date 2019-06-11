Shader "Lens/1"
{
	Properties
	{
		[HideInInspector] _MainTex("Dummy texture (Blit API)", 2D) = "black" {}

		_DebugProjectionTexAlpha("Debug projection alpha texture", 2D) = "black" {}
		
		[HideInInspector] _RenderTex0("Forward", 2D) = "black" {}
		[HideInInspector] _RenderTex1("Left", 2D) = "black" {}
		[HideInInspector] _RenderTex2("Right", 2D) = "black" {}
		[HideInInspector] _RenderTex3("Up", 2D) = "black" {}
		[HideInInspector] _RenderTex4("Down", 2D) = "black" {}
		[HideInInspector] _RenderTex5("Back", 2D) = "black" {}

		[HideInInspector] _Red("Red", Vector) =    (1, 0, 0, 1)
		[HideInInspector] _Green("Green", Vector) =  (0, 1, 0, 1)
		[HideInInspector] _Blue("Blue", Vector) =   (0.336, 0.722893, 1, 1)
		[HideInInspector] _Yellow("Yellow", Vector) = (1, 1, 0, 1)
		[HideInInspector] _Purple("Purple", Vector) = (0.2833333, 0, 1, 1)
		[HideInInspector]_Orange("Orange", Vector) =   (255, 154, 0, 255)

		_Zoom("Zoom Factor", float) = 1
		_DebugColCoef("Debug color intensity", Range(0, 1)) = 0
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

			sampler2D _DebugProjectionTexAlpha;
			float4 _DebugProjectionTexAlpha_ST;

			sampler2D _RenderTex0;
			sampler2D _RenderTex1;
			sampler2D _RenderTex2;
			sampler2D _RenderTex3;
			sampler2D _RenderTex4;
			sampler2D _RenderTex5;

			float4 _Red;
			float4 _Green;
			float4 _Blue;
			float4 _Yellow;
			float4 _Purple;
			float4 _Orange;

			float _Zoom;
			float _DebugColCoef;

			float4 not_mapped_col() { return float4(0.5, 0.2, 0.5, 1); }

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			// LENSES_____________________START

			/*float3 panini(float2 uv)
			{
				float d = 1;

				uv *= _Zoom;
				float x = uv.x;
				float y = uv.y;

				float k = x * x / ((d + 1)*(d + 1));
				float dscr = k * k*d*d - (k + 1)*(k*d*d - 1);
				float clon = (-k * d + sqrt(dscr)) / (k + 1);
				float S = (d + 1) / (d + clon);
				float lon = atan2(x, S*clon);
				float lat = atan2(y, S);

				return latlon_to_ray(lat, lon);
			}*/


			//float3 winkelTripel(float2 uv)
			//{
			//	/*if abs(y) >= lens_height / 2 then
			//		return nil
			//		end
			//		if is_inside_artifact_box(x, y) then
			//			return nil
			//			end*/
			//	float x = uv.x;
			//	float y = uv.y;

			//	local lambda = x
			//		local phi = y
			//		eps = 0.0001
			//		halfpi = pi / 2

			//		for iter = 1, 25 do
			//		{
			//			float cosphi = cos(phi);
			//			float sinphi = sin(phi);
			//			float sin_2phi = sin(2 * phi);
			//			float sin2phi = sinphi * sinphi;
			//			float cos2phi = cosphi * cosphi;
			//			float sinlambda = sin(lambda);
			//			float coslambda_2 = cos(lambda / 2);
			//			float sinlambda_2 = sin(lambda / 2);
			//			float sin2lambda_2 = sinlambda_2 * sinlambda_2;
			//			float C = 1 - cos2phi * coslambda_2 * coslambda_2;
			//			float E;
			//			float F;
			//			if (C ~= 0)
			//			{
			//				F = 1 / C;
			//				E = acos(cosphi * coslambda_2) * sqrt(F);
			//			}
			//			else
			//			{
			//				E = 0;
			//				F = 0;
			//			}
			//			float fx = .5 * (2 * E * cosphi * sinlambda_2 + lambda / halfpi) - x;
			//			float fy = .5 * (E * sinphi + phi) - y;
			//			float sigxsiglambda = .5 * F * (cos2phi * sin2lambda_2 + E * cosphi * coslambda_2 * sin2phi) + .5 / halfpi;
			//			float sigxsigphi = F * (sinlambda * sin_2phi / 4 - E * sinphi * sinlambda_2);
			//			float sigysiglambda = .125 * F * (sin_2phi * sinlambda_2 - E * sinphi * cos2phi * sinlambda);
			//			float sigysigphi = .5 * F * (sin2phi * coslambda_2 + E * sin2lambda_2 * cosphi) + .5;
			//			float denominator = sigxsigphi * sigysiglambda - sigysigphi * sigxsiglambda;
			//			float siglambda = (fy * sigxsigphi - fx * sigysigphi) / denominator;
			//			float sigphi = (fx * sigysiglambda - fy * sigxsiglambda) / denominator;
			//			lambda = lambda - siglambda;
			//			phi = phi - sigphi;
			//			if abs(siglambda) < eps and abs(sigphi) < eps)
			//			{
			//				break;
			//			}
			//		}

			//	lat, lon = phi, lambda
			//	x0, y0 = lens_forward(latlon_to_ray(lat, pi))
			//	if abs(x) < abs(x0) then
			//		return latlon_to_ray(lat, lon)
			//	return not_mapped_col();
			//}

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

			float4 debugGrid(float4 color, float2 uv) 
			{
				float4 debugCol = color * _DebugColCoef;				
				debugCol *= 1 - tex2D(_DebugProjectionTexAlpha, uv * _DebugProjectionTexAlpha_ST.xy + _DebugProjectionTexAlpha_ST.zw).x;
				return debugCol;
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
				float2 uv = CenteredUVToUV(x, y);
				if (dirVec.z >= 0 && 
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex0, uv) + debugGrid(_Red, uv);
				}
				uv = CenteredUVToUV(-x, y);
				if (dirVec.z < 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex5, uv) + debugGrid(_Green, uv);
				}

				x = left.z;
				y = left.y;
				uv = CenteredUVToUV(-x, y);
				if (dirVec.x >= 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex2, uv) + debugGrid(_Blue, uv);
				}
				uv = CenteredUVToUV(x, y);
				if (dirVec.x < 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex1, uv) + debugGrid(_Yellow, uv);
				}

				x = down.x;
				y = down.z;
				uv = CenteredUVToUV(x, -y);
				if (dirVec.y >= 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					
					return tex2D(_RenderTex3, uv) + debugGrid(_Purple, uv);
				}
				uv = CenteredUVToUV(x, y);
				if (dirVec.y < 0 &&
					x <= bounds && x >= -bounds &&
					y <= bounds && y >= -bounds)
				{
					return tex2D(_RenderTex4, uv) + debugGrid(_Orange, uv);
				}

				return not_mapped_col();
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