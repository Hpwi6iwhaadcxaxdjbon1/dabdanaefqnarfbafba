using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000139 RID: 313
public class AIInformationZone : MonoBehaviour
{
	// Token: 0x040008C6 RID: 2246
	public static List<AIInformationZone> zones = new List<AIInformationZone>();

	// Token: 0x040008C7 RID: 2247
	public List<AICoverPoint> coverPoints = new List<AICoverPoint>();

	// Token: 0x040008C8 RID: 2248
	public List<AIMovePoint> movePoints = new List<AIMovePoint>();

	// Token: 0x040008C9 RID: 2249
	public List<NavMeshLink> navMeshLinks = new List<NavMeshLink>();

	// Token: 0x040008CA RID: 2250
	public Bounds bounds;

	// Token: 0x040008CB RID: 2251
	private OBB areaBox;

	// Token: 0x06000C07 RID: 3079 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnValidate()
	{
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x0005AF20 File Offset: 0x00059120
	public void Start()
	{
		this.Process();
		this.areaBox = new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
		AIInformationZone.zones.Add(this);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0005AF70 File Offset: 0x00059170
	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position + this.bounds.center, this.bounds.size);
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x0005AFC8 File Offset: 0x000591C8
	public void Process()
	{
		AICoverPoint[] componentsInChildren = base.transform.GetComponentsInChildren<AICoverPoint>();
		this.coverPoints.AddRange(componentsInChildren);
		AIMovePoint[] componentsInChildren2 = base.transform.GetComponentsInChildren<AIMovePoint>(true);
		this.movePoints.AddRange(componentsInChildren2);
		NavMeshLink[] componentsInChildren3 = base.transform.GetComponentsInChildren<NavMeshLink>(true);
		this.navMeshLinks.AddRange(componentsInChildren3);
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x0005B020 File Offset: 0x00059220
	public static AIInformationZone GetForPoint(Vector3 point, BaseEntity from = null)
	{
		if (AIInformationZone.zones == null || AIInformationZone.zones.Count == 0)
		{
			return null;
		}
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			if (aiinformationZone.areaBox.Contains(point))
			{
				return aiinformationZone;
			}
		}
		return AIInformationZone.zones[0];
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x0005B0A0 File Offset: 0x000592A0
	public AIMovePoint GetBestMovePointNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false, BaseEntity forObject = null)
	{
		AIMovePoint result = null;
		float num = -1f;
		foreach (AIMovePoint aimovePoint in this.movePoints)
		{
			float num2 = 0f;
			Vector3 position = aimovePoint.transform.position;
			float num3 = Vector3.Distance(targetPosition, position);
			if (num3 <= maxRange && aimovePoint.transform.parent.gameObject.activeSelf)
			{
				num2 += (aimovePoint.CanBeUsedBy(forObject) ? 100f : 0f);
				num2 += (1f - Mathf.InverseLerp(minRange, maxRange, num3)) * 100f;
				if (num2 >= num && (!checkLOS || !Physics.Linecast(targetPosition + Vector3.up * 1f, position + Vector3.up * 1f, 1218519297, 1)) && num2 > num)
				{
					result = aimovePoint;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x0005B1C0 File Offset: 0x000593C0
	public Vector3 GetBestPositionNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false)
	{
		AIMovePoint aimovePoint = null;
		float num = -1f;
		foreach (AIMovePoint aimovePoint2 in this.movePoints)
		{
			float num2 = 0f;
			Vector3 position = aimovePoint2.transform.position;
			float num3 = Vector3.Distance(targetPosition, position);
			if (num3 <= maxRange && aimovePoint2.transform.parent.gameObject.activeSelf)
			{
				num2 += (1f - Mathf.InverseLerp(minRange, maxRange, num3)) * 100f;
				if ((!checkLOS || !Physics.Linecast(targetPosition + Vector3.up * 1f, position + Vector3.up * 1f, 1218650369, 1)) && num2 > num)
				{
					aimovePoint = aimovePoint2;
					num = num2;
				}
			}
		}
		if (aimovePoint != null)
		{
			return aimovePoint.transform.position;
		}
		return targetPosition;
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x0005B2D0 File Offset: 0x000594D0
	public AICoverPoint GetBestCoverPoint(Vector3 currentPosition, Vector3 hideFromPosition, float minRange = 0f, float maxRange = 20f, BaseEntity forObject = null)
	{
		AICoverPoint aicoverPoint = null;
		float num = 0f;
		foreach (AICoverPoint aicoverPoint2 in this.coverPoints)
		{
			if (!aicoverPoint2.InUse() || aicoverPoint2.IsUsedBy(forObject))
			{
				Vector3 position = aicoverPoint2.transform.position;
				Vector3 normalized = (hideFromPosition - position).normalized;
				float num2 = Vector3.Dot(aicoverPoint2.transform.forward, normalized);
				if (num2 >= 1f - aicoverPoint2.coverDot)
				{
					float value = Vector3.Distance(currentPosition, position);
					float num3 = 0f;
					if (minRange > 0f)
					{
						num3 -= (1f - Mathf.InverseLerp(0f, minRange, value)) * 100f;
					}
					float value2 = Mathf.Abs(position.y - currentPosition.y);
					num3 += (1f - Mathf.InverseLerp(1f, 5f, value2)) * 500f;
					num3 += Mathf.InverseLerp(1f - aicoverPoint2.coverDot, 1f, num2) * 50f;
					num3 += (1f - Mathf.InverseLerp(2f, maxRange, value)) * 100f;
					float num4 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
					float value3 = Vector3.Dot((aicoverPoint2.transform.position - currentPosition).normalized, normalized);
					num3 -= Mathf.InverseLerp(-1f, 0.25f, value3) * 50f * num4;
					if (num3 > num)
					{
						aicoverPoint = aicoverPoint2;
						num = num3;
					}
				}
			}
		}
		if (aicoverPoint)
		{
			return aicoverPoint;
		}
		return null;
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x0005B4B8 File Offset: 0x000596B8
	public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
	{
		NavMeshLink result = null;
		float num = float.PositiveInfinity;
		foreach (NavMeshLink navMeshLink in this.navMeshLinks)
		{
			float num2 = Vector3.Distance(navMeshLink.gameObject.transform.position, pos);
			if (num2 < num)
			{
				result = navMeshLink;
				num = num2;
				if (num2 < 0.25f)
				{
					break;
				}
			}
		}
		return result;
	}
}
