#include "Noise.fxh"

float uFalloff;
float uIntensity;
float uScale;
float uRenderScale;
float3 uColor;
float2 uOffset;

float noise(float2 p) {
    return cnoise(float3(p, 0)) * 0.5 + 0.5;
}

float nebula(float2 p) {
    const int steps = 5;
    float scale = pow(2.0, float(steps));
    float2 displace = float2(0,0);
    for (int i = 0; i < steps; i++) {
        displace = float2(noise(p.xy * scale + displace), noise(p.yx * scale + displace));
        scale *= 0.5;
    }
    return noise(p * scale + displace);
}

float4 PixelShaderFunc(float4 position: SV_POSITION): SV_Target
{
    float2 p = position.xy/uRenderScale+uOffset;
    float c = abs(min(1.0, nebula(p * uScale) * uIntensity));
    c = pow(c, uFalloff);
    return float4(uColor, c);
	//return float4(1,1,1,1);
}

technique SpaceGen_Nebula
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunc();
    }
}

