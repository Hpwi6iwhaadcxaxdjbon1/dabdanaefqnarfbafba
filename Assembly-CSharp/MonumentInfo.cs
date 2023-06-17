using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003D9 RID: 985
public class MonumentInfo : MonoBehaviour
{
	// Token: 0x04001527 RID: 5415
	public MonumentType Type = MonumentType.Building;

	// Token: 0x04001528 RID: 5416
	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	// Token: 0x04001529 RID: 5417
	[ReadOnly]
	public Bounds Bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x0400152A RID: 5418
	public bool HasNavmesh;

	// Token: 0x0400152B RID: 5419
	public bool shouldDisplayOnMap;

	// Token: 0x0400152C RID: 5420
	public Translate.Phrase displayPhrase;

	// Token: 0x0400152D RID: 5421
	private Dictionary<InfrastructureType, List<TerrainPathConnect>> targets = new Dictionary<InfrastructureType, List<TerrainPathConnect>>();

	// Token: 0x060018B8 RID: 6328 RVA: 0x0008DAF0 File Offset: 0x0008BCF0
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Monuments.Add(this);
		}
		foreach (TerrainPathConnect target in base.GetComponentsInChildren<TerrainPathConnect>())
		{
			this.AddTarget(target);
		}
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x0008DB3C File Offset: 0x0008BD3C
	public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		OBB obb;
		obb..ctor(pos, scale, rot, this.Bounds);
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point3 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		int topology = TerrainMeta.TopologyMap.GetTopology(point);
		int topology2 = TerrainMeta.TopologyMap.GetTopology(point2);
		int topology3 = TerrainMeta.TopologyMap.GetTopology(point3);
		int topology4 = TerrainMeta.TopologyMap.GetTopology(point4);
		int num = 0;
		if ((this.Tier & MonumentTier.Tier0) != (MonumentTier)0)
		{
			num |= 67108864;
		}
		if ((this.Tier & MonumentTier.Tier1) != (MonumentTier)0)
		{
			num |= 134217728;
		}
		if ((this.Tier & MonumentTier.Tier2) != (MonumentTier)0)
		{
			num |= 268435456;
		}
		return (num & topology) != 0 && (num & topology2) != 0 && (num & topology3) != 0 && (num & topology4) != 0;
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x0008DC50 File Offset: 0x0008BE50
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 0.7f, 1f, 0.1f);
		Gizmos.DrawCube(this.Bounds.center, this.Bounds.size);
		Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
		Gizmos.DrawWireCube(this.Bounds.center, this.Bounds.size);
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x0008DCE0 File Offset: 0x0008BEE0
	public void AddTarget(TerrainPathConnect target)
	{
		InfrastructureType type = target.Type;
		if (!this.targets.ContainsKey(type))
		{
			this.targets.Add(type, new List<TerrainPathConnect>());
		}
		this.targets[type].Add(target);
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x00014AB8 File Offset: 0x00012CB8
	public List<TerrainPathConnect> GetTargets(InfrastructureType type)
	{
		if (!this.targets.ContainsKey(type))
		{
			this.targets.Add(type, new List<TerrainPathConnect>());
		}
		return this.targets[type];
	}
}
