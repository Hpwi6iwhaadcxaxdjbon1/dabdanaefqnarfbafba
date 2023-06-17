using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000375 RID: 885
public class CullingManager : SingletonComponent<CullingManager>
{
	// Token: 0x04001388 RID: 5000
	private List<CullingVolume> volumes = new List<CullingVolume>();

	// Token: 0x0600169C RID: 5788 RVA: 0x00087A14 File Offset: 0x00085C14
	public void MarkSeen(Vector3 pos)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, 0.01f, list, 262144, 2);
		for (int i = 0; i < list.Count; i++)
		{
			CullingVolume component = list[i].gameObject.GetComponent<CullingVolume>();
			if (component != null)
			{
				if (!this.volumes.Contains(component))
				{
					this.volumes.Add(component);
				}
				component.MarkSeen(true);
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x00087A90 File Offset: 0x00085C90
	private void RemoveInvisible()
	{
		for (int i = 0; i < this.volumes.Count; i++)
		{
			if (this.volumes[i].UpdateVisible(true))
			{
				ListEx.RemoveUnordered<CullingVolume>(this.volumes, i--);
			}
		}
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x000131BC File Offset: 0x000113BC
	protected void LateUpdate()
	{
		if (MainCamera.isValid)
		{
			this.MarkSeen(MainCamera.position);
		}
		this.RemoveInvisible();
	}
}
