using System;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class PlatformEntity : BaseEntity
{
	// Token: 0x04001152 RID: 4434
	private const float movementSpeed = 1f;

	// Token: 0x04001153 RID: 4435
	private const float rotationSpeed = 10f;

	// Token: 0x04001154 RID: 4436
	private const float radius = 10f;

	// Token: 0x04001155 RID: 4437
	private Vector3 targetPosition = Vector3.zero;

	// Token: 0x04001156 RID: 4438
	private Quaternion targetRotation = Quaternion.identity;

	// Token: 0x06001453 RID: 5203 RVA: 0x0007DFC4 File Offset: 0x0007C1C4
	protected void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.targetPosition == Vector3.zero || Vector3.Distance(base.transform.position, this.targetPosition) < 0.01f)
		{
			Vector2 vector = Random.insideUnitCircle * 10f;
			this.targetPosition = base.transform.position + new Vector3(vector.x, 0f, vector.y);
			if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
			{
				float height = TerrainMeta.HeightMap.GetHeight(this.targetPosition);
				float height2 = TerrainMeta.WaterMap.GetHeight(this.targetPosition);
				this.targetPosition.y = Mathf.Max(height, height2) + 1f;
			}
			this.targetRotation = Quaternion.LookRotation(this.targetPosition - base.transform.position);
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, Time.fixedDeltaTime * 1f);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, Time.fixedDeltaTime * 10f);
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool PhysicsDriven()
	{
		return true;
	}
}
