using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000607 RID: 1543
public class ChatEntry : MonoBehaviour
{
	// Token: 0x04001EE8 RID: 7912
	public Text text;

	// Token: 0x04001EE9 RID: 7913
	public RawImage avatar;

	// Token: 0x04001EEA RID: 7914
	public CanvasGroup canvasGroup;

	// Token: 0x04001EEB RID: 7915
	public float lifeStarted;

	// Token: 0x04001EEC RID: 7916
	public ulong steamid;

	// Token: 0x04001EED RID: 7917
	private Texture defaultTexture;

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06002299 RID: 8857 RVA: 0x0001B7E8 File Offset: 0x000199E8
	public float age
	{
		get
		{
			return Time.realtimeSinceStartup - this.lifeStarted;
		}
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x000B9348 File Offset: 0x000B7548
	public void Setup(ulong steamid, string message, float volume)
	{
		this.steamid = steamid;
		this.text.text = message;
		this.lifeStarted = Time.realtimeSinceStartup;
		if (this.defaultTexture == null)
		{
			this.defaultTexture = this.avatar.texture;
		}
		if (steamid > 0UL && SingletonComponent<SteamClient>.Instance)
		{
			this.avatar.texture = SingletonComponent<SteamClient>.Instance.GetAvatarTexture(steamid);
			return;
		}
		this.avatar.texture = this.defaultTexture;
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x000B93CC File Offset: 0x000B75CC
	protected void Update()
	{
		float num = 1f;
		if (this.age > 10f && !UIChat.isOpen)
		{
			num = 0f;
		}
		if (this.canvasGroup.alpha == num)
		{
			return;
		}
		this.canvasGroup.alpha = Mathf.Lerp(this.canvasGroup.alpha, num, Time.smoothDeltaTime * 5f);
		if (this.canvasGroup.alpha < 0.1f && num == 0f)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x0001B7F6 File Offset: 0x000199F6
	public void ViewSteamProfile()
	{
		if (this.steamid == 0UL)
		{
			return;
		}
		Client.Steam.Overlay.OpenProfile(this.steamid);
	}
}
