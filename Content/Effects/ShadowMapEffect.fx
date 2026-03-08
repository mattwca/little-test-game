    sampler2D OccluderTexture : register(s0);

    float2 LightPosition : register(c0);
    float2 Resolution : register(c1);

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

        // Convert the angle to a given point on the circle.
        float2 direction = float2(cos(angle), sin(angle));

        // Convert the light position to UV space (between 0-1).
        float2 position = LightPosition / Resolution;

        for (int i = 0; i < STEPS; i++) {
            // Calculate the distance from the centre of the circle for this iteration.
            // Increasing the steps const will increase the number of samples.
            float stepDistance = (float)i / STEPS;

            // Map from unit circle space -> pixel space (using the light position).
            float2 sampleCoord = position + direction * stepDistance;

            if (sampleCoord.x > 1 || sampleCoord.x < 0 || sampleCoord.y > 1 || sampleCoord.y < 0) {
                return float4(1, 1, 1, 1);
            }

            // Get the colour in the occluder texture.
            float4 colourAtCoord = tex2D(OccluderTexture, sampleCoord);
            float alpha = colourAtCoord.a;

            // Check if we've hit an occluding pixel.
            if (alpha > 0.1) {
                return float4(stepDistance, stepDistance, stepDistance, 1.0);
            }
        }

        return float4(1, 1, 1, 1);
    }

    technique ShadowMap
    {
        pass Pass0
        {
            PixelShader = compile ps_3_0 ShadowMapEffect();
        }
    }