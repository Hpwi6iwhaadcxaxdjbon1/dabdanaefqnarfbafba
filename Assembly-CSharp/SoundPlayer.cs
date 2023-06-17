using System;
using Rust;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class SoundPlayer : BaseMonoBehaviour, IClientComponent, IOnParentDestroying
{
	// Token: 0x04000B09 RID: 2825
	public SoundDefinition soundDefinition;

	// Token: 0x04000B0A RID: 2826
	public bool playImmediately = true;

	// Token: 0x04000B0B RID: 2827
	public bool debugRepeat;

	// Token: 0x04000B0C RID: 2828
	public bool pending;

	// Token: 0x04000B0D RID: 2829
	public Vector3 soundOffset = Vector3.zero;

	// Token: 0x04000B0E RID: 2830
	private int playOnFrame;

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x0000CC32 File Offset: 0x0000AE32
	// (set) Token: 0x06000DEA RID: 3562 RVA: 0x0000CC3A File Offset: 0x0000AE3A
	public Sound sound { get; protected set; }

	// Token: 0x06000DEB RID: 3563 RVA: 0x0000CC43 File Offset: 0x0000AE43
	protected void Awake()
	{
		if (this.soundDefinition == null)
		{
			Debug.LogError("SoundDefinition missing from SoundPlayer on " + base.gameObject);
		}
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0000CC68 File Offset: 0x0000AE68
	protected void OnEnable()
	{
		if (Application.isLoadingPrefabs)
		{
			return;
		}
		if (this.playImmediately)
		{
			this.playOnFrame = Time.frameCount + 1;
			this.pending = true;
			SoundManager.AddPendingSoundPlayer(this);
		}
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0000CC94 File Offset: 0x0000AE94
	public void DoPendingUpdate()
	{
		if (this.ShouldStartThisFrame())
		{
			this.Play();
		}
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x0000CCA4 File Offset: 0x0000AEA4
	public bool ShouldStartThisFrame()
	{
		return this.pending && Time.frameCount >= this.playOnFrame;
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x00063068 File Offset: 0x00061268
	public void Play()
	{
		if (this.debugRepeat)
		{
			this.DestroySound();
			base.Invoke(new Action(this.Play), Random.Range(2f, 6f));
		}
		this.CreateSound();
		if (this.sound)
		{
			this.sound.Play();
		}
		this.pending = false;
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x0000CCC0 File Offset: 0x0000AEC0
	public void PlayOneshot()
	{
		this.CreateSound();
		if (this.sound)
		{
			this.sound.Play();
			this.sound.RecycleAfterPlaying();
			this.sound = null;
		}
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0000CCF2 File Offset: 0x0000AEF2
	public void FadeInAndPlay(float time = 0.5f)
	{
		this.CreateSound();
		if (this.sound)
		{
			this.sound.FadeInAndPlay(time);
		}
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0000CD13 File Offset: 0x0000AF13
	public void FadeOutAndRecycle(float time = 0.5f)
	{
		if (this.sound != null)
		{
			this.sound.FadeOutAndRecycle(time);
			this.sound = null;
		}
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x000630CC File Offset: 0x000612CC
	public virtual void CreateSound()
	{
		if (this.sound != null)
		{
			return;
		}
		this.sound = SoundManager.RequestSoundInstance(this.soundDefinition, base.gameObject, default(Vector3), false);
		if (this.sound != null)
		{
			this.sound.transform.localPosition += this.soundOffset;
		}
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0000CD36 File Offset: 0x0000AF36
	public bool IsPlaying()
	{
		return this.sound != null && this.sound.playing;
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0000CD53 File Offset: 0x0000AF53
	public bool HasSound()
	{
		return this.sound != null;
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0000CD61 File Offset: 0x0000AF61
	public void DestroySound()
	{
		if (this.sound == null)
		{
			return;
		}
		this.sound.StopAndRecycle(0f);
		this.sound.DisconnectFromParent();
		this.sound = null;
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0000CD94 File Offset: 0x0000AF94
	public void Stop()
	{
		if (this.sound)
		{
			this.sound.Stop();
		}
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0000CDAE File Offset: 0x0000AFAE
	public void MakeThirdPerson()
	{
		this.CreateSound();
		if (this.sound)
		{
			this.sound.MakeThirdPerson();
		}
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x0000CDCE File Offset: 0x0000AFCE
	public void MakeFirstPerson()
	{
		this.CreateSound();
		if (this.sound)
		{
			this.sound.MakeFirstPerson();
		}
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x0000CDEE File Offset: 0x0000AFEE
	public float GetLength()
	{
		if (this.soundDefinition)
		{
			return this.soundDefinition.GetLength();
		}
		return 0f;
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0000CE0E File Offset: 0x0000B00E
	public void OnParentDestroying()
	{
		this.sound = null;
	}
}
