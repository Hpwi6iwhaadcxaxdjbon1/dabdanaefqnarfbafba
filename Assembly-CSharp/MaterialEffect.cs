using System;
using UnityEngine;

// Token: 0x02000473 RID: 1139
[CreateAssetMenu(menuName = "Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
	// Token: 0x04001792 RID: 6034
	public GameObjectRef DefaultEffect;

	// Token: 0x04001793 RID: 6035
	public MaterialEffect.Entry[] Entries;

	// Token: 0x06001AB0 RID: 6832 RVA: 0x000936C8 File Offset: 0x000918C8
	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = default(Vector3))
	{
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(ray, 0f, out raycastHit, length, mask, 0))
		{
			Effect.client.Run(this.DefaultEffect.resourcePath, ray.origin, ray.direction * -1f, forward);
			return;
		}
		Effect.client.Run(this.DefaultEffect.resourcePath, raycastHit.point, raycastHit.normal, forward);
	}

	// Token: 0x02000474 RID: 1140
	[Serializable]
	public class Entry
	{
		// Token: 0x04001794 RID: 6036
		public PhysicMaterial Material;

		// Token: 0x04001795 RID: 6037
		public GameObjectRef Effect;
	}
}
