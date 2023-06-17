using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C2 RID: 1730
public class UIStyle_Menu_Input : MonoBehaviour, IClientComponent
{
	// Token: 0x04002293 RID: 8851
	public bool apply;

	// Token: 0x0600266A RID: 9834 RVA: 0x000CA3B0 File Offset: 0x000C85B0
	private void OnValidate()
	{
		base.GetComponent<Image>().color = Color.white;
		ColorBlock colors = base.GetComponent<InputField>().colors;
		colors.normalColor = new Color32(43, 41, 36, byte.MaxValue);
		colors.highlightedColor = new Color32(72, 86, 46, byte.MaxValue);
		colors.pressedColor = new Color32(37, 86, 122, byte.MaxValue);
		colors.disabledColor = new Color32(33, 31, 26, byte.MaxValue);
		base.GetComponent<InputField>().colors = colors;
	}
}
