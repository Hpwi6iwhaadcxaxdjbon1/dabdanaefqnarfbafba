using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class OreHotSpot : BaseCombatEntity, ILOD
{
	// Token: 0x04000768 RID: 1896
	public float visualDistance = 20f;

	// Token: 0x04000769 RID: 1897
	public GameObjectRef visualEffect;

	// Token: 0x0400076A RID: 1898
	public GameObjectRef finishEffect;

	// Token: 0x0400076B RID: 1899
	public GameObjectRef damageEffect;

	// Token: 0x0400076C RID: 1900
	private GameObject visualInstance;

	// Token: 0x0400076D RID: 1901
	private LODCell cell;

	// Token: 0x06000B1B RID: 2843 RVA: 0x0000AC2E File Offset: 0x00008E2E
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		LODGrid.Add(this, base.transform, ref this.cell);
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0000AC49 File Offset: 0x00008E49
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		this.visualInstance = null;
		LODGrid.Remove(this, base.transform, ref this.cell);
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0000AC6A File Offset: 0x00008E6A
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0000AC7E File Offset: 0x00008E7E
	public void ChangeLOD()
	{
		if (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(this.visualDistance))
		{
			this.SpawnVisual();
			return;
		}
		this.DestroyVisual();
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x000571A4 File Offset: 0x000553A4
	private void SpawnVisual()
	{
		if (this.visualInstance)
		{
			return;
		}
		this.visualInstance = EffectLibrary.CreateEffect(this.visualEffect.resourcePath, base.transform, base.transform.position, base.transform.rotation);
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0000ACA6 File Offset: 0x00008EA6
	private void DestroyVisual()
	{
		if (!this.visualInstance)
		{
			return;
		}
		this.visualInstance.SendOnParentDestroying();
		this.visualInstance = null;
	}
}
