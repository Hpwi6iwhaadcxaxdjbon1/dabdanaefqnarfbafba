using System;
using System.Diagnostics;
using ConVar;
using Facepunch.Rust;
using Rust;

// Token: 0x02000318 RID: 792
public class PlayerVoiceRecorder : EntityComponent<BasePlayer>
{
	// Token: 0x040011FB RID: 4603
	internal static byte[] readBuffer = new byte[16384];

	// Token: 0x040011FC RID: 4604
	internal static byte[] readBufferUncompressed = new byte[16384];

	// Token: 0x040011FD RID: 4605
	private Stopwatch TalkTimer = new Stopwatch();

	// Token: 0x06001503 RID: 5379 RVA: 0x00011D70 File Offset: 0x0000FF70
	public void Init()
	{
		if (global::Client.Steam != null)
		{
			global::Client.Steam.Voice.OnCompressedData = new Action<byte[], int>(this.OnCompressedData);
		}
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x000823BC File Offset: 0x000805BC
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (global::Client.Steam != null && global::Client.Steam.Voice.OnCompressedData == new Action<byte[], int>(this.OnCompressedData))
		{
			global::Client.Steam.Voice.OnCompressedData = null;
		}
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x00011D94 File Offset: 0x0000FF94
	private void OnCompressedData(byte[] data, int length)
	{
		if (!this.CanTalk())
		{
			return;
		}
		base.baseEntity.SendVoiceData(data, length);
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x0008240C File Offset: 0x0008060C
	public void ClientFrame(BasePlayer player)
	{
		if (SingletonComponent<UI_LocalVoice>.Instance)
		{
			SingletonComponent<UI_LocalVoice>.Instance.SetLevel((global::Client.Steam.Voice.TimeSinceLastVoiceRecord.TotalSeconds < 0.20000000298023224) ? 1f : 0.2f);
		}
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x00082460 File Offset: 0x00080660
	public void ClientInput(InputState state)
	{
		bool flag = this.CanTalk() && ((Buttons.Voice.IsDown && !NeedsKeyboard.AnyActive()) || !ConVar.Client.pushtotalk);
		if (global::Client.Steam.Voice.WantsRecording && !flag)
		{
			this.StopRecording();
		}
		if (!global::Client.Steam.Voice.WantsRecording && flag)
		{
			this.StartRecording();
		}
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x000824D0 File Offset: 0x000806D0
	public bool CanTalk()
	{
		return base.baseEntity.IsAdmin || (!base.baseEntity.HasPlayerFlag(BasePlayer.PlayerFlags.VoiceMuted) && !base.baseEntity.IsDead() && !base.baseEntity.IsSleeping());
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x00082520 File Offset: 0x00080720
	public void StopRecording()
	{
		global::Client.Steam.Voice.WantsRecording = false;
		if (SingletonComponent<UI_LocalVoice>.Instance)
		{
			SingletonComponent<UI_LocalVoice>.Instance.SetRecording(false);
		}
		this.TalkTimer.Stop();
		Analytics.SecondsSpeaking += (float)this.TalkTimer.Elapsed.TotalSeconds;
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x00011DAC File Offset: 0x0000FFAC
	public void StartRecording()
	{
		global::Client.Steam.Voice.WantsRecording = true;
		if (SingletonComponent<UI_LocalVoice>.Instance)
		{
			SingletonComponent<UI_LocalVoice>.Instance.SetRecording(true);
		}
		this.TalkTimer.Reset();
		this.TalkTimer.Start();
	}
}
