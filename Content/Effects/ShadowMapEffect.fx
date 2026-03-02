sampler2D OcclurerTexture : register(s0);

float2 LightPosition;
float2 Resolution;

#define STEPS 400

float4 ShadowMapEffect(float2 texCoord: TEXCOORD0): COLOR0 {
    float angle = texCoord.x * 6.28318;
    float2 dir = float2(cos(angle), sin(angle));

    for (int i = 0; i < STEPS; i++) {
        float r = (float)i / STEPS;
        float2 sampleUV = (LightPosition / Resolution) + dir * r;

        if (sampleUV.x < 0 || sampleUV.x > 1 || sampleUV.y < 0 || sampleUV.y > 1) {
            return float4(1, 1, 1, 1);
        }

        float4 col = tex2D(OcclurerTexture, sampleUV);
        if (col.a > 0.1) {
            return float4(r, r, r, 1.0);
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