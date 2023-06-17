using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C0 RID: 1728
public class SteamUserButton : MonoBehaviour
{
	// Token: 0x0400228D RID: 8845
	public Text steamName;

	// Token: 0x0400228E RID: 8846
	public RawImage avatar;

	// Token: 0x0400228F RID: 8847
	public Color textColorInGame;

	// Token: 0x04002290 RID: 8848
	public Color textColorOnline;

	// Token: 0x04002291 RID: 8849
	public Color textColorNormal;

	// Token: 0x06002666 RID: 9830 RVA: 0x000CA250 File Offset: 0x000C8450
	public void UpdateFromUser(ulong steamid, bool isInThisGame, bool isOnline)
	{
		if (Client.Steam == null)
		{
			return;
		}
		this.steamName.text = Client.Steam.Friends.GetName(steamid);
		this.avatar.texture = SingletonComponent<SteamClient>.Instance.GetAvatarTexture(steamid);
		if (isInThisGame)
		{
			this.steamName.color = this.textColorInGame;
			return;
		}
		if (isOnline)
		{
			this.steamName.color = this.textColorOnline;
			return;
		}
		this.steamName.color = this.textColorNormal;
	}
}
