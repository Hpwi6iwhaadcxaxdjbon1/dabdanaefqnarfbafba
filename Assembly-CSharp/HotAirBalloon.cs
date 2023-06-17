using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class HotAirBalloon : BaseCombatEntity
{
	// Token: 0x040001EE RID: 494
	private Option __menuOption_Menu_BurnOff;

	// Token: 0x040001EF RID: 495
	private Option __menuOption_Menu_BurnOn;

	// Token: 0x040001F0 RID: 496
	private Option __menuOption_Menu_FuelStorage;

	// Token: 0x040001F1 RID: 497
	protected const BaseEntity.Flags Flag_HasFuel = BaseEntity.Flags.Reserved6;

	// Token: 0x040001F2 RID: 498
	protected const BaseEntity.Flags Flag_HalfInflated = BaseEntity.Flags.Reserved1;

	// Token: 0x040001F3 RID: 499
	protected const BaseEntity.Flags Flag_FullInflated = BaseEntity.Flags.Reserved2;

	// Token: 0x040001F4 RID: 500
	public Transform centerOfMass;

	// Token: 0x040001F5 RID: 501
	public Rigidbody myRigidbody;

	// Token: 0x040001F6 RID: 502
	public Transform buoyancyPoint;

	// Token: 0x040001F7 RID: 503
	public float liftAmount = 10f;

	// Token: 0x040001F8 RID: 504
	public Transform windSock;

	// Token: 0x040001F9 RID: 505
	public Transform[] windFlags;

	// Token: 0x040001FA RID: 506
	public GameObject staticBalloonDeflated;

	// Token: 0x040001FB RID: 507
	public GameObject staticBalloon;

	// Token: 0x040001FC RID: 508
	public GameObject animatedBalloon;

	// Token: 0x040001FD RID: 509
	public Animator balloonAnimator;

	// Token: 0x040001FE RID: 510
	public Transform groundSample;

	// Token: 0x040001FF RID: 511
	public float inflationLevel;

	// Token: 0x04000200 RID: 512
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x04000201 RID: 513
	public Transform fuelStoragePoint;

	// Token: 0x04000202 RID: 514
	public EntityRef fuelStorageInstance;

	// Token: 0x04000203 RID: 515
	public float fuelPerSec = 0.25f;

	// Token: 0x04000204 RID: 516
	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	// Token: 0x04000205 RID: 517
	public Transform storageUnitPoint;

	// Token: 0x04000206 RID: 518
	public EntityRef storageUnitInstance;

	// Token: 0x04000207 RID: 519
	public Transform engineHeight;

	// Token: 0x04000208 RID: 520
	public GameObject[] killTriggers;

	// Token: 0x04000209 RID: 521
	[ServerVar(Help = "Population active on the server")]
	public static float population = 1f;

	// Token: 0x0400020A RID: 522
	[ServerVar(Help = "How long before a HAB is killed while outside")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x0400020B RID: 523
	private float currentClientInflationLevel;

	// Token: 0x0400020C RID: 524
	private Vector3 windSockVec = Vector3.forward;

	// Token: 0x06000476 RID: 1142 RVA: 0x0003B964 File Offset: 0x00039B64
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("HotAirBalloon.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_BurnOff", 0.1f))
			{
				if (this.Menu_BurnOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_BurnOff.show = true;
					this.__menuOption_Menu_BurnOff.showDisabled = false;
					this.__menuOption_Menu_BurnOff.longUseOnly = false;
					this.__menuOption_Menu_BurnOff.order = 0;
					this.__menuOption_Menu_BurnOff.icon = "close";
					this.__menuOption_Menu_BurnOff.desc = "ignite_off_balloon_desc";
					this.__menuOption_Menu_BurnOff.title = "off";
					if (this.__menuOption_Menu_BurnOff.function == null)
					{
						this.__menuOption_Menu_BurnOff.function = new Action<BasePlayer>(this.Menu_BurnOff);
					}
					list.Add(this.__menuOption_Menu_BurnOff);
				}
			}
			using (TimeWarning.New("Menu_BurnOn", 0.1f))
			{
				if (this.Menu_BurnOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_BurnOn.show = true;
					this.__menuOption_Menu_BurnOn.showDisabled = false;
					this.__menuOption_Menu_BurnOn.longUseOnly = false;
					this.__menuOption_Menu_BurnOn.order = 0;
					this.__menuOption_Menu_BurnOn.icon = "ignite";
					this.__menuOption_Menu_BurnOn.desc = "ignite_balloon_desc";
					this.__menuOption_Menu_BurnOn.title = "ignite";
					if (this.__menuOption_Menu_BurnOn.function == null)
					{
						this.__menuOption_Menu_BurnOn.function = new Action<BasePlayer>(this.Menu_BurnOn);
					}
					list.Add(this.__menuOption_Menu_BurnOn);
				}
			}
			using (TimeWarning.New("Menu_FuelStorage", 0.1f))
			{
				if (this.Menu_FuelStorage_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_FuelStorage.show = true;
					this.__menuOption_Menu_FuelStorage.showDisabled = false;
					this.__menuOption_Menu_FuelStorage.longUseOnly = false;
					this.__menuOption_Menu_FuelStorage.order = 800;
					this.__menuOption_Menu_FuelStorage.icon = "open";
					this.__menuOption_Menu_FuelStorage.desc = "fuelstorage_desc";
					this.__menuOption_Menu_FuelStorage.title = "Fuel Storage";
					if (this.__menuOption_Menu_FuelStorage.function == null)
					{
						this.__menuOption_Menu_FuelStorage.function = new Action<BasePlayer>(this.Menu_FuelStorage);
					}
					list.Add(this.__menuOption_Menu_FuelStorage);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000477 RID: 1143 RVA: 0x00005F90 File Offset: 0x00004190
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_BurnOff_ShowIf(LocalPlayer.Entity) || this.Menu_BurnOn_ShowIf(LocalPlayer.Entity) || this.Menu_FuelStorage_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0003BC30 File Offset: 0x00039E30
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HotAirBalloon.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0003BC74 File Offset: 0x00039E74
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.hotAirBalloon != null)
		{
			this.inflationLevel = info.msg.hotAirBalloon.inflationAmount;
		}
		if (info.msg.motorBoat != null)
		{
			this.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00005FC5 File Offset: 0x000041C5
	public bool WaterLogged()
	{
		return WaterLevel.Test(this.engineHeight.position);
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00005FD7 File Offset: 0x000041D7
	[BaseEntity.Menu.Description("ignite_balloon_desc", "Raise Altitude")]
	[BaseEntity.Menu("ignite", "Burn")]
	[BaseEntity.Menu.ShowIf("Menu_BurnOn_ShowIf")]
	[BaseEntity.Menu.Icon("ignite")]
	public void Menu_BurnOn(BasePlayer player)
	{
		base.ServerRPC<bool>("EngineSwitch", true);
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00005FE5 File Offset: 0x000041E5
	public bool Menu_BurnOn_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && base.HasFlag(BaseEntity.Flags.Reserved6) && player.lookingAtCollider.CompareTag("Usable Primary") && !this.WaterLogged();
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00006019 File Offset: 0x00004219
	[BaseEntity.Menu.ShowIf("Menu_BurnOff_ShowIf")]
	[BaseEntity.Menu("off", "Turn Off")]
	[BaseEntity.Menu.Description("ignite_off_balloon_desc", "Disengage burner")]
	[BaseEntity.Menu.Icon("close")]
	public void Menu_BurnOff(BasePlayer player)
	{
		base.ServerRPC<bool>("EngineSwitch", false);
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00006027 File Offset: 0x00004227
	public bool Menu_BurnOff_ShowIf(BasePlayer player)
	{
		return base.IsOn() && player.lookingAtCollider.CompareTag("Usable Primary");
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00003059 File Offset: 0x00001259
	[BaseEntity.Menu.Icon("open")]
	[BaseEntity.Menu.Description("fuelstorage_desc", "Open the fuel storage")]
	[BaseEntity.Menu("Fuel Storage", "Fuel Storage", Order = 800)]
	[BaseEntity.Menu.ShowIf("Menu_FuelStorage_ShowIf")]
	public void Menu_FuelStorage(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenFuel");
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00003066 File Offset: 0x00001266
	public bool Menu_FuelStorage_ShowIf(BasePlayer player)
	{
		return player.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00006043 File Offset: 0x00004243
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.currentClientInflationLevel = this.inflationLevel;
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x0003BCF0 File Offset: 0x00039EF0
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		Vector3 vector = this.GetWindAtPos(this.buoyancyPoint.position);
		if (vector == Vector3.zero)
		{
			vector = Vector3.forward;
		}
		this.windSockVec = Vector3.Lerp(this.windSockVec, vector, Time.deltaTime * 2f);
		this.windSock.rotation = Quaternion.LookRotation(this.windSockVec.normalized, base.transform.up);
		foreach (Transform transform in this.windFlags)
		{
			Vector3 normalized = (transform.transform.position + this.windSockVec.normalized * 1000f - transform.transform.position).normalized;
			transform.rotation = QuaternionEx.LookRotationForcedUp(normalized, base.transform.up);
		}
		this.currentClientInflationLevel = Mathf.Lerp(this.currentClientInflationLevel, this.inflationLevel, Time.deltaTime * 4f);
		this.UpdateBalloonMesh(this.currentClientInflationLevel);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00006058 File Offset: 0x00004258
	public void OptimizedEnable(GameObject obj, bool wantsEnabled)
	{
		if (obj.activeSelf == wantsEnabled)
		{
			return;
		}
		obj.SetActive(wantsEnabled);
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0003BE10 File Offset: 0x0003A010
	public void UpdateBalloonMesh(float inf)
	{
		if (inf == 0f)
		{
			this.OptimizedEnable(this.animatedBalloon, false);
			this.OptimizedEnable(this.staticBalloon, false);
			this.OptimizedEnable(this.staticBalloonDeflated, true);
			return;
		}
		if (inf == 1f)
		{
			this.OptimizedEnable(this.animatedBalloon, false);
			this.OptimizedEnable(this.staticBalloon, true);
			this.OptimizedEnable(this.staticBalloonDeflated, false);
			return;
		}
		this.OptimizedEnable(this.animatedBalloon, true);
		this.OptimizedEnable(this.staticBalloon, false);
		this.OptimizedEnable(this.staticBalloonDeflated, false);
		this.balloonAnimator.SetFloat("inflation", inf);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0000606B File Offset: 0x0000426B
	public override bool DisplayHealthInfo(BasePlayer player)
	{
		return this.ShowHealthInfo && (base.healthFraction < 0.95f || player.IsHoldingEntity<Hammer>()) && player.lookingAtCollider != null && player.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0003BEB8 File Offset: 0x0003A0B8
	public Vector3 GetWindAtPos(Vector3 pos)
	{
		float num = pos.y * 6f;
		Vector3 vector = new Vector3(Mathf.Sin(num * 0.017453292f), 0f, Mathf.Cos(num * 0.017453292f));
		return vector.normalized * 1f;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool PhysicsDriven()
	{
		return true;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool SupportsChildDeployables()
	{
		return false;
	}
}
