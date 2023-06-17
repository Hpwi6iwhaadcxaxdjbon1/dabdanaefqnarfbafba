using System;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class OnePoleLowpassFilter : MonoBehaviour
{
	// Token: 0x04000A54 RID: 2644
	[Range(10f, 20000f)]
	public float frequency = 20000f;

	// Token: 0x04000A55 RID: 2645
	private int sampleRate = 441000;

	// Token: 0x04000A56 RID: 2646
	private float c;

	// Token: 0x04000A57 RID: 2647
	private float a1;

	// Token: 0x04000A58 RID: 2648
	private float b1;

	// Token: 0x04000A59 RID: 2649
	private OnePoleLowpassFilter.ChannelData[] channelData = new OnePoleLowpassFilter.ChannelData[2];

	// Token: 0x04000A5A RID: 2650
	private float prevFrequency;

	// Token: 0x06000D58 RID: 3416 RVA: 0x0000C45B File Offset: 0x0000A65B
	public void Update()
	{
		this.sampleRate = AudioSettings.outputSampleRate;
		if (this.prevFrequency != this.frequency)
		{
			this.UpdateFilterCoefficients();
			this.prevFrequency = this.frequency;
		}
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x0006070C File Offset: 0x0005E90C
	private void OnAudioFilterRead(float[] data, int channels)
	{
		if (this.channelData.Length < channels)
		{
			this.SetupChannelData(channels);
		}
		for (int i = 0; i < data.Length; i++)
		{
			int num = i % channels;
			OnePoleLowpassFilter.ChannelData channelData = this.channelData[num];
			data[i] = this.a1 * data[i] - this.b1 * channelData.out1;
			channelData.out1 = data[i];
		}
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x0006076C File Offset: 0x0005E96C
	private void SetupChannelData(int channels)
	{
		this.channelData = new OnePoleLowpassFilter.ChannelData[channels];
		for (int i = 0; i < channels; i++)
		{
			this.channelData[i] = new OnePoleLowpassFilter.ChannelData();
		}
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x0000C488 File Offset: 0x0000A688
	private void UpdateFilterCoefficients()
	{
		this.c = Mathf.Exp(-6.2831855f * this.frequency / (float)this.sampleRate);
		this.a1 = 1f - this.c;
		this.b1 = -this.c;
	}

	// Token: 0x0200017C RID: 380
	private class ChannelData
	{
		// Token: 0x04000A5B RID: 2651
		public float out1;
	}
}
