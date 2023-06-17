using System;
using UnityEngine;

// Token: 0x02000614 RID: 1556
public class Crosshair : MonoBehaviour
{
	// Token: 0x04001F21 RID: 7969
	public static bool Enabled = true;

	// Token: 0x04001F22 RID: 7970
	internal Animator anim;

	// Token: 0x04001F23 RID: 7971
	internal RectTransform rectTransform;

	// Token: 0x060022E8 RID: 8936 RVA: 0x0001BA9B File Offset: 0x00019C9B
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.rectTransform = (base.transform as RectTransform);
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000BA65C File Offset: 0x000B885C
	private void Update()
	{
		bool flag = this.ShouldShowCrosshair();
		this.anim.SetBool("visible", flag);
		if (flag)
		{
			if (MainCamera.isValid && LocalPlayer.Entity)
			{
				Vector2 vector = MainCamera.mainCamera.WorldToViewportPoint(LocalPlayer.Entity.lookingAtPoint);
				vector.x = (float)Screen.width * vector.x - (float)Screen.width * 0.5f;
				vector.y = (float)Screen.height * vector.y - (float)Screen.height * 0.5f;
				this.rectTransform.anchoredPosition = vector;
			}
			else
			{
				this.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			}
		}
		Crosshair.Enabled = true;
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000BA724 File Offset: 0x000B8924
	private bool ShouldShowCrosshair()
	{
		if (!Crosshair.Enabled)
		{
			return false;
		}
		if (LocalPlayer.Entity == null)
		{
			return false;
		}
		if (LocalPlayer.Entity.IsSpectating())
		{
			return false;
		}
		using (TimeWarning.New("NeedsCrosshair", 0.1f))
		{
			BaseEntity interactionEntity = LocalPlayer.Entity.GetInteractionEntity();
			if (interactionEntity && interactionEntity.NeedsCrosshair())
			{
				return true;
			}
		}
		using (TimeWarning.New("GetHeldEntity", 0.1f))
		{
			HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
			using (TimeWarning.New("NeedsCrosshair", 0.1f))
			{
				if (heldEntity && heldEntity.NeedsCrosshair())
				{
					return true;
				}
			}
		}
		return false;
	}
}
