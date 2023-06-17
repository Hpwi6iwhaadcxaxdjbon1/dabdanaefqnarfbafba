using System;
using Facepunch.Steamworks;
using UnityEngine;

// Token: 0x020006A5 RID: 1701
public class YourFriends : BaseMonoBehaviour
{
	// Token: 0x040021E9 RID: 8681
	public Transform PanelList;

	// Token: 0x040021EA RID: 8682
	public MenuFriendPanel[] FriendPanels;

	// Token: 0x060025EE RID: 9710 RVA: 0x0001D97C File Offset: 0x0001BB7C
	private void OnEnable()
	{
		this.FriendPanels = this.PanelList.GetComponentsInChildren<MenuFriendPanel>(true);
		this.ClearPanels();
		base.InvokeRepeating(new Action(this.Init), 0.1f, 30f);
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000C74C4 File Offset: 0x000C56C4
	private void Init()
	{
		this.ClearPanels();
		if (Client.Steam == null)
		{
			return;
		}
		foreach (SteamFriend steamFriend in Client.Steam.Friends.AllFriends)
		{
			if (steamFriend.IsPlayingThisGame)
			{
				MenuFriendPanel menuFriendPanel = null;
				foreach (MenuFriendPanel menuFriendPanel2 in this.FriendPanels)
				{
					if (!menuFriendPanel2.gameObject.activeSelf)
					{
						menuFriendPanel = menuFriendPanel2;
						break;
					}
				}
				if (menuFriendPanel == null)
				{
					break;
				}
				using (TimeWarning.New("Setup Friends Panel", 0.1f))
				{
					menuFriendPanel.friend = steamFriend;
					menuFriendPanel.friendName.text = steamFriend.Name;
					menuFriendPanel.friendAvatar.texture = SingletonComponent<SteamClient>.Instance.GetAvatarTexture(steamFriend.Id);
					using (TimeWarning.New("Activate Friends Panel", 0.1f))
					{
						menuFriendPanel.gameObject.SetActive(true);
					}
				}
			}
		}
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x000C7608 File Offset: 0x000C5808
	private void ClearPanels()
	{
		using (TimeWarning.New("ClearPanels", 0.1f))
		{
			foreach (MenuFriendPanel menuFriendPanel in this.FriendPanels)
			{
				if (menuFriendPanel.gameObject.activeSelf)
				{
					menuFriendPanel.gameObject.SetActive(false);
				}
			}
		}
	}
}
