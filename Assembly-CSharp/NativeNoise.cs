using System;
using System.Runtime.InteropServices;
using System.Security;

// Token: 0x02000488 RID: 1160
[SuppressUnmanagedCodeSecurity]
public static class NativeNoise
{
	// Token: 0x06001B0B RID: 6923
	[DllImport("RustNative", EntryPoint = "snoise1_32")]
	public static extern float Simplex1D(float x);

	// Token: 0x06001B0C RID: 6924
	[DllImport("RustNative", EntryPoint = "sdnoise1_32")]
	public static extern float Simplex1D(float x, out float dx);

	// Token: 0x06001B0D RID: 6925
	[DllImport("RustNative", EntryPoint = "snoise2_32")]
	public static extern float Simplex2D(float x, float y);

	// Token: 0x06001B0E RID: 6926
	[DllImport("RustNative", EntryPoint = "sdnoise2_32")]
	public static extern float Simplex2D(float x, float y, out float dx, out float dy);

	// Token: 0x06001B0F RID: 6927
	[DllImport("RustNative", EntryPoint = "turbulence_32")]
	public static extern float Turbulence(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B10 RID: 6928
	[DllImport("RustNative", EntryPoint = "billow_32")]
	public static extern float Billow(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B11 RID: 6929
	[DllImport("RustNative", EntryPoint = "ridge_32")]
	public static extern float Ridge(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B12 RID: 6930
	[DllImport("RustNative", EntryPoint = "sharp_32")]
	public static extern float Sharp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B13 RID: 6931
	[DllImport("RustNative", EntryPoint = "turbulence_iq_32")]
	public static extern float TurbulenceIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B14 RID: 6932
	[DllImport("RustNative", EntryPoint = "billow_iq_32")]
	public static extern float BillowIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B15 RID: 6933
	[DllImport("RustNative", EntryPoint = "ridge_iq_32")]
	public static extern float RidgeIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B16 RID: 6934
	[DllImport("RustNative", EntryPoint = "sharp_iq_32")]
	public static extern float SharpIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06001B17 RID: 6935
	[DllImport("RustNative", EntryPoint = "turbulence_warp_32")]
	public static extern float TurbulenceWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06001B18 RID: 6936
	[DllImport("RustNative", EntryPoint = "billow_warp_32")]
	public static extern float BillowWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06001B19 RID: 6937
	[DllImport("RustNative", EntryPoint = "ridge_warp_32")]
	public static extern float RidgeWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06001B1A RID: 6938
	[DllImport("RustNative", EntryPoint = "sharp_warp_32")]
	public static extern float SharpWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06001B1B RID: 6939
	[DllImport("RustNative", EntryPoint = "jordan_32")]
	public static extern float Jordan(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp, float damp, float damp_scale);
}
