using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C5 RID: 1733
public class TeamUI : MonoBehaviour
{
	// Token: 0x0400229F RID: 8863
	public RectTransform MemberPanel;

	// Token: 0x040022A0 RID: 8864
	public GameObject memberEntryPrefab;

	// Token: 0x040022A1 RID: 8865
	public TeamMemberElement[] elements;

	// Token: 0x040022A2 RID: 8866
	public GameObject NoTeamPanel;

	// Token: 0x040022A3 RID: 8867
	public GameObject TeamPanel;

	// Token: 0x040022A4 RID: 8868
	public GameObject LeaveTeamButton;

	// Token: 0x040022A5 RID: 8869
	public GameObject InviteAcceptPanel;

	// Token: 0x040022A6 RID: 8870
	public Text inviteText;

	// Token: 0x040022A7 RID: 8871
	public static bool dirty = true;

	// Token: 0x040022A8 RID: 8872
	[NonSerialized]
	public static ulong pendingTeamID;

	// Token: 0x040022A9 RID: 8873
	[NonSerialized]
	public static string pendingTeamLeaderName;

	// Token: 0x06002677 RID: 9847 RVA: 0x000CA604 File Offset: 0x000C8804
	public void Update()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		bool flag = LocalPlayer.Entity.clientTeam != null;
		bool flag2 = UIInventory.isOpen && RelationshipManager.TeamsEnabled();
		this.TeamPanel.SetActive(flag && RelationshipManager.TeamsEnabled() && !UICrafting.isOpen);
		this.LeaveTeamButton.SetActive(flag && flag2);
		this.InviteAcceptPanel.SetActive(!flag && flag2 && TeamUI.pendingTeamID > 0UL);
		this.NoTeamPanel.SetActive(!flag && flag2 && !this.InviteAcceptPanel.activeSelf);
		if (this.InviteAcceptPanel.activeSelf)
		{
			this.inviteText.text = "You were invited to join " + TeamUI.pendingTeamLeaderName + "'s team";
		}
		if (TeamUI.dirty)
		{
			TeamMemberElement[] array = this.elements;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(false);
			}
			TeamUI.dirty = false;
		}
		if (!flag)
		{
			return;
		}
		if (!RelationshipManager.TeamsEnabled())
		{
			return;
		}
		for (int j = 0; j < LocalPlayer.Entity.clientTeam.members.Count; j++)
		{
			if (j >= this.elements.Length)
			{
				return;
			}
			this.elements[j].gameObject.SetActive(true);
			PlayerTeam.TeamMember teamMember = LocalPlayer.Entity.clientTeam.members[j];
			string name = Global.streamermode ? RandomUsernames.Get(teamMember.userID) : teamMember.displayName;
			bool isLeader = teamMember.userID == LocalPlayer.Entity.clientTeam.teamLeader;
			this.elements[j].UpdateState(name, teamMember.online, teamMember.healthFraction > 0f, isLeader);
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x0001DF4C File Offset: 0x0001C14C
	public void LeaveTeamClicked()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "leaveteam", Array.Empty<object>());
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x0001DF63 File Offset: 0x0001C163
	public void CreateTeam()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "trycreateteam", Array.Empty<object>());
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x0001DF7A File Offset: 0x0001C17A
	public void AcceptJoin()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "acceptinvite", new object[]
		{
			TeamUI.pendingTeamID
		});
		TeamUI.pendingTeamID = 0UL;
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x0001DFA6 File Offset: 0x0001C1A6
	public void RejectJoin()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "rejectinvite", new object[]
		{
			TeamUI.pendingTeamID
		});
		TeamUI.pendingTeamID = 0UL;
	}
}
