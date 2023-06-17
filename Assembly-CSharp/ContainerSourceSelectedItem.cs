using System;

// Token: 0x0200061A RID: 1562
public class ContainerSourceSelectedItem : ItemContainerSource
{
	// Token: 0x060022FB RID: 8955 RVA: 0x000BA818 File Offset: 0x000B8A18
	public override ItemContainer GetItemContainer()
	{
		Item item = SelectedItem.item;
		if (item == null)
		{
			return null;
		}
		return item.contents;
	}
}
