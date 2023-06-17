using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

// Token: 0x0200018E RID: 398
[RequireComponent(typeof(OnePoleLowpassFilter))]
public class SoundOcclusion : MonoBehaviour
{
	// Token: 0x04000AFE RID: 2814
	public LayerMask occlusionLayerMask;

	// Token: 0x04000B00 RID: 2816
	private float occlusionAmount;

	// Token: 0x04000B01 RID: 2817
	private Sound sound;

	// Token: 0x04000B02 RID: 2818
	private OnePoleLowpassFilter lowpass;

	// Token: 0x04000B03 RID: 2819
	private SoundModulation.Modulator gainMod;

	// Token: 0x04000B04 RID: 2820
	private Vector3 soundOffset = new Vector3(0f, 0.15f, 0f);

	// Token: 0x04000B05 RID: 2821
	private float lastOcclusionCheck;

	// Token: 0x04000B06 RID: 2822
	private float occlusionCheckInterval = 0.25f;

	// Token: 0x04000B07 RID: 2823
	private Ray ray;

	// Token: 0x04000B08 RID: 2824
	private List<RaycastHit> hits = new List<RaycastHit>();

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x0000CBCF File Offset: 0x0000ADCF
	// (set) Token: 0x06000DE3 RID: 3555 RVA: 0x0000CBD7 File Offset: 0x0000ADD7
	public bool isOccluded { get; private set; }

	// Token: 0x06000DE4 RID: 3556 RVA: 0x0000CBE0 File Offset: 0x0000ADE0
	public void Awake()
	{
		this.sound = base.GetComponent<Sound>();
		this.lowpass = base.GetComponent<OnePoleLowpassFilter>();
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x00062C98 File Offset: 0x00060E98
	public void Init()
	{
		if (this.lowpass == null)
		{
			return;
		}
		this.lowpass.enabled = false;
		this.lastOcclusionCheck = 0f;
		this.isOccluded = false;
		if (this.gainMod != null)
		{
			this.sound.modulation.RemoveModulator(this.gainMod);
			this.gainMod = null;
		}
		if (this.sound.definition.soundClass.occlusionGain != 1f)
		{
			this.gainMod = this.sound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
		}
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00062D2C File Offset: 0x00060F2C
	public void UpdateOcclusion(bool lerp = false)
	{
		using (TimeWarning.New("SoundOcclusion.UpdateOcclusion", 0.1f))
		{
			if (MainCamera.Distance(base.transform.position) <= this.sound.maxDistance)
			{
				if (this.sound.definition.soundClass.enableOcclusion)
				{
					this.DoOcclusionCheck();
					if (this.sound.definition.soundClass.occlusionGain != 1f)
					{
						float num = 1f - this.sound.definition.soundClass.occlusionGain;
						float num2 = 1f - num * this.occlusionAmount;
						float num3 = this.isOccluded ? num2 : 1f;
						this.gainMod.value = (lerp ? Mathf.Lerp(this.gainMod.value, num3, UnityEngine.Time.deltaTime * 10f) : num3);
					}
				}
			}
		}
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00062E2C File Offset: 0x0006102C
	public void DoOcclusionCheck()
	{
		if (this.sound.isFirstPerson)
		{
			this.isOccluded = false;
			this.occlusionAmount = 0f;
			return;
		}
		if (Audio.advancedocclusion && this.sound.soundSource != null && this.sound.soundSource.handleOcclusionChecks && this.sound.soundSource.inRange)
		{
			this.isOccluded = this.sound.soundSource.isOccluded;
			this.occlusionAmount = this.sound.soundSource.occlusionAmount;
			return;
		}
		if (UnityEngine.Time.time < this.lastOcclusionCheck + this.occlusionCheckInterval)
		{
			return;
		}
		using (TimeWarning.New("SoundOcclusion.DoOcclusionCheck", 0.1f))
		{
			this.hits.Clear();
			this.isOccluded = false;
			this.occlusionAmount = 0f;
			Vector3 vector = base.transform.position + this.soundOffset;
			float maxDistance = Vector3.Distance(vector, MainCamera.position);
			this.ray.origin = vector;
			this.ray.direction = MainCamera.position - vector;
			GamePhysics.TraceAll(this.ray, 0f, this.hits, maxDistance, this.occlusionLayerMask, 0);
			for (int i = 0; i < this.hits.Count; i++)
			{
				BaseEntity componentInParent = this.hits[i].collider.GetComponentInParent<BaseEntity>();
				if (!this.sound.transform.IsChildOf(this.hits[i].collider.transform) && (componentInParent == null || (!this.sound.transform.IsChildOf(componentInParent.transform) && componentInParent.isClient)))
				{
					this.isOccluded = true;
					this.occlusionAmount = 1f;
				}
			}
			if (this.sound.definition.forceOccludedPlayback)
			{
				this.isOccluded = true;
				this.occlusionAmount = 1f;
			}
			this.lastOcclusionCheck = UnityEngine.Time.time;
		}
	}
}
