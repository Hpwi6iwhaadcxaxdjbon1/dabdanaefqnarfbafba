using System;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class MusicUtil
{
	// Token: 0x04000A50 RID: 2640
	public const float OneSixteenth = 0.0625f;

	// Token: 0x06000D47 RID: 3399 RVA: 0x0000C3B5 File Offset: 0x0000A5B5
	public static double BeatsToSeconds(float tempo, float beats)
	{
		return 60.0 / (double)tempo * (double)beats;
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x0000C3C6 File Offset: 0x0000A5C6
	public static double BarsToSeconds(float tempo, float bars)
	{
		return MusicUtil.BeatsToSeconds(tempo, bars * 4f);
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x0000C3D5 File Offset: 0x0000A5D5
	public static int SecondsToSamples(double seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, AudioSettings.outputSampleRate);
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x0000C3E2 File Offset: 0x0000A5E2
	public static int SecondsToSamples(double seconds, int sampleRate)
	{
		return (int)((double)sampleRate * seconds);
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x0000C3E9 File Offset: 0x0000A5E9
	public static int SecondsToSamples(float seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, AudioSettings.outputSampleRate);
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x0000C3F6 File Offset: 0x0000A5F6
	public static int SecondsToSamples(float seconds, int sampleRate)
	{
		return (int)((float)sampleRate * seconds);
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x0000C3FD File Offset: 0x0000A5FD
	public static int BarsToSamples(float tempo, float bars, int sampleRate)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars), sampleRate);
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x0000C40C File Offset: 0x0000A60C
	public static int BarsToSamples(float tempo, float bars)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars));
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0000C41A File Offset: 0x0000A61A
	public static int BeatsToSamples(float tempo, float beats)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BeatsToSeconds(tempo, beats));
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x0000C428 File Offset: 0x0000A628
	public static float SecondsToBeats(float tempo, double seconds)
	{
		return tempo / 60f * (float)seconds;
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x0000C434 File Offset: 0x0000A634
	public static float SecondsToBars(float tempo, double seconds)
	{
		return MusicUtil.SecondsToBeats(tempo, seconds) / 4f;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0000C443 File Offset: 0x0000A643
	public static float Quantize(float position, float gridSize)
	{
		return Mathf.Round(position / gridSize) * gridSize;
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x0000C44F File Offset: 0x0000A64F
	public static float FlooredQuantize(float position, float gridSize)
	{
		return Mathf.Floor(position / gridSize) * gridSize;
	}
}
