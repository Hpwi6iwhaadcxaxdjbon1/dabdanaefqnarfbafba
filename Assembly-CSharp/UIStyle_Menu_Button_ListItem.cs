using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C1 RID: 1729
public class UIStyle_Menu_Button_ListItem : MonoBehaviour, IClientComponent
{
	// Token: 0x04002292 RID: 8850
	public bool apply;

	// Token: 0x06002668 RID: 9832 RVA: 0x000CA2D4 File Offset: 0x000C84D4
	private void OnValidate()
	{
		if (base.GetComponent<Image>() == null)
		{
			return;
		}
		if (base.GetComponent<Button>() == null)
		{
			return;
		}
		base.GetComponent<Image>().color = Color.white;
		ColorBlock colors = base.GetComponent<Button>().colors;
		colors.normalColor = new Color32(43, 41, 36, byte.MaxValue);
		colors.highlightedColor = new Color32(72, 86, 46, byte.MaxValue);
		colors.pressedColor = new Color32(37, 86, 122, byte.MaxValue);
		colors.disabledColor = new Color32(72, 86, 46, byte.MaxValue);
		colors.colorMultiplier = 1f;
		colors.fadeDuration = 0.1f;
		base.GetComponent<Button>().colors = colors;
	}
}
