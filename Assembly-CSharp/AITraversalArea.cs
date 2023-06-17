using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class AITraversalArea : TriggerBase
{
	// Token: 0x040008D0 RID: 2256
	public Transform entryPoint1;

	// Token: 0x040008D1 RID: 2257
	public Transform entryPoint2;

	// Token: 0x040008D2 RID: 2258
	public AITraversalWaitPoint[] waitPoints;

	// Token: 0x040008D3 RID: 2259
	public Bounds movementArea;

	// Token: 0x040008D4 RID: 2260
	public Transform activeEntryPoint;

	// Token: 0x040008D5 RID: 2261
	public float nextFreeTime;

	// Token: 0x06000C1A RID: 3098 RVA: 0x0000B5DD File Offset: 0x000097DD
	public void OnValidate()
	{
		this.movementArea.center = base.transform.position;
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0005B53C File Offset: 0x0005973C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0000B5F5 File Offset: 0x000097F5
	public bool CanTraverse(BaseEntity ent)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x0005B58C File Offset: 0x0005978C
	public Transform GetClosestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num < num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x0005B5CC File Offset: 0x000597CC
	public Transform GetFarthestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num > num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0000B604 File Offset: 0x00009804
	public void SetBusyFor(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0000B5F5 File Offset: 0x000097F5
	public bool CanUse(Vector3 dirFrom)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0000B613 File Offset: 0x00009813
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		ent.GetComponent<HumanNPC>();
		Debug.Log("Enter Traversal Area");
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0005B60C File Offset: 0x0005980C
	public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
	{
		Vector3 position = this.GetClosestEntry(pos).position;
		Vector3 position2 = this.GetFarthestEntry(pos).position;
		new BaseEntity[1];
		AITraversalWaitPoint result = null;
		float num = 0f;
		foreach (AITraversalWaitPoint aitraversalWaitPoint in this.waitPoints)
		{
			if (!aitraversalWaitPoint.Occupied())
			{
				Vector3 position3 = aitraversalWaitPoint.transform.position;
				float num2 = Vector3.Distance(position, position3);
				if (Vector3.Distance(position2, position3) >= num2)
				{
					float value = Vector3.Distance(position3, pos);
					float num3 = (1f - Mathf.InverseLerp(0f, 20f, value)) * 100f;
					if (num3 > num)
					{
						num = num3;
						result = aitraversalWaitPoint;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0000B62D File Offset: 0x0000982D
	public bool EntityFilter(BaseEntity ent)
	{
		return ent.IsNpc && ent.isServer;
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0000B63F File Offset: 0x0000983F
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.GetComponent<HumanNPC>();
		Debug.Log("Leave Traversal Area");
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0005B6C8 File Offset: 0x000598C8
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(this.entryPoint1.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawCube(this.entryPoint2.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
		Gizmos.DrawCube(this.movementArea.center, this.movementArea.size);
		Gizmos.color = Color.magenta;
		AITraversalWaitPoint[] array = this.waitPoints;
		for (int i = 0; i < array.Length; i++)
		{
			GizmosUtil.DrawCircleY(array[i].transform.position, 0.5f);
		}
	}
}
