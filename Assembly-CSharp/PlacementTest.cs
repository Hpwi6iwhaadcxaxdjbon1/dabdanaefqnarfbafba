using System;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class PlacementTest : MonoBehaviour
{
	// Token: 0x0400076E RID: 1902
	public MeshCollider myMeshCollider;

	// Token: 0x0400076F RID: 1903
	public Transform testTransform;

	// Token: 0x04000770 RID: 1904
	public Transform visualTest;

	// Token: 0x04000771 RID: 1905
	public float hemisphere = 45f;

	// Token: 0x04000772 RID: 1906
	public float clampTest = 45f;

	// Token: 0x04000773 RID: 1907
	public float testDist = 2f;

	// Token: 0x04000774 RID: 1908
	private float nextTest;

	// Token: 0x06000B22 RID: 2850 RVA: 0x000571F4 File Offset: 0x000553F4
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector3 b = new Vector3(insideUnitCircle.x * degreesOffset, Random.Range(-1f, 1f) * degreesOffset, insideUnitCircle.y * degreesOffset);
		return (input + b).normalized;
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x00057258 File Offset: 0x00055458
	public Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f)
	{
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector3 normalized = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y).normalized;
		return new Vector3(normalized.x * distance, Random.Range(minHeight, maxHeight), normalized.z * distance);
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x000572A8 File Offset: 0x000554A8
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec.normalized;
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0005733C File Offset: 0x0005553C
	private void Update()
	{
		if (Time.realtimeSinceStartup < this.nextTest)
		{
			return;
		}
		this.nextTest = Time.realtimeSinceStartup + 0f;
		Vector3 position = this.RandomCylinderPointAroundVector(Vector3.up, 0.5f, 0.25f, 0.5f);
		position = base.transform.TransformPoint(position);
		this.testTransform.transform.position = position;
		if (this.testTransform != null && this.visualTest != null)
		{
			Vector3 position2 = base.transform.position;
			RaycastHit raycastHit;
			if (this.myMeshCollider.Raycast(new Ray(this.testTransform.position, (base.transform.position - this.testTransform.position).normalized), ref raycastHit, 5f))
			{
				position2 = raycastHit.point;
			}
			else
			{
				Debug.LogError("Missed");
			}
			this.visualTest.transform.position = position2;
		}
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnDrawGizmos()
	{
	}
}
