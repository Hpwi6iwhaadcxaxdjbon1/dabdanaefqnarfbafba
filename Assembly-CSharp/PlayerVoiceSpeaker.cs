using System;
using System.IO;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class PlayerVoiceSpeaker : EntityComponent<BasePlayer>
{
	// Token: 0x040011FE RID: 4606
	public AudioSource mouthSpeaker;

	// Token: 0x040011FF RID: 4607
	[NonSerialized]
	public float currentVolume;

	// Token: 0x04001200 RID: 4608
	private uint optimalRate;

	// Token: 0x04001201 RID: 4609
	private uint bufferSize;

	// Token: 0x04001202 RID: 4610
	private float[] buffer;

	// Token: 0x04001203 RID: 4611
	private uint dataReceived;

	// Token: 0x04001204 RID: 4612
	private uint playbackBuffer;

	// Token: 0x04001205 RID: 4613
	private uint dataPosition;

	// Token: 0x04001206 RID: 4614
	private bool Initialized;

	// Token: 0x04001207 RID: 4615
	private bool isPlaying;

	// Token: 0x04001208 RID: 4616
	private bool stopping;

	// Token: 0x04001209 RID: 4617
	private float[] volumeData = new float[32];

	// Token: 0x0400120A RID: 4618
	internal float volumeVelocity;

	// Token: 0x0400120B RID: 4619
	private static MemoryStream decompressStream = new MemoryStream();

	// Token: 0x0600150D RID: 5389 RVA: 0x00011E1E File Offset: 0x0001001E
	public bool IsSpeaking()
	{
		return this.mouthSpeaker && this.mouthSpeaker.isPlaying;
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x00082580 File Offset: 0x00080780
	private void InitializeSpeaker()
	{
		if (this.Initialized)
		{
			return;
		}
		this.Initialized = true;
		this.optimalRate = global::Client.Steam.Voice.OptimalSampleRate;
		this.bufferSize = this.optimalRate * 5U;
		this.buffer = new float[this.bufferSize];
		this.mouthSpeaker.clip = AudioClip.Create("VoiceData", 256, 1, (int)this.optimalRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead), null);
		this.mouthSpeaker.loop = true;
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x0008260C File Offset: 0x0008080C
	public void ClientFrame(BasePlayer player)
	{
		if (!this.Initialized)
		{
			return;
		}
		if (this.playbackBuffer == 0U)
		{
			if (!this.stopping)
			{
				this.stopping = true;
				base.Invoke(new Action(this.StopPlayback), 1f);
			}
		}
		else if (this.stopping)
		{
			base.CancelInvoke(new Action(this.StopPlayback));
			this.stopping = false;
		}
		if (!this.isPlaying)
		{
			if (this.playbackBuffer > 0U)
			{
				this.StartPlaying();
			}
			this.currentVolume = Mathf.SmoothDamp(this.currentVolume, 0f, ref this.volumeVelocity, 0.05f);
			return;
		}
		this.mouthSpeaker.GetOutputData(this.volumeData, 0);
		float num = 0f;
		foreach (float num2 in this.volumeData)
		{
			num += num2 * num2;
		}
		num = Mathf.Sqrt(num / (float)this.volumeData.Length);
		num = Mathf.Clamp01(num * 10f);
		num = Mathf.Clamp01(num * 3f - 1f);
		this.currentVolume = Mathf.SmoothDamp(this.currentVolume, num, ref this.volumeVelocity, 0.03f);
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x00011E3A File Offset: 0x0001003A
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.StopPlayback();
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x00082734 File Offset: 0x00080934
	private void StopPlayback()
	{
		this.currentVolume = 0f;
		this.volumeVelocity = 0f;
		this.isPlaying = false;
		if (this.mouthSpeaker)
		{
			this.mouthSpeaker.Pause();
		}
		base.baseEntity;
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x00082784 File Offset: 0x00080984
	private void StartPlaying()
	{
		this.isPlaying = true;
		this.mouthSpeaker.Play();
		this.mouthSpeaker.UnPause();
		if ((base.baseEntity.IsLocalPlayer() && !Voice.loopback) || base.baseEntity.HasPlayerFlag(BasePlayer.PlayerFlags.VoiceMuted))
		{
			this.mouthSpeaker.mute = true;
			return;
		}
		this.mouthSpeaker.mute = false;
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x000827F0 File Offset: 0x000809F0
	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = 0f;
			if (this.playbackBuffer > 0U)
			{
				this.dataPosition += 1U;
				this.playbackBuffer -= 1U;
				data[i] = this.buffer[(int)(this.dataPosition % this.bufferSize)];
			}
		}
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x00082850 File Offset: 0x00080A50
	public void Receive(byte[] data)
	{
		this.InitializeSpeaker();
		if (!this.Initialized)
		{
			return;
		}
		if (!this.ShouldReceiveVoice())
		{
			return;
		}
		PlayerVoiceSpeaker.decompressStream.Position = 0L;
		if (!global::Client.Steam.Voice.Decompress(data, PlayerVoiceSpeaker.decompressStream, this.optimalRate))
		{
			return;
		}
		this.WriteToClip(PlayerVoiceSpeaker.decompressStream.GetBuffer(), (int)PlayerVoiceSpeaker.decompressStream.Position);
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x000828BC File Offset: 0x00080ABC
	private void WriteToClip(byte[] uncompressed, int iSize)
	{
		for (int i = 0; i < iSize; i += 2)
		{
			this.WriteToClip((float)((short)((int)uncompressed[i] | (int)uncompressed[i + 1] << 8)) / 32767f);
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x00011E4A File Offset: 0x0001004A
	private void WriteToClip(float f)
	{
		this.buffer[(int)(this.dataReceived % this.bufferSize)] = f;
		this.dataReceived += 1U;
		this.playbackBuffer += 1U;
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x000828F0 File Offset: 0x00080AF0
	private bool ShouldReceiveVoice()
	{
		return !(base.baseEntity == null) && !base.baseEntity.IsDead() && (!LocalPlayer.Entity || !LocalPlayer.Entity.IsDead() || LocalPlayer.Entity.IsAdmin || LocalPlayer.Entity.IsDeveloper);
	}
}
