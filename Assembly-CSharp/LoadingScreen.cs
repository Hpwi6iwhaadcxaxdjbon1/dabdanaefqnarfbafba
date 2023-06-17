using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000685 RID: 1669
public class LoadingScreen : SingletonComponent<LoadingScreen>
{
	// Token: 0x04002148 RID: 8520
	public CanvasRenderer panel;

	// Token: 0x04002149 RID: 8521
	public Text title;

	// Token: 0x0400214A RID: 8522
	public Text subtitle;

	// Token: 0x0400214B RID: 8523
	public Button skipButton;

	// Token: 0x0400214C RID: 8524
	public AudioSource music;

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06002536 RID: 9526 RVA: 0x0001D270 File Offset: 0x0001B470
	public static bool isOpen
	{
		get
		{
			return SingletonComponent<LoadingScreen>.Instance && SingletonComponent<LoadingScreen>.Instance.panel && SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf;
		}
	}

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06002537 RID: 9527 RVA: 0x0001D2A5 File Offset: 0x0001B4A5
	// (set) Token: 0x06002538 RID: 9528 RVA: 0x0001D2AC File Offset: 0x0001B4AC
	public static bool WantsSkip { get; private set; }

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x0600253A RID: 9530 RVA: 0x0001D2BC File Offset: 0x0001B4BC
	// (set) Token: 0x06002539 RID: 9529 RVA: 0x0001D2B4 File Offset: 0x0001B4B4
	public static string Text { get; private set; }

	// Token: 0x0600253B RID: 9531 RVA: 0x0001D2C3 File Offset: 0x0001B4C3
	protected override void Awake()
	{
		base.Awake();
		LoadingScreen.HideSkip();
		LoadingScreen.Hide();
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x000C4CF0 File Offset: 0x000C2EF0
	public static void Show()
	{
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			Debug.LogWarning("Wanted to show loading screen but not ready");
			return;
		}
		if (SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.panel.gameObject.SetActive(true);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(false);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(true);
		MusicManager.RaiseIntensityTo(0.5f, 999);
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x000C4D70 File Offset: 0x000C2F70
	public static void Hide()
	{
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.panel)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.panel.gameObject)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.panel.gameObject.SetActive(false);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(false);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(true);
		if (LevelManager.isLoaded && SingletonComponent<MusicManager>.Instance != null)
		{
			SingletonComponent<MusicManager>.Instance.StopMusic();
		}
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x0001D2D5 File Offset: 0x0001B4D5
	public static void ShowSkip()
	{
		LoadingScreen.WantsSkip = false;
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.skipButton)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.skipButton.gameObject.SetActive(true);
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x0001D311 File Offset: 0x0001B511
	public static void HideSkip()
	{
		LoadingScreen.WantsSkip = false;
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.skipButton)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.skipButton.gameObject.SetActive(false);
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000C4E20 File Offset: 0x000C3020
	public static void Update(string strType)
	{
		if (LoadingScreen.Text == strType)
		{
			return;
		}
		LoadingScreen.Text = strType;
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.subtitle.text = strType.ToUpper();
		GameObject gameObject = GameObject.Find("MenuMusic");
		if (gameObject)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component)
			{
				component.Pause();
			}
		}
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x0001D34D File Offset: 0x0001B54D
	public void UpdateFromServer(string strTitle, string strSubtitle)
	{
		this.title.text = strTitle;
		this.subtitle.text = strSubtitle;
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x000C4E8C File Offset: 0x000C308C
	public void CancelLoading()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet(), "client.disconnect", Array.Empty<object>());
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x0001D367 File Offset: 0x0001B567
	public void SkipLoading()
	{
		LoadingScreen.WantsSkip = true;
	}
}
