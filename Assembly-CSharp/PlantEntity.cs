using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class PlantEntity : BaseCombatEntity, IInstanceDataReceiver
{
	// Token: 0x04000271 RID: 625
	private Option __menuOption_MenuClone;

	// Token: 0x04000272 RID: 626
	private Option __menuOption_MenuPick;

	// Token: 0x04000273 RID: 627
	public PlantProperties plantProperty;

	// Token: 0x04000274 RID: 628
	public int water = -1;

	// Token: 0x04000275 RID: 629
	public int consumedWater = -1;

	// Token: 0x04000276 RID: 630
	public PlantProperties.State state;

	// Token: 0x04000277 RID: 631
	public float realAge;

	// Token: 0x04000278 RID: 632
	public float growthAge;

	// Token: 0x04000279 RID: 633
	private float stageAge;

	// Token: 0x0400027A RID: 634
	private DeferredAction skinChange;

	// Token: 0x0400027B RID: 635
	private GameObject skin;

	// Token: 0x0400027C RID: 636
	private float client_healthScale;

	// Token: 0x0400027D RID: 637
	public float client_yieldFraction;

	// Token: 0x0400027E RID: 638
	private MaterialColorLerp mcl;

	// Token: 0x060004E8 RID: 1256 RVA: 0x0003D9E8 File Offset: 0x0003BBE8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("PlantEntity.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("MenuClone", 0.1f))
			{
				if (this.MenuCanClone(LocalPlayer.Entity))
				{
					this.__menuOption_MenuClone.show = true;
					this.__menuOption_MenuClone.showDisabled = false;
					this.__menuOption_MenuClone.longUseOnly = false;
					this.__menuOption_MenuClone.order = 0;
					this.__menuOption_MenuClone.copyOptionsFrom = this.plantProperty.cloneOption;
					if (this.__menuOption_MenuClone.function == null)
					{
						this.__menuOption_MenuClone.function = new Action<BasePlayer>(this.MenuClone);
					}
					list.Add(this.__menuOption_MenuClone);
				}
			}
			using (TimeWarning.New("MenuPick", 0.1f))
			{
				if (this.MenuCanPick(LocalPlayer.Entity))
				{
					this.__menuOption_MenuPick.show = true;
					this.__menuOption_MenuPick.showDisabled = false;
					this.__menuOption_MenuPick.longUseOnly = false;
					this.__menuOption_MenuPick.order = 0;
					this.__menuOption_MenuPick.copyOptionsFrom = this.plantProperty.pickOption;
					if (this.__menuOption_MenuPick.function == null)
					{
						this.__menuOption_MenuPick.function = new Action<BasePlayer>(this.MenuPick);
					}
					list.Add(this.__menuOption_MenuPick);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00006568 File Offset: 0x00004768
	public override bool HasMenuOptions
	{
		get
		{
			return this.MenuCanClone(LocalPlayer.Entity) || this.MenuCanPick(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0003DBA0 File Offset: 0x0003BDA0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlantEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060004EB RID: 1259 RVA: 0x0000658E File Offset: 0x0000478E
	private PlantProperties.Stage currentStage
	{
		get
		{
			return this.plantProperty.stages[(int)this.state];
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060004EC RID: 1260 RVA: 0x000065A6 File Offset: 0x000047A6
	public float stageAgeFraction
	{
		get
		{
			return this.stageAge / (this.currentStage.lifeLength * 60f);
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x000065C0 File Offset: 0x000047C0
	public override void ResetState()
	{
		base.ResetState();
		this.state = PlantProperties.State.Seed;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x000065CF File Offset: 0x000047CF
	public bool CanPick()
	{
		return this.currentStage.resources > 0f;
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x000065E3 File Offset: 0x000047E3
	public float GetGrowthAge()
	{
		return this.growthAge;
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x000065EB File Offset: 0x000047EB
	public float GetStageAge()
	{
		return this.stageAge;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x000065F3 File Offset: 0x000047F3
	public float GetRealAge()
	{
		return this.realAge;
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x000065FB File Offset: 0x000047FB
	public bool CanClone()
	{
		return this.currentStage.resources > 0f && this.plantProperty.cloneItem != null;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00002ECE File Offset: 0x000010CE
	public void ReceiveInstanceData(Item.InstanceData data)
	{
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0003DBE4 File Offset: 0x0003BDE4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.plantEntity != null)
		{
			this.growthAge = info.msg.plantEntity.age;
			this.water = info.msg.plantEntity.water;
			this.realAge = info.msg.plantEntity.totalAge;
			this.growthAge = info.msg.plantEntity.growthAge;
			this.stageAge = info.msg.plantEntity.stageAge;
			if (!info.fromDisk)
			{
				this.client_healthScale = info.msg.plantEntity.healthy;
				this.client_yieldFraction = info.msg.plantEntity.yieldFraction;
			}
			this.BecomeState((PlantProperties.State)info.msg.plantEntity.state, false);
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0003DCC4 File Offset: 0x0003BEC4
	private void BecomeState(PlantProperties.State state, bool resetAge = true)
	{
		if (base.isClient && this.state == state && this.skin != null)
		{
			return;
		}
		this.state = state;
		if (base.isClient)
		{
			if (!this.skinChange)
			{
				this.skinChange = new DeferredAction(this, new Action(this.ChangeSkin), ActionPriority.Medium);
			}
			if (this.skinChange.Idle)
			{
				this.skinChange.Invoke();
			}
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00006622 File Offset: 0x00004822
	private void ChangeSkin()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this.DestroySkin();
		this.SpawnSkin();
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00006639 File Offset: 0x00004839
	private void DestroySkin()
	{
		if (this.skin)
		{
			GameManager.client.Retire(this.skin);
			this.skin = null;
		}
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0003DD40 File Offset: 0x0003BF40
	private void SpawnSkin()
	{
		if (this.currentStage.skinObject != null)
		{
			this.skin = GameManager.client.CreatePrefab(this.currentStage.skinObject.resourcePath, base.gameObject.transform, true);
			this.UpdateSkinParameters();
		}
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0003DD8C File Offset: 0x0003BF8C
	private void UpdateSkinParameters()
	{
		if (this.skin != null)
		{
			this.GetMaterialColorLerp();
			this.mcl.RefreshRenderers();
			PlantSkin component = this.skin.GetComponent<PlantSkin>();
			if (component != null)
			{
				if (this.state == PlantProperties.State.Fruiting)
				{
					float progress = this.plantProperty.fruitCurve.Evaluate(this.stageAgeFraction);
					foreach (FruitScale fruitScale in component.FruitscaleComponents)
					{
						fruitScale.SetProgress(progress);
					}
				}
				foreach (LifeScale lifeScale in component.LifescaleComponents)
				{
					lifeScale.SetProgress(this.stageAgeFraction);
				}
			}
			this.UpdateWaterVisiblity();
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x0000665F File Offset: 0x0000485F
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		this.DestroySkin();
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0000666D File Offset: 0x0000486D
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		this.UpdateSkinParameters();
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0000667B File Offset: 0x0000487B
	public MaterialColorLerp GetMaterialColorLerp()
	{
		if (this.mcl == null)
		{
			this.mcl = base.GetComponent<MaterialColorLerp>();
		}
		return this.mcl;
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0003DE84 File Offset: 0x0003C084
	public void UpdateWaterVisiblity()
	{
		if (this.state == PlantProperties.State.Dying)
		{
			return;
		}
		float num = (float)this.water / (float)this.plantProperty.maxHeldWater;
		if (this.GetMaterialColorLerp())
		{
			this.mcl.SetColorScale(1f - num - this.client_healthScale);
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool NeedsCrosshair()
	{
		return true;
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0000669D File Offset: 0x0000489D
	public override bool DisplayHealthInfo(BasePlayer player)
	{
		return this.state == PlantProperties.State.Dying;
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x000066A8 File Offset: 0x000048A8
	[BaseEntity.Menu.ShowIf("MenuCanPick")]
	[BaseEntity.Menu(UseVariable = "plantProperty.pickOption")]
	public void MenuPick(BasePlayer ply)
	{
		base.ServerRPC("RPC_PickFruit");
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x000066B5 File Offset: 0x000048B5
	public bool MenuCanPick(BasePlayer player)
	{
		return this.CanPick();
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x000066BD File Offset: 0x000048BD
	[BaseEntity.Menu(UseVariable = "plantProperty.cloneOption")]
	[BaseEntity.Menu.ShowIf("MenuCanClone")]
	public void MenuClone(BasePlayer ply)
	{
		base.ServerRPC("RPC_TakeClone");
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x000066CA File Offset: 0x000048CA
	public bool MenuCanClone(BasePlayer player)
	{
		return this.CanClone();
	}
}
