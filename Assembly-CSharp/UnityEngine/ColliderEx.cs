using System;

namespace UnityEngine
{
	// Token: 0x0200082C RID: 2092
	public static class ColliderEx
	{
		// Token: 0x06002D74 RID: 11636 RVA: 0x00023583 File Offset: 0x00021783
		public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
		{
			if (obj is TerrainCollider)
			{
				return TerrainMeta.Physics.GetMaterial(pos);
			}
			return obj.sharedMaterial;
		}
	}
}
