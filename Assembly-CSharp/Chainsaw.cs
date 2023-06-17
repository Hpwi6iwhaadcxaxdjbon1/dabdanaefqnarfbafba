using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class Chainsaw : BaseMelee
{
	// Token: 0x04000528 RID: 1320
	[Header("Chainsaw")]
	public float fuelPerSec = 1f;

	// Token: 0x04000529 RID: 1321
	public int maxAmmo = 100;

	// Token: 0x0400052A RID: 1322
	public int ammo = 100;

	// Token: 0x0400052B RID: 1323
	public ItemDefinition fuelType;

	// Token: 0x0400052C RID: 1324
	public float reloadDuration = 2.5f;

	// Token: 0x0400052D RID: 1325
	[Header("Sounds")]
	public SoundPlayer idleLoop;

	// Token: 0x0400052E RID: 1326
	public SoundPlayer attackLoopAir;

	// Token: 0x0400052F RID: 1327
	public SoundPlayer revUp;

	// Token: 0x04000530 RID: 1328
	public SoundPlayer revDown;

	// Token: 0x04000531 RID: 1329
	public SoundPlayer offSound;

	// Token: 0x04000532 RID: 1330
	private string lastHitMaterial;

	// Token: 0x04000533 RID: 1331
	private float lastHitTime;

	// Token: 0x04000534 RID: 1332
	private float nextReleaseTime;

	// Token: 0x04000535 RID: 1333
	private float nextPressTime;

	// Token: 0x04000536 RID: 1334
	private bool wasAttacking;

	// Token: 0x04000537 RID: 1335
	private float reloadFinishedTime;

	// Token: 0x04000538 RID: 1336
	public float attackFadeInTime = 0.1f;

	// Token: 0x04000539 RID: 1337
	public float attackFadeInDelay = 0.1f;

	// Token: 0x0400053A RID: 1338
	public float attackFadeOutTime = 0.1f;

	// Token: 0x0400053B RID: 1339
	public float idleFadeInTimeFromOff = 0.1f;

	// Token: 0x0400053C RID: 1340
	public float idleFadeInTimeFromAttack = 0.3f;

	// Token: 0x0400053D RID: 1341
	public float idleFadeInDelay = 0.1f;

	// Token: 0x0400053E RID: 1342
	public float idleFadeOutTime = 0.1f;

	// Token: 0x0400053F RID: 1343
	private bool wasEngineOn;

	// Token: 0x04000540 RID: 1344
	private bool wasAttackingAudio;

	// Token: 0x04000541 RID: 1345
	public Renderer chainRenderer;

	// Token: 0x04000542 RID: 1346
	private MaterialPropertyBlock block;

	// Token: 0x04000543 RID: 1347
	private Vector2 saveST;

	// Token: 0x04000544 RID: 1348
	private float chainSpeed;

	// Token: 0x04000545 RID: 1349
	private float chainAmount;

	// Token: 0x04000546 RID: 1350
	private float chainSpinUpRate = 5f;

	// Token: 0x06000841 RID: 2113 RVA: 0x0004B38C File Offset: 0x0004958C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Chainsaw.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool EngineOn()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00007B8D File Offset: 0x00005D8D
	public bool IsAttacking()
	{
		return base.HasFlag(BaseEntity.Flags.Busy);
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x00008B8B File Offset: 0x00006D8B
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		if (base.isClient)
		{
			this.SetupVisuals();
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00008BA2 File Offset: 0x00006DA2
	public void Update()
	{
		if (base.isClient)
		{
			this.UpdateChain(this.EngineOn(), this.IsAttacking());
		}
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x00008BBE File Offset: 0x00006DBE
	private bool IsReloading()
	{
		return Time.realtimeSinceStartup < this.reloadFinishedTime;
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0004B3D0 File Offset: 0x000495D0
	public override void GetItemOptions(List<Option> options)
	{
		base.GetItemOptions(options);
		if (this.ammo > 0)
		{
			options.Add(new Option
			{
				icon = "menu_dots",
				title = "unload_ammo",
				desc = "unload_ammo_desc",
				command = "unload_ammo",
				order = 10
			});
		}
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0004B438 File Offset: 0x00049638
	protected override void DoAttack()
	{
		if (this.viewModel == null)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		HitTest hitTest = new HitTest();
		hitTest.AttackRay = ownerPlayer.eyes.BodyRay();
		hitTest.MaxDistance = this.maxDistance;
		hitTest.BestHit = true;
		hitTest.damageProperties = this.damageProperties;
		hitTest.ignoreEntity = ownerPlayer;
		hitTest.Radius = 0f;
		hitTest.Forgiveness = Mathf.Min(this.attackRadius, 0.05f);
		hitTest.type = HitTest.Type.MeleeAttack;
		GameTrace.Trace(hitTest, 1269916417);
		ownerPlayer.BlockSprint(this.repeatDelay * 0.5f);
		if (!hitTest.DidHit)
		{
			hitTest.Forgiveness = Mathf.Max(0.05f, this.attackRadius);
			if (!GameTrace.Trace(hitTest, 1269916417))
			{
				return;
			}
		}
		if (!this.CanHit(hitTest))
		{
			return;
		}
		this.lastHitTime = Time.time;
		this.lastHitMaterial = hitTest.HitMaterial;
		this.ProcessAttack(hitTest);
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00008BCD File Offset: 0x00006DCD
	public void SendServerReload()
	{
		base.ServerRPC("DoReload");
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00008BDA File Offset: 0x00006DDA
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (this.viewModel && name == "cordpull")
		{
			base.ServerRPC("Server_StartEngine");
		}
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0004B53C File Offset: 0x0004973C
	public override void OnInput()
	{
		base.ProcessInputTime();
		if (!this.IsFullyDeployed())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		ownerPlayer.modelState.aiming = false;
		bool flag = ownerPlayer.CanAttack();
		bool flag2 = (ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY) || Time.realtimeSinceStartup < this.nextReleaseTime) && Time.realtimeSinceStartup > this.nextPressTime;
		bool flag3 = flag && flag2 && this.ammo > 0 && this.EngineOn();
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.RELOAD) && !flag2 && this.GetAmmo() != null && this.ammo < this.maxAmmo && !this.IsReloading())
		{
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Reload, "");
			this.viewModel.Trigger("reload");
			this.reloadFinishedTime = Time.realtimeSinceStartup + this.reloadDuration;
			this.SendServerReload();
			return;
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY) && !base.HasAttackCooldown())
		{
			base.StartAttackCooldown(0.7f);
			if (this.EngineOn())
			{
				base.ServerRPC("Server_StopEngine");
			}
			else
			{
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Alt_Attack, "");
				if (this.viewModel)
				{
					this.viewModel.Trigger("enginestart");
				}
			}
		}
		if (flag3)
		{
			ownerPlayer.BlockSprint(0.5f);
		}
		if (flag3 && !this.wasAttacking)
		{
			this.nextReleaseTime = Time.realtimeSinceStartup + 1f;
		}
		if (!flag3 && this.wasAttacking)
		{
			this.nextPressTime = Time.realtimeSinceStartup + 0.7f;
			base.StartAttackCooldown(0.5f);
		}
		if (this.wasAttacking != flag3)
		{
			base.ServerRPC<bool>("Server_SetAttacking", flag3);
		}
		this.wasAttacking = flag3;
		ownerPlayer.modelState.aiming = flag3;
		if (!base.HasAttackCooldown() && flag3)
		{
			base.StartAttackCooldown(this.repeatDelay);
			this.DoAttack();
		}
		v_chainsaw chainsawViewmodel = this.GetChainsawViewmodel();
		if (chainsawViewmodel)
		{
			chainsawViewmodel.bAttacking = flag3;
		}
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x00008C08 File Offset: 0x00006E08
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		this.UpdateAudio();
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x00008C16 File Offset: 0x00006E16
	public v_chainsaw GetChainsawViewmodel()
	{
		if (this.viewModel && this.viewModel.instance)
		{
			return this.viewModel.instance.GetComponent<v_chainsaw>();
		}
		return null;
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0004B750 File Offset: 0x00049950
	public override void OnFrame()
	{
		base.OnFrame();
		v_chainsaw chainsawViewmodel = this.GetChainsawViewmodel();
		if (chainsawViewmodel)
		{
			chainsawViewmodel.bHitFlesh = false;
			chainsawViewmodel.bHitWood = false;
			chainsawViewmodel.bHitMetal = false;
			chainsawViewmodel.bEngineOn = this.EngineOn();
			if (Time.time < this.lastHitTime + 0.25f)
			{
				if (this.lastHitMaterial.Contains("Flesh"))
				{
					chainsawViewmodel.bHitFlesh = true;
					return;
				}
				if (this.lastHitMaterial.Contains("Wood"))
				{
					chainsawViewmodel.bHitWood = true;
					return;
				}
				chainsawViewmodel.bHitMetal = true;
			}
		}
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0004B7E4 File Offset: 0x000499E4
	public void CleanupViewmodel()
	{
		v_chainsaw chainsawViewmodel = this.GetChainsawViewmodel();
		if (chainsawViewmodel)
		{
			chainsawViewmodel.bHitFlesh = false;
			chainsawViewmodel.bHitWood = false;
			chainsawViewmodel.bHitMetal = false;
		}
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x00008C49 File Offset: 0x00006E49
	public override void OnHolster()
	{
		this.CleanupViewmodel();
		base.OnHolster();
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x00008C57 File Offset: 0x00006E57
	public override void OnDeploy()
	{
		base.OnDeploy();
		this.CleanupViewmodel();
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0004B818 File Offset: 0x00049A18
	protected virtual void UpdateAmmoDisplay()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.amountTextOverride = string.Format("{0}", this.ammo);
		LocalPlayer.OnItemAmountChanged();
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x00008C65 File Offset: 0x00006E65
	public override void DoAttackShared(HitInfo info)
	{
		base.DoAttackShared(info);
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0004B850 File Offset: 0x00049A50
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
			this.UpdateAmmoDisplay();
		}
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x00008C6E File Offset: 0x00006E6E
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0004B8A4 File Offset: 0x00049AA4
	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
		}
		return item;
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00008C79 File Offset: 0x00006E79
	public void DelayedAttackLoop()
	{
		this.attackLoopAir.FadeInAndPlay(this.attackFadeInTime);
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00008C8C File Offset: 0x00006E8C
	public void DelayedIdleLoop()
	{
		this.idleLoop.FadeInAndPlay(this.idleFadeInTimeFromAttack);
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0004B900 File Offset: 0x00049B00
	public void UpdateAudio()
	{
		if (this.wasEngineOn && !this.EngineOn())
		{
			this.offSound.Play();
		}
		if (!this.wasEngineOn && this.EngineOn())
		{
			this.idleLoop.FadeInAndPlay(this.idleFadeInTimeFromOff);
		}
		this.wasEngineOn = this.EngineOn();
		if (this.EngineOn())
		{
			if (this.IsAttacking())
			{
				if (!this.wasAttackingAudio)
				{
					this.revUp.Play();
				}
				if (this.idleLoop.IsPlaying())
				{
					this.idleLoop.FadeOutAndRecycle(this.idleFadeOutTime);
				}
				if (!this.attackLoopAir.IsPlaying() && !base.IsInvoking(new Action(this.DelayedAttackLoop)))
				{
					base.Invoke(new Action(this.DelayedAttackLoop), this.attackFadeInDelay);
				}
			}
			else
			{
				if (this.wasAttackingAudio)
				{
					this.revDown.Play();
				}
				if (this.attackLoopAir.IsPlaying())
				{
					this.attackLoopAir.FadeOutAndRecycle(this.attackFadeOutTime);
				}
				if (!this.idleLoop.IsPlaying() && !base.IsInvoking(new Action(this.DelayedIdleLoop)))
				{
					base.Invoke(new Action(this.DelayedIdleLoop), this.idleFadeInDelay);
				}
			}
		}
		else
		{
			if (this.attackLoopAir.IsPlaying())
			{
				this.attackLoopAir.FadeOutAndRecycle(this.attackFadeOutTime);
			}
			if (this.idleLoop.IsPlaying())
			{
				this.idleLoop.FadeOutAndRecycle(this.idleFadeOutTime);
			}
			if (base.IsInvoking(new Action(this.DelayedIdleLoop)))
			{
				base.CancelInvoke(new Action(this.DelayedIdleLoop));
			}
			if (base.IsInvoking(new Action(this.DelayedAttackLoop)))
			{
				base.CancelInvoke(new Action(this.DelayedAttackLoop));
			}
		}
		this.wasAttackingAudio = this.IsAttacking();
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x00008C9F File Offset: 0x00006E9F
	public void SetupVisuals()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.saveST = this.chainRenderer.sharedMaterial.GetVector("_MainTex_ST");
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0004BAE8 File Offset: 0x00049CE8
	private void UpdateChain(bool on, bool attacking)
	{
		float b;
		if (on)
		{
			if (attacking)
			{
				b = 15f;
			}
			else
			{
				b = 2f;
			}
		}
		else
		{
			b = 0f;
		}
		this.chainSpeed = Mathf.Lerp(this.chainSpeed, b, Time.deltaTime * this.chainSpinUpRate);
		float num = this.chainAmount = (this.chainAmount + Time.deltaTime * this.chainSpeed) % 1f;
		this.block.Clear();
		this.block.SetVector("_MainTex_ST", new Vector4(this.saveST.x, this.saveST.y, -num, 0f));
		this.chainRenderer.SetPropertyBlock(this.block);
		v_chainsaw chainsawViewmodel = this.GetChainsawViewmodel();
		if (chainsawViewmodel != null)
		{
			chainsawViewmodel.chainRenderer.SetPropertyBlock(this.block);
		}
	}
}
