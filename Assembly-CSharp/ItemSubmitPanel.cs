using System;

// Token: 0x02000660 RID: 1632
public class ItemSubmitPanel : LootPanel
{
	// Token: 0x06002461 RID: 9313 RVA: 0x000C0844 File Offset: 0x000BEA44
	public void SubmitClicked()
	{
		BaseEntity containerEntity = base.GetContainerEntity();
		if (containerEntity == null)
		{
			return;
		}
		containerEntity.SendMessage("TrySubmit");
	}
}
