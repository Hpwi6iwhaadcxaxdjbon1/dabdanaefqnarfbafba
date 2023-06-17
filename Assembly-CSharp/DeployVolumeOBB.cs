using System;
using UnityEngine;

// Token: 0x0200037C RID: 892
public class DeployVolumeOBB : DeployVolume
{
	// Token: 0x040013A3 RID: 5027
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x060016C4 RID: 5828 RVA: 0x000882AC File Offset: 0x000864AC
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		OBB obb;
		obb..ctor(position, this.bounds.size, rotation * this.worldRotation);
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

	// Token: 0x060016C5 RID: 5829 RVA: 0x0008835C File Offset: 0x0008655C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		OBB obb;
		obb..ctor(position, this.bounds.size, rotation * this.worldRotation);
		if ((this.layers & mask) != 0 && obb.Intersects(test))
		{
			if (Input.GetKey(KeyCode.KeypadDivide))
			{
				DDraw.Box(obb.position, obb.rotation, obb.extents * 2f, Color.red, 0.1f);
			}
			return true;
		}
		return false;
	}
}
