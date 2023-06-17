using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020005F8 RID: 1528
public class OpenSteamURL : MonoBehaviour
{
	// Token: 0x04001EBA RID: 7866
	public bool openInSteam = true;

	// Token: 0x0600224D RID: 8781 RVA: 0x0001B3F9 File Offset: 0x000195F9
	public void OpenURL(string url)
	{
		Debug.Log("Opening " + url);
		if (this.openInSteam)
		{
			Client.Steam.Overlay.OpenUrl(url);
			return;
		}
		Process.Start(url);
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000B80DC File Offset: 0x000B62DC
	public void ActivateGameOverlay(string url)
	{
		if (url == "inventory")
		{
			this.OpenURL("http://steamcommunity.com/profiles/" + Client.Steam.SteamId + "/inventory#252490");
			return;
		}
		Debug.Log("Opening " + url);
		Client.Steam.Overlay.OpenUserPage(url, Client.Steam.SteamId);
	}
}
