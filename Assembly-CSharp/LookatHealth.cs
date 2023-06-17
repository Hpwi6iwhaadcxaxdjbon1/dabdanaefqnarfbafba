using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200067E RID: 1662
public class LookatHealth : MonoBehaviour
{
	// Token: 0x04002102 RID: 8450
	public static bool Enabled = true;

	// Token: 0x04002103 RID: 8451
	public GameObject container;

	// Token: 0x04002104 RID: 8452
	public Text textHealth;

	// Token: 0x04002105 RID: 8453
	public Text textStability;

	// Token: 0x04002106 RID: 8454
	public Image healthBar;

	// Token: 0x04002107 RID: 8455
	public Image healthBarBG;

	// Token: 0x04002108 RID: 8456
	public Color barBGColorNormal;

	// Token: 0x04002109 RID: 8457
	public Color barBGColorUnstable;

	// Token: 0x0400210A RID: 8458
	internal BaseEntity previousLookingAt;

	// Token: 0x0400210B RID: 8459
	internal float idealWidth;

	// Token: 0x0400210C RID: 8460
	internal Animator anim;

	// Token: 0x0600250F RID: 9487 RVA: 0x0001D0F1 File Offset: 0x0001B2F1
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.idealWidth = this.healthBar.rectTransform.sizeDelta.x;
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x000C38D4 File Offset: 0x000C1AD4
	private void Update()
	{
		bool flag = this.UpdateInfo();
		if (!flag)
		{
			this.previousLookingAt = null;
			this.anim.ResetTrigger("updated");
		}
		this.anim.SetBool("visible", flag);
		LookatHealth.Enabled = true;
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x000C391C File Offset: 0x000C1B1C
	private bool UpdateInfo()
	{
		if (!LookatHealth.Enabled)
		{
			return false;
		}
		if (LocalPlayer.Entity == null)
		{
			return false;
		}
		if (LocalPlayer.Entity.IsDead())
		{
			return false;
		}
		if (LocalPlayer.Entity.IsSpectating())
		{
			return false;
		}
		BaseEntity lookingAtEntity = LocalPlayer.Entity.lookingAtEntity;
		if (!lookingAtEntity.IsValid())
		{
			return false;
		}
		BaseCombatEntity baseCombatEntity = lookingAtEntity as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		if (!baseCombatEntity.DisplayHealthInfo(LocalPlayer.Entity))
		{
			return false;
		}
		if (this.previousLookingAt != null && this.previousLookingAt != lookingAtEntity)
		{
			this.anim.SetTrigger("updated");
		}
		this.previousLookingAt = lookingAtEntity;
		this.textHealth.text = string.Format("{0} / {1}", Mathf.CeilToInt(baseCombatEntity.Health()), Mathf.RoundToInt(baseCombatEntity.MaxHealth()));
		this.healthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.idealWidth * Mathf.Clamp01(baseCombatEntity.Health() / baseCombatEntity.MaxHealth()));
		StabilityEntity stabilityEntity = lookingAtEntity as StabilityEntity;
		if (stabilityEntity && stabilityEntity.cachedStability >= 0f)
		{
			this.healthBarBG.color = Color.Lerp(this.barBGColorUnstable, this.barBGColorNormal, stabilityEntity.cachedStability);
			this.textStability.text = string.Format("{0}% STABLE", Mathf.RoundToInt(stabilityEntity.cachedStability * 100f));
		}
		else
		{
			this.textStability.text = "";
			this.healthBarBG.color = this.barBGColorNormal;
		}
		return true;
	}
}
