sampler2D Texture : register(s0);
sampler2D ShadowMap : register(s1);

float2 LightPosition : register(c0);
float4 LightColour : register(c1);
float LightRadius : register(c2);
float2 ScreenSize : register(c3);

static const float4 UNLIT_PIXEL = float4(0, 0, 0, 0);

static const int SAMPLES = 5;
static const float SAMPLE_SPREAD = 0.008;

static const float SHADOW_BIAS = 0.025;

/**
 * A shader which additively renders shadows for a given light source, using a 1D shadow map,
 * and a base texture to be shaded.
 *
 * Converts the pixel coordinate to 1D polar space, samplpes the distance value from the map
 * and compares the distance stored in the map against the distance for the current pixel to
 * the light source.
 *
 * Uses inverse-square falloff to dictate the amount of shadow.
 */
float4 ShadowPixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Convert the light position from pixel space to uv space (0-1).
    float2 lightPositionUV = LightPosition / ScreenSize;

    // Work in aspect-ratio-corrected UV space so distances and angles match the shadow map,
    // and the light boundary is a circle in pixel space rather than an ellipse.
    float aspectRatio = ScreenSize.x / ScreenSize.y;
    float2 texCoordCorrected = float2(texCoord.x * aspectRatio, texCoord.y);
    float2 lightPosCorrected = float2(lightPositionUV.x * aspectRatio, lightPositionUV.y);

    // Calculate the distance from the given position to the light.
    float distanceToLight = distance(texCoordCorrected, lightPosCorrected);

    // We're outside the light's radius, the pixel is unlit, therefore transparent.
    if (distanceToLight > LightRadius) {
        return UNLIT_PIXEL;
    }

    // Get the direction and angle from the pixel to the light.
    float2 direction = texCoordCorrected - lightPosCorrected;
    float angle = atan2(direction.y, direction.x);

    // Convert back from the angle to the corresponding X co-ordinate we would have used for the distance
    // value when building the shadow map.. We convert the angle from radians to "turns" around the circle
    // (a proportion between 0-1), and use frac to ensure we wrap around.
    float uv = frac(angle / 6.28318);

    // Use the uv to lookup the corresponding distance from the shadow map.
    float shadowMapValue = tex2D(ShadowMap, float2(uv, 0)).r;
    
    float bias = SHADOW_BIAS;

    // We're unlit, so we render a transparent pixel.
    if (distanceToLight > shadowMapValue + bias) {
        return UNLIT_PIXEL;
    }

    float visibility = 0.0;
    for (int i = 0; i < SAMPLES; i++) {
        float offset = (i / float(SAMPLES - 1) - 0.5) * SAMPLE_SPREAD;
        float sampleUv = frac(uv + offset);
        float sampleShadow = tex2D(ShadowMap, float2(sampleUv, 0)).r;
        visibility += distanceToLight > sampleShadow + bias ? 0.0 : 1.0;
    }
    visibility /= SAMPLES;

    // This pixel is lit, so we render the light colour scaled by the falloff curve. The falloff is calculated
    // using the inverse-square law. The closer a pixel is to the light source, the brighter.
    // Each lighting layer is summed with the last, so the brightness of a given pixel is the accumulation of each
    // light source's contribution.
    float normalisedDistance = distanceToLight / LightRadius;
    float attenuation = 1.0 / (1.0 + 25.0 * normalisedDistance * normalisedDistance);

    float window = saturate(1.0 - normalisedDistance * normalisedDistance * normalisedDistance * normalisedDistance);
    window = window * window;

    float falloff = attenuation * window;
    return LightColour * falloff * visibility;
}

technique SpriteBatch { 
    pass { 
        PixelShader = compile ps_3_0 ShadowPixelShader();
    }
}