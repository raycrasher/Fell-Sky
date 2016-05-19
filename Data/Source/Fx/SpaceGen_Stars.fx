#include "Noise.fxh"

float uDensity;
float uRenderScale;

float qnoise(float2 p) {
    return frac(sin(dot(p.xy, float2(12.9898, 78.233))) * 43758.5453);
}

float exprand(float2 p) {
    float u = qnoise(p);
    return log(1.0 - u)/(-2.0 * 3.14159);
}

float noise(float2 p) {
    return qnoise(p) * 0.5 + 0.5;
}

float4 PixelShaderFunc(float4 position: SV_Position) : COLOR0
{
	float2 p = position.xy / uRenderScale;
	float c = 0.0f;
	
	if (qnoise(p*2.0+11.0) < uDensity) {
        c = exprand(p);
    }
	
    return float4(1,1,1,c);
}

technique SpaceGen_Nebula
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunc();
    }
}
