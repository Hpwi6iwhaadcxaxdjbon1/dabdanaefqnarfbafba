using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D9 RID: 1753
public class UIBlackoutOverlay : MonoBehaviour
{
	// Token: 0x040022DD RID: 8925
	public CanvasGroup group;

	// Token: 0x040022DE RID: 8926
	public static Dictionary<UIBlackoutOverlay.blackoutType, UIBlackoutOverlay> instances;

	// Token: 0x040022DF RID: 8927
	public UIBlackoutOverlay.blackoutType overlayType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x040022E0 RID: 8928
	private Vector3 initialPosition = Vector3.zero;

	// Token: 0x040022E1 RID: 8929
	private float myAlpha;

	// Token: 0x060026D7 RID: 9943 RVA: 0x0001E4F1 File Offset: 0x0001C6F1
	public static void AddOverlay(UIBlackoutOverlay.blackoutType type, UIBlackoutOverlay overlay)
	{
		if (UIBlackoutOverlay.instances == null)
		{
			UIBlackoutOverlay.instances = new Dictionary<UIBlackoutOverlay.blackoutType, UIBlackoutOverlay>();
		}
		UIBlackoutOverlay.instances[type] = overlay;
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x0001E510 File Offset: 0x0001C710
	public static UIBlackoutOverlay Get(UIBlackoutOverlay.blackoutType type)
	{
		if (UIBlackoutOverlay.instances == null)
		{
			Debug.LogError("UIBlackoutOverlay.Get attempted before initialization, type : " + type);
			return null;
		}
		return UIBlackoutOverlay.instances[type];
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x0001E53B File Offset: 0x0001C73B
	private void Awake()
	{
		UIBlackoutOverlay.AddOverlay(this.overlayType, this);
		this.initialPosition = base.GetComponent<RectTransform>().position;
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x0001E55A File Offset: 0x0001C75A
	public void ResetPosition()
	{
		this.SetPosition(Vector3.zero);
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x0001E567 File Offset: 0x0001C767
	public void SetPosition(Vector3 position)
	{
		base.transform.position = new Vector3(position.x, position.y, this.initialPosition.z);
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000079E3 File Offset: 0x00005BE3
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000CB408 File Offset: 0x000C9608
	public void Update()
	{
		if (this.group == null)
		{
			return;
		}
		if (!LocalPlayer.Entity || !LocalPlayer.Entity.InFirstPersonMode())
		{
			this.group.alpha = 0f;
			return;
		}
		this.group.alpha = this.myAlpha;
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x0001E590 File Offset: 0x0001C790
	public void SetAlpha(float alpha)
	{
		if (this.group == null)
		{
			return;
		}
		this.myAlpha = alpha;
		this.group.alpha = alpha;
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x0001E5B4 File Offset: 0x0001C7B4
	public float GetAlpha()
	{
		if (this.group == null)
		{
			return 0f;
		}
		return this.group.alpha;
	}

	// Token: 0x020006DA RID: 1754
	public enum blackoutType
	{
		// Token: 0x040022E3 RID: 8931
		FULLBLACK,
		// Token: 0x040022E4 RID: 8932
		BINOCULAR,
		// Token: 0x040022E5 RID: 8933
		SCOPE,
		// Token: 0x040022E6 RID: 8934
		HELMETSLIT,
		// Token: 0x040022E7 RID: 8935
		SNORKELGOGGLE,
		// Token: 0x040022E8 RID: 8936
		NONE = 64
	}
}
