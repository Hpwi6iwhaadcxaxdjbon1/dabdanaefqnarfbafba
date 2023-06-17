using System;
using UnityEngine;

// Token: 0x02000377 RID: 887
public class Deployable : PrefabAttribute
{
	// Token: 0x04001392 RID: 5010
	public Mesh guideMesh;

	// Token: 0x04001393 RID: 5011
	public Vector3 guideMeshScale = Vector3.one;

	// Token: 0x04001394 RID: 5012
	public bool guideLights = true;

	// Token: 0x04001395 RID: 5013
	public bool wantsInstanceData;

	// Token: 0x04001396 RID: 5014
	public bool copyInventoryFromItem;

	// Token: 0x04001397 RID: 5015
	public bool setSocketParent;

	// Token: 0x04001398 RID: 5016
	public bool toSlot;

	// Token: 0x04001399 RID: 5017
	public BaseEntity.Slot slot;

	// Token: 0x0400139A RID: 5018
	public GameObjectRef placeEffect;

	// Token: 0x060016AC RID: 5804 RVA: 0x00013230 File Offset: 0x00011430
	protected override Type GetIndexedType()
	{
		return typeof(Deployable);
	}
}
