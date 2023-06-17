using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class RHIB : MotorRowboat
{
	// Token: 0x040002AF RID: 687
	private Option __menuOption_Release;

	// Token: 0x040002B0 RID: 688
	public GameObject steeringWheel;

	// Token: 0x040002B1 RID: 689
	[ServerVar(Help = "Population active on the server")]
	public static float rhibpopulation = 1f;

	// Token: 0x06000566 RID: 1382 RVA: 0x0003F2F4 File Offset: 0x0003D4F4
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("RHIB.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Release", 0.1f))
			{
				if (this.Release_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Release.show = true;
					this.__menuOption_Release.showDisabled = false;
					this.__menuOption_Release.longUseOnly = false;
					this.__menuOption_Release.order = -200;
					this.__menuOption_Release.icon = "close";
					this.__menuOption_Release.desc = "release_desc";
					this.__menuOption_Release.title = "release";
					if (this.__menuOption_Release.function == null)
					{
						this.__menuOption_Release.function = new Action<BasePlayer>(this.Release);
					}
					list.Add(this.__menuOption_Release);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000567 RID: 1383 RVA: 0x00006C80 File Offset: 0x00004E80
	public override bool HasMenuOptions
	{
		get
		{
			return this.Release_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0003F400 File Offset: 0x0003D600
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RHIB.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x00006C97 File Offset: 0x00004E97
	[BaseEntity.Menu.ShowIf("Release_ShowIf")]
	[BaseEntity.Menu("release", "Release", Order = -200)]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.Description("release_desc", "Release the boat")]
	public void Release(BasePlayer player)
	{
		base.ServerRPC("Server_Release");
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x00006CA4 File Offset: 0x00004EA4
	public bool Release_ShowIf(BasePlayer player)
	{
		return this.LookingAtEngine(player) && player.isMounted && base.GetParentEntity() && base.GetParentEntity().GetComponent<CargoShip>() != null;
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0000647F File Offset: 0x0000467F
	public override bool LookingAtEngine(BasePlayer player)
	{
		return !(player == null) && !(player.lookingAtCollider == null) && player.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x000064AA File Offset: 0x000046AA
	public override bool LookingAtFuelArea(BasePlayer player)
	{
		return this.LookingAtEngine(player);
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00006CDA File Offset: 0x00004EDA
	public new void Update()
	{
		base.Update();
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0003F444 File Offset: 0x0003D644
	public override void UpdateEngineRotation()
	{
		float num = 0f;
		if (base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			num -= 90f;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved4))
		{
			num = 90f;
		}
		this.currentEngineRotation = Mathf.Lerp(this.currentEngineRotation, num, Time.deltaTime * 3f);
		if (this.engineRotate != null)
		{
			this.engineRotate.transform.localRotation = Quaternion.Euler(0f, this.currentEngineRotation, 0f);
		}
		float b = base.HasFlag(BaseEntity.Flags.Reserved2) ? 1f : 0f;
		this.propellerRotationSpeed = Mathf.Lerp(this.propellerRotationSpeed, b, Time.deltaTime * 3f);
		if (this.propellerRotate != null)
		{
			this.propellerRotate.Rotate(Vector3.forward, this.propellerRotationSpeed * -6000f * Time.deltaTime);
		}
	}
}
