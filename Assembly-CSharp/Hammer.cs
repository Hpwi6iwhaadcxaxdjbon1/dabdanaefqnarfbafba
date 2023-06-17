using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x020002A2 RID: 674
public class Hammer : BaseMelee
{
	// Token: 0x060012ED RID: 4845 RVA: 0x0007A8D0 File Offset: 0x00078AD0
	private BuildingBlock GetBuildingBlock()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		return ownerPlayer.lookingAtEntity as BuildingBlock;
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x0007A8FC File Offset: 0x00078AFC
	public override void OnFrame()
	{
		base.OnFrame();
		BuildingBlock buildingBlock = this.GetBuildingBlock();
		if (buildingBlock != null)
		{
			buildingBlock.DrawHighlight();
		}
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x0007A928 File Offset: 0x00078B28
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY) && (ownerPlayer.CanBuild() || ownerPlayer.IsAdmin))
		{
			this.OpenContextMenu();
			return;
		}
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x0007A97C File Offset: 0x00078B7C
	private void OpenContextMenu()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		BuildingBlock buildingBlock = this.GetBuildingBlock();
		if (buildingBlock == null)
		{
			return;
		}
		List<Option> buildMenu = buildingBlock.GetBuildMenu(ownerPlayer);
		if (buildMenu.Count == 0)
		{
			return;
		}
		ContextMenuUI.Open(buildMenu, ContextMenuUI.MenuType.RightClick);
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x000102DF File Offset: 0x0000E4DF
	public override bool CanHit(HitTest info)
	{
		return !(info.HitEntity == null) && !(info.HitEntity is BasePlayer) && info.HitEntity is BaseCombatEntity;
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x0007A9C4 File Offset: 0x00078BC4
	public override void DoAttackShared(HitInfo info)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (info.HitEntity as BaseCombatEntity != null)
		{
			ownerPlayer != null;
		}
		if (base.isServer)
		{
			Effect.server.ImpactEffect(info);
			return;
		}
		Effect.client.ImpactEffect(info);
	}
}
