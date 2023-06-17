using System;
using ProtoBuf;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006C4 RID: 1732
public class TeamMemberElement : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	// Token: 0x04002295 RID: 8853
	public Text nameText;

	// Token: 0x04002296 RID: 8854
	public RawImage icon;

	// Token: 0x04002297 RID: 8855
	public Color onlineColor;

	// Token: 0x04002298 RID: 8856
	public Color offlineColor;

	// Token: 0x04002299 RID: 8857
	public Color deadColor;

	// Token: 0x0400229A RID: 8858
	public GameObject hoverOverlay;

	// Token: 0x0400229B RID: 8859
	public RawImage memberIcon;

	// Token: 0x0400229C RID: 8860
	public RawImage leaderIcon;

	// Token: 0x0400229D RID: 8861
	public RawImage deadIcon;

	// Token: 0x0400229E RID: 8862
	public int teamIndex;

	// Token: 0x0600266E RID: 9838 RVA: 0x000CA458 File Offset: 0x000C8658
	public void UpdateState(string name, bool isOnline, bool isAlive = true, bool isLeader = false)
	{
		this.nameText.text = name;
		Color color = isAlive ? (isOnline ? this.onlineColor : this.offlineColor) : this.deadColor;
		this.nameText.color = color;
		this.memberIcon.color = color;
		this.leaderIcon.color = color;
		this.deadIcon.color = color;
		this.leaderIcon.gameObject.SetActive(isLeader);
		this.memberIcon.gameObject.SetActive(!isLeader && isAlive);
		this.deadIcon.gameObject.SetActive(!isLeader && !isAlive);
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnEnable()
	{
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x0001DEFB File Offset: 0x0001C0FB
	public void OnDisable()
	{
		this.SetOverlayVisible(false);
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x0001DF04 File Offset: 0x0001C104
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.SetOverlayVisible(true);
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x0001DF1B File Offset: 0x0001C11B
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.SetOverlayVisible(false);
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x0001DF32 File Offset: 0x0001C132
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.hoverOverlay.gameObject.activeSelf)
		{
			this.OverlayClick();
		}
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000CA504 File Offset: 0x000C8704
	public void OverlayClick()
	{
		PlayerTeam clientTeam = LocalPlayer.Entity.clientTeam;
		if (clientTeam == null)
		{
			return;
		}
		if (this.teamIndex >= clientTeam.members.Count)
		{
			return;
		}
		PlayerTeam.TeamMember teamMember = clientTeam.members[this.teamIndex];
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "kickmember", new object[]
		{
			teamMember.userID
		});
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000CA56C File Offset: 0x000C876C
	public void SetOverlayVisible(bool wantsVisible)
	{
		if (wantsVisible)
		{
			if (LocalPlayer.Entity.clientTeam == null)
			{
				return;
			}
			if (LocalPlayer.Entity.clientTeam.teamLeader != LocalPlayer.Entity.userID)
			{
				return;
			}
			if (this.teamIndex >= LocalPlayer.Entity.clientTeam.members.Count)
			{
				return;
			}
			if (LocalPlayer.Entity.clientTeam.members[this.teamIndex].userID == LocalPlayer.Entity.clientTeam.teamLeader)
			{
				return;
			}
		}
		this.hoverOverlay.gameObject.SetActive(wantsVisible);
	}
}
