#version 330

uniform sampler2D uTextureSampler;
uniform float uTextureFlag;
uniform vec4 uStartColour;
uniform vec4 uEndColour;
uniform int uGradientMode;
uniform vec2 uGradientStart;
uniform vec2 uGradientDimension;

uniform vec2 uScreen; 
uniform vec2 uResolution;
uniform vec4 uViewport;

uniform mat4 uGradientRotation;
uniform vec2 uGradientOffset;

uniform int uGrayScaleAlpha;

layout(origin_upper_left) in vec4 gl_FragCoord;
in vec2 oTexCoords;

out vec4 FragColour;

vec2 getGradientOffset()
{
	vec2 offset = vec2(0.0);

	if (uGradientMode == 4)
	{
		offset.x = -uGradientDimension.x / 2;
	}
	else if (uGradientMode == 5)
	{
		offset.y = -uGradientDimension.y / 2;
	}

	return offset;
}

vec4 getLocalFragCoord()
{
	vec4 fragCoord = gl_FragCoord;

	fragCoord.x -= uViewport.x;
	fragCoord.y -= uViewport.y;

	float scalarX = uResolution.x / uViewport.z;
	float scalarY = uResolution.y / uViewport.w;

	fragCoord.x *= scalarX;
	fragCoord.y *= scalarY;

	return fragCoord;
}

vec4 getGradientColour()
{
	vec4 gradientColour = uStartColour;

	vec4 frag = getLocalFragCoord();

	if (uGradientMode != 0)
	{
		vec2 gradientPos = vec2(frag.x, frag.y);

		vec2 gradientCenter = uGradientStart + -uGradientOffset;
		gradientPos -= gradientCenter;
		vec4 localGradientPos = vec4(gradientPos, 0, 1);
		localGradientPos = uGradientRotation * localGradientPos;

		gradientPos = localGradientPos.xy;
		gradientPos += gradientCenter + getGradientOffset();

		if (uGradientMode <= 6)
		{
			float xPercent = abs((gradientPos.x - uGradientStart.x) / uGradientDimension.x);
			float yPercent = abs((gradientPos.y - uGradientStart.y) / uGradientDimension.y);

			if (uGradientMode == 4 || uGradientMode == 6)
				xPercent *= 2.0;
			if (uGradientMode == 5 || uGradientMode == 6)
				yPercent *= 2.0;

			if (uGradientMode == 1 || uGradientMode == 4)
			{
				gradientColour = vec4(mix(uStartColour, uEndColour, max(min(xPercent, 1.0), 0.0)));
			}
			else if (uGradientMode == 2 || uGradientMode == 5)
			{
				gradientColour = vec4(mix(uStartColour, uEndColour, max(min(yPercent, 1.0), 0.0)));
			}
			else if (uGradientMode == 3 || uGradientMode == 6)
			{
				float percent = (xPercent + yPercent) / 2;
				if (uGradientMode == 6)
				{
					percent = abs(percent - 1.0);
				}
				gradientColour = vec4(mix(uStartColour, uEndColour, max(min(percent, 1.0), 0.0)));
			}
		}
		else
		{
			vec2 gradientCenter = uGradientStart + (uGradientDimension / 2);

			if (uGradientMode == 7 || uGradientMode == 8)
			{
				vec2 gradientOffset = gradientPos - gradientCenter;
				vec2 gradientEnd = vec2((uGradientDimension.x / 2), (uGradientDimension.y / 2));

				if (uGradientMode == 8)
				{
					if (uGradientDimension.x > uGradientDimension.y)
					    gradientEnd.x = 0;
					else
						gradientEnd.y = 0;
				}

				float percent = length(gradientOffset) / length(gradientEnd);
				gradientColour = vec4(mix(uStartColour, uEndColour, max(min(percent, 1.0), 0.0)));

			}
			else if (uGradientMode == 9 || uGradientMode == 10)
			{
				float xPercent = abs((gradientPos.x - gradientCenter.x) / (uGradientDimension.x / 2));
				float yPercent = abs((gradientPos.y - gradientCenter.y) / (uGradientDimension.y / 2));

				float percent = (xPercent + yPercent) / 2;

				if (uGradientMode == 10)
				{
					vec2 gradientCorner = vec2((uGradientDimension.x / 2), (uGradientDimension.y / 2));
					vec2 gradientEnd;
					vec2 gradientRight = vec2(gradientCorner.x, 0);
					vec2 gradientBottom = vec2(0, gradientCorner.y);

					float ratioDif;

					if (uGradientDimension.x > uGradientDimension.y)
					{
					    gradientEnd = gradientBottom;
						ratioDif = length(gradientBottom) / length(gradientRight);
					}
					else
					{
						gradientEnd = gradientRight;
						ratioDif = length(gradientRight) / length(gradientBottom);
					}

					float ratio = (length(gradientCorner) / length(gradientEnd)) * sqrt(ratioDif) * 1.4;
					percent *= ratio;
				}

				gradientColour = vec4(mix(uStartColour, uEndColour, max(min(percent, 1.0), 0.0)));
			}
		}
	}
	
	return gradientColour;
}

void main()
{
	vec4 gradientColour = getGradientColour();

	vec4 texColour = texture(uTextureSampler, oTexCoords);
	if (uGrayScaleAlpha == 1)
		texColour.w = texColour.x * texColour.y * texColour.z;
	vec4 colour = (texColour * gradientColour) * uTextureFlag;
	colour += (1.0 - uTextureFlag) * gradientColour;

	FragColour = colour;
}