using System;
using UnityEngine;

// Token: 0x0200077D RID: 1917
public class SwapRPG : MonoBehaviour
{
	// Token: 0x040024C5 RID: 9413
	public GameObject[] rpgModels;

	// Token: 0x040024C6 RID: 9414
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x060029BB RID: 10683 RVA: 0x000D41EC File Offset: 0x000D23EC
	public void SelectRPGType(int iType)
	{
		GameObject[] array = this.rpgModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.rpgModels[iType].SetActive(true);
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x000D4228 File Offset: 0x000D2428
	public void UpdateAmmoType(ItemDefinition ammoType)
	{
		if (this.curAmmoType == ammoType.shortname)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		string text = this.curAmmoType;
		if (!(text == "ammo.rocket.basic"))
		{
			if (text == "ammo.rocket.fire")
			{
				this.SelectRPGType(1);
				return;
			}
			if (text == "ammo.rocket.hv")
			{
				this.SelectRPGType(2);
				return;
			}
			if (text == "ammo.rocket.smoke")
			{
				this.SelectRPGType(3);
				return;
			}
		}
		this.SelectRPGType(0);
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x0200077E RID: 1918
	public enum RPGType
	{
		// Token: 0x040024C8 RID: 9416
		One,
		// Token: 0x040024C9 RID: 9417
		Two,
		// Token: 0x040024CA RID: 9418
		Three,
		// Token: 0x040024CB RID: 9419
		Four
	}
}
