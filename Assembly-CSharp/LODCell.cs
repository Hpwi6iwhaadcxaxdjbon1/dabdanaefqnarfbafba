using System;
using UnityEngine;

// Token: 0x020003BE RID: 958
public class LODCell
{
	// Token: 0x040014B2 RID: 5298
	public Vector3 Position;

	// Token: 0x040014B3 RID: 5299
	public float Size;

	// Token: 0x040014B4 RID: 5300
	public float Distance;

	// Token: 0x040014B5 RID: 5301
	private ListHashSet<ILOD> members = new ListHashSet<ILOD>(8);

	// Token: 0x0600180C RID: 6156 RVA: 0x000140D5 File Offset: 0x000122D5
	public LODCell(Vector3 position, float size)
	{
		this.Position = position;
		this.Size = size;
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x0008C2E4 File Offset: 0x0008A4E4
	public void ChangeLOD()
	{
		this.Distance = LODUtil.GetDistance(this.Position, LODDistanceMode.XZ);
		for (int i = 0; i < this.members.Count; i++)
		{
			this.members.Values[i].ChangeLOD();
		}
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000140F7 File Offset: 0x000122F7
	public void Add(ILOD component, Transform transform)
	{
		this.members.Add(component);
		component.ChangeLOD();
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x0001410B File Offset: 0x0001230B
	public void Remove(ILOD component, Transform transform)
	{
		if (!this.members.Remove(component))
		{
			Debug.LogError("Removing component from LOD cell it does not belong to. Did it move after wake?", transform);
		}
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x00014126 File Offset: 0x00012326
	public float GetDistance(Transform transform, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		return this.GetDistance(transform.position, mode);
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x00014135 File Offset: 0x00012335
	public float GetDistance(Vector3 worldPos, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		if (this.Distance > this.Size + this.Size)
		{
			return this.Distance;
		}
		return LODUtil.GetDistance(worldPos, mode);
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x0001415A File Offset: 0x0001235A
	public static implicit operator bool(LODCell cell)
	{
		return cell != null;
	}
}
