using System;
using UnityEngine;

// Token: 0x0200037A RID: 890
public class DeployVolumeEntityBounds : DeployVolume
{
	// Token: 0x040013A0 RID: 5024
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x060016BC RID: 5820 RVA: 0x000880F0 File Offset: 0x000862F0
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		OBB obb;
		obb..ctor(position, this.bounds.size, rotation);
		if (DeployVolume.CheckOBB(obb, this.layers & mask, this.ignore))
		{
			if (Input.GetKey(KeyCode.KeypadDivide))
			{
				DDraw.Box(obb.position, obb.rotation, obb.extents * 2f, Color.red, 0.1f);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x0000508F File Offset: 0x0000328F
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x000132A3 File Offset: 0x000114A3
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}
}
