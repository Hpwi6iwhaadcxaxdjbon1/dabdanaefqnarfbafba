using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000646 RID: 1606
public class CraftingNotice : MonoBehaviour
{
	// Token: 0x04001FEF RID: 8175
	public RectTransform rotatingIcon;

	// Token: 0x04001FF0 RID: 8176
	public CanvasGroup canvasGroup;

	// Token: 0x04001FF1 RID: 8177
	public Text itemName;

	// Token: 0x04001FF2 RID: 8178
	public Text craftSeconds;

	// Token: 0x060023CA RID: 9162 RVA: 0x000BDCE0 File Offset: 0x000BBEE0
	private void Update()
	{
		if (this.canvasGroup.alpha <= 0.1f)
		{
			return;
		}
		if (SingletonComponent<CraftingQueue>.Instance == null)
		{
			return;
		}
		this.rotatingIcon.localRotation = Quaternion.Euler(0f, 0f, Time.realtimeSinceStartup * 45f);
		CraftingQueueIcon active = SingletonComponent<CraftingQueue>.Instance.GetActive();
		if (active == null || active.item == null)
		{
			this.craftSeconds.text = "";
			this.itemName.text = "";
			return;
		}
		this.craftSeconds.text = active.timeLeftString;
		if (active.amount > 1)
		{
			this.itemName.text = string.Format("{0} (x{1})", active.item.displayName.translated, active.amount);
			return;
		}
		this.itemName.text = active.item.displayName.translated;
	}
}
