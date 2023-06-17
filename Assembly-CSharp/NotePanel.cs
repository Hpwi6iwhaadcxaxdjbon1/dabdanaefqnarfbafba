using System;
using Facepunch.Extend;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F6 RID: 246
public class NotePanel : MonoBehaviour
{
	// Token: 0x04000762 RID: 1890
	public InputField input;

	// Token: 0x04000763 RID: 1891
	private Item item;

	// Token: 0x06000B13 RID: 2835 RVA: 0x0000ABB6 File Offset: 0x00008DB6
	private void OnItem(Item item)
	{
		this.item = item;
		if (item.text == null)
		{
			item.text = "";
		}
		this.input.text = item.text;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00057070 File Offset: 0x00055270
	public void OnChanged()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.text = StringExtensions.Truncate(this.input.text, 1024, null);
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "note.update", new object[]
		{
			this.item.uid,
			this.item.text
		});
	}
}
