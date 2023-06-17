using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200016B RID: 363
public class MixerSnapshotManager : MonoBehaviour
{
	// Token: 0x040009E6 RID: 2534
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x040009E7 RID: 2535
	public AudioMixerSnapshot underwaterSnapshot;

	// Token: 0x040009E8 RID: 2536
	public AudioMixerSnapshot loadingSnapshot;

	// Token: 0x040009E9 RID: 2537
	public AudioMixerSnapshot woundedSnapshot;

	// Token: 0x040009EA RID: 2538
	public SoundDefinition underwaterInSound;

	// Token: 0x040009EB RID: 2539
	public SoundDefinition underwaterOutSound;

	// Token: 0x040009EC RID: 2540
	public SoundDefinition woundedLoop;

	// Token: 0x040009ED RID: 2541
	private Sound woundedLoopSound;

	// Token: 0x040009EE RID: 2542
	internal AudioMixerSnapshot currentState;

	// Token: 0x06000CF1 RID: 3313 RVA: 0x0000BF4A File Offset: 0x0000A14A
	private void Awake()
	{
		this.woundedLoopSound = SoundManager.RequestSoundInstance(this.woundedLoop, base.gameObject, Vector3.zero, true);
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x0005E93C File Offset: 0x0005CB3C
	private void Update()
	{
		AudioMixerSnapshot targetState = this.GetTargetState();
		this.DoWoundedLoop();
		if (targetState != this.currentState)
		{
			if (targetState != null)
			{
				targetState.TransitionTo(0.25f);
			}
			if (targetState == this.underwaterSnapshot)
			{
				SoundManager.PlayOneshot(this.underwaterInSound, base.gameObject, true, default(Vector3));
			}
			if (this.currentState == this.underwaterSnapshot)
			{
				SoundManager.PlayOneshot(this.underwaterOutSound, base.gameObject, true, default(Vector3));
			}
			this.currentState = targetState;
		}
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x0005E9D8 File Offset: 0x0005CBD8
	private void DoWoundedLoop()
	{
		if (this.woundedLoopSound == null)
		{
			return;
		}
		if (LocalPlayer.Entity != null && LocalPlayer.Entity.IsWounded())
		{
			if (!this.woundedLoopSound.playing)
			{
				this.woundedLoopSound.Play();
				return;
			}
		}
		else if (this.woundedLoopSound != null && this.woundedLoopSound.playing)
		{
			this.woundedLoopSound.Stop();
		}
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x0005EA4C File Offset: 0x0005CC4C
	private AudioMixerSnapshot GetTargetState()
	{
		if (LoadingScreen.isOpen)
		{
			return this.loadingSnapshot;
		}
		if (WaterSystem.Collision != null && MainCamera.mainCamera != null && MainCamera.isWaterVisible && WaterLevel.Test(MainCamera.mainCamera.transform.position))
		{
			return this.underwaterSnapshot;
		}
		if (LocalPlayer.Entity != null && LocalPlayer.Entity.IsWounded())
		{
			return this.woundedSnapshot;
		}
		return this.defaultSnapshot;
	}
}
