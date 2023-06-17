using System;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
public class TweakUI : SingletonComponent<TweakUI>
{
	// Token: 0x040022C0 RID: 8896
	public static bool isOpen;

	// Token: 0x060026AD RID: 9901 RVA: 0x0001E284 File Offset: 0x0001C484
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2) && this.CanToggle())
		{
			this.SetVisible(!TweakUI.isOpen);
		}
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x0001E2A8 File Offset: 0x0001C4A8
	protected bool CanToggle()
	{
		return LevelManager.isLoaded;
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x0001E2B4 File Offset: 0x0001C4B4
	public void SetVisible(bool b)
	{
		if (b)
		{
			TweakUI.isOpen = true;
			return;
		}
		TweakUI.isOpen = false;
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "trackir.refresh", Array.Empty<object>());
	}
}
