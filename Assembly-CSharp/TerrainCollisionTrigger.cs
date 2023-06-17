using System;
using UnityEngine;

// Token: 0x020004C5 RID: 1221
public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x06001C0A RID: 7178 RVA: 0x00016F60 File Offset: 0x00015160
	protected void OnTriggerEnter(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, true);
	}

	// Token: 0x06001C0B RID: 7179 RVA: 0x00016F7F File Offset: 0x0001517F
	protected void OnTriggerExit(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, false);
	}

	// Token: 0x06001C0C RID: 7180 RVA: 0x0009A994 File Offset: 0x00098B94
	private void UpdateCollider(Collider other, bool state)
	{
		TerrainMeta.Collision.SetIgnore(other, base.volume.trigger, state);
		TerrainCollisionProxy component = other.GetComponent<TerrainCollisionProxy>();
		if (component)
		{
			for (int i = 0; i < component.colliders.Length; i++)
			{
				TerrainMeta.Collision.SetIgnore(component.colliders[i], base.volume.trigger, state);
			}
		}
	}
}
