using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C3 RID: 1731
public class UIStyle_Menu_Panel : MonoBehaviour, IClientComponent
{
	// Token: 0x04002294 RID: 8852
	public bool toggle;

	// Token: 0x0600266C RID: 9836 RVA: 0x0001DED9 File Offset: 0x0001C0D9
	private void OnValidate()
	{
		base.GetComponent<Image>().color = new Color32(29, 32, 31, byte.MaxValue);
	}
}
