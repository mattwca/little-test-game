sampler2D Texture : register(s0);

float4 SpritePixelShader(
    float4 position : SV_Position,
    float4 color : COLOR0,
    float2 texCoord : TEXCOORD0
) : SV_Target0
{
    return tex2D(Texture, texCoord) * color;
}

technique SpriteBatch { 
    pass { 
        PixelShader = compile ps_2_0 SpritePixelShader();
    }
}