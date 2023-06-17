using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class ProceduralLift : BaseEntity
{
	// Token: 0x0400028F RID: 655
	private Option __menuOption_Menu_UseLift;

	// Token: 0x04000290 RID: 656
	public float movementSpeed = 1f;

	// Token: 0x04000291 RID: 657
	public float resetDelay = 5f;

	// Token: 0x04000292 RID: 658
	public ProceduralLiftCabin cabin;

	// Token: 0x04000293 RID: 659
	public ProceduralLiftStop[] stops;

	// Token: 0x04000294 RID: 660
	public GameObjectRef triggerPrefab;

	// Token: 0x04000295 RID: 661
	public string triggerBone;

	// Token: 0x04000296 RID: 662
	private int floorIndex = -1;

	// Token: 0x06000523 RID: 1315 RVA: 0x0003E4D8 File Offset: 0x0003C6D8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("ProceduralLift.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_UseLift", 0.1f))
			{
				if (this.Menu_UseLift_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_UseLift.show = true;
					this.__menuOption_Menu_UseLift.showDisabled = false;
					this.__menuOption_Menu_UseLift.longUseOnly = false;
					this.__menuOption_Menu_UseLift.order = 0;
					this.__menuOption_Menu_UseLift.icon = "open_door";
					this.__menuOption_Menu_UseLift.desc = "use_lift_desc";
					this.__menuOption_Menu_UseLift.title = "use_lift";
					if (this.__menuOption_Menu_UseLift.function == null)
					{
						this.__menuOption_Menu_UseLift.function = new Action<BasePlayer>(this.Menu_UseLift);
					}
					list.Add(this.__menuOption_Menu_UseLift);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x06000524 RID: 1316 RVA: 0x0000690A File Offset: 0x00004B0A
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_UseLift_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0003E5E0 File Offset: 0x0003C7E0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ProceduralLift.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0003E624 File Offset: 0x0003C824
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.lift != null)
		{
			if (this.floorIndex == -1)
			{
				this.SnapToFloor(info.msg.lift.floor);
			}
			else
			{
				this.MoveToFloor(info.msg.lift.floor);
			}
		}
		base.Load(info);
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00006246 File Offset: 0x00004446
	[BaseEntity.Menu.ShowIf("Menu_UseLift_ShowIf")]
	[BaseEntity.Menu.Icon("open_door")]
	[BaseEntity.Menu("use_lift", "Use Lift")]
	[BaseEntity.Menu.Description("use_lift_desc", "Activate the lift")]
	public void Menu_UseLift(BasePlayer player)
	{
		base.ServerRPC("RPC_UseLift");
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00006921 File Offset: 0x00004B21
	public bool Menu_UseLift_ShowIf(BasePlayer player)
	{
		return !base.IsBusy();
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool NeedsCrosshair()
	{
		return true;
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0000692C File Offset: 0x00004B2C
	private void ResetLift()
	{
		this.MoveToFloor(0);
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x00006935 File Offset: 0x00004B35
	private void MoveToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0003E67C File Offset: 0x0003C87C
	private void SnapToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		this.cabin.transform.position = proceduralLiftStop.transform.position;
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x00002ECE File Offset: 0x000010CE
	private void OnFinishedMoving()
	{
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0003E6CC File Offset: 0x0003C8CC
	protected void Update()
	{
		if (this.floorIndex < 0 || this.floorIndex > this.stops.Length - 1)
		{
			return;
		}
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			return;
		}
		this.cabin.transform.position = Vector3.MoveTowards(this.cabin.transform.position, proceduralLiftStop.transform.position, this.movementSpeed * Time.deltaTime);
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			this.OnFinishedMoving();
		}
	}
}
