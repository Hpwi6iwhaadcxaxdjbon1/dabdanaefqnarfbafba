using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006CF RID: 1743
public class ToggleTerrainTrees : MonoBehaviour
{
	// Token: 0x040022BE RID: 8894
	public Toggle toggleControl;

	// Token: 0x040022BF RID: 8895
	public Text textControl;

	// Token: 0x060026A9 RID: 9897 RVA: 0x0001E21F File Offset: 0x0001C41F
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawTreesAndFoliage;
		}
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x0001E242 File Offset: 0x0001C442
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawTreesAndFoliage = this.toggleControl.isOn;
		}
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x0001E265 File Offset: 0x0001C465
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Trees";
		}
	}
}
