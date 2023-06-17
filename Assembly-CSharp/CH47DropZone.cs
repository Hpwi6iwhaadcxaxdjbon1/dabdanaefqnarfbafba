using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class CH47DropZone : MonoBehaviour
{
	// Token: 0x040012E4 RID: 4836
	public float lastDropTime;

	// Token: 0x040012E5 RID: 4837
	private static List<CH47DropZone> dropZones = new List<CH47DropZone>();

	// Token: 0x060015E1 RID: 5601 RVA: 0x000127A3 File Offset: 0x000109A3
	public void Awake()
	{
		if (!CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Add(this);
		}
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x00085D28 File Offset: 0x00083F28
	public static CH47DropZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47DropZone result = null;
		foreach (CH47DropZone ch47DropZone in CH47DropZone.dropZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47DropZone.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = ch47DropZone;
			}
		}
		return result;
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x000127BD File Offset: 0x000109BD
	public void OnDestroy()
	{
		if (CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Remove(this);
		}
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x000127D8 File Offset: 0x000109D8
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x000127E6 File Offset: 0x000109E6
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x000127F3 File Offset: 0x000109F3
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 5f);
	}
}
