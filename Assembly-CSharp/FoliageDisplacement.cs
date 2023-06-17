using System;
using Rust;
using UnityEngine;

// Token: 0x02000599 RID: 1433
public class FoliageDisplacement : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x04001CA0 RID: 7328
	public bool moving;

	// Token: 0x04001CA1 RID: 7329
	public bool billboard;

	// Token: 0x04001CA2 RID: 7330
	public Mesh mesh;

	// Token: 0x04001CA3 RID: 7331
	public Material material;

	// Token: 0x04001CA4 RID: 7332
	private bool visible;

	// Token: 0x04001CA5 RID: 7333
	private LODCell cell;

	// Token: 0x060020BB RID: 8379 RVA: 0x00019F02 File Offset: 0x00018102
	protected void OnEnable()
	{
		LODGrid.Add(this, base.transform, ref this.cell);
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x00019F16 File Offset: 0x00018116
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		LODGrid.Remove(this, base.transform, ref this.cell);
		this.SetVisible(false);
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x00019F39 File Offset: 0x00018139
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x00019F4D File Offset: 0x0001814D
	public void ChangeLOD()
	{
		if (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < 50f)
		{
			this.SetVisible(true);
			return;
		}
		this.SetVisible(false);
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x00019F71 File Offset: 0x00018171
	private void SetVisible(bool state)
	{
		if (state)
		{
			if (!this.visible)
			{
				this.visible = true;
				FoliageDisplacementManager.Add(this);
				return;
			}
		}
		else if (this.visible)
		{
			this.visible = false;
			FoliageDisplacementManager.Remove(this);
		}
	}
}
