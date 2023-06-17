using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000680 RID: 1664
public class LookAtPlant : MonoBehaviour
{
	// Token: 0x0400212F RID: 8495
	public CanvasGroup group;

	// Token: 0x04002130 RID: 8496
	public Text waterText;

	// Token: 0x04002131 RID: 8497
	public Text ageText;

	// Token: 0x04002132 RID: 8498
	public Text maturityText;

	// Token: 0x04002133 RID: 8499
	public Text yieldText;

	// Token: 0x04002134 RID: 8500
	public Color defaultColor;

	// Token: 0x04002135 RID: 8501
	public Color goodColor;

	// Token: 0x04002136 RID: 8502
	public Color avgColor;

	// Token: 0x04002137 RID: 8503
	public Color badColor;

	// Token: 0x06002521 RID: 9505 RVA: 0x000C46C0 File Offset: 0x000C28C0
	private void Update()
	{
		bool flag = this.UpdateLookingAtPlant();
		this.group.alpha = Mathf.MoveTowards(this.group.alpha, flag ? 1f : 0f, Time.deltaTime * 5f);
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000C470C File Offset: 0x000C290C
	public bool UpdateLookingAtPlant()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (!entity)
		{
			return false;
		}
		PlantEntity plantEntity = (entity.lookingAtEntity != null && entity.lookingAtEntity is PlantEntity) ? entity.lookingAtEntity.GetComponent<PlantEntity>() : null;
		if (plantEntity == null)
		{
			return false;
		}
		this.waterText.text = plantEntity.water.ToString("N0") + " mL";
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)plantEntity.GetRealAge());
		TimeSpan timeSpan2 = TimeSpan.FromSeconds((double)plantEntity.GetGrowthAge());
		if (timeSpan.Days > 0)
		{
			this.ageText.text = string.Format("{0:N0}d {1:N0}h {2:N0}m", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
		}
		else if (timeSpan.Hours > 0)
		{
			this.ageText.text = string.Format("{0:N0}h {1:N0}m", timeSpan.Hours, timeSpan.Minutes);
		}
		else
		{
			this.ageText.text = string.Format("{0:N0} Minutes", timeSpan.Minutes);
		}
		this.maturityText.text = ((float)timeSpan2.TotalMinutes / plantEntity.plantProperty.waterConsumptionLifetime * 100f).ToString("N1") + " % ";
		float num = plantEntity.client_yieldFraction;
		if (plantEntity.plantProperty.pickupItem.condition.enabled)
		{
			num *= plantEntity.plantProperty.fruitCurve.Evaluate(plantEntity.stageAgeFraction);
		}
		this.yieldText.text = (num * 100f).ToString("N0") + " %";
		return true;
	}
}
