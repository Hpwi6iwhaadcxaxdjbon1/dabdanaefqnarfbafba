using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class WheelSwitch : IOEntity
{
	// Token: 0x04000328 RID: 808
	private Option __menuOption_Menu_Turn;

	// Token: 0x04000329 RID: 809
	public Transform wheelObj;

	// Token: 0x0400032A RID: 810
	public float rotateSpeed = 90f;

	// Token: 0x0400032B RID: 811
	public BaseEntity.Flags BeingRotated = BaseEntity.Flags.Reserved1;

	// Token: 0x0400032C RID: 812
	public BaseEntity.Flags RotatingLeft = BaseEntity.Flags.Reserved2;

	// Token: 0x0400032D RID: 813
	public BaseEntity.Flags RotatingRight = BaseEntity.Flags.Reserved3;

	// Token: 0x0400032E RID: 814
	public float rotateProgress;

	// Token: 0x0400032F RID: 815
	public Animator animator;

	// Token: 0x04000330 RID: 816
	public float kineticEnergyPerSec = 1f;

	// Token: 0x04000331 RID: 817
	private BasePlayer rotatorPlayer;

	// Token: 0x04000332 RID: 818
	private float animProgress;

	// Token: 0x0600061B RID: 1563 RVA: 0x00042900 File Offset: 0x00040B00
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("WheelSwitch.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Turn", 0.1f))
			{
				if (this.Menu_Turn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Turn.show = true;
					this.__menuOption_Menu_Turn.showDisabled = false;
					this.__menuOption_Menu_Turn.longUseOnly = true;
					this.__menuOption_Menu_Turn.order = 0;
					this.__menuOption_Menu_Turn.icon = "rotate";
					this.__menuOption_Menu_Turn.desc = "turn_switch_desc";
					this.__menuOption_Menu_Turn.title = "turn";
					if (this.__menuOption_Menu_Turn.function == null)
					{
						this.__menuOption_Menu_Turn.function = new Action<BasePlayer>(this.Menu_Turn);
					}
					list.Add(this.__menuOption_Menu_Turn);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x0600061C RID: 1564 RVA: 0x000075C1 File Offset: 0x000057C1
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Turn_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00042A08 File Offset: 0x00040C08
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WheelSwitch.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x000075D8 File Offset: 0x000057D8
	public override void OnUseStopped(BasePlayer player)
	{
		base.OnUseStopped(player);
		base.ServerRPC("CancelRotate");
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x000075EC File Offset: 0x000057EC
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00042A4C File Offset: 0x00040C4C
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		this.animProgress = Mathf.Lerp(this.animProgress, this.rotateProgress, Time.deltaTime * 5f);
		this.animator.SetFloat("turnAmount", this.animProgress % 1f);
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x000075F4 File Offset: 0x000057F4
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu.Description("turn_switch_desc", "Turn")]
	[BaseEntity.Menu.ShowIf("Menu_Turn_ShowIf")]
	[BaseEntity.Menu("turn", "Turn", LongUseOnly = true)]
	public void Menu_Turn(BasePlayer player)
	{
		base.ServerRPC("BeginRotate");
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x00007601 File Offset: 0x00005801
	public bool Menu_Turn_ShowIf(BasePlayer player)
	{
		return !this.IsBeingRotated();
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x0000760C File Offset: 0x0000580C
	public bool IsBeingRotated()
	{
		return base.HasFlag(this.BeingRotated);
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x0000761A File Offset: 0x0000581A
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity == null)
		{
			return;
		}
		this.rotateProgress = info.msg.sphereEntity.radius;
	}
}
