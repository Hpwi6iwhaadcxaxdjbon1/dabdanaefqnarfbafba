using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class RHIBAIController : FacepunchBehaviour
{
	// Token: 0x040012CC RID: 4812
	public List<Vector3> nodes = new List<Vector3>();

	// Token: 0x060015D5 RID: 5589 RVA: 0x00085830 File Offset: 0x00083A30
	[ContextMenu("Calculate Path")]
	public void SetupPatrolPath()
	{
		float x = TerrainMeta.Size.x;
		float num = x * 2f * 3.1415927f;
		float num2 = 30f;
		int num3 = Mathf.CeilToInt(num / num2);
		this.nodes = new List<Vector3>();
		float num4 = x;
		float y = 0f;
		for (int i = 0; i < num3; i++)
		{
			float num5 = (float)i / (float)num3 * 360f;
			this.nodes.Add(new Vector3(Mathf.Sin(num5 * 0.017453292f) * num4, y, Mathf.Cos(num5 * 0.017453292f) * num4));
		}
		float d = 2f;
		float num6 = 200f;
		float num7 = 150f;
		float num8 = 8f;
		bool flag = true;
		int num9 = 1;
		float num10 = 20f;
		Vector3[] array = new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(num10, 0f, 0f),
			new Vector3(-num10, 0f, 0f),
			new Vector3(0f, 0f, num10),
			new Vector3(0f, 0f, -num10)
		};
		while (flag)
		{
			Debug.Log("Loop # :" + num9);
			num9++;
			flag = false;
			for (int j = 0; j < num3; j++)
			{
				Vector3 vector = this.nodes[j];
				int num11 = (j == 0) ? (num3 - 1) : (j - 1);
				int num12 = (j == num3 - 1) ? 0 : (j + 1);
				Vector3 b = this.nodes[num12];
				Vector3 b2 = this.nodes[num11];
				Vector3 vector2 = vector;
				Vector3 normalized = (Vector3.zero - vector).normalized;
				Vector3 vector3 = vector + normalized * d;
				if (Vector3.Distance(vector3, b) <= num6 && Vector3.Distance(vector3, b2) <= num6)
				{
					bool flag2 = true;
					for (int k = 0; k < array.Length; k++)
					{
						Vector3 vector4 = vector3 + array[k];
						if (this.GetWaterDepth(vector4) < num8)
						{
							flag2 = false;
						}
						Vector3 vector5 = normalized;
						if (vector4 != Vector3.zero)
						{
							vector5 = (vector4 - vector2).normalized;
						}
						RaycastHit raycastHit;
						if (Physics.Raycast(vector2, vector5, ref raycastHit, num7, 1218511105))
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						flag = true;
						this.nodes[j] = vector3;
					}
				}
			}
		}
		List<int> list = new List<int>();
		LineUtility.Simplify(this.nodes, 15f, list);
		List<Vector3> list2 = this.nodes;
		this.nodes = new List<Vector3>();
		foreach (int num13 in list)
		{
			this.nodes.Add(list2[num13]);
		}
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x00085B50 File Offset: 0x00083D50
	public float GetWaterDepth(Vector3 pos)
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(pos, Vector3.down, ref raycastHit, 100f, 8388608))
		{
			return 100f;
		}
		return raycastHit.distance;
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x00085B84 File Offset: 0x00083D84
	public void OnDrawGizmosSelected()
	{
		if (TerrainMeta.Path.OceanPatrolClose != null)
		{
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolClose.Count; i++)
			{
				Vector3 vector = TerrainMeta.Path.OceanPatrolClose[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(vector, 3f);
				Vector3 to = (i + 1 == TerrainMeta.Path.OceanPatrolClose.Count) ? TerrainMeta.Path.OceanPatrolClose[0] : TerrainMeta.Path.OceanPatrolClose[i + 1];
				Gizmos.DrawLine(vector, to);
			}
		}
	}
}
