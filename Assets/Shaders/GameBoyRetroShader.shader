Shader "Hidden/Game Boy Retro Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_XRes ("Horizontal resolution", Int) = 800
		_YRes ("Vertical resolution", Int) = 720
		_RPSize ("Retropixel size", Int) = 4
		_PaddingSize ("Padding size", Int) = 1
		_ShadeCount ("Shade count", Int) = 4
		_ShadeColor ("Shade color", Color) = (0.6078, 0.7333, 0.0549, 1.0)
		_Quality ("Quality level", int) = 3
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct fragPosTex
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			int _XRes;
			int _YRes;
			int _RPSize;
			int _PaddingSize;
			int _ShadeCount;
			float4 _ShadeColor;
			int _Quality;

			fixed4 frag (fragPosTex f) : SV_Target
			{
				// Calculate retropixel grid dimensions.
			    float2 rpGrid = float2(ceil((float)_XRes / (_RPSize + _PaddingSize)), ceil((float)_YRes / (_RPSize + _PaddingSize)));

				// Obtain retropixel grid cell for the pixel being shaded.
				float2 rpCell = float2(floor(f.uv0.x * rpGrid.x), floor(f.uv0.y * rpGrid.y));
				
				// Calculate the shift in texture coordinate per pixel.
				float deltaUPerPx = 1.0 / (rpGrid.x * (_RPSize + _PaddingSize));
				float deltaVPerPx = 1.0 / (rpGrid.y * (_RPSize + _PaddingSize));
				
				if((deltaUPerPx * (rpCell.x + 1) * (_RPSize + _PaddingSize) - f.uv0.x) <= (_PaddingSize * deltaUPerPx) ||
					(deltaVPerPx * (rpCell.y + 1) * (_RPSize + _PaddingSize) - f.uv0.y) <= (_PaddingSize * deltaVPerPx))
				{
					return _ShadeColor;
				}
				
				float4 rpColor = float4(0.0, 0.0, 0.0, 0.0);
				if(_Quality == 1)
				{
					// For each pixel in the retropixel cell...
					for(int uPos = 0; uPos < 1; uPos++)
					{
						for(int vPos = 0; vPos < 1; vPos++)
						{
							// Sample sprite texture, and add value to rpColor.
							rpColor += tex2D(_MainTex, float2(
								deltaUPerPx * (((_RPSize + _PaddingSize) * rpCell.x) + uPos * ((_RPSize + _PaddingSize) / _Quality)),
								deltaVPerPx * (((_RPSize + _PaddingSize) * rpCell.y) + vPos * ((_RPSize + _PaddingSize) / _Quality))));
						}
					}
				}

				if(_Quality == 2)
				{
					// For each pixel in the retropixel cell...
					for(int uPos = 0; uPos < 2; uPos++)
					{
						for(int vPos = 0; vPos < 2; vPos++)
						{
							// Sample sprite texture, and add value to rpColor.
							rpColor += tex2D(_MainTex, float2(
								deltaUPerPx * (((_RPSize + _PaddingSize) * rpCell.x) + uPos * ((_RPSize + _PaddingSize) / _Quality)),
								deltaVPerPx * (((_RPSize + _PaddingSize) * rpCell.y) + vPos * ((_RPSize + _PaddingSize) / _Quality))));
						}
					}
				}

				if(_Quality == 3)
				{
					// For each pixel in the retropixel cell...
					for(int uPos = 0; uPos < 3; uPos++)
					{
						for(int vPos = 0; vPos < 3; vPos++)
						{
							// Sample sprite texture, and add value to rpColor.
							rpColor += tex2D(_MainTex, float2(
								deltaUPerPx * (((_RPSize + _PaddingSize) * rpCell.x) + uPos * ((_RPSize + _PaddingSize) / _Quality)),
								deltaVPerPx * (((_RPSize + _PaddingSize) * rpCell.y) + vPos * ((_RPSize + _PaddingSize) / _Quality))));
						}
					}
				}

				if(_Quality == 4)
				{
					// For each pixel in the retropixel cell...
					for(int uPos = 0; uPos < 4; uPos++)
					{
						for(int vPos = 0; vPos < 4; vPos++)
						{
							// Sample sprite texture, and add value to rpColor.
							rpColor += tex2D(_MainTex, float2(
								deltaUPerPx * (((_RPSize + _PaddingSize) * rpCell.x) + uPos * ((_RPSize + _PaddingSize) / _Quality)),
								deltaVPerPx * (((_RPSize + _PaddingSize) * rpCell.y) + vPos * ((_RPSize + _PaddingSize) / _Quality))));
						}
					}
				}

				if(_Quality >= 5)
				{
					// For each pixel in the retropixel cell...
					for(int uPos = 0; uPos < 5; uPos++)
					{
						for(int vPos = 0; vPos < 5; vPos++)
						{
							// Sample sprite texture, and add value to rpColor.
							rpColor += tex2D(_MainTex, float2(
								deltaUPerPx * (((_RPSize + _PaddingSize) * rpCell.x) + uPos * ((_RPSize + _PaddingSize) / _Quality)),
								deltaVPerPx * (((_RPSize + _PaddingSize) * rpCell.y) + vPos * ((_RPSize + _PaddingSize) / _Quality))));
						}
					}
				}
						
				// Average out rpColor for this cell, and return it.
				rpColor = rpColor / pow(min(5.0, _Quality), 2.0);
				// Convert retropixel color to grayscale.
				//float gScale = 0.2989 * rpColor.r + 0.5870 * rpColor.g + 0.1140 * rpColor.b;
				// Clamp grayscale to n-color palette.
				//gScale += 1.0 / (2.0 * (_ShadeCount - 1.0));
				//gScale = (1.0 / (_ShadeCount - 1.0)) * floor(gScale / (1.0 / (_ShadeCount - 1.0)));
				// Brighten grayscale slightly so that darkest shade can still get a slight green tint.
				//gScale = min(1.0, gScale + 0.1);
				// Apply grayscale and tint to retropixel color.
				//rpColor = _ShadeColor * float4(gScale, gScale, gScale, 1.0f);
				return rpColor;
			}
			ENDCG
		}
	}
}
