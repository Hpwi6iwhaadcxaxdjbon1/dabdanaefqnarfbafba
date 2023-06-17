using System;
using GameMenu;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000681 RID: 1665
public class LookatTooltip : MonoBehaviour
{
	// Token: 0x04002138 RID: 8504
	public static bool Enabled = true;

	// Token: 0x04002139 RID: 8505
	public Animator tooltipAnimator;

	// Token: 0x0400213A RID: 8506
	public BaseEntity currentlyLookingAt;

	// Token: 0x0400213B RID: 8507
	public Text textLabel;

	// Token: 0x0400213C RID: 8508
	public Image icon;

	// Token: 0x0400213D RID: 8509
	private int lastInfoHash;

	// Token: 0x06002524 RID: 9508 RVA: 0x000C48E0 File Offset: 0x000C2AE0
	private void Update()
	{
		BaseEntity ent = null;
		if (LookatTooltip.Enabled && LocalPlayer.Entity != null)
		{
			ent = LocalPlayer.Entity.GetInteractionEntity();
		}
		using (TimeWarning.New("UpdateLookingAtHud", 0.1f))
		{
			this.UpdateLookingAtHud(ent);
		}
		LookatTooltip.Enabled = true;
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000C4948 File Offset: 0x000C2B48
	private void UpdateLookingAtHud(BaseEntity ent)
	{
		if (!this.HasChanged(ent))
		{
			return;
		}
		if (ent == null)
		{
			this.tooltipAnimator.SetBool("visible", false);
			this.currentlyLookingAt = null;
			return;
		}
		if (this.currentlyLookingAt != null)
		{
			this.tooltipAnimator.SetTrigger("changed");
		}
		this.currentlyLookingAt = ent;
		this.tooltipAnimator.SetBool("visible", true);
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000C49B8 File Offset: 0x000C2BB8
	public bool HasChanged(BaseEntity ent)
	{
		bool result;
		using (TimeWarning.New("HasChanged", 0.1f))
		{
			int num = 0;
			if (ent)
			{
				num++;
				num += ent.GetHashCode();
				Info info = Util.GetInfo(ent.gameObject, LocalPlayer.Entity);
				if (info.IsValid)
				{
					num += info.action.GetHashCode();
					num += info.hasMoreOptions.GetHashCode();
					if (!string.IsNullOrEmpty(info.icon))
					{
						num += info.icon.GetHashCode();
					}
				}
			}
			if (this.lastInfoHash == num)
			{
				result = false;
			}
			else
			{
				this.lastInfoHash = num;
				this.currentlyLookingAt = ent;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x000C4A78 File Offset: 0x000C2C78
	public void UpdateTooltipInfo()
	{
		if (this.currentlyLookingAt == null)
		{
			this.textLabel.text = "";
			this.icon.sprite = FileSystem.Load<Sprite>("Assets/Icons/warning.png", true);
			return;
		}
		Info info = Util.GetInfo(this.currentlyLookingAt.gameObject, LocalPlayer.Entity);
		if (!info.IsValid)
		{
			this.tooltipAnimator.SetBool("hasMenu", false);
			this.textLabel.text = "";
			this.icon.sprite = FileSystem.Load<Sprite>("Assets/Icons/warning.png", true);
			return;
		}
		this.tooltipAnimator.SetBool("hasMenu", info.hasMoreOptions);
		this.textLabel.text = Translate.Get(info.action, null).ToUpper();
		this.icon.sprite = info.iconSprite;
		if (this.icon.sprite == null)
		{
			this.icon.sprite = FileSystem.Load<Sprite>("Assets/Icons/" + info.icon + ".png", true);
		}
		if (this.icon.sprite == null)
		{
			this.icon.sprite = FileSystem.Load<Sprite>("Assets/Icons/warning.png", true);
		}
	}
}
