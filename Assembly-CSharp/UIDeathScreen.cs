using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Steamworks;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006DB RID: 1755
public class UIDeathScreen : SingletonComponent<UIDeathScreen>, IUIScreen
{
	// Token: 0x040022E9 RID: 8937
	public GameObject sleepingBagIconPrefab;

	// Token: 0x040022EA RID: 8938
	public GameObject sleepingBagContainer;

	// Token: 0x040022EB RID: 8939
	public LifeInfographic previousLifeInfographic;

	// Token: 0x040022EC RID: 8940
	public Animator screenAnimator;

	// Token: 0x040022ED RID: 8941
	public bool fadeIn;

	// Token: 0x040022EE RID: 8942
	public Button ReportCheatButton;

	// Token: 0x040022EF RID: 8943
	protected CanvasGroup canvasGroup;

	// Token: 0x040022F0 RID: 8944
	protected GraphicRaycaster graphicRaycaster;

	// Token: 0x040022F1 RID: 8945
	protected NeedsCursor needsCursor;

	// Token: 0x060026E1 RID: 9953 RVA: 0x000CB464 File Offset: 0x000C9664
	public void SetVisible(bool b)
	{
		if (b && this.canvasGroup.alpha < 0.5f)
		{
			if (LocalPlayer.Entity)
			{
				Debug.Log("LocalPlayer.Entity.SecondsSinceDeath: " + LocalPlayer.Entity.SecondsSinceDeath);
			}
			if (this.fadeIn || (LocalPlayer.Entity && LocalPlayer.Entity.SecondsSinceDeath < 5f))
			{
				SingletonComponent<UIDeathScreen>.Instance.screenAnimator.Play("fadeIn");
			}
			else
			{
				SingletonComponent<UIDeathScreen>.Instance.screenAnimator.Play("show");
			}
		}
		this.canvasGroup.alpha = (b ? 1f : 0f);
		this.graphicRaycaster.enabled = b;
		this.needsCursor.enabled = b;
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x0001E5F0 File Offset: 0x0001C7F0
	private void OnEnable()
	{
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		this.graphicRaycaster = base.GetComponent<GraphicRaycaster>();
		this.needsCursor = base.GetComponent<NeedsCursor>();
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x0001E616 File Offset: 0x0001C816
	public void OnRespawn()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "respawn", Array.Empty<object>());
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x000CB534 File Offset: 0x000C9734
	private void UpdateRespawnInformation(RespawnInformation info)
	{
		this.ReportCheatButton.gameObject.SetActive(false);
		this.fadeIn = info.fadeIn;
		PlayerLifeStory previousLife = info.previousLife;
		if (previousLife != null)
		{
			PlayerLifeStory.DeathInfo deathInfo = previousLife.deathInfo;
			if (deathInfo != null)
			{
				ulong attackerSteamID = deathInfo.attackerSteamID;
				if (attackerSteamID > 0UL)
				{
					if (LocalPlayer.Entity && LocalPlayer.Entity.userID != deathInfo.attackerSteamID && deathInfo.attackerSteamID > 10000000UL)
					{
						LocalPlayer.LastAttackerSteamID = deathInfo.attackerSteamID;
						LocalPlayer.LastAttackerName = deathInfo.attackerName;
						this.ReportCheatButton.gameObject.SetActive(true);
					}
					if (Client.Steam != null)
					{
						SteamFriend steamFriend = Client.Steam.Friends.Get(attackerSteamID);
						if (steamFriend != null)
						{
							deathInfo.attackerName = steamFriend.Name;
						}
					}
				}
			}
		}
		this.previousLifeInfographic.life = previousLife;
		this.previousLifeInfographic.Refresh();
		this.UpdateRespawnOptions(info.spawnOptions);
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000CB624 File Offset: 0x000C9824
	private void UpdateRespawnOptions(List<RespawnInformation.SpawnOptions> spawnOptions)
	{
		this.sleepingBagContainer.transform.DestroyChildren();
		foreach (RespawnInformation.SpawnOptions options in spawnOptions)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.sleepingBagIconPrefab);
			gameObject.transform.SetParent(this.sleepingBagContainer.transform, false);
			gameObject.GetComponent<SleepingBagButton>().Setup(options);
		}
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x0001E62D File Offset: 0x0001C82D
	public static void OnRespawnInformation(RespawnInformation info)
	{
		if (SingletonComponent<UIDeathScreen>.Instance == null)
		{
			return;
		}
		SingletonComponent<UIDeathScreen>.Instance.UpdateRespawnInformation(info);
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x0001E648 File Offset: 0x0001C848
	public void ReportCheater()
	{
		Feedback.Open("cheat");
	}
}
