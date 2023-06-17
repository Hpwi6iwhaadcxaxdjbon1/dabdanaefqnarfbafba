using System;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class LockedByEntCrate : LootContainer
{
	// Token: 0x040010BC RID: 4284
	public GameObject lockingEnt;

	// Token: 0x060013FF RID: 5119 RVA: 0x0007D298 File Offset: 0x0007B498
	public void SetLockingEnt(GameObject ent)
	{
		base.CancelInvoke(new Action(this.Think));
		this.SetLocked(false);
		this.lockingEnt = ent;
		if (this.lockingEnt != null)
		{
			base.InvokeRepeating(new Action(this.Think), Random.Range(0f, 1f), 1f);
			this.SetLocked(true);
		}
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x000110CD File Offset: 0x0000F2CD
	public void SetLocked(bool isLocked)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, isLocked, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, isLocked, false, true);
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x000110E4 File Offset: 0x0000F2E4
	public void Think()
	{
		if (this.lockingEnt == null && base.IsLocked())
		{
			this.SetLockingEnt(null);
		}
	}
}
