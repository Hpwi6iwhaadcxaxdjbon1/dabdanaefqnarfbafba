using System;

// Token: 0x020006DE RID: 1758
public class UIIngame : SingletonComponent<UIIngame>
{
	// Token: 0x060026EF RID: 9967 RVA: 0x000CB874 File Offset: 0x000C9A74
	private void Update()
	{
		IUIScreen iuiscreen = this.DetermineActiveUI();
		if (iuiscreen != SingletonComponent<UISleepingScreen>.Instance)
		{
			SingletonComponent<UISleepingScreen>.Instance.SetVisible(false);
		}
		if (iuiscreen != SingletonComponent<UIDeathScreen>.Instance)
		{
			SingletonComponent<UIDeathScreen>.Instance.SetVisible(false);
		}
		if (iuiscreen != SingletonComponent<UIHUD>.Instance)
		{
			SingletonComponent<UIHUD>.Instance.SetVisible(false);
		}
		if (iuiscreen != null)
		{
			iuiscreen.SetVisible(true);
		}
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000CB8CC File Offset: 0x000C9ACC
	private IUIScreen DetermineActiveUI()
	{
		if (LoadingScreen.isOpen)
		{
			return null;
		}
		if (MainMenuSystem.isOpen)
		{
			return null;
		}
		if (!LevelManager.isLoaded)
		{
			return null;
		}
		if (UIDialog.isOpen)
		{
			return null;
		}
		if (LocalPlayer.Entity && LocalPlayer.Entity.IsSpectating())
		{
			return SingletonComponent<UIHUD>.Instance;
		}
		if (LocalPlayer.Entity && LocalPlayer.Entity.IsDead())
		{
			return SingletonComponent<UIDeathScreen>.Instance;
		}
		if (LocalPlayer.Entity && LocalPlayer.Entity.IsSleeping())
		{
			return SingletonComponent<UISleepingScreen>.Instance;
		}
		return SingletonComponent<UIHUD>.Instance;
	}
}
