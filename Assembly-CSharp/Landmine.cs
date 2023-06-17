using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class Landmine : BaseTrap
{
	// Token: 0x0400021B RID: 539
	private Option __menuOption_Arm_Landmine;

	// Token: 0x0400021C RID: 540
	public GameObjectRef explosionEffect;

	// Token: 0x0400021D RID: 541
	public GameObjectRef triggeredEffect;

	// Token: 0x0400021E RID: 542
	public float minExplosionRadius;

	// Token: 0x0400021F RID: 543
	public float explosionRadius;

	// Token: 0x04000220 RID: 544
	public bool blocked;

	// Token: 0x04000221 RID: 545
	private ulong triggerPlayerID;

	// Token: 0x04000222 RID: 546
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x0600049E RID: 1182 RVA: 0x0003C44C File Offset: 0x0003A64C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Landmine.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Arm_Landmine", 0.1f))
			{
				if (this.Menu_Disarm_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Arm_Landmine.show = true;
					this.__menuOption_Arm_Landmine.showDisabled = false;
					this.__menuOption_Arm_Landmine.longUseOnly = false;
					this.__menuOption_Arm_Landmine.order = 0;
					this.__menuOption_Arm_Landmine.icon = "rotate";
					this.__menuOption_Arm_Landmine.desc = "disarm_landmine_desc";
					this.__menuOption_Arm_Landmine.title = "disarm_landmine";
					if (this.__menuOption_Arm_Landmine.function == null)
					{
						this.__menuOption_Arm_Landmine.function = new Action<BasePlayer>(this.Arm_Landmine);
					}
					list.Add(this.__menuOption_Arm_Landmine);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x0600049F RID: 1183 RVA: 0x000061CA File Offset: 0x000043CA
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Disarm_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0003C554 File Offset: 0x0003A754
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Landmine.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x000061E1 File Offset: 0x000043E1
	public bool Triggered()
	{
		return base.HasFlag(BaseEntity.Flags.Open);
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool Armed()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0003C598 File Offset: 0x0003A798
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		bool flag = this.Triggered();
		base.Load(info);
		if (!info.fromDisk && info.msg.landmine != null)
		{
			this.triggerPlayerID = info.msg.landmine.triggeredID;
		}
		if (base.isClient && this.Triggered() && !flag)
		{
			Effect.client.Run(this.triggeredEffect.resourcePath, base.transform.position, default(Vector3), default(Vector3));
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x000061EA File Offset: 0x000043EA
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu.Description("disarm_landmine_desc", "Try and disarm the landmine")]
	[BaseEntity.Menu("disarm_landmine", "Disarm")]
	[BaseEntity.Menu.ShowIf("Menu_Disarm_ShowIf")]
	public void Arm_Landmine(BasePlayer player)
	{
		base.ServerRPC("RPC_Disarm");
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x000061F7 File Offset: 0x000043F7
	public bool Menu_Disarm_ShowIf(BasePlayer player)
	{
		return this.Armed() && this.Triggered() && player.userID != this.triggerPlayerID;
	}
}
