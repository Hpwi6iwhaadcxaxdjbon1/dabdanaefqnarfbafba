using System;
using UnityEngine;

// Token: 0x020003ED RID: 1005
public class RealmedCollider : BasePrefab
{
	// Token: 0x04001568 RID: 5480
	public Collider ServerCollider;

	// Token: 0x04001569 RID: 5481
	public Collider ClientCollider;

	// Token: 0x06001906 RID: 6406 RVA: 0x0008EEDC File Offset: 0x0008D0DC
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		if (this.ServerCollider != this.ClientCollider)
		{
			if (clientside)
			{
				if (this.ServerCollider)
				{
					process.RemoveComponent(this.ServerCollider);
					this.ServerCollider = null;
				}
			}
			else if (this.ClientCollider)
			{
				process.RemoveComponent(this.ClientCollider);
				this.ClientCollider = null;
			}
		}
		process.RemoveComponent(this);
	}
}
