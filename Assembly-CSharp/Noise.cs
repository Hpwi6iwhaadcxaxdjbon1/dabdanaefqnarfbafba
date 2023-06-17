using System;

// Token: 0x02000489 RID: 1161
public static class Noise
{
	// Token: 0x040017E8 RID: 6120
	public const float MIN = -1000000f;

	// Token: 0x040017E9 RID: 6121
	public const float MAX = 1000000f;

	// Token: 0x06001B1C RID: 6940 RVA: 0x00016572 File Offset: 0x00014772
	public static float Simplex1D(float x)
	{
		return NativeNoise.Simplex1D(x);
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x0001657A File Offset: 0x0001477A
	public static float Simplex2D(float x, float y)
	{
		return NativeNoise.Simplex2D(x, y);
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x00016583 File Offset: 0x00014783
	public static float Turbulence(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Turbulence(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x00016594 File Offset: 0x00014794
	public static float Billow(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Billow(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B20 RID: 6944 RVA: 0x000165A5 File Offset: 0x000147A5
	public static float Ridge(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Ridge(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B21 RID: 6945 RVA: 0x000165B6 File Offset: 0x000147B6
	public static float Sharp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Sharp(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B22 RID: 6946 RVA: 0x000165C7 File Offset: 0x000147C7
	public static float TurbulenceIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.TurbulenceIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B23 RID: 6947 RVA: 0x000165D8 File Offset: 0x000147D8
	public static float BillowIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.BillowIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B24 RID: 6948 RVA: 0x000165E9 File Offset: 0x000147E9
	public static float RidgeIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.RidgeIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B25 RID: 6949 RVA: 0x000165FA File Offset: 0x000147FA
	public static float SharpIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.SharpIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06001B26 RID: 6950 RVA: 0x0001660B File Offset: 0x0001480B
	public static float TurbulenceWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.TurbulenceWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06001B27 RID: 6951 RVA: 0x0001661E File Offset: 0x0001481E
	public static float BillowWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.BillowWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x00016631 File Offset: 0x00014831
	public static float RidgeWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.RidgeWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x00016644 File Offset: 0x00014844
	public static float SharpWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.SharpWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x00095ECC File Offset: 0x000940CC
	public static float Jordan(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 1f, float damp = 1f, float damp_scale = 1f)
	{
		return NativeNoise.Jordan(x, y, octaves, frequency, amplitude, lacunarity, gain, warp, damp, damp_scale);
	}
}
