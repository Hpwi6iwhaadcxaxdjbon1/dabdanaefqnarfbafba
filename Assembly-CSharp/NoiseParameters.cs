using System;

// Token: 0x0200048A RID: 1162
[Serializable]
public struct NoiseParameters
{
	// Token: 0x040017EA RID: 6122
	public int Octaves;

	// Token: 0x040017EB RID: 6123
	public float Frequency;

	// Token: 0x040017EC RID: 6124
	public float Amplitude;

	// Token: 0x040017ED RID: 6125
	public float Offset;

	// Token: 0x06001B2B RID: 6955 RVA: 0x00016657 File Offset: 0x00014857
	public NoiseParameters(int octaves, float frequency, float amplitude, float offset)
	{
		this.Octaves = octaves;
		this.Frequency = frequency;
		this.Amplitude = amplitude;
		this.Offset = offset;
	}
}
