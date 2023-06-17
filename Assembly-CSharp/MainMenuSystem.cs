using System;
using System.Collections;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000686 RID: 1670
public class MainMenuSystem : SingletonComponent<MainMenuSystem>
{
	// Token: 0x0400214D RID: 8525
	public static bool isOpen = true;

	// Token: 0x06002545 RID: 9541 RVA: 0x0001D377 File Offset: 0x0001B577
	protected override void Awake()
	{
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject.transform.root.gameObject);
	}

	// Token: 0x06002546 RID: 9542 RVA: 0x000C4EB8 File Offset: 0x000C30B8
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.OnEscape();
		}
		if (LocalPlayer.Entity == null)
		{
			Input.Frame();
			Input.Update();
		}
		if (MainMenuSystem.isOpen && SuppressMenu.Any)
		{
			MainMenuSystem.Hide();
		}
		if (MainMenuSystem.isOpen)
		{
			CursorManager.HoldOpen(false);
		}
	}

	// Token: 0x06002547 RID: 9543 RVA: 0x000C4F0C File Offset: 0x000C310C
	private void OnEscape()
	{
		if (UIEscapeCapture.EscapePressed())
		{
			return;
		}
		if (!LevelManager.isLoaded)
		{
			return;
		}
		if (DeveloperTools.isOpen)
		{
			DeveloperTools.Close();
			return;
		}
		if (UIInventory.isOpen)
		{
			UIInventory.Close();
			return;
		}
		if (UICrafting.isOpen)
		{
			UICrafting.Close();
			return;
		}
		if (MainMenuSystem.isOpen)
		{
			MainMenuSystem.Hide();
			return;
		}
		MainMenuSystem.Show();
	}

	// Token: 0x06002548 RID: 9544 RVA: 0x0001D399 File Offset: 0x0001B599
	public static void Hide()
	{
		if (!MainMenuSystem.isOpen)
		{
			return;
		}
		MainMenuSystem.isOpen = false;
		if (SingletonComponent<MainMenuSystem>.Instance == null)
		{
			return;
		}
		SingletonComponent<MainMenuSystem>.Instance.OnEnabledState(false);
	}

	// Token: 0x06002549 RID: 9545 RVA: 0x0001D3C2 File Offset: 0x0001B5C2
	public static void Show()
	{
		if (MainMenuSystem.isOpen)
		{
			return;
		}
		MainMenuSystem.isOpen = true;
		if (SingletonComponent<MainMenuSystem>.Instance == null)
		{
			return;
		}
		SingletonComponent<MainMenuSystem>.Instance.OnEnabledState(true);
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000C4F64 File Offset: 0x000C3164
	private void OnEnabledState(bool b)
	{
		base.GetComponent<NeedsKeyboard>().enabled = b;
		base.GetComponent<CanvasGroup>().alpha = (b ? 1f : 0f);
		GraphicRaycaster[] componentsInChildren = base.GetComponentsInChildren<GraphicRaycaster>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = b;
		}
		Client.EventSystem.SetSelectedGameObject(null);
		if (CommunityEntity.ClientInstance)
		{
			CommunityEntity.ClientInstance.SetVisible(!b);
		}
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x0001D3EB File Offset: 0x0001B5EB
	public void OpenWorkshop()
	{
		base.StartCoroutine(this.DoOpenWorkshop());
	}

	// Token: 0x0600254C RID: 9548 RVA: 0x0001D3FA File Offset: 0x0001B5FA
	private IEnumerator DoOpenWorkshop()
	{
		LoadingScreen.Show();
		LoadingScreen.Update("Loading Skinnables");
		yield return null;
		yield return null;
		GameManifest.LoadAssets();
		LoadingScreen.Update("Opening Workshop");
		yield return null;
		yield return null;
		yield return base.StartCoroutine(LevelManager.LoadLevelAsync("UIWorkshop", true));
		LoadingScreen.Hide();
		yield break;
	}
}
