using System;
using System.Collections.Generic;
using GameMenu;
using UnityEngine;

// Token: 0x02000611 RID: 1553
public static class ContextMenuUI
{
	// Token: 0x04001F18 RID: 7960
	public static ContextMenuUI.MenuType type;

	// Token: 0x060022DC RID: 8924 RVA: 0x0001BA1C File Offset: 0x00019C1C
	public static void Start(ContextMenuUI.MenuType menuType)
	{
		ContextMenuUI.Cancel();
		ContextMenuUI.type = menuType;
		PieMenu.Instance.Clear();
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000BA358 File Offset: 0x000B8558
	public static void AddOption(string name, string desc, Sprite icon, Action<BasePlayer> action, int order = 0, bool disabled = false, bool selected = false, string requirements = "")
	{
		PieMenu.MenuOption menuOption = new PieMenu.MenuOption();
		menuOption.name = Translate.Get(name, null);
		menuOption.desc = Translate.Get(desc, null);
		menuOption.sprite = icon;
		if (menuOption.sprite == null)
		{
			menuOption.sprite = FileSystem.Load<Sprite>("Assets/Icons/warning.png", true);
		}
		menuOption.action = action;
		menuOption.order = order;
		menuOption.disabled = disabled;
		menuOption.selected = selected;
		menuOption.requirements = requirements;
		PieMenu.Instance.AddOption(menuOption);
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x0001BA33 File Offset: 0x00019C33
	public static void End()
	{
		PieMenu.Instance.FinishAndOpen();
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x0001BA3F File Offset: 0x00019C3F
	public static bool IsOpen()
	{
		return PieMenu.Instance.IsOpen;
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x0001BA4B File Offset: 0x00019C4B
	public static void Cancel()
	{
		if (PieMenu.Instance != null)
		{
			PieMenu.Instance.PlayCancelSound();
			PieMenu.Instance.Close(false);
		}
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x0001BA6F File Offset: 0x00019C6F
	public static void DoSelect()
	{
		if (PieMenu.Instance.DoSelect())
		{
			PieMenu.Instance.Close(true);
		}
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x000BA3E0 File Offset: 0x000B85E0
	public static void Open(List<Option> options, ContextMenuUI.MenuType menuType)
	{
		ContextMenuUI.Start(menuType);
		foreach (Option option in options)
		{
			if (option.show)
			{
				Sprite sprite = option.iconSprite;
				if (sprite == null)
				{
					sprite = FileSystem.Load<Sprite>("Assets/Icons/" + option.icon + ".png", true);
				}
				ContextMenuUI.AddOption(option.title, option.desc, sprite, option.function, option.order, option.showDisabled, false, option.requirements);
			}
		}
		ContextMenuUI.End();
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000BA494 File Offset: 0x000B8694
	public static void FrameUpdate(BasePlayer player)
	{
		if (!ContextMenuUI.IsOpen())
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape) || player.IsDead() || player.IsSleeping())
		{
			ContextMenuUI.Cancel();
			return;
		}
		if (ContextMenuUI.type == ContextMenuUI.MenuType.Use)
		{
			if (!Buttons.Use.IsDown)
			{
				ContextMenuUI.Cancel();
				return;
			}
			if (Buttons.Attack2.JustPressed)
			{
				ContextMenuUI.Cancel();
				return;
			}
			if (Buttons.Attack.JustPressed)
			{
				ContextMenuUI.DoSelect();
				return;
			}
			return;
		}
		else if (ContextMenuUI.type == ContextMenuUI.MenuType.RightClick)
		{
			if (!Buttons.Attack2.IsDown)
			{
				ContextMenuUI.Cancel();
				return;
			}
			if (Buttons.Attack.IsDown)
			{
				ContextMenuUI.DoSelect();
				return;
			}
			return;
		}
		else
		{
			if (ContextMenuUI.type != ContextMenuUI.MenuType.Reload)
			{
				return;
			}
			if (!Buttons.Reload.IsDown)
			{
				ContextMenuUI.Cancel();
				return;
			}
			if (Buttons.Attack.IsDown)
			{
				ContextMenuUI.DoSelect();
				return;
			}
			return;
		}
	}

	// Token: 0x02000612 RID: 1554
	public enum MenuType
	{
		// Token: 0x04001F1A RID: 7962
		Use,
		// Token: 0x04001F1B RID: 7963
		RightClick,
		// Token: 0x04001F1C RID: 7964
		Reload
	}
}
