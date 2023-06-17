using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class ConstructionSkin : BasePrefab
{
	// Token: 0x04000B6B RID: 2923
	private RendererBatch[] rendererBatches;

	// Token: 0x04000B6C RID: 2924
	private List<GameObject> conditionals;

	// Token: 0x06000E48 RID: 3656 RVA: 0x000647E0 File Offset: 0x000629E0
	private void RefreshRendererBatching()
	{
		if (this.rendererBatches == null)
		{
			this.rendererBatches = base.GetComponentsInChildren<RendererBatch>(true);
		}
		for (int i = 0; i < this.rendererBatches.Length; i++)
		{
			this.rendererBatches[i].Refresh();
		}
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x00064824 File Offset: 0x00062A24
	public int DetermineConditionalModelState(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.client.FindAll<ConditionalModel>(this.prefabID);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].RunTests(parent))
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x00064868 File Offset: 0x00062A68
	private void CreateConditionalModels(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.client.FindAll<ConditionalModel>(this.prefabID);
		for (int i = 0; i < array.Length; i++)
		{
			if (parent.GetConditionalModel(i))
			{
				GameObject gameObject = array[i].InstantiateSkin(parent);
				if (!(gameObject == null))
				{
					if (this.conditionals == null)
					{
						this.conditionals = new List<GameObject>();
					}
					this.conditionals.Add(gameObject);
				}
			}
		}
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x000648D0 File Offset: 0x00062AD0
	private void DestroyConditionalModels(BuildingBlock parent)
	{
		if (this.conditionals == null)
		{
			return;
		}
		for (int i = 0; i < this.conditionals.Count; i++)
		{
			parent.gameManager.Retire(this.conditionals[i]);
		}
		this.conditionals.Clear();
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x0000D1D2 File Offset: 0x0000B3D2
	public void Refresh(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		if (parent.isClient)
		{
			this.RefreshRendererBatching();
		}
		this.CreateConditionalModels(parent);
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x0000D1F0 File Offset: 0x0000B3F0
	public void Destroy(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		parent.gameManager.Retire(base.gameObject);
	}
}
