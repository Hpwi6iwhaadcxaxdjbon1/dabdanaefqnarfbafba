using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class BaseMountable : BaseCombatEntity
{
	// Token: 0x0400010B RID: 267
	private Option __menuOption_Menu_Mount;

	// Token: 0x0400010C RID: 268
	protected BasePlayer _mounted;

	// Token: 0x0400010D RID: 269
	[Header("View")]
	public Transform eyeOverride;

	// Token: 0x0400010E RID: 270
	public Vector2 pitchClamp = new Vector2(-80f, 50f);

	// Token: 0x0400010F RID: 271
	public Vector2 yawClamp = new Vector2(-80f, 80f);

	// Token: 0x04000110 RID: 272
	public bool canWieldItems = true;

	// Token: 0x04000111 RID: 273
	[Header("Mounting")]
	public PlayerModel.MountPoses mountPose;

	// Token: 0x04000112 RID: 274
	public float maxMountDistance = 1.5f;

	// Token: 0x04000113 RID: 275
	public Transform mountAnchor;

	// Token: 0x04000114 RID: 276
	public Transform dismountAnchor;

	// Token: 0x04000115 RID: 277
	public Transform[] dismountPositions;

	// Token: 0x04000116 RID: 278
	public Transform dismountCheckEyes;

	// Token: 0x04000117 RID: 279
	public SoundDefinition mountSoundDef;

	// Token: 0x04000118 RID: 280
	public SoundDefinition dismountSoundDef;

	// Token: 0x04000119 RID: 281
	public bool isMobile;

	// Token: 0x060002A7 RID: 679 RVA: 0x00032A3C File Offset: 0x00030C3C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("BaseMountable.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Mount", 0.1f))
			{
				if (this.Menu_Mount_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Mount.show = true;
					this.__menuOption_Menu_Mount.showDisabled = false;
					this.__menuOption_Menu_Mount.longUseOnly = false;
					this.__menuOption_Menu_Mount.order = 0;
					this.__menuOption_Menu_Mount.icon = "drop";
					this.__menuOption_Menu_Mount.desc = "mount_desc";
					this.__menuOption_Menu_Mount.title = "mount";
					if (this.__menuOption_Menu_Mount.function == null)
					{
						this.__menuOption_Menu_Mount.function = new Action<BasePlayer>(this.Menu_Mount);
					}
					list.Add(this.__menuOption_Menu_Mount);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060002A8 RID: 680 RVA: 0x00004969 File Offset: 0x00002B69
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Mount_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00032B44 File Offset: 0x00030D44
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseMountable.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00004980 File Offset: 0x00002B80
	public virtual bool CanHoldItems()
	{
		return this.canWieldItems;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool DirectlyMountable()
	{
		return true;
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00004988 File Offset: 0x00002B88
	public virtual Vector3 EyePositionForPlayer(BasePlayer player)
	{
		if (player.GetMounted() == this)
		{
			return this.eyeOverride.transform.position;
		}
		return Vector3.zero;
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00032B88 File Offset: 0x00030D88
	public virtual float WaterFactorForPlayer(BasePlayer player)
	{
		return WaterLevel.Factor(player.WorldSpaceBounds().ToBounds());
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00032BA8 File Offset: 0x00030DA8
	public override float MaxVelocity()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			return parentEntity.MaxVelocity();
		}
		return base.MaxVelocity();
	}

	// Token: 0x060002AF RID: 687 RVA: 0x000049AE File Offset: 0x00002BAE
	public BaseVehicle VehicleParent()
	{
		return base.GetParentEntity() as BaseVehicle;
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00032BD4 File Offset: 0x00030DD4
	public virtual void UpdatePlayerModel(BasePlayer player)
	{
		player.playerModel.leftHandTargetPosition = Vector3.zero;
		player.modelState.poseType = (int)this.mountPose;
		player.modelState.mounted = true;
		player.modelState.flying = false;
		player.modelState.onground = true;
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x000049BB File Offset: 0x00002BBB
	public virtual void UpdatePlayerRotation(BasePlayer player)
	{
		player.transform.localRotation = this.GetMountedRotation();
		player.playerModel.mountedRotation = this.GetMountedRotation();
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x000049DF File Offset: 0x00002BDF
	public override void SetNetworkPosition(Vector3 vPos)
	{
		base.SetNetworkPosition(vPos);
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000049E8 File Offset: 0x00002BE8
	public void UpdatePlayerPosition(BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (player.GetMounted() == this)
		{
			player.transform.position = this.GetMountedPosition();
		}
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00004A13 File Offset: 0x00002C13
	public Quaternion GetMountedRotation()
	{
		return Quaternion.LookRotation(base.transform.forward, base.transform.up);
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00004A30 File Offset: 0x00002C30
	public Vector3 GetMountedPosition()
	{
		return this.mountAnchor.transform.position;
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00032C28 File Offset: 0x00030E28
	public void PlayerMounted(BasePlayer player)
	{
		if (player.movement)
		{
			player.movement.TeleportTo(this.mountAnchor.transform.position, player);
		}
		if (player.IsLocalPlayer())
		{
			player.input.SetViewVars(Quaternion.LookRotation(base.transform.forward, base.transform.up).eulerAngles);
			player.input.ApplyViewAngles();
		}
		player.eyes.rotation = Quaternion.LookRotation(base.transform.forward);
		if (player.model != null && this.mountSoundDef != null)
		{
			SoundManager.PlayOneshot(this.mountSoundDef, player.model.gameObject, false, default(Vector3));
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00032CF8 File Offset: 0x00030EF8
	public void PlayerDismounted(BasePlayer player)
	{
		if (player.IsLocalPlayer())
		{
			Vector3 eulerAngles = Quaternion.LookRotation(player.eyes.BodyForward(), Vector3.up).eulerAngles;
			eulerAngles.x = 0f;
			player.input.SetViewVars(eulerAngles);
			player.playerModel.leftHandTargetPosition = Vector3.zero;
			player.playerModel.leftHandTargetRotation = Quaternion.identity;
		}
		if (player.model != null && this.dismountSoundDef != null)
		{
			SoundManager.PlayOneshot(this.dismountSoundDef, player.model.gameObject, false, default(Vector3));
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x00004A42 File Offset: 0x00002C42
	public virtual void OverrideViewAngles(BasePlayer player)
	{
		player.input.ApplyViewAngles();
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x00032DA0 File Offset: 0x00030FA0
	public virtual void DoPlayerMovement(BasePlayer player, InputState input, ModelState modelState)
	{
		modelState.poseType = (int)this.mountPose;
		modelState.mounted = true;
		modelState.flying = false;
		modelState.onground = true;
		this.UpdatePlayerPosition(player);
		if (input != null && input.WasJustPressed(BUTTON.JUMP))
		{
			base.ServerRPC("RPC_WantsDismount");
		}
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00004A4F File Offset: 0x00002C4F
	[BaseEntity.Menu.ShowIf("Menu_Mount_ShowIf")]
	[BaseEntity.Menu.Icon("drop")]
	[BaseEntity.Menu("mount", "Mount")]
	[BaseEntity.Menu.Description("mount_desc", "Mount this Object")]
	public void Menu_Mount(BasePlayer player)
	{
		base.ServerRPC("RPC_WantsMount");
	}

	// Token: 0x060002BB RID: 699 RVA: 0x00004A5C File Offset: 0x00002C5C
	public bool Menu_Mount_ShowIf(BasePlayer player)
	{
		return this.NearMountPoint(player) && !base.HasFlag(BaseEntity.Flags.Busy) && this.DirectlyMountable() && !player.isMounted && this.MountMenuVisible();
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool MountMenuVisible()
	{
		return true;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00032DF0 File Offset: 0x00030FF0
	public bool NearMountPoint(BasePlayer player)
	{
		if (Vector3.Distance(player.transform.position, this.mountAnchor.position) <= this.maxMountDistance)
		{
			RaycastHit hit;
			if (!Physics.SphereCast(player.eyes.HeadRay(), 0.25f, ref hit, 2f, 1218652417))
			{
				return false;
			}
			if (hit.GetEntity() && hit.GetEntity().net.ID == this.net.ID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00032E74 File Offset: 0x00031074
	public static Vector3 ConvertVector(Vector3 vec)
	{
		for (int i = 0; i < 3; i++)
		{
			if (vec[i] > 180f)
			{
				ref Vector3 ptr = ref vec;
				int index = i;
				ptr[index] -= 360f;
			}
			else if (vec[i] < -180f)
			{
				ref Vector3 ptr = ref vec;
				int index = i;
				ptr[index] += 360f;
			}
		}
		return vec;
	}
}
