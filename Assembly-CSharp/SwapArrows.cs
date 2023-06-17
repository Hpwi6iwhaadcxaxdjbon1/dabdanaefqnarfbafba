using System;
using Rust;
using UnityEngine;

// Token: 0x0200077A RID: 1914
public class SwapArrows : MonoBehaviour, IClientComponent
{
	// Token: 0x040024BD RID: 9405
	public GameObject[] arrowModels;

	// Token: 0x040024BE RID: 9406
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x060029B2 RID: 10674 RVA: 0x000206EB File Offset: 0x0001E8EB
	public void SelectArrowType(int iType)
	{
		this.HideAllArrowHeads();
		if (iType < this.arrowModels.Length)
		{
			this.arrowModels[iType].SetActive(true);
		}
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x000D40D4 File Offset: 0x000D22D4
	public void HideAllArrowHeads()
	{
		GameObject[] array = this.arrowModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000D4100 File Offset: 0x000D2300
	public void UpdateAmmoType(ItemDefinition ammoType)
	{
		if (this.curAmmoType == ammoType.shortname)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		string text = this.curAmmoType;
		if (!(text == "ammo_arrow"))
		{
			if (text == "arrow.bone")
			{
				this.SelectArrowType(0);
				return;
			}
			if (text == "arrow.fire")
			{
				this.SelectArrowType(1);
				return;
			}
			if (text == "arrow.hv")
			{
				this.SelectArrowType(2);
				return;
			}
			if (text == "ammo_arrow_poison")
			{
				this.SelectArrowType(3);
				return;
			}
			if (text == "ammo_arrow_stone")
			{
				this.SelectArrowType(4);
				return;
			}
		}
		this.HideAllArrowHeads();
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x0002070C File Offset: 0x0001E90C
	private void Cleanup()
	{
		this.HideAllArrowHeads();
		this.curAmmoType = "";
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x0002071F File Offset: 0x0001E91F
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Cleanup();
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x0002072F File Offset: 0x0001E92F
	public void OnEnable()
	{
		this.Cleanup();
	}

	// Token: 0x0200077B RID: 1915
	public enum ArrowType
	{
		// Token: 0x040024C0 RID: 9408
		One,
		// Token: 0x040024C1 RID: 9409
		Two,
		// Token: 0x040024C2 RID: 9410
		Three,
		// Token: 0x040024C3 RID: 9411
		Four
	}
}
