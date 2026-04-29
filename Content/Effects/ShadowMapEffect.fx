    sampler2D OccluderTexture : register(s0);

    float2 LightPosition : register(c0);
    float2 Resolution : register(c1);
    float LightRadius : register(c2);

    #define STEPS 400

    /**
     * A shader which generates a shadow map.
     *
     * For each pixel around a given circle, it casts a ray at a given angle. We sample the frame containing the occluders,
     * moving out from the centre of the circle a given number of times (defined as #STEPS).
     */
    float4 ShadowMapEffect(float2 texCoord: TEXCOORD0): COLOR0 {
        // Convert the x coordinate to a corresponding angle on the unit circle.
        float angle = texCoord.x * 6.28318;

        // Cast rays in aspect-ratio-corrected UV space so they form a true circle in pixel space.
        float aspectRatio = Resolution.x / Resolution.y;
        float2 direction = float2(cos(angle), sin(angle));

        // Convert the light position to aspect-corrected UV space.
        float2 position = LightPosition / Resolution;
        float2 positionCorrected = float2(position.x * aspectRatio, position.y);

        float stepSize = LightRadius / STEPS;

        for (int i = 0; i < STEPS; i++) {
            float stepDistance = (float)i * stepSize;

            // March in corrected UV space, then convert back to regular UV for texture sampling.
            float2 sampleCorrected = positionCorrected + direction * stepDistance;
            float2 sampleCoord = float2(sampleCorrected.x / aspectRatio, sampleCorrected.y);

            if (sampleCoord.x > 1 || sampleCoord.x < 0 || sampleCoord.y > 1 || sampleCoord.y < 0) {
                continue;
                //return float4(LightRadius, LightRadius, LightRadius, 1.0);
            }

            // Get the colour in the occluder texture.
            float4 colourAtCoord = tex2Dlod(OccluderTexture, float4(sampleCoord, 0, 0));
            float alpha = colourAtCoord.a;

            // Check if we've hit an occluding pixel.
            if (alpha > 0.1) {
                return float4(stepDistance, stepDistance, stepDistance, 1.0);
            }
        }

        return float4(LightRadius, LightRadius, LightRadius, 1.0);
    }

    technique ShadowMap
    {
        pass Pass0
        {
            PixelShader = compile ps_3_0 ShadowMapEffect();
        }
    }