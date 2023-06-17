using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class BasePath : MonoBehaviour
{
	// Token: 0x040007BD RID: 1981
	public List<BasePathNode> nodes;

	// Token: 0x040007BE RID: 1982
	public List<PathInterestNode> interestZones;

	// Token: 0x040007BF RID: 1983
	public List<PathSpeedZone> speedZones;

	// Token: 0x06000B51 RID: 2897 RVA: 0x00002ECE File Offset: 0x000010CE
	public void Start()
	{
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x000585D0 File Offset: 0x000567D0
	public void GetNodesNear(Vector3 point, ref List<BasePathNode> nearNodes, float dist = 10f)
	{
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if ((Vector3Ex.XZ(point) - Vector3Ex.XZ(basePathNode.transform.position)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(basePathNode);
			}
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0005864C File Offset: 0x0005684C
	public BasePathNode GetClosestToPoint(Vector3 point)
	{
		BasePathNode result = this.nodes[0];
		float num = float.PositiveInfinity;
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if (!(basePathNode == null) && !(basePathNode.transform == null))
			{
				float sqrMagnitude = (point - basePathNode.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = basePathNode;
				}
			}
		}
		return result;
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x000586EC File Offset: 0x000568EC
	public PathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		PathInterestNode pathInterestNode = null;
		int num = 0;
		while (pathInterestNode == null && num < 20)
		{
			pathInterestNode = this.interestZones[Random.Range(0, this.interestZones.Count)];
			if ((pathInterestNode.transform.position - from).sqrMagnitude >= 100f)
			{
				break;
			}
			pathInterestNode = null;
			num++;
		}
		if (pathInterestNode == null)
		{
			pathInterestNode = this.interestZones[0];
		}
		return pathInterestNode;
	}
}
