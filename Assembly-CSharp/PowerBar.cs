using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006B5 RID: 1717
public class PowerBar : MonoBehaviour
{
	// Token: 0x0400225D RID: 8797
	public static PowerBar Instance;

	// Token: 0x0400225E RID: 8798
	public Image powerInner;

	// Token: 0x0400225F RID: 8799
	public float fullSize;

	// Token: 0x04002260 RID: 8800
	public CanvasGroup group;

	// Token: 0x04002261 RID: 8801
	private bool visible;

	// Token: 0x04002262 RID: 8802
	private float progress;

	// Token: 0x06002632 RID: 9778 RVA: 0x0001DC5A File Offset: 0x0001BE5A
	public void Awake()
	{
		PowerBar.Instance = this;
		this.fullSize = this.powerInner.rectTransform.sizeDelta.x;
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x0001DC7D File Offset: 0x0001BE7D
	public void OnDestroy()
	{
		PowerBar.Instance = null;
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x0001DC85 File Offset: 0x0001BE85
	public void SetProgress(float newprogress)
	{
		this.progress = Mathf.Clamp01(newprogress);
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x0001DC93 File Offset: 0x0001BE93
	public void SetVisible(bool wantsVis)
	{
		this.visible = wantsVis;
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000C98F8 File Offset: 0x000C7AF8
	public void Update()
	{
		this.group.alpha = Mathf.MoveTowards(this.group.alpha, this.visible ? 1f : 0f, Time.deltaTime * 8f);
		this.powerInner.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.fullSize * this.progress);
	}
}
