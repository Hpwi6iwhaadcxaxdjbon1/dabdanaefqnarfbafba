using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200037B RID: 891
public class DeployVolumeEntityBoundsReverse : DeployVolume
{
	// Token: 0x040013A1 RID: 5025
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x040013A2 RID: 5026
	private int layer;

	// Token: 0x060016C0 RID: 5824 RVA: 0x00088180 File Offset: 0x00086380
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		OBB obb;
		obb..ctor(position, this.bounds.size, rotation);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, obb.extents.magnitude, list, this.layers & mask, 2);
		foreach (BaseEntity baseEntity in list)
		{
			DeployVolume[] volumes = PrefabAttribute.client.FindAll<DeployVolume>(baseEntity.prefabID);
			if (DeployVolume.Check(baseEntity.transform.position, baseEntity.transform.rotation, volumes, obb, 1 << this.layer))
			{
				if (Input.GetKey(KeyCode.KeypadDivide))
				{
					DDraw.Box(obb.position, obb.rotation, obb.extents * 2f, Color.red, 0.1f);
				}
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return false;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0000508F File Offset: 0x0000328F
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		return false;
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x000132D3 File Offset: 0x000114D3
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.layer = rootObj.layer;
	}
}
