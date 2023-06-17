using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class MiniCopter : BaseHelicopterVehicle
{
	// Token: 0x0400005E RID: 94
	public Transform waterSample;

	// Token: 0x0400005F RID: 95
	public WheelCollider leftWheel;

	// Token: 0x04000060 RID: 96
	public WheelCollider rightWheel;

	// Token: 0x04000061 RID: 97
	public WheelCollider frontWheel;

	// Token: 0x04000062 RID: 98
	public Transform leftWheelTrans;

	// Token: 0x04000063 RID: 99
	public Transform rightWheelTrans;

	// Token: 0x04000064 RID: 100
	public Transform frontWheelTrans;

	// Token: 0x04000065 RID: 101
	public float cachedrotation_left;

	// Token: 0x04000066 RID: 102
	public float cachedrotation_right;

	// Token: 0x04000067 RID: 103
	public float cachedrotation_front;

	// Token: 0x04000068 RID: 104
	public AnimationCurve bladeEngineCurve;

	// Token: 0x04000069 RID: 105
	public const BaseEntity.Flags Flag_EngineStart = BaseEntity.Flags.Reserved4;

	// Token: 0x0400006A RID: 106
	public Transform mainRotorBlur;

	// Token: 0x0400006B RID: 107
	public Transform mainRotorBlades;

	// Token: 0x0400006C RID: 108
	public Transform rearRotorBlades;

	// Token: 0x0400006D RID: 109
	public Transform rearRotorBlur;

	// Token: 0x0400006E RID: 110
	public float motorForceConstant = 150f;

	// Token: 0x0400006F RID: 111
	public float brakeForceConstant = 500f;

	// Token: 0x04000070 RID: 112
	public GameObject preventBuildingObject;

	// Token: 0x04000071 RID: 113
	[ServerVar(Help = "Population active on the server")]
	public static float population = 1f;

	// Token: 0x04000072 RID: 114
	[ServerVar(Help = "How long before a minicopter is killed while outside")]
	public static float outsidedecayminutes = 240f;

	// Token: 0x04000073 RID: 115
	public float rotorBlurThreshold = 15f;

	// Token: 0x04000074 RID: 116
	private bool wasEngineOn;

	// Token: 0x04000075 RID: 117
	private float leftWheelVelocity;

	// Token: 0x04000076 RID: 118
	private float rightWheelVelocity;

	// Token: 0x04000077 RID: 119
	private float rotorSpeed;

	// Token: 0x04000078 RID: 120
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x04000079 RID: 121
	public Transform fuelStoragePoint;

	// Token: 0x0400007A RID: 122
	public EntityRef fuelStorageInstance;

	// Token: 0x0400007B RID: 123
	public float fuelPerSec = 0.25f;

	// Token: 0x0400007C RID: 124
	private Option __menuOption_Menu_FuelStorage;

	// Token: 0x06000051 RID: 81 RVA: 0x00002FD4 File Offset: 0x000011D4
	public bool IsStartingUp()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00002FE1 File Offset: 0x000011E1
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (!this.wasEngineOn)
		{
			base.IsOn();
		}
		if (this.wasEngineOn)
		{
			base.IsOn();
		}
		this.wasEngineOn = base.IsOn();
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00027F5C File Offset: 0x0002615C
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		Vector3 worldVelocity = base.GetWorldVelocity();
		ref Vector3 ptr = base.transform.InverseTransformDirection(worldVelocity);
		float num = 1.8849558f;
		float num2 = ptr.z / num;
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			this.leftWheelVelocity = 360f * num2 * Time.deltaTime;
		}
		else
		{
			this.leftWheelVelocity = Mathf.Lerp(this.rightWheelVelocity, 0f, Time.deltaTime);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			this.rightWheelVelocity = 360f * num2 * Time.deltaTime;
		}
		else
		{
			this.rightWheelVelocity = Mathf.Lerp(this.rightWheelVelocity, 0f, Time.deltaTime);
		}
		this.rightWheelTrans.transform.Rotate(Vector3.right, this.rightWheelVelocity);
		this.leftWheelTrans.transform.Rotate(Vector3.right, this.leftWheelVelocity);
		if (base.IsOn() || this.IsStartingUp())
		{
			this.rotorSpeed += 10f * Time.deltaTime / 7f;
		}
		else
		{
			this.rotorSpeed -= 10f * Time.deltaTime / 10f;
		}
		this.rotorSpeed = Mathf.Clamp(this.rotorSpeed, 0f, 10f);
		bool flag = this.rotorSpeed >= 8f;
		this.mainRotorBlades.transform.Rotate(Vector3.up, 360f * Time.deltaTime * this.rotorSpeed);
		this.rearRotorBlades.transform.Rotate(Vector3.right, 360f * Time.deltaTime * this.rotorSpeed);
		this.mainRotorBlur.localScale = (flag ? Vector3.one : Vector3.zero);
		this.mainRotorBlades.localScale = (flag ? Vector3.zero : Vector3.one);
		this.rearRotorBlur.localScale = (flag ? Vector3.one : Vector3.zero);
		this.rearRotorBlades.localScale = (flag ? Vector3.zero : Vector3.one);
		float num3 = this.rotorSpeed * 0.4f;
		this.mainRotorBlur.transform.Rotate(Vector3.up, 360f * Time.deltaTime * num3);
		this.rearRotorBlur.transform.Rotate(Vector3.right, 360f * Time.deltaTime * num3);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003013 File Offset: 0x00001213
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.motorBoat != null)
		{
			this.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00003044 File Offset: 0x00001244
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.preventBuildingObject.SetActive(true);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003059 File Offset: 0x00001259
	[BaseEntity.Menu("Fuel Storage", "Fuel Storage", Order = 800)]
	[BaseEntity.Menu.ShowIf("Menu_FuelStorage_ShowIf")]
	[BaseEntity.Menu.Icon("open")]
	[BaseEntity.Menu.Description("fuelstorage_desc", "Open the fuel storage")]
	public void Menu_FuelStorage(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenFuel");
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00003066 File Offset: 0x00001266
	public bool Menu_FuelStorage_ShowIf(BasePlayer player)
	{
		return player.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000281C8 File Offset: 0x000263C8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("MiniCopter.GetMenuOptions", 0.1f))
		{
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

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000059 RID: 89 RVA: 0x00003078 File Offset: 0x00001278
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_FuelStorage_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600005A RID: 90 RVA: 0x000282D4 File Offset: 0x000264D4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MiniCopter.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}
