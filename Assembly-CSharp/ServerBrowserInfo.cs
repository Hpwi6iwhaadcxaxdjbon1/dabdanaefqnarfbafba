using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch.Extend;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000694 RID: 1684
public class ServerBrowserInfo : SingletonComponent<ServerBrowserInfo>
{
	// Token: 0x04002185 RID: 8581
	public bool isMain;

	// Token: 0x04002186 RID: 8582
	public Text serverName;

	// Token: 0x04002187 RID: 8583
	public Text serverMeta;

	// Token: 0x04002188 RID: 8584
	public Text serverText;

	// Token: 0x04002189 RID: 8585
	public RawImage headerImage;

	// Token: 0x0400218A RID: 8586
	public Button viewWebpage;

	// Token: 0x0400218B RID: 8587
	public Button refresh;

	// Token: 0x0400218C RID: 8588
	public ServerList.Server currentServer;

	// Token: 0x0400218D RID: 8589
	public Texture defaultServerImage;

	// Token: 0x0400218E RID: 8590
	private string weburl;

	// Token: 0x0400218F RID: 8591
	private Texture loadedTexture;

	// Token: 0x04002190 RID: 8592
	private string descriptionText;

	// Token: 0x06002589 RID: 9609 RVA: 0x0001D5B9 File Offset: 0x0001B7B9
	public override void Setup()
	{
		if (this.isMain)
		{
			base.Setup();
		}
	}

	// Token: 0x0600258A RID: 9610 RVA: 0x0001D5C9 File Offset: 0x0001B7C9
	public void Open(ServerList.Server server)
	{
		base.gameObject.SetActive(true);
		this.currentServer = server;
		this.Refresh();
	}

	// Token: 0x0600258B RID: 9611 RVA: 0x00010E9A File Offset: 0x0000F09A
	public void Close()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x000C5B84 File Offset: 0x000C3D84
	public void JoinServer()
	{
		this.currentServer.AddToHistory();
		ConnectionScreen.UpdateServer(this.currentServer);
		ConnectionScreen.SetStatus("connecting");
		ConnectionScreen.Show();
		ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet(), "client.disconnect", new object[]
		{
			"Joining another server"
		});
		this.Close();
		base.Invoke(new Action(this.JoinDelayed), 0.2f);
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x000C5BFC File Offset: 0x000C3DFC
	public void JoinDelayed()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "client.connect", new object[]
		{
			this.currentServer.Address.ToString() + ":" + this.currentServer.ConnectionPort
		});
	}

	// Token: 0x0600258E RID: 9614 RVA: 0x000C5C4C File Offset: 0x000C3E4C
	public void Refresh()
	{
		if (this.currentServer == null)
		{
			return;
		}
		this.serverName.text = StringExtensions.Truncate(this.currentServer.Name, 32, null);
		if (Global.streamermode)
		{
			this.serverName.text = "STREAMER MODE ACTIVE";
		}
		this.serverMeta.text = string.Format("{0} - {1}/{2} - {3}ms", new object[]
		{
			this.currentServer.Map,
			this.currentServer.Players,
			this.currentServer.MaxPlayers,
			this.currentServer.Ping
		});
		this.serverText.text = "";
		this.descriptionText = "";
		this.headerImage.texture = this.defaultServerImage;
		this.viewWebpage.gameObject.SetActive(false);
		if (this.refresh)
		{
			this.refresh.interactable = false;
			base.Invoke(new Action(this.EnableRefreshButton), 2f);
		}
		this.currentServer.OnReceivedRules = new Action<bool>(this.OnServerRules);
		this.currentServer.FetchRules();
	}

	// Token: 0x0600258F RID: 9615 RVA: 0x000C5D88 File Offset: 0x000C3F88
	private void OnServerRules(bool Success)
	{
		if (!Success)
		{
			return;
		}
		if (this.currentServer == null)
		{
			return;
		}
		if (this.currentServer.Rules == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> keyValuePair in this.currentServer.Rules)
		{
			if (keyValuePair.Key.StartsWith("description"))
			{
				this.descriptionText += keyValuePair.Value;
				this.serverText.text = this.descriptionText.Replace("\\n", "\n").Replace("\\t", "\t");
				if (Global.streamermode)
				{
					this.serverText.text = "Streamer mode is active. In steamer mode we hide the server description and header images so that if you're streaming people won't be able to easily see which server you're on.";
				}
			}
			if (keyValuePair.Key == "headerimage" && (keyValuePair.Value.StartsWith("http://") || keyValuePair.Value.StartsWith("https://")))
			{
				if (Global.streamermode)
				{
					break;
				}
				base.StopAllCoroutines();
				if (base.gameObject.activeInHierarchy)
				{
					base.StartCoroutine(this.LoadTextureFromWWW(this.headerImage, keyValuePair.Value));
				}
			}
			if (keyValuePair.Key == "url" && (keyValuePair.Value.StartsWith("http://") || keyValuePair.Value.StartsWith("https://")))
			{
				this.viewWebpage.gameObject.SetActive(true);
				this.weburl = keyValuePair.Value;
			}
		}
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x0001D5E4 File Offset: 0x0001B7E4
	private void EnableRefreshButton()
	{
		this.refresh.interactable = true;
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x0001D5F2 File Offset: 0x0001B7F2
	public void VisitWebpage()
	{
		Application.OpenURL(this.weburl);
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x0001D5FF File Offset: 0x0001B7FF
	private IEnumerator LoadTextureFromWWW(RawImage c, string p)
	{
		if (this.loadedTexture != null)
		{
			Object.Destroy(this.loadedTexture);
			this.loadedTexture = null;
		}
		c.texture = this.defaultServerImage;
		WWW www = new WWW(p);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			www.Dispose();
			yield break;
		}
		Texture2D texture = www.texture;
		if (texture == null || c == null)
		{
			www.Dispose();
			yield break;
		}
		if (texture.width != 512 && texture.height != 256)
		{
			Object.Destroy(texture);
			yield break;
		}
		this.loadedTexture = texture;
		c.texture = texture;
		www.Dispose();
		yield break;
	}
}
