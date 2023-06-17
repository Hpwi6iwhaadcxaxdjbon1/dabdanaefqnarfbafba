using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006CD RID: 1741
public class ToggleLayer : MonoBehaviour, IClientComponent
{
	// Token: 0x040022B9 RID: 8889
	public Toggle toggleControl;

	// Token: 0x040022BA RID: 8890
	public Text textControl;

	// Token: 0x040022BB RID: 8891
	public LayerSelect layer;

	// Token: 0x060026A1 RID: 9889 RVA: 0x0001E163 File Offset: 0x0001C363
	protected void OnEnable()
	{
		if (MainCamera.mainCamera)
		{
			this.toggleControl.isOn = ((MainCamera.mainCamera.cullingMask & this.layer.Mask) != 0);
		}
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000CAFB0 File Offset: 0x000C91B0
	public void OnToggleChanged()
	{
		if (MainCamera.mainCamera)
		{
			if (this.toggleControl.isOn)
			{
				MainCamera.mainCamera.cullingMask |= this.layer.Mask;
				return;
			}
			MainCamera.mainCamera.cullingMask &= ~this.layer.Mask;
		}
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x0001E195 File Offset: 0x0001C395
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = this.layer.Name;
		}
	}
}
