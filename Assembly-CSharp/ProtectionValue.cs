using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200066C RID: 1644
public class ProtectionValue : MonoBehaviour, IClothingChanged
{
	// Token: 0x04002093 RID: 8339
	public CanvasGroup group;

	// Token: 0x04002094 RID: 8340
	public Text text;

	// Token: 0x04002095 RID: 8341
	public DamageType damageType;

	// Token: 0x04002096 RID: 8342
	public bool selectedItem;

	// Token: 0x04002097 RID: 8343
	public bool displayBaseProtection;

	// Token: 0x06002497 RID: 9367 RVA: 0x0001CD6E File Offset: 0x0001AF6E
	private void OnEnable()
	{
		GlobalMessages.onClothingChanged.Add(this);
		this.OnClothingChanged();
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x0001CD81 File Offset: 0x0001AF81
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onClothingChanged.Remove(this);
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000C177C File Offset: 0x000BF97C
	public void OnClothingChanged()
	{
		float num = 0f;
		if (this.selectedItem)
		{
			return;
		}
		if (LocalPlayer.Entity)
		{
			if (this.displayBaseProtection)
			{
				num = LocalPlayer.Entity.baseProtection.Get(this.damageType);
			}
			else if (PaperDollSegment.selectedAreas == (HitArea)0)
			{
				num = 0f;
			}
			else
			{
				num = this.GetProtectionForArea(PaperDollSegment.selectedAreas);
			}
		}
		this.SetValue(num * 100f);
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000C17EC File Offset: 0x000BF9EC
	public void UpdateFromItem(Item item)
	{
		float num = 0f;
		if (item != null)
		{
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (component)
			{
				num = component.GetProtection(item, this.damageType);
			}
		}
		this.SetValue(num * 100f);
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000C1834 File Offset: 0x000BFA34
	public float GetProtectionForArea(HitArea area)
	{
		if (LocalPlayer.Entity == null)
		{
			this.SetValue(0f);
			return 0f;
		}
		float num = 0f;
		foreach (Item item in LocalPlayer.Entity.inventory.containerWear.itemList)
		{
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null) && component.ProtectsArea(area) && component.protectionProperties != null)
			{
				num += component.protectionProperties.amounts[(int)this.damageType];
			}
		}
		return num;
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000C18F4 File Offset: 0x000BFAF4
	public void UpdateFromArea(HitArea area)
	{
		if (LocalPlayer.Entity == null)
		{
			this.SetValue(0f);
			return;
		}
		float num = 0f;
		foreach (Item item in LocalPlayer.Entity.inventory.containerWear.itemList)
		{
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null) && component.ProtectsArea(area) && component.protectionProperties != null)
			{
				num += component.protectionProperties.amounts[(int)this.damageType];
			}
		}
		this.SetValue(num);
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000C19B4 File Offset: 0x000BFBB4
	public void SetValue(float val)
	{
		if (val < 1f)
		{
			this.group.alpha = 0.1f;
			this.text.text = "";
			if (this.selectedItem)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			this.group.alpha = Mathf.Lerp(0.3f, 0.9f, Mathf.InverseLerp(0f, 50f, val));
			this.text.text = string.Format("{0:n0}", val);
			if (!this.selectedItem)
			{
				Text text = this.text;
				text.text += " %";
			}
			if (this.selectedItem)
			{
				base.gameObject.SetActive(true);
			}
		}
	}
}
