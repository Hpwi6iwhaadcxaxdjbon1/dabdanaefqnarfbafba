using System;
using Rust;
using UnityEngine;

// Token: 0x0200045B RID: 1115
public class ItemModProjectile : MonoBehaviour
{
	// Token: 0x0400173B RID: 5947
	public GameObjectRef projectileObject = new GameObjectRef();

	// Token: 0x0400173C RID: 5948
	public ItemModProjectileMod[] mods;

	// Token: 0x0400173D RID: 5949
	public AmmoTypes ammoType;

	// Token: 0x0400173E RID: 5950
	public int numProjectiles = 1;

	// Token: 0x0400173F RID: 5951
	public float projectileSpread;

	// Token: 0x04001740 RID: 5952
	public float projectileVelocity = 100f;

	// Token: 0x04001741 RID: 5953
	public float projectileVelocitySpread;

	// Token: 0x04001742 RID: 5954
	public bool useCurve;

	// Token: 0x04001743 RID: 5955
	public AnimationCurve spreadScalar;

	// Token: 0x04001744 RID: 5956
	public string category = "bullet";

	// Token: 0x06001A63 RID: 6755 RVA: 0x00015D68 File Offset: 0x00013F68
	public float GetRandomVelocity()
	{
		return this.projectileVelocity + Random.Range(-this.projectileVelocitySpread, this.projectileVelocitySpread);
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x00015D83 File Offset: 0x00013F83
	public float GetSpreadScalar()
	{
		if (this.useCurve)
		{
			return this.spreadScalar.Evaluate(Random.Range(0f, 1f));
		}
		return 1f;
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x00092CB4 File Offset: 0x00090EB4
	public float GetIndexedSpreadScalar(int shotIndex, int maxShots)
	{
		float time;
		if (shotIndex != -1)
		{
			float num = 1f / (float)maxShots;
			time = (float)shotIndex * num;
		}
		else
		{
			time = Random.Range(0f, 1f);
		}
		return this.spreadScalar.Evaluate(time);
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x00015DAD File Offset: 0x00013FAD
	public float GetAverageVelocity()
	{
		return this.projectileVelocity;
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x00015DB5 File Offset: 0x00013FB5
	public float GetMinVelocity()
	{
		return this.projectileVelocity - this.projectileVelocitySpread;
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x00015DC4 File Offset: 0x00013FC4
	public float GetMaxVelocity()
	{
		return this.projectileVelocity + this.projectileVelocitySpread;
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x00015DD3 File Offset: 0x00013FD3
	public bool IsAmmo(AmmoTypes ammo)
	{
		return (this.ammoType & ammo) > 0;
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x00092CF8 File Offset: 0x00090EF8
	public virtual void ServerProjectileHit(HitInfo info)
	{
		if (this.mods == null)
		{
			return;
		}
		foreach (ItemModProjectileMod itemModProjectileMod in this.mods)
		{
			if (!(itemModProjectileMod == null))
			{
				itemModProjectileMod.ServerProjectileHit(info);
			}
		}
	}
}
