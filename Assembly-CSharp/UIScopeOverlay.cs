using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E4 RID: 1764
public class UIScopeOverlay : MonoBehaviour
{
	// Token: 0x040022FE RID: 8958
	public CanvasGroup group;

	// Token: 0x040022FF RID: 8959
	public static UIScopeOverlay instance;

	// Token: 0x04002300 RID: 8960
	public Image scopeImage;

	// Token: 0x04002301 RID: 8961
	private Vector3 initialPosition = Vector3.zero;

	// Token: 0x06002704 RID: 9988 RVA: 0x0001E70A File Offset: 0x0001C90A
	public void SetScopeMaterial(Material newMat)
	{
		this.scopeImage.material = newMat;
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x0001E718 File Offset: 0x0001C918
	private void Awake()
	{
		UIScopeOverlay.instance = this;
		this.initialPosition = base.GetComponent<RectTransform>().position;
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x0001E731 File Offset: 0x0001C931
	public void SetPosition(Vector3 position)
	{
		base.transform.position = new Vector3(position.x, position.y, this.initialPosition.z);
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000079E3 File Offset: 0x00005BE3
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x0001E75A File Offset: 0x0001C95A
	public void SetAlpha(float alpha)
	{
		if (this.group == null)
		{
			return;
		}
		this.group.alpha = alpha;
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x0001E777 File Offset: 0x0001C977
	public float GetAlpha()
	{
		if (this.group == null)
		{
			return 0f;
		}
		return this.group.alpha;
	}
}
