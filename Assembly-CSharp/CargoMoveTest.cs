using System;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class CargoMoveTest : FacepunchBehaviour
{
	// Token: 0x04000028 RID: 40
	public int targetNodeIndex = -1;

	// Token: 0x04000029 RID: 41
	private float currentThrottle;

	// Token: 0x0400002A RID: 42
	private float turnScale;

	// Token: 0x06000037 RID: 55 RVA: 0x00002ED0 File Offset: 0x000010D0
	private void Awake()
	{
		base.Invoke(new Action(this.FindInitialNode), 2f);
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002EE9 File Offset: 0x000010E9
	public void FindInitialNode()
	{
		this.targetNodeIndex = this.GetClosestNodeToUs();
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002EF7 File Offset: 0x000010F7
	private void Update()
	{
		this.UpdateMovement();
	}

	// Token: 0x0600003A RID: 58 RVA: 0x000274F0 File Offset: 0x000256F0
	public void UpdateMovement()
	{
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0)
		{
			return;
		}
		if (this.targetNodeIndex == -1)
		{
			return;
		}
		Vector3 vector = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
		Vector3 normalized = (vector - base.transform.position).normalized;
		float value = Vector3.Dot(base.transform.forward, normalized);
		float b = Mathf.InverseLerp(0.5f, 1f, value);
		float num = Vector3.Dot(base.transform.right, normalized);
		float num2 = 5f;
		float b2 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num));
		this.turnScale = Mathf.Lerp(this.turnScale, b2, Time.deltaTime * 0.2f);
		float num3 = (float)((num < 0f) ? -1 : 1);
		base.transform.Rotate(Vector3.up, num2 * Time.deltaTime * this.turnScale * num3, Space.World);
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, b, Time.deltaTime * 0.2f);
		base.transform.position += base.transform.forward * 5f * Time.deltaTime * this.currentThrottle;
		if (Vector3.Distance(base.transform.position, vector) < 60f)
		{
			this.targetNodeIndex++;
			if (this.targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				this.targetNodeIndex = 0;
			}
		}
	}

	// Token: 0x0600003B RID: 59 RVA: 0x000276A4 File Offset: 0x000258A4
	public int GetClosestNodeToUs()
	{
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 b = TerrainMeta.Path.OceanPatrolFar[i];
			float num2 = Vector3.Distance(base.transform.position, b);
			if (num2 < num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00027704 File Offset: 0x00025904
	public void OnDrawGizmosSelected()
	{
		if (TerrainMeta.Path.OceanPatrolFar != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex], 10f);
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
			{
				Vector3 vector = TerrainMeta.Path.OceanPatrolFar[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(vector, 3f);
				Vector3 to = (i + 1 == TerrainMeta.Path.OceanPatrolFar.Count) ? TerrainMeta.Path.OceanPatrolFar[0] : TerrainMeta.Path.OceanPatrolFar[i + 1];
				Gizmos.DrawLine(vector, to);
			}
		}
	}
}
