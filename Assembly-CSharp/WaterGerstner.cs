using System;
using UnityEngine;

// Token: 0x0200055C RID: 1372
public class WaterGerstner
{
	// Token: 0x06001F07 RID: 7943 RVA: 0x000A9464 File Offset: 0x000A7664
	public static WaterGerstner.Wave[] SetupWaves(Vector3 wind, WaterGerstner.WaveSettings settings)
	{
		Random.State state = Random.state;
		Random.InitState(settings.RandomSeed);
		int waveCount = settings.WaveCount;
		float num = Mathf.Atan2(wind.z, wind.x);
		float num2 = (float)(1 / waveCount);
		float amplitude = settings.Amplitude;
		float length = settings.Length;
		float steepness = settings.Steepness;
		WaterGerstner.Wave[] array = new WaterGerstner.Wave[waveCount];
		for (int i = 0; i < waveCount; i++)
		{
			float num3 = Mathf.Lerp(0.5f, 1.5f, (float)i * num2);
			float f = num + 0.017453292f * Random.Range(-settings.AngleSpread, settings.AngleSpread);
			Vector2 direction = new Vector2(-Mathf.Cos(f), -Mathf.Sin(f));
			float amplitude2 = amplitude * num3 * Random.Range(0.8f, 1.2f);
			float length2 = length * num3 * Random.Range(0.6f, 1.4f);
			float steepness2 = Mathf.Clamp01(steepness * num3 * Random.Range(0.6f, 1.4f));
			array[i] = new WaterGerstner.Wave(waveCount, direction, amplitude2, length2, steepness2);
			Random.InitState(settings.RandomSeed + i + 1);
		}
		Random.state = state;
		return array;
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x000A9598 File Offset: 0x000A7798
	public static void SampleWaves(WaterGerstner.Wave[] waves, Vector3 location, out Vector3 position, out Vector3 normal)
	{
		Vector2 rhs = new Vector2(location.x, location.z);
		float waveTime = WaterSystem.WaveTime;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		uint num = 0U;
		while ((ulong)num < (ulong)((long)waves.Length))
		{
			float wi = waves[(int)num].wi;
			float phi = waves[(int)num].phi;
			float wa = waves[(int)num].WA;
			Vector2 di = waves[(int)num].Di;
			float ai = waves[(int)num].Ai;
			float qi = waves[(int)num].Qi;
			float f = wi * Vector2.Dot(di, rhs) + phi * waveTime;
			float num2 = Mathf.Sin(f);
			float num3 = Mathf.Cos(f);
			zero.x += qi * ai * di.x * num3;
			zero.y += qi * ai * di.y * num3;
			zero.z += ai * num2;
			zero2.x += di.x * wa * num3;
			zero2.y += di.y * wa * num3;
			zero2.z += qi * wa * num2;
			num += 1U;
		}
		position = new Vector3(zero.x, zero.z, zero.y);
		normal = new Vector3(-zero2.x, 1f - zero2.z, -zero2.y);
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x000A9724 File Offset: 0x000A7924
	public static float SampleHeight(WaterGerstner.Wave[] waves, Vector3 location)
	{
		Vector2 rhs = new Vector2(location.x, location.z);
		float waveTime = WaterSystem.WaveTime;
		float num = 0f;
		uint num2 = 0U;
		while ((ulong)num2 < (ulong)((long)waves.Length))
		{
			float wi = waves[(int)num2].wi;
			float phi = waves[(int)num2].phi;
			Vector2 di = waves[(int)num2].Di;
			float ai = waves[(int)num2].Ai;
			float num3 = Mathf.Sin(wi * Vector2.Dot(di, rhs) + phi * waveTime);
			num += ai * num3;
			num2 += 1U;
		}
		return num;
	}

	// Token: 0x0200055D RID: 1373
	[Serializable]
	public class WaveSettings
	{
		// Token: 0x04001B40 RID: 6976
		[Range(1f, 16f)]
		public int WaveCount = 6;

		// Token: 0x04001B41 RID: 6977
		public float Amplitude = 0.33f;

		// Token: 0x04001B42 RID: 6978
		public float Length = 10f;

		// Token: 0x04001B43 RID: 6979
		public float AngleSpread = 45f;

		// Token: 0x04001B44 RID: 6980
		[Range(0f, 1f)]
		public float Steepness = 0.8f;

		// Token: 0x04001B45 RID: 6981
		public int RandomSeed = 1234;
	}

	// Token: 0x0200055E RID: 1374
	[Serializable]
	public struct Wave
	{
		// Token: 0x04001B46 RID: 6982
		private const float MaxFrequency = 5f;

		// Token: 0x04001B47 RID: 6983
		public float wi;

		// Token: 0x04001B48 RID: 6984
		public float phi;

		// Token: 0x04001B49 RID: 6985
		public float WA;

		// Token: 0x04001B4A RID: 6986
		public Vector2 Di;

		// Token: 0x04001B4B RID: 6987
		public float Ai;

		// Token: 0x04001B4C RID: 6988
		public float Qi;

		// Token: 0x06001F0C RID: 7948 RVA: 0x000A9808 File Offset: 0x000A7A08
		public Wave(int waveCount, Vector2 direction, float amplitude, float length, float steepness)
		{
			this.wi = 2f / length;
			this.phi = Mathf.Min(5f, Mathf.Sqrt(30.819027f * this.wi)) * this.wi;
			this.WA = this.wi * amplitude;
			this.Di = direction;
			this.Ai = amplitude;
			this.Qi = steepness / (this.WA * (float)waveCount);
		}
	}
}
