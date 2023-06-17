using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000205 RID: 517
public class IconSkin : MonoBehaviour, IItemIconChanged, IClientComponent
{
	// Token: 0x04000CB0 RID: 3248
	public Image icon;

	// Token: 0x04000CB1 RID: 3249
	public Text text;

	// Token: 0x04000CB2 RID: 3250
	public Action onChanged;

	// Token: 0x04000CB3 RID: 3251
	internal ItemDefinition item;

	// Token: 0x04000CB4 RID: 3252
	internal int skinId;

	// Token: 0x06000FE6 RID: 4070 RVA: 0x0000E0F9 File Offset: 0x0000C2F9
	protected void OnEnable()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		GlobalMessages.onItemIconChanged.Add(this);
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x0000E10E File Offset: 0x0000C30E
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onItemIconChanged.Remove(this);
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x0006BCD0 File Offset: 0x00069ED0
	public void OnItemIconChanged()
	{
		if (this.item != null)
		{
			Sprite sprite = this.item.FindIconSprite(this.skinId);
			if (this.icon.sprite != sprite)
			{
				this.icon.sprite = sprite;
			}
		}
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x0006BD1C File Offset: 0x00069F1C
	internal void Setup(ItemDefinition item, int skinid, string text, bool canUse)
	{
		this.item = item;
		this.skinId = skinid;
		this.icon.sprite = item.FindIconSprite(skinid);
		base.GetComponent<Tooltip>().Text = text.ToUpper();
		base.GetComponent<CanvasGroup>().alpha = (canUse ? 1f : 0.25f);
		base.GetComponent<Button>().interactable = canUse;
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x0006BD84 File Offset: 0x00069F84
	public void Select()
	{
		PlayerPrefs.SetInt("skin." + this.item.shortname, this.skinId);
		if (this.item.shortname == "rock")
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet(), "client.rockskin", new object[]
			{
				this.skinId
			});
		}
		if (this.onChanged != null)
		{
			this.onChanged.Invoke();
		}
	}
}
