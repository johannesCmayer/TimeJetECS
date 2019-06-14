Shader "Lens/Post"
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

		[HideInInspector] _Red("Red", Vector) = (1, 0, 0, 1)
		[HideInInspector] _Green("Green", Vector) = (0, 1, 0, 1)
		[HideInInspector] _Blue("Blue", Vector) = (0.336, 0.722893, 1, 1)
		[HideInInspector] _Yellow("Yellow", Vector) = (1, 1, 0, 1)
		[HideInInspector] _Purple("Purple", Vector) = (0.2833333, 0, 1, 1)
		[HideInInspector] _Orange("Orange", Vector) = (1, 0.6, 0, 1)

		[HideInInspector] _ErrCol("Orange", Vector) = (1, 0, 0, 1)
		[HideInInspector] _NotMappedCol("Orange", Vector) = (0.1, 0, 0.33, 1)

		_Zoom("Zoom Factor", float) = 1
		_DebugColCoef("Debug color intensity", Range(0, 1)) = 0
	}

	HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Blend;

	TEXTURE2D_SAMPLER2D(_DebugProjectionTexAlpha, sampler_DebugProjectionTexAlpha);
	float4 _DebugProjectionTexAlpha_ST;

	TEXTURE2D_SAMPLER2D(_RenderTex0, sampler_RenderTex0);
	TEXTURE2D_SAMPLER2D(_RenderTex1, sampler_RenderTex1);
	TEXTURE2D_SAMPLER2D(_RenderTex2, sampler_RenderTex2);
	TEXTURE2D_SAMPLER2D(_RenderTex3, sampler_RenderTex3);
	TEXTURE2D_SAMPLER2D(_RenderTex4, sampler_RenderTex4);
	TEXTURE2D_SAMPLER2D(_RenderTex5, sampler_RenderTex5);

	float4 _Red;
	float4 _Green;
	float4 _Blue;
	float4 _Yellow;
	float4 _Purple;
	float4 _Orange;

	float4 _ErrCol;
	float4 _NotMappedCol;

	float _Zoom;
	float _LenseStretchX;
	float _LenseStretchY;

	float _DebugColCoef;

	float _AspectRatio;

	#define NOCOLVEC float3(0, 0, 0)

	float3 latlon_to_ray(float2 latlon)
	{
		float x = sin(latlon.y) * cos(latlon.x);
		float y = sin(latlon.x);
		float z = cos(latlon.y) * cos(latlon.x);
		return float3(x, y, z);
	}
	float2 ray_to_latlon(float3 dir)
	{
		return float2(atan2(dir.y, length(float2(dir.z, dir.x))),
			          atan2(dir.x, dir.z));
	}

	// LENSES_____________________START

	float3 hammer_inv(float2 uv)
	{
		float x = uv.x;
		float y = uv.y;
		if (x*x / 8 + y * y / 2 > 1)
		{
			return NOCOLVEC;
		}

		float z = sqrt(1 - 0.0625*x*x - 0.25*y*y);
		float lon = 2 * atan(z*x / (2 * (2 * z*z - 1)));
		float lat = asin(z*y);
		return latlon_to_ray(float2(lat, lon));
	}

	float3 psudo_inv(float2 uv)
	{
		float r = length(uv);
		float n = sqrt(1 - 0.06*uv.x*uv.x - 0.25*uv.y*uv.y);

		/*if (r > 2.5)
		{
		return NOCOLVEC;
		}*/

		float a = 1;

		float lat = asin(uv.y*n);
		float lon = 2 * atan2(n*uv.x, (2 * (2 * n*n - 1)));
		return latlon_to_ray(float2(lat, lon));

		float3 ret = float3(
			uv.x * sin(n) / n,
			uv.y * sin(n) / n,
			n);

		return normalize(ret);
	}

	float3 panini_inv(float2 uv)
	{
		float d = 1;

		float x = uv.x;
		float y = uv.y;

		float k = x * x / ((d + 1)*(d + 1));
		float dscr = k * k*d*d - (k + 1)*(k*d*d - 1);
		float clon = (-k * d + sqrt(dscr)) / (k + 1);
		float S = (d + 1) / (d + clon);
		float lon = atan2(x, S*clon);
		float lat = atan2(y, S);

		return latlon_to_ray(float2(lat, lon));
	}
		
	// WinkelTripel ______START

	bool is_inside_artifact_box(float x, float y, float lensHeight, float lensWidth)
	{
		float artifact_x = lensWidth / 2 * 0.71;
		float artifact_y = lensHeight / 2 * 0.81;

		return (abs(x) > artifact_x && abs(y) > artifact_y);
	}
		
	float2 winkelTripel_forward(float3 vec)
	{
		const float clat0 = 20 / PI;

		float2 latlon = ray_to_latlon(vec);
		float clat = cos(latlon.x);
		float temp = clat * cos(latlon.y*0.5);
		float D = acos(temp);
		float C = 1 - temp * temp;
		temp = D / sqrt(C);

		float x = 0.5 * (2 * temp*clat*sin(latlon.y*0.5) + latlon.y * clat0);
		float y = 0.5 * (temp*sin(latlon.x) + latlon.x);

		return float2(x, y);
	}

	bool cull(float x, float y)
	{
		float2 f1 = winkelTripel_forward(latlon_to_ray(float2(PI / 2, 0)));
		float lens_height = 2 * f1.y;

		float2 f2 = winkelTripel_forward(latlon_to_ray(float2(0, PI)));
		float lens_width = 2 * f2.x;

		if (abs(y) >= lens_height / 2)
		{
			return true;
		}

		if (abs(x) + abs(y) > 0.5f)
		{
			float2 n_uv = normalize(float2(x, y));
			float2 c_uv = float2(x, y) * float2(0.68, 1);
			if (length(c_uv - n_uv) >= 0.8)
			{
				return true;
			}
		}
		return false;
	}

	float3 winkelTripel_inv(float2 uv)
	{
		//float3 temp = latlon_to_ray(uv);
		//uv = ray_to_latlon(temp);

		float x = uv.x;
		float y = uv.y;

		if (cull(x, y))
		{
			return NOCOLVEC;
		}

		float lambda = x;
		float phi = y;
		float eps = 0.0001;
		float halfpi = PI / 2;

		for(uint i=1; i < 25; i++)
		{
			float cosphi = cos(phi);
			float sinphi = sin(phi);
			float sin_2phi = sin(2 * phi);
			float sin2phi = sinphi * sinphi;
			float cos2phi = cosphi * cosphi;
			float sinlambda = sin(lambda);
			float coslambda_2 = cos(lambda / 2);
			float sinlambda_2 = sin(lambda / 2);
			float sin2lambda_2 = sinlambda_2 * sinlambda_2;
			float C = 1 - cos2phi * coslambda_2 * coslambda_2;
			float E;
			float F;
			if (C != 0)
			{
				F = 1 / C;
				E = acos(cosphi * coslambda_2) * sqrt(F);
			}
			else
			{
				E = 0;
				F = 0;
			}
			float fx = .5 * (2 * E * cosphi * sinlambda_2 + lambda / halfpi) - x;
			float fy = .5 * (E * sinphi + phi) - y;
			float sigxsiglambda = .5 * F * (cos2phi * sin2lambda_2 + E * cosphi * coslambda_2 * sin2phi) + .5 / halfpi;
			float sigxsigphi = F * (sinlambda * sin_2phi / 4 - E * sinphi * sinlambda_2);
			float sigysiglambda = .125 * F * (sin_2phi * sinlambda_2 - E * sinphi * cos2phi * sinlambda);
			float sigysigphi = .5 * F * (sin2phi * coslambda_2 + E * sin2lambda_2 * cosphi) + .5;
			float denominator = sigxsigphi * sigysiglambda - sigysigphi * sigxsiglambda;
			float siglambda = (fy * sigxsigphi - fx * sigysigphi) / denominator;
			float sigphi = (fx * sigysiglambda - fy * sigxsiglambda) / denominator;
			lambda = lambda - siglambda;
			phi = phi - sigphi;
			if (abs(siglambda) < eps && abs(sigphi) < eps)
			{
				break;
			}
		}
		float lat = phi;
		float lon = lambda;
		float2 f = winkelTripel_forward(latlon_to_ray(float2(lat, PI)));
		if (abs(x) < abs(f.x)) 
		{
			return latlon_to_ray(float2(lat, lon));
		}
		return NOCOLVEC;
	}

	// WinkelTripel ______END

	float3 fisheye_inv(float2 uv)
	{
		float u = uv.x;
		float v = uv.y;

		float r = sqrt(u * u + v * v);
		float theta = r;
		float s = sin(theta);

		float x = u / r * s;
		float y = v / r * s;
		float z = cos(theta);

		return normalize(float3(x, y, z));
	}

	float3 fisheye_2_inv(float2 uv)
	{
		const float maxr = 2 * sin(PI*0.5);

		float x = uv.x;
		float y = uv.y;

		float r = sqrt(x*x + y * y);
		if (r > maxr)
		{
			return NOCOLVEC;
		}

		float theta = 2 * asin(r*0.5);

		float s = sin(theta);
		return float3(x / r * s, y / r * s, cos(theta));
	}

	float3 prism_inv(float2 uv)
	{
		float coef = 8;
		return normalize(float3(sin(uv.x * coef), sin(uv.y * coef), cos(uv.x * coef)));
	}

	float3 octagonZoom_inv(float2 uv)
	{
		return normalize(float3(uv.x, uv.y, 1 - abs(uv.x) - abs(uv.y)));
	}

	float3 stereo_inv(float2 uv)
	{
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
		float4 tiling_scale = float4(1, 1, 0, 0);
		float4 debugCol = color * _DebugColCoef;
		debugCol *= SAMPLE_TEXTURE2D(_DebugProjectionTexAlpha, sampler_DebugProjectionTexAlpha, uv * tiling_scale.xy + tiling_scale.zw).x;
		return debugCol;
	}

	float4 dirVecToCol(float3 dirVec)
	{
		
		if (dirVec.x == 0 && dirVec.y == 0 && dirVec.z == 0)
		{
			return _NotMappedCol;
		}

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
			return SAMPLE_TEXTURE2D(_RenderTex0, sampler_RenderTex0, uv) + debugGrid(_Red, uv);
		}
		uv = CenteredUVToUV(-x, y);
		if (dirVec.z < 0 &&
			x <= bounds && x >= -bounds &&
			y <= bounds && y >= -bounds)
		{
			return SAMPLE_TEXTURE2D(_RenderTex5, sampler_RenderTex5, uv) + debugGrid(_Green, uv);
		}
		
		x = left.z;
		y = left.y;
		uv = CenteredUVToUV(-x, y);
		if (dirVec.x >= 0 &&
			x <= bounds && x >= -bounds &&
			y <= bounds && y >= -bounds)
		{
			return SAMPLE_TEXTURE2D(_RenderTex2, sampler_RenderTex2, uv) + debugGrid(_Blue, uv);
		}
		uv = CenteredUVToUV(x, y);
		if (dirVec.x < 0 &&
			x <= bounds && x >= -bounds &&
			y <= bounds && y >= -bounds)
		{
			return SAMPLE_TEXTURE2D(_RenderTex1, sampler_RenderTex1, uv) + debugGrid(_Yellow, uv);
		}
		
		x = down.x;
		y = down.z;
		uv = CenteredUVToUV(x, -y);
		if (dirVec.y >= 0 &&
			x <= bounds && x >= -bounds &&
			y <= bounds && y >= -bounds)
		{

			return SAMPLE_TEXTURE2D(_RenderTex3, sampler_RenderTex3, uv) + debugGrid(_Purple, uv);
		}
		uv = CenteredUVToUV(x, y);
		if (dirVec.y < 0 &&
			x <= bounds && x >= -bounds &&
			y <= bounds && y >= -bounds)
		{
			return SAMPLE_TEXTURE2D(_RenderTex4, sampler_RenderTex4, uv) + debugGrid(_Orange, uv);
		}

		return _ErrCol;
	}

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 centeredUV = UVToCenteredUV(i.texcoord) * _Zoom;
		centeredUV.y /= _AspectRatio;
		centeredUV *= float2(1 - _LenseStretchX, 1 - _LenseStretchY);

		//float3 dirVec = fisheye_2_inv(centeredUV);
		//float3 dirVec = hammer_inv(centeredUV);
		float3 dirVec = winkelTripel_inv(centeredUV);
		//float3 dirVec = psudo_inv(centeredUV);
		float4 col = dirVecToCol(dirVec);
		return col;
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}