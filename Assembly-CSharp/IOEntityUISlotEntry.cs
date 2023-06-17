using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200067A RID: 1658
public class IOEntityUISlotEntry : MonoBehaviour
{
	// Token: 0x040020F5 RID: 8437
	public RawImage icon;

	// Token: 0x040020F6 RID: 8438
	public Text leftText;

	// Token: 0x040020F7 RID: 8439
	public Text rightText;

	// Token: 0x06002502 RID: 9474 RVA: 0x00010E9A File Offset: 0x0000F09A
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x0001D091 File Offset: 0x0001B291
	public void UpdateText(bool visible, string left, string right, bool wantsSelected = false)
	{
		base.gameObject.SetActive(visible);
		if (visible)
		{
			this.leftText.text = left;
			this.rightText.text = right;
		}
	}
}
