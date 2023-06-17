using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000640 RID: 1600
public class ArmorInformationPanel : ItemInformationPanel
{
	// Token: 0x04001FBF RID: 8127
	public ItemTextValue projectileDisplay;

	// Token: 0x04001FC0 RID: 8128
	public ItemTextValue meleeDisplay;

	// Token: 0x04001FC1 RID: 8129
	public ItemTextValue coldDisplay;

	// Token: 0x04001FC2 RID: 8130
	public ItemTextValue explosionDisplay;

	// Token: 0x04001FC3 RID: 8131
	public ItemTextValue radiationDisplay;

	// Token: 0x04001FC4 RID: 8132
	public ItemTextValue biteDisplay;

	// Token: 0x04001FC5 RID: 8133
	public ItemTextValue spacer;

	// Token: 0x04001FC6 RID: 8134
	public Text areaProtectionText;

	// Token: 0x04001FC7 RID: 8135
	public Translate.Phrase LegText;

	// Token: 0x04001FC8 RID: 8136
	public Translate.Phrase ChestText;

	// Token: 0x04001FC9 RID: 8137
	public Translate.Phrase HeadText;

	// Token: 0x04001FCA RID: 8138
	public Translate.Phrase ChestLegsText;

	// Token: 0x04001FCB RID: 8139
	public Translate.Phrase WholeBodyText;

	// Token: 0x060023AC RID: 9132 RVA: 0x000BD328 File Offset: 0x000BB528
	public override bool EligableForDisplay(ItemDefinition info)
	{
		if (info.selectionPanel == ItemSelectionPanel.Vessel)
		{
			return false;
		}
		ItemModWearable component = info.GetComponent<ItemModWearable>();
		return !(component == null) && !(component.protectionProperties == null);
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000BD364 File Offset: 0x000BB564
	public string ProtectionAreaText(ItemModWearable wearable)
	{
		bool flag = wearable.ProtectsArea(HitArea.Head);
		bool flag2 = wearable.ProtectsArea(HitArea.Chest);
		bool flag3 = wearable.ProtectsArea(HitArea.Leg);
		if (flag && !flag2 && !flag3)
		{
			return this.HeadText.translated;
		}
		if (flag2 && !flag && !flag3)
		{
			return this.ChestText.translated;
		}
		if (flag3 && !flag && !flag2)
		{
			return this.LegText.translated;
		}
		if (flag && flag3 && flag2)
		{
			return this.WholeBodyText.translated;
		}
		if (!flag && flag3 && flag2)
		{
			return this.ChestLegsText.translated;
		}
		return "";
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000BD3F8 File Offset: 0x000BB5F8
	public override void SetupForItem(ItemDefinition info, Item item = null)
	{
		ItemModWearable component = info.GetComponent<ItemModWearable>();
		if (component == null || component.protectionProperties == null)
		{
			return;
		}
		ProtectionProperties protectionProperties = component.protectionProperties;
		float num = 0.16666667f;
		float num2 = protectionProperties.amounts[9] * 100f;
		float num3 = protectionProperties.amounts[15] * 100f;
		float num4 = protectionProperties.amounts[18] * 100f * num;
		float num5 = protectionProperties.amounts[16] * 100f * num;
		float num6 = protectionProperties.amounts[14] * 100f * num;
		float num7 = protectionProperties.amounts[17] * 100f;
		this.projectileDisplay.SetValue(num2, 0, "");
		this.meleeDisplay.SetValue(num3, 0, "");
		this.coldDisplay.SetValue(num4, 0, "");
		this.radiationDisplay.SetValue(num7, 0, "");
		this.explosionDisplay.SetValue(num5, 0, "");
		this.biteDisplay.SetValue(num6, 0, "");
		this.projectileDisplay.gameObject.SetActive(num2 != 0f);
		this.meleeDisplay.gameObject.SetActive(num3 != 0f);
		this.coldDisplay.gameObject.SetActive(num4 != 0f);
		this.explosionDisplay.gameObject.SetActive(num5 != 0f);
		this.radiationDisplay.gameObject.SetActive(num7 != 0f);
		this.biteDisplay.gameObject.SetActive(num6 != 0f);
		this.areaProtectionText.text = this.ProtectionAreaText(component);
		this.projectileDisplay.gameObject.transform.parent.gameObject.SetActive(false);
		this.projectileDisplay.gameObject.transform.parent.gameObject.SetActive(true);
		Canvas.ForceUpdateCanvases();
	}
}
