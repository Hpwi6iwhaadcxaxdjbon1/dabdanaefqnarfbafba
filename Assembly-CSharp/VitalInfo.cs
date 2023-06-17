using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006F1 RID: 1777
public class VitalInfo : MonoBehaviour, IClientComponent
{
	// Token: 0x04002314 RID: 8980
	public VitalInfo.Vital VitalType;

	// Token: 0x04002315 RID: 8981
	public Animator animator;

	// Token: 0x04002316 RID: 8982
	public Text text;

	// Token: 0x04002317 RID: 8983
	private bool show = true;

	// Token: 0x06002734 RID: 10036 RVA: 0x000CC040 File Offset: 0x000CA240
	private void Update()
	{
		if (LocalPlayer.Entity == null)
		{
			this.Hide();
			return;
		}
		switch (this.VitalType)
		{
		case VitalInfo.Vital.BuildingBlocked:
			if ((LocalPlayer.Entity.GetHeldEntity() is Planner || LocalPlayer.Entity.GetHeldEntity() is Deployer || LocalPlayer.Entity.GetHeldEntity() is Hammer) && LocalPlayer.Entity.IsBuildingBlocked())
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		case VitalInfo.Vital.CanBuild:
			if (LocalPlayer.Entity.IsBuildingAuthed())
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		case VitalInfo.Vital.Crafting:
			if (CraftingQueue.isCrafting)
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		case VitalInfo.Vital.CraftLevel1:
			if (LocalPlayer.Entity.HasPlayerFlag(BasePlayer.PlayerFlags.Workbench1))
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		case VitalInfo.Vital.CraftLevel2:
			if (LocalPlayer.Entity.HasPlayerFlag(BasePlayer.PlayerFlags.Workbench2))
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		case VitalInfo.Vital.CraftLevel3:
			if (LocalPlayer.Entity.HasPlayerFlag(BasePlayer.PlayerFlags.Workbench3))
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		case VitalInfo.Vital.DecayProtected:
		{
			if (!LocalPlayer.Entity.IsBuildingAuthed())
			{
				this.Hide();
				return;
			}
			float protectedMinutes = LocalPlayer.Entity.GetBuildingPrivilege().GetProtectedMinutes(false);
			int num = Mathf.FloorToInt(protectedMinutes / 60f);
			int num2 = Mathf.FloorToInt(protectedMinutes - (float)num * 60f);
			if (protectedMinutes <= 0f)
			{
				this.Hide();
				return;
			}
			if (num >= 72)
			{
				this.text.text = "> 72 hrs";
			}
			else if (num >= 12)
			{
				this.text.text = num.ToString("N0") + " hrs";
			}
			else
			{
				this.text.text = num.ToString("N0") + "h" + num2.ToString("N0") + "m";
			}
			this.Show();
			return;
		}
		case VitalInfo.Vital.Decaying:
			if (!LocalPlayer.Entity.IsBuildingAuthed())
			{
				this.Hide();
				return;
			}
			if (LocalPlayer.Entity.GetBuildingPrivilege().GetProtectedMinutes(false) <= 0f)
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		case VitalInfo.Vital.SafeZone:
			if (LocalPlayer.Entity.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone))
			{
				this.Show();
				return;
			}
			this.Hide();
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x0001E8E7 File Offset: 0x0001CAE7
	private void Hide()
	{
		if (!this.show)
		{
			return;
		}
		this.show = false;
		this.animator.SetBool("show", false);
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x0001E90A File Offset: 0x0001CB0A
	private void Show()
	{
		if (this.show)
		{
			return;
		}
		this.show = true;
		this.animator.SetBool("show", true);
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x0001E92D File Offset: 0x0001CB2D
	private void ClientConnected()
	{
		this.Hide();
	}

	// Token: 0x020006F2 RID: 1778
	public enum Vital
	{
		// Token: 0x04002319 RID: 8985
		BuildingBlocked,
		// Token: 0x0400231A RID: 8986
		CanBuild,
		// Token: 0x0400231B RID: 8987
		Crafting,
		// Token: 0x0400231C RID: 8988
		CraftLevel1,
		// Token: 0x0400231D RID: 8989
		CraftLevel2,
		// Token: 0x0400231E RID: 8990
		CraftLevel3,
		// Token: 0x0400231F RID: 8991
		DecayProtected,
		// Token: 0x04002320 RID: 8992
		Decaying,
		// Token: 0x04002321 RID: 8993
		SafeZone
	}
}
