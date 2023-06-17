using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x0200073F RID: 1855
public class GlobalMessages
{
	// Token: 0x040023E9 RID: 9193
	public static ListHashSet<IInventoryChanged> onInventoryChanged = new ListHashSet<IInventoryChanged>(8);

	// Token: 0x040023EA RID: 9194
	public static Action OnInventoryChangedAction = new Action(GlobalMessages.OnInventoryChanged);

	// Token: 0x040023EB RID: 9195
	public static ListHashSet<ILanguageChanged> onLanguageChanged = new ListHashSet<ILanguageChanged>(8);

	// Token: 0x040023EC RID: 9196
	public static Action OnLanguageChangedAction = new Action(GlobalMessages.OnLanguageChanged);

	// Token: 0x040023ED RID: 9197
	public static ListHashSet<IViewModeChanged> onViewModeChanged = new ListHashSet<IViewModeChanged>(8);

	// Token: 0x040023EE RID: 9198
	public static Action OnViewModeChangedAction = new Action(GlobalMessages.OnViewModeChanged);

	// Token: 0x040023EF RID: 9199
	public static ListHashSet<IClothingChanged> onClothingChanged = new ListHashSet<IClothingChanged>(8);

	// Token: 0x040023F0 RID: 9200
	public static Action OnClothingChangedAction = new Action(GlobalMessages.OnClothingChanged);

	// Token: 0x040023F1 RID: 9201
	public static ListHashSet<IViewModelUpdated> onViewModelUpdated = new ListHashSet<IViewModelUpdated>(8);

	// Token: 0x040023F2 RID: 9202
	public static Action OnViewModelUpdatedAction = new Action(GlobalMessages.OnViewModelUpdated);

	// Token: 0x040023F3 RID: 9203
	public static ListHashSet<IBlueprintsChanged> onBlueprintsChanged = new ListHashSet<IBlueprintsChanged>(8);

	// Token: 0x040023F4 RID: 9204
	public static Action OnBlueprintsChangedAction = new Action(GlobalMessages.OnBlueprintsChanged);

	// Token: 0x040023F5 RID: 9205
	public static ListHashSet<IItemAmountChanged> onItemAmountChanged = new ListHashSet<IItemAmountChanged>(8);

	// Token: 0x040023F6 RID: 9206
	public static Action OnItemAmountChangedAction = new Action(GlobalMessages.OnItemAmountChanged);

	// Token: 0x040023F7 RID: 9207
	public static ListHashSet<IItemIconChanged> onItemIconChanged = new ListHashSet<IItemIconChanged>(8);

	// Token: 0x040023F8 RID: 9208
	public static Action OnItemIconChangedAction = new Action(GlobalMessages.OnItemIconChanged);

	// Token: 0x06002868 RID: 10344 RVA: 0x000D0138 File Offset: 0x000CE338
	public static void OnInventoryChanged()
	{
		IInventoryChanged[] buffer = GlobalMessages.onInventoryChanged.Values.Buffer;
		int count = GlobalMessages.onInventoryChanged.Count;
		List<IInventoryChanged> list = Pool.GetList<IInventoryChanged>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnInventoryChanged();
		}
		Pool.FreeList<IInventoryChanged>(ref list);
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x000D01A0 File Offset: 0x000CE3A0
	public static void OnLanguageChanged()
	{
		ILanguageChanged[] buffer = GlobalMessages.onLanguageChanged.Values.Buffer;
		int count = GlobalMessages.onLanguageChanged.Count;
		List<ILanguageChanged> list = Pool.GetList<ILanguageChanged>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnLanguageChanged();
		}
		Pool.FreeList<ILanguageChanged>(ref list);
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x000D0208 File Offset: 0x000CE408
	public static void OnViewModeChanged()
	{
		IViewModeChanged[] buffer = GlobalMessages.onViewModeChanged.Values.Buffer;
		int count = GlobalMessages.onViewModeChanged.Count;
		List<IViewModeChanged> list = Pool.GetList<IViewModeChanged>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnViewModeChanged();
		}
		Pool.FreeList<IViewModeChanged>(ref list);
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000D0270 File Offset: 0x000CE470
	public static void OnClothingChanged()
	{
		IClothingChanged[] buffer = GlobalMessages.onClothingChanged.Values.Buffer;
		int count = GlobalMessages.onClothingChanged.Count;
		List<IClothingChanged> list = Pool.GetList<IClothingChanged>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnClothingChanged();
		}
		Pool.FreeList<IClothingChanged>(ref list);
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000D02D8 File Offset: 0x000CE4D8
	public static void OnViewModelUpdated()
	{
		IViewModelUpdated[] buffer = GlobalMessages.onViewModelUpdated.Values.Buffer;
		int count = GlobalMessages.onViewModelUpdated.Count;
		List<IViewModelUpdated> list = Pool.GetList<IViewModelUpdated>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnViewModelUpdated();
		}
		Pool.FreeList<IViewModelUpdated>(ref list);
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x000D0340 File Offset: 0x000CE540
	public static void OnBlueprintsChanged()
	{
		IBlueprintsChanged[] buffer = GlobalMessages.onBlueprintsChanged.Values.Buffer;
		int count = GlobalMessages.onBlueprintsChanged.Count;
		List<IBlueprintsChanged> list = Pool.GetList<IBlueprintsChanged>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnBlueprintsChanged();
		}
		Pool.FreeList<IBlueprintsChanged>(ref list);
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000D03A8 File Offset: 0x000CE5A8
	public static void OnItemAmountChanged()
	{
		IItemAmountChanged[] buffer = GlobalMessages.onItemAmountChanged.Values.Buffer;
		int count = GlobalMessages.onItemAmountChanged.Count;
		List<IItemAmountChanged> list = Pool.GetList<IItemAmountChanged>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnItemAmountChanged();
		}
		Pool.FreeList<IItemAmountChanged>(ref list);
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000D0410 File Offset: 0x000CE610
	public static void OnItemIconChanged()
	{
		IItemIconChanged[] buffer = GlobalMessages.onItemIconChanged.Values.Buffer;
		int count = GlobalMessages.onItemIconChanged.Count;
		List<IItemIconChanged> list = Pool.GetList<IItemIconChanged>();
		for (int i = 0; i < count; i++)
		{
			list.Add(buffer[i]);
		}
		for (int j = 0; j < count; j++)
		{
			list[j].OnItemIconChanged();
		}
		Pool.FreeList<IItemIconChanged>(ref list);
	}
}
