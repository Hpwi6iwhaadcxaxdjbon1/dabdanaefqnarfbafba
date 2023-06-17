using System;
using UnityEngine;

// Token: 0x020002CF RID: 719
public struct EntityRef
{
	// Token: 0x04000FFA RID: 4090
	internal BaseEntity ent_cached;

	// Token: 0x04000FFB RID: 4091
	internal uint id_cached;

	// Token: 0x060013A9 RID: 5033 RVA: 0x00010AAC File Offset: 0x0000ECAC
	public bool IsSet()
	{
		return this.id_cached > 0U;
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x00010AB7 File Offset: 0x0000ECB7
	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x00010AC5 File Offset: 0x0000ECC5
	public void Set(BaseEntity ent)
	{
		this.ent_cached = ent;
		this.id_cached = 0U;
		if (this.ent_cached.IsValid())
		{
			this.id_cached = this.ent_cached.net.ID;
		}
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0007C15C File Offset: 0x0007A35C
	public BaseEntity Get(bool serverside)
	{
		if (this.ent_cached == null && this.id_cached > 0U)
		{
			if (!serverside)
			{
				this.ent_cached = (BaseNetworkable.clientEntities.Find(this.id_cached) as BaseEntity);
			}
			else
			{
				Debug.LogWarning("EntityRef: Looking for serverside entities on pure client!");
			}
		}
		if (!this.ent_cached.IsValid())
		{
			this.ent_cached = null;
		}
		return this.ent_cached;
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060013AD RID: 5037 RVA: 0x00010AF8 File Offset: 0x0000ECF8
	// (set) Token: 0x060013AE RID: 5038 RVA: 0x0007C1C4 File Offset: 0x0007A3C4
	public uint uid
	{
		get
		{
			if (this.ent_cached.IsValid())
			{
				this.id_cached = this.ent_cached.net.ID;
			}
			return this.id_cached;
		}
		set
		{
			this.id_cached = value;
			if (this.id_cached == 0U)
			{
				this.ent_cached = null;
				return;
			}
			if (this.ent_cached.IsValid() && this.ent_cached.net.ID == this.id_cached)
			{
				return;
			}
			this.ent_cached = null;
		}
	}
}
