using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020006BA RID: 1722
public class SteamFriendsList : MonoBehaviour
{
	// Token: 0x0400227E RID: 8830
	public RectTransform targetPanel;

	// Token: 0x0400227F RID: 8831
	public SteamUserButton userButton;

	// Token: 0x04002280 RID: 8832
	public bool IncludeFriendsList = true;

	// Token: 0x04002281 RID: 8833
	public bool IncludeRecentlySeen;

	// Token: 0x04002282 RID: 8834
	public SteamFriendsList.onFriendSelectedEvent onFriendSelected;

	// Token: 0x04002283 RID: 8835
	private List<SteamFriend> playerList = new List<SteamFriend>();

	// Token: 0x04002284 RID: 8836
	internal static List<ulong> SeenList = new List<ulong>();

	// Token: 0x06002651 RID: 9809 RVA: 0x0001DE35 File Offset: 0x0001C035
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000C9F4C File Offset: 0x000C814C
	public void Refresh()
	{
		if (Client.Steam == null)
		{
			return;
		}
		Client.Steam.Friends.Refresh();
		if (this.IncludeFriendsList)
		{
			foreach (SteamFriend f in Client.Steam.Friends.All)
			{
				this.AddPlayer(f);
			}
		}
		if (this.IncludeRecentlySeen)
		{
			foreach (ulong num in SteamFriendsList.SeenList)
			{
				this.AddPlayer(Client.Steam.Friends.Get(num));
			}
		}
		this.FilterSearch("");
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000CA024 File Offset: 0x000C8224
	public void AddPlayer(SteamFriend f)
	{
		if (Enumerable.Any<SteamFriend>(this.playerList, (SteamFriend x) => x.Id == f.Id))
		{
			return;
		}
		this.playerList.Add(f);
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000CA06C File Offset: 0x000C826C
	public void FilterSearch(string name)
	{
		this.targetPanel.DestroyChildren();
		this.AddToPanel(Enumerable.Where<SteamFriend>(this.playerList, (SteamFriend x) => StringEx.Contains(x.Name, name, 1)), 40);
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000CA0B0 File Offset: 0x000C82B0
	public void AddToPanel(IEnumerable<SteamFriend> list, int count = -1)
	{
		IEnumerable<SteamFriend> enumerable = Enumerable.ThenBy<SteamFriend, string>(Enumerable.ThenBy<SteamFriend, bool>(Enumerable.OrderBy<SteamFriend, bool>(list, (SteamFriend x) => x.IsPlayingThisGame), (SteamFriend x) => x.IsOnline), (SteamFriend x) => x.Name);
		int num = 0;
		foreach (SteamFriend steamFriend in enumerable)
		{
			if (count >= 0 && num >= count)
			{
				break;
			}
			GameObject gameObject = Facepunch.Instantiate.GameObject(this.userButton.gameObject, null);
			gameObject.transform.SetParent(this.targetPanel, false);
			gameObject.GetComponent<SteamUserButton>().UpdateFromUser(steamFriend.Id, steamFriend.IsPlayingThisGame, steamFriend.IsOnline);
			ulong steamid = steamFriend.Id;
			gameObject.GetComponent<Button>().onClick.AddListener(delegate()
			{
				this.FriendSelected(steamid);
			});
			num++;
		}
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x0001DE3D File Offset: 0x0001C03D
	private void FriendSelected(ulong steamid)
	{
		this.onFriendSelected.Invoke(steamid);
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000CA1EC File Offset: 0x000C83EC
	public static void JustSeen(ulong steamid)
	{
		if (steamid == SteamClient.localSteamID)
		{
			return;
		}
		if (Client.Steam != null)
		{
			Client.Steam.Friends.UpdateInformation(steamid);
		}
		SteamFriendsList.SeenList.Remove(steamid);
		SteamFriendsList.SeenList.Add(steamid);
		while (SteamFriendsList.SeenList.Count > 512)
		{
			SteamFriendsList.SeenList.RemoveAt(0);
		}
	}

	// Token: 0x020006BB RID: 1723
	[Serializable]
	public class onFriendSelectedEvent : UnityEvent<ulong>
	{
	}
}
