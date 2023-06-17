using System;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class EntityComponent<T> : EntityComponentBase where T : BaseEntity
{
	// Token: 0x04000FA7 RID: 4007
	[NonSerialized]
	private T _baseEntity;

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x0600134C RID: 4940 RVA: 0x00010622 File Offset: 0x0000E822
	protected T baseEntity
	{
		get
		{
			if (this._baseEntity == null)
			{
				this.UpdateBaseEntity();
			}
			return this._baseEntity;
		}
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x00010643 File Offset: 0x0000E843
	protected void UpdateBaseEntity()
	{
		if (!this)
		{
			return;
		}
		if (!base.gameObject)
		{
			return;
		}
		this._baseEntity = (base.gameObject.ToBaseEntity() as T);
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x00010677 File Offset: 0x0000E877
	protected override BaseEntity GetBaseEntity()
	{
		return this.baseEntity;
	}
}
