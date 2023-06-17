using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

// Token: 0x020002F8 RID: 760
public class SmokeGrenade : TimedExplosive
{
	// Token: 0x040010F2 RID: 4338
	public float smokeDuration = 45f;

	// Token: 0x040010F3 RID: 4339
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x040010F4 RID: 4340
	public GameObjectRef igniteSound;

	// Token: 0x040010F5 RID: 4341
	public SoundPlayer soundLoop;

	// Token: 0x040010F6 RID: 4342
	private GameObject smokeEffectInstance;

	// Token: 0x040010F7 RID: 4343
	public static List<SmokeGrenade> activeGrenades = new List<SmokeGrenade>();

	// Token: 0x06001416 RID: 5142 RVA: 0x0001125A File Offset: 0x0000F45A
	public override void DestroyShared()
	{
		SmokeGrenade.activeGrenades.Remove(this);
		base.DestroyShared();
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x0007D568 File Offset: 0x0007B768
	public void EnsureSmokeEffectEnabled()
	{
		if (this.smokeEffectInstance == null)
		{
			Effect.client.Run(this.igniteSound.resourcePath, base.transform.position, Vector3.up, default(Vector3));
			this.smokeEffectInstance = GameManager.client.CreatePrefab(this.smokeEffectPrefab.resourcePath, base.transform.position, Quaternion.identity, true);
			this.soundLoop.FadeInAndPlay(0.25f);
		}
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x0001126E File Offset: 0x0000F46E
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		SmokeGrenade.activeGrenades.Add(this);
		this.EnsureSmokeEffectEnabled();
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x00011288 File Offset: 0x0000F488
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && base.HasFlag(BaseEntity.Flags.On))
		{
			this.EnsureSmokeEffectEnabled();
		}
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x0007D5E8 File Offset: 0x0007B7E8
	public void OnDestroy()
	{
		if (base.isClient && this.smokeEffectInstance)
		{
			this.smokeEffectInstance.BroadcastOnParentDestroying();
			if (this.soundLoop != null && this.soundLoop.gameObject != null)
			{
				this.soundLoop.FadeOutAndRecycle(1f);
				this.soundLoop.transform.parent = null;
				this.soundLoop.transform.position = base.transform.position;
				GameManager.Destroy(this.soundLoop.gameObject, 2f);
			}
			this.smokeEffectInstance = null;
		}
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0007D694 File Offset: 0x0007B894
	public void Update()
	{
		if (this.smokeEffectInstance)
		{
			this.smokeEffectInstance.transform.position = base.transform.position + Vector3.up * 1.25f;
			this.smokeEffectInstance.transform.rotation = Quaternion.LookRotation(Vector3.up);
		}
	}
}
