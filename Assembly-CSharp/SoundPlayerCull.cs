using System;
using Rust;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class SoundPlayerCull : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x04000B10 RID: 2832
	public SoundPlayer soundPlayer;

	// Token: 0x04000B11 RID: 2833
	public float cullDistance = 100f;

	// Token: 0x04000B12 RID: 2834
	private LODCell cell;

	// Token: 0x06000DFD RID: 3581 RVA: 0x0000CE31 File Offset: 0x0000B031
	protected void OnEnable()
	{
		LODGrid.Add(this, base.transform, ref this.cell);
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x0000CE45 File Offset: 0x0000B045
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		LODGrid.Remove(this, base.transform, ref this.cell);
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x0000CE61 File Offset: 0x0000B061
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x00063138 File Offset: 0x00061338
	public void ChangeLOD()
	{
		if (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(this.cullDistance))
		{
			if (!this.soundPlayer.HasSound())
			{
				this.soundPlayer.CreateSound();
				this.soundPlayer.Play();
				return;
			}
		}
		else if (this.soundPlayer.HasSound())
		{
			this.soundPlayer.DestroySound();
		}
	}
}
