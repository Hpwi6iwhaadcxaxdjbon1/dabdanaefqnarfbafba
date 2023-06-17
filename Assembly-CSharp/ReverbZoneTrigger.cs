using System;
using Rust;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class ReverbZoneTrigger : TriggerBase, IClientComponentEx, ILOD, ISoundBudgetedUpdate
{
	// Token: 0x04000A69 RID: 2665
	public Collider trigger;

	// Token: 0x04000A6A RID: 2666
	public AudioReverbZone reverbZone;

	// Token: 0x04000A6B RID: 2667
	public float lodDistance = 100f;

	// Token: 0x04000A6C RID: 2668
	public bool inRange;

	// Token: 0x04000A6D RID: 2669
	public ReverbSettings reverbSettings;

	// Token: 0x04000A6E RID: 2670
	private int initialReverbLevel;

	// Token: 0x04000A6F RID: 2671
	private int targetReverbLevel;

	// Token: 0x04000A70 RID: 2672
	private int currentReverbLevel;

	// Token: 0x04000A71 RID: 2673
	private LODCell cell;

	// Token: 0x04000A72 RID: 2674
	private float decayHFRatio;

	// Token: 0x04000A73 RID: 2675
	private float decayTime;

	// Token: 0x04000A74 RID: 2676
	private float density;

	// Token: 0x04000A75 RID: 2677
	private float diffusion;

	// Token: 0x04000A76 RID: 2678
	private float HFReference;

	// Token: 0x04000A77 RID: 2679
	private float LFReference;

	// Token: 0x04000A78 RID: 2680
	private float maxDistance;

	// Token: 0x04000A79 RID: 2681
	private float minDistance;

	// Token: 0x04000A7A RID: 2682
	private int reflections;

	// Token: 0x04000A7B RID: 2683
	private float reflectionsDelay;

	// Token: 0x04000A7C RID: 2684
	private int reverb;

	// Token: 0x04000A7D RID: 2685
	private float reverbDelay;

	// Token: 0x04000A7E RID: 2686
	private int room;

	// Token: 0x04000A7F RID: 2687
	private int roomHF;

	// Token: 0x04000A80 RID: 2688
	private int roomLF;

	// Token: 0x06000D5F RID: 3423 RVA: 0x0000C4F2 File Offset: 0x0000A6F2
	protected void Awake()
	{
		this.initialReverbLevel = this.reverbZone.room;
		this.targetReverbLevel = -10000;
		this.currentReverbLevel = this.targetReverbLevel;
		this.DisableReverbZone();
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x0000C522 File Offset: 0x0000A722
	protected void OnEnable()
	{
		LODGrid.Add(this, base.transform, ref this.cell);
		SoundManager.AddBudgetedUpdatable(this);
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x0000C53C File Offset: 0x0000A73C
	protected override void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		base.OnDisable();
		LODGrid.Remove(this, base.transform, ref this.cell);
		SoundManager.RemoveBudgetedUpdatable(this);
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x000607A0 File Offset: 0x0005E9A0
	public void DoUpdate()
	{
		if (!this.inRange)
		{
			return;
		}
		using (TimeWarning.New("ReverbZoneTrigger.DoUpdate", 0.1f))
		{
			this.currentReverbLevel = (int)Mathf.Lerp((float)this.currentReverbLevel, (float)this.targetReverbLevel, Time.deltaTime);
			if (this.reverbZone != null)
			{
				this.reverbZone.room = this.currentReverbLevel;
			}
			if (this.currentReverbLevel <= -9900)
			{
				this.DisableReverbZone();
			}
			else
			{
				this.EnableReverbZone();
			}
		}
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0006083C File Offset: 0x0005EA3C
	private void ApplyReverbSettings()
	{
		if (this.reverbZone == null)
		{
			return;
		}
		if (this.reverbSettings != null)
		{
			this.reverbZone.reverbPreset = 27;
			this.reverbZone.decayHFRatio = this.reverbSettings.decayHFRatio;
			this.reverbZone.decayTime = this.reverbSettings.decayTime;
			this.reverbZone.density = this.reverbSettings.density;
			this.reverbZone.diffusion = this.reverbSettings.diffusion;
			this.reverbZone.HFReference = this.reverbSettings.HFReference;
			this.reverbZone.LFReference = this.reverbSettings.LFReference;
			this.reverbZone.reflections = this.reverbSettings.reflections;
			this.reverbZone.reflectionsDelay = this.reverbSettings.reflectionsDelay;
			this.reverbZone.reverb = this.reverbSettings.reverb;
			this.reverbZone.reverbDelay = this.reverbSettings.reverbDelay;
			this.reverbZone.room = this.reverbSettings.room;
			this.reverbZone.roomHF = this.reverbSettings.roomHF;
			this.reverbZone.roomLF = this.reverbSettings.roomLF;
		}
		else
		{
			this.reverbZone.decayHFRatio = this.decayHFRatio;
			this.reverbZone.decayTime = this.decayTime;
			this.reverbZone.density = this.density;
			this.reverbZone.diffusion = this.diffusion;
			this.reverbZone.HFReference = this.HFReference;
			this.reverbZone.LFReference = this.LFReference;
			this.reverbZone.reflections = this.reflections;
			this.reverbZone.reflectionsDelay = this.reflectionsDelay;
			this.reverbZone.reverb = this.reverb;
			this.reverbZone.reverbDelay = this.reverbDelay;
			this.reverbZone.room = this.room;
			this.reverbZone.roomHF = this.roomHF;
			this.reverbZone.roomLF = this.roomLF;
		}
		this.reverbZone.minDistance = this.minDistance;
		this.reverbZone.maxDistance = this.maxDistance;
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00060A98 File Offset: 0x0005EC98
	private void DisableReverbZone()
	{
		if (this.reverbZone == null)
		{
			return;
		}
		this.maxDistance = this.reverbZone.maxDistance;
		this.minDistance = this.reverbZone.minDistance;
		if (this.reverbSettings != null)
		{
			this.decayHFRatio = this.reverbZone.decayHFRatio;
			this.decayTime = this.reverbZone.decayTime;
			this.density = this.reverbZone.density;
			this.diffusion = this.reverbZone.diffusion;
			this.HFReference = this.reverbZone.HFReference;
			this.LFReference = this.reverbZone.LFReference;
			this.reflections = this.reverbZone.reflections;
			this.reflectionsDelay = this.reverbZone.reflectionsDelay;
			this.reverb = this.reverbZone.reverb;
			this.reverbDelay = this.reverbZone.reverbDelay;
			this.room = this.reverbZone.room;
			this.roomHF = this.reverbZone.roomHF;
			this.roomLF = this.reverbZone.roomLF;
		}
		Object.Destroy(this.reverbZone);
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0000C564 File Offset: 0x0000A764
	private void EnableReverbZone()
	{
		if (this.reverbZone != null)
		{
			return;
		}
		this.reverbZone = base.gameObject.AddComponent<AudioReverbZone>();
		this.ApplyReverbSettings();
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x00060BD0 File Offset: 0x0005EDD0
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if (basePlayer == null || !basePlayer.IsLocalPlayer())
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0000C58C File Offset: 0x0000A78C
	internal override void OnEntityEnter(BaseEntity ent)
	{
		this.targetReverbLevel = this.initialReverbLevel;
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x0000C59A File Offset: 0x0000A79A
	internal override void OnEntityLeave(BaseEntity ent)
	{
		this.targetReverbLevel = -10000;
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0000C5A7 File Offset: 0x0000A7A7
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x0000C5BB File Offset: 0x0000A7BB
	public void ChangeLOD()
	{
		this.inRange = (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(this.lodDistance));
		this.trigger.enabled = this.inRange;
		if (!this.inRange)
		{
			this.DisableReverbZone();
		}
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x0000C5FB File Offset: 0x0000A7FB
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.trigger);
		p.RemoveComponent(this.reverbZone);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}
}
