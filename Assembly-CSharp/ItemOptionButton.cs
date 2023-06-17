using System;
using GameMenu;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200065A RID: 1626
public class ItemOptionButton : MonoBehaviour
{
	// Token: 0x04002056 RID: 8278
	public new Text name;

	// Token: 0x04002057 RID: 8279
	public Image icon;

	// Token: 0x04002058 RID: 8280
	internal Item item;

	// Token: 0x04002059 RID: 8281
	internal Option option;

	// Token: 0x0600244A RID: 9290 RVA: 0x000C054C File Offset: 0x000BE74C
	public void Setup(Item item, Option option)
	{
		this.name.text = Translate.Get(option.title, null);
		this.icon.sprite = option.iconSprite;
		if (this.icon.sprite == null && !string.IsNullOrEmpty(option.icon))
		{
			Sprite sprite = FileSystem.Load<Sprite>("Assets/Icons/" + option.icon + ".png", true);
			if (sprite)
			{
				this.icon.sprite = sprite;
			}
			else
			{
				Debug.LogError("Missing Icon: " + option.icon);
			}
		}
		this.item = item;
		this.option = option;
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x0001CA1A File Offset: 0x0001AC1A
	public void Pressed()
	{
		if (this.option.function != null)
		{
			this.option.function.Invoke(LocalPlayer.Entity);
			return;
		}
		LocalPlayer.ItemCommand(this.item.uid, this.option.command);
	}
}
