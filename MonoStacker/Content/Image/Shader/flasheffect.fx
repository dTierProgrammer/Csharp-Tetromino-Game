#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float opacity;
float4 fadeColor;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
	
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates); // range 0 - 1
    float4 heldColor = color;
    float4 targetColor = float4(fadeColor.rgb, color.a);
	
    
    if (color.a > 0)
        color = lerp(color, targetColor, opacity);
	
    return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};