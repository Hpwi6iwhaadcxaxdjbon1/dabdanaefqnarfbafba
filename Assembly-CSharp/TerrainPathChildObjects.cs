using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004B5 RID: 1205
public class TerrainPathChildObjects : MonoBehaviour
{
	// Token: 0x040018C8 RID: 6344
	public bool Spline = true;

	// Token: 0x040018C9 RID: 6345
	public float Width;

	// Token: 0x040018CA RID: 6346
	public float Offset;

	// Token: 0x040018CB RID: 6347
	public float Fade;

	// Token: 0x040018CC RID: 6348
	[InspectorFlags]
	public TerrainSplat.Enum Splat = 1;

	// Token: 0x040018CD RID: 6349
	[InspectorFlags]
	public TerrainTopology.Enum Topology = 2048;

	// Token: 0x040018CE RID: 6350
	public InfrastructureType Type;

	// Token: 0x06001BE4 RID: 7140 RVA: 0x00099E6C File Offset: 0x0009806C
	protected void Awake()
	{
		List<Vector3> list = new List<Vector3>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.position);
		}
		if (list.Count >= 2)
		{
			InfrastructureType type = this.Type;
			if (type != InfrastructureType.Road)
			{
				if (type == InfrastructureType.Power)
				{
					PathList pathList = new PathList("Powerline " + TerrainMeta.Path.Powerlines.Count, list.ToArray());
					pathList.Width = this.Width;
					pathList.InnerFade = this.Fade * 0.5f;
					pathList.OuterFade = this.Fade * 0.5f;
					pathList.MeshOffset = this.Offset * 0.3f;
					pathList.TerrainOffset = this.Offset;
					pathList.Topology = this.Topology;
					pathList.Splat = this.Splat;
					pathList.Spline = this.Spline;
					TerrainMeta.Path.Powerlines.Add(pathList);
				}
			}
			else
			{
				PathList pathList2 = new PathList("Road " + TerrainMeta.Path.Roads.Count, list.ToArray());
				pathList2.Width = this.Width;
				pathList2.InnerFade = this.Fade * 0.5f;
				pathList2.OuterFade = this.Fade * 0.5f;
				pathList2.MeshOffset = this.Offset * 0.3f;
				pathList2.TerrainOffset = this.Offset;
				pathList2.Topology = this.Topology;
				pathList2.Splat = this.Splat;
				pathList2.Spline = this.Spline;
				TerrainMeta.Path.Roads.Add(pathList2);
			}
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x0009A078 File Offset: 0x00098278
	protected void OnDrawGizmos()
	{
		bool flag = false;
		Vector3 a = Vector3.zero;
		foreach (object obj in base.transform)
		{
			Vector3 position = ((Transform)obj).position;
			if (flag)
			{
				Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				GizmosUtil.DrawWirePath(a, position, 0.5f * this.Width);
			}
			a = position;
			flag = true;
		}
	}
}
