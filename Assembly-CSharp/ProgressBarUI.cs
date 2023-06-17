using System;
using GameMenu;
using UnityEngine;

// Token: 0x020006B7 RID: 1719
public static class ProgressBarUI
{
	// Token: 0x04002271 RID: 8817
	private static Option option;

	// Token: 0x06002640 RID: 9792 RVA: 0x0001DCA4 File Offset: 0x0001BEA4
	public static bool IsOpen()
	{
		return ProgressBar.Instance.IsOpen;
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x0001DCB0 File Offset: 0x0001BEB0
	public static void Cancel()
	{
		if (ProgressBar.Instance != null)
		{
			ProgressBar.Instance.PlayCancelSound();
			ProgressBar.Instance.Close(false);
		}
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000C9C24 File Offset: 0x000C7E24
	public static void Open(Option sourceOption)
	{
		ProgressBarUI.Cancel();
		ProgressBarUI.option = sourceOption;
		if (!ProgressBarUI.option.show)
		{
			return;
		}
		string name = Translate.Get(ProgressBarUI.option.title, null);
		Sprite sprite = ProgressBarUI.option.iconSprite;
		Action<BasePlayer> function = ProgressBarUI.option.function;
		float time = ProgressBarUI.option.time;
		if (sprite == null)
		{
			sprite = FileSystem.Load<Sprite>("Assets/Icons/" + ProgressBarUI.option.icon + ".png", true);
		}
		if (sprite == null)
		{
			sprite = FileSystem.Load<Sprite>("Assets/Icons/warning.png", true);
		}
		ProgressBar.Instance.Open(name, sprite, function, time);
		ProgressBarUI.option.RunTimeStart();
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x000C9CD4 File Offset: 0x000C7ED4
	public static void FrameUpdate(BasePlayer player)
	{
		if (!ProgressBarUI.IsOpen())
		{
			return;
		}
		if (!ProgressBarUI.option.show)
		{
			ProgressBarUI.Cancel();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape) || player.IsDead() || player.IsSleeping())
		{
			ProgressBarUI.Cancel();
			return;
		}
		if (!Buttons.Use.IsDown)
		{
			ProgressBarUI.Cancel();
			return;
		}
		ProgressBarUI.option.RunTimeProgress();
	}
}
