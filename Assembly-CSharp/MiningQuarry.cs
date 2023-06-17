using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class MiningQuarry : BaseResourceExtractor
{
	// Token: 0x04001027 RID: 4135
	public Animator beltAnimator;

	// Token: 0x04001028 RID: 4136
	public Renderer beltScrollRenderer;

	// Token: 0x04001029 RID: 4137
	public int scrollMatIndex = 3;

	// Token: 0x0400102A RID: 4138
	public SoundPlayer[] onSounds;

	// Token: 0x0400102B RID: 4139
	public float processRate = 5f;

	// Token: 0x0400102C RID: 4140
	public float workToAdd = 15f;

	// Token: 0x0400102D RID: 4141
	public GameObjectRef bucketDropEffect;

	// Token: 0x0400102E RID: 4142
	public GameObject bucketDropTransform;

	// Token: 0x0400102F RID: 4143
	public MiningQuarry.ChildPrefab engineSwitchPrefab;

	// Token: 0x04001030 RID: 4144
	public MiningQuarry.ChildPrefab hopperPrefab;

	// Token: 0x04001031 RID: 4145
	public MiningQuarry.ChildPrefab fuelStoragePrefab;

	// Token: 0x04001032 RID: 4146
	public bool isStatic;

	// Token: 0x04001033 RID: 4147
	private List<SoundModulation.Modulator> soundGainMods = new List<SoundModulation.Modulator>();

	// Token: 0x04001034 RID: 4148
	private List<SoundModulation.Modulator> soundPitchMods = new List<SoundModulation.Modulator>();

	// Token: 0x04001035 RID: 4149
	private float beltSpeed;

	// Token: 0x060013C7 RID: 5063 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool IsEngineOn()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x0007C350 File Offset: 0x0007A550
	public void Update()
	{
		if (base.isClient)
		{
			bool flag = base.HasFlag(BaseEntity.Flags.On);
			this.beltSpeed = Mathf.MoveTowards(this.beltSpeed, flag ? 1f : 0f, Time.deltaTime);
			this.beltAnimator.SetFloat("beltspeed", Mathf.Clamp(this.beltSpeed, 1E-08f, 1f));
			for (int i = 0; i < this.onSounds.Length; i++)
			{
				SoundPlayer soundPlayer = this.onSounds[i];
				if (!(soundPlayer == null))
				{
					if (this.beltSpeed > 0f && !soundPlayer.IsPlaying())
					{
						soundPlayer.Play();
					}
					else if (this.beltSpeed <= 0f && soundPlayer.IsPlaying())
					{
						soundPlayer.Stop();
					}
				}
			}
			for (int j = 0; j < this.soundGainMods.Count; j++)
			{
				this.soundGainMods[j].value = Mathf.Lerp(0f, 1f, this.beltSpeed);
			}
			for (int k = 0; k < this.soundPitchMods.Count; k++)
			{
				this.soundPitchMods[k].value = Mathf.Lerp(0.5f, 1f, this.beltSpeed);
			}
		}
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x00010D63 File Offset: 0x0000EF63
	public void BucketDrop()
	{
		Effect.client.Run(this.bucketDropEffect.resourcePath, this.bucketDropTransform);
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0007C494 File Offset: 0x0007A694
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		foreach (SoundPlayer soundPlayer in this.onSounds)
		{
			if (soundPlayer)
			{
				soundPlayer.CreateSound();
				this.soundGainMods.Add(soundPlayer.sound.modulation.CreateModulator(SoundModulation.Parameter.Gain));
				this.soundPitchMods.Add(soundPlayer.sound.modulation.CreateModulator(SoundModulation.Parameter.Pitch));
			}
		}
	}

	// Token: 0x020002D9 RID: 729
	[Serializable]
	public class ChildPrefab
	{
		// Token: 0x04001036 RID: 4150
		public GameObjectRef prefabToSpawn;

		// Token: 0x04001037 RID: 4151
		public GameObject origin;

		// Token: 0x04001038 RID: 4152
		public BaseEntity instance;
	}
}
