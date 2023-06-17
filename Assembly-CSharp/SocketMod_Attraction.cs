using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class SocketMod_Attraction : SocketMod
{
	// Token: 0x04000BBC RID: 3004
	public float outerRadius = 1f;

	// Token: 0x04000BBD RID: 3005
	public float innerRadius = 0.1f;

	// Token: 0x04000BBE RID: 3006
	public string groupName = "wallbottom";

	// Token: 0x06000EA8 RID: 3752 RVA: 0x00065E64 File Offset: 0x00064064
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
		Gizmos.DrawSphere(Vector3.zero, this.outerRadius);
		Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
		Gizmos.DrawSphere(Vector3.zero, this.innerRadius);
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool DoCheck(Construction.Placement place)
	{
		return true;
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x00065EE0 File Offset: 0x000640E0
	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(vector, this.outerRadius * 2f, list, -1, 2);
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer == this.isServer)
			{
				AttractionPoint[] array = this.prefabAttribute.FindAll<AttractionPoint>(baseEntity.prefabID);
				if (array != null)
				{
					foreach (AttractionPoint attractionPoint in array)
					{
						if (!(attractionPoint.groupName != this.groupName))
						{
							Vector3 vector2 = baseEntity.transform.position + baseEntity.transform.rotation * attractionPoint.worldPosition;
							float magnitude = (vector2 - vector).magnitude;
							if (magnitude <= this.outerRadius)
							{
								Quaternion b = QuaternionEx.LookRotationWithOffset(this.worldPosition, vector2 - place.position, Vector3.up);
								float num = Mathf.InverseLerp(this.outerRadius, this.innerRadius, magnitude);
								if (Input.GetKeyDown(KeyCode.KeypadDivide))
								{
									DDraw.Arrow(vector, vector2, 0.1f, Color.yellow, 0.1f);
								}
								place.rotation = Quaternion.Lerp(place.rotation, b, num);
								vector = place.position + place.rotation * this.worldPosition;
								Vector3 a = vector2 - vector;
								if (Input.GetKeyDown(KeyCode.KeypadDivide))
								{
									DDraw.Arrow(vector, vector2, 0.1f, Color.green, 0.1f);
								}
								place.position += a * num;
							}
						}
					}
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}
}
