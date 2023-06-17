using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200065C RID: 1628
public class ItemPickupNotice : MonoBehaviour
{
	// Token: 0x0400205B RID: 8283
	public GameObject objectDeleteOnFinish;

	// Token: 0x0400205C RID: 8284
	public Text Text;

	// Token: 0x0400205D RID: 8285
	public Text Amount;

	// Token: 0x1700025C RID: 604
	// (set) Token: 0x06002451 RID: 9297 RVA: 0x0001CA8F File Offset: 0x0001AC8F
	public ItemDefinition itemInfo
	{
		set
		{
			this.Text.text = value.displayName.translated;
		}
	}

	// Token: 0x1700025D RID: 605
	// (set) Token: 0x06002452 RID: 9298 RVA: 0x0001CAA7 File Offset: 0x0001ACA7
	public int amount
	{
		set
		{
			this.Amount.text = ((value > 0) ? value.ToString("+0") : value.ToString("0"));
		}
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x0001CAD2 File Offset: 0x0001ACD2
	public void PopupNoticeEnd()
	{
		GameManager.Destroy(this.objectDeleteOnFinish, 0f);
	}
}
