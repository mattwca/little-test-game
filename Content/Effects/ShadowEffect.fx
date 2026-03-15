sampler2D Texture : register(s0);
sampler2D ShadowMap : register(s1);

float2 LightPosition : register(c0);
float4 LightColour : register(c1);
float2 ScreenSize : register(c2);

float4 ShadowPixelShader(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
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

    // Get the direction and angle from the pixel to the light.
    float2 direction = texCoordCorrected - lightPosCorrected;
    float angle = atan2(direction.y, direction.x);

    // Convert back from the angle to the corresponding X co-ordinate we would have used for the distance
    // value when buildling the shadow map.. We convert the angle from radians to "turns" around the circle
    // (a proportion between 0-1), and use frac to ensure we wrap around.
    float uv = frac(angle / 6.28318);

    // Use the uv to lookup the corresponding distance from the shadow map.
    float shadowMapValue = tex2D(ShadowMap, float2(uv, 0)).r;

    // Get the colour from the rendered frame.
    float4 colour = lerp(tex2D(Texture, texCoord) * color, LightColour, 0.05);
    
    float bias = 0.031;

    // If the rendered frame contains a transparent pixel at this position, and we're further away than the encoded
    // distance, we render a shadow.
    if (distanceToLight > shadowMapValue + bias) {
        return lerp(colour, float4(0.25, 0.25, 0.25, 1), clamp(pow(distanceToLight, 2.0), 0.0, 1.0)) * LightColour;
    }

    return colour;
}

technique SpriteBatch { 
    pass { 
        PixelShader = compile ps_3_0 ShadowPixelShader();
    }
}