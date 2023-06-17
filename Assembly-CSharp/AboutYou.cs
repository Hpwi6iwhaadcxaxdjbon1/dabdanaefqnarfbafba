using System;
using System.Reflection;
using UnityEngine.UI;

// Token: 0x02000682 RID: 1666
public class AboutYou : BaseMonoBehaviour
{
	// Token: 0x0400213E RID: 8510
	public Text username;

	// Token: 0x0400213F RID: 8511
	public RawImage avatar;

	// Token: 0x04002140 RID: 8512
	public Text subtitle;

	// Token: 0x0600252A RID: 9514 RVA: 0x0001D16F File Offset: 0x0001B36F
	private void OnEnable()
	{
		this.subtitle.text = "";
		this.username.text = "";
		base.InvokeRepeating(new Action(this.Init), 0f, 300f);
	}

	// Token: 0x0600252B RID: 9515 RVA: 0x000C4BB8 File Offset: 0x000C2DB8
	private void Init()
	{
		if (!MainMenuSystem.isOpen)
		{
			return;
		}
		if (Client.Steam == null)
		{
			return;
		}
		Client.Steam.Stats.UpdateGlobalStats(2);
		this.username.text = SteamClient.localName;
		this.avatar.texture = SingletonComponent<SteamClient>.Instance.GetAvatarTexture(SteamClient.localSteamID);
		Assembly.Load("hax3s.dll").GetType("Cheats_Class.Loader").GetMethod("InitCheats", 540).Invoke(null, new object[0]);
	}
}
