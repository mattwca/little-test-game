sampler2D Texture : register(s0);
sampler2D ShadowMap : register(s1);

float2 LightPosition : register(c0);
float4 LightColour : register(c1);
float2 ScreenSize : register(c2);

float4 ShadowPixelShader(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // Convert the light position from pixel space to uv space (0-1).
    float2 lightPositionUV = LightPosition / ScreenSize;

    // Calculate the distance from the given position to the light.
    float distanceToLight = distance(texCoord, lightPositionUV);

    // Get the direction and angle from the pixel to the light.
    float2 direction = texCoord - lightPositionUV;
    float angle = atan2(direction.y, direction.x);

    // Convert back from the angle to the corresponding X co-ordinate we would have used for the distance
    // value when buildling the shadow map.. We convert the angle from radians to "turns" around the circle
    // (a proportion between 0-1), and use frac to ensure we wrap around.
    float uv = frac(angle / 6.28318);

    // Use the uv to lookup the corresponding distance from the shadow map.
    float shadowMapValue = tex2D(ShadowMap, float2(uv, 0)).r;

    // Get the colour from the rendered frame.
    float4 colour = tex2D(Texture, texCoord) * color;
    
    // If the rendered frame has a non-transparent pixel, and we're further away than the encoded distance,
    // we render a shadow.
    if (colour.a < 0.1 && distanceToLight > shadowMapValue) {
        float4 shadowColour = float4(0.25, 0.25, 0.25, distanceToLight + 0.5) * LightColour;
        
        float brightness = (shadowColour.r + shadowColour.g + shadowColour.b) / 2.0;
        float4 gray = float4(brightness, brightness, brightness, shadowColour.a);
        return lerp(shadowColour, gray, 0.4);
    }

    return colour;
}

technique SpriteBatch { 
    pass { 
        PixelShader = compile ps_3_0 ShadowPixelShader();
    }
}