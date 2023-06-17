using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006CE RID: 1742
public class ToggleTerrainRenderer : MonoBehaviour
{
	// Token: 0x040022BC RID: 8892
	public Toggle toggleControl;

	// Token: 0x040022BD RID: 8893
	public Text textControl;

	// Token: 0x060026A5 RID: 9893 RVA: 0x0001E1BA File Offset: 0x0001C3BA
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawHeightmap;
		}
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x0001E1DD File Offset: 0x0001C3DD
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawHeightmap = this.toggleControl.isOn;
		}
	}

	// Token: 0x060026A7 RID: 9895 RVA: 0x0001E200 File Offset: 0x0001C400
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Renderer";
		}
	}
}
