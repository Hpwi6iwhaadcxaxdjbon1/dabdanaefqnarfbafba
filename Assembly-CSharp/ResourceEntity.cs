using System;
using UnityEngine.Serialization;

// Token: 0x02000321 RID: 801
public class ResourceEntity : BaseEntity
{
	// Token: 0x0400124C RID: 4684
	[FormerlySerializedAs("health")]
	public float startHealth;

	// Token: 0x0400124D RID: 4685
	[FormerlySerializedAs("protection")]
	public ProtectionProperties baseProtection;

	// Token: 0x0400124E RID: 4686
	protected float health;

	// Token: 0x0400124F RID: 4687
	[NonSerialized]
	protected bool isKilled;

	// Token: 0x06001583 RID: 5507 RVA: 0x00012359 File Offset: 0x00010559
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		this.health = info.msg.resource.health;
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x00084930 File Offset: 0x00082B30
	public override void InitShared()
	{
		base.InitShared();
		if (base.isClient)
		{
			DecorComponent[] components = PrefabAttribute.client.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(components);
		}
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void OnAttacked(HitInfo info)
	{
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x00005B85 File Offset: 0x00003D85
	public override float BoundsPadding()
	{
		return 1f;
	}
}
