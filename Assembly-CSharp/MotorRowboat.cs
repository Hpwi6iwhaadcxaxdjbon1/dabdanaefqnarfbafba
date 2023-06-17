using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class MotorRowboat : MotorBoat
{
	// Token: 0x04000233 RID: 563
	private Option __menuOption_Menu_FuelStorage;

	// Token: 0x04000234 RID: 564
	private Option __menuOption_Menu_Push;

	// Token: 0x04000235 RID: 565
	private Option __menuOption_Menu_StartEngine;

	// Token: 0x04000236 RID: 566
	private Option __menuOption_Menu_StopEngine;

	// Token: 0x04000237 RID: 567
	protected const BaseEntity.Flags Flag_EngineOn = BaseEntity.Flags.Reserved1;

	// Token: 0x04000238 RID: 568
	protected const BaseEntity.Flags Flag_ThrottleOn = BaseEntity.Flags.Reserved2;

	// Token: 0x04000239 RID: 569
	protected const BaseEntity.Flags Flag_TurnLeft = BaseEntity.Flags.Reserved3;

	// Token: 0x0400023A RID: 570
	protected const BaseEntity.Flags Flag_TurnRight = BaseEntity.Flags.Reserved4;

	// Token: 0x0400023B RID: 571
	protected const BaseEntity.Flags Flag_Submerged = BaseEntity.Flags.Reserved5;

	// Token: 0x0400023C RID: 572
	protected const BaseEntity.Flags Flag_HasFuel = BaseEntity.Flags.Reserved6;

	// Token: 0x0400023D RID: 573
	protected const BaseEntity.Flags Flag_Stationary = BaseEntity.Flags.Reserved7;

	// Token: 0x0400023E RID: 574
	protected const BaseEntity.Flags Flag_RecentlyPushed = BaseEntity.Flags.Reserved8;

	// Token: 0x0400023F RID: 575
	private const float submergeFractionMinimum = 0.85f;

	// Token: 0x04000240 RID: 576
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x04000241 RID: 577
	public Transform fuelStoragePoint;

	// Token: 0x04000242 RID: 578
	public EntityRef fuelStorageInstance;

	// Token: 0x04000243 RID: 579
	public float fuelPerSec;

	// Token: 0x04000244 RID: 580
	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	// Token: 0x04000245 RID: 581
	public Transform storageUnitPoint;

	// Token: 0x04000246 RID: 582
	public EntityRef storageUnitInstance;

	// Token: 0x04000247 RID: 583
	[Header("Effects")]
	public Transform boatRear;

	// Token: 0x04000248 RID: 584
	public ParticleSystemContainer wakeEffect;

	// Token: 0x04000249 RID: 585
	public ParticleSystemContainer engineEffectIdle;

	// Token: 0x0400024A RID: 586
	public ParticleSystemContainer engineEffectThrottle;

	// Token: 0x0400024B RID: 587
	public Projector causticsProjector;

	// Token: 0x0400024C RID: 588
	public Transform causticsDepthTest;

	// Token: 0x0400024D RID: 589
	public Transform engineLeftHandPosition;

	// Token: 0x0400024E RID: 590
	public Transform engineRotate;

	// Token: 0x0400024F RID: 591
	public Transform propellerRotate;

	// Token: 0x04000250 RID: 592
	[ServerVar(Help = "Population active on the server")]
	public static float population = 4f;

	// Token: 0x04000251 RID: 593
	[ServerVar(Help = "How long before a boat is killed while outside")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x04000252 RID: 594
	public Transform[] stationaryDismounts;

	// Token: 0x04000253 RID: 595
	public Collider mainCollider;

	// Token: 0x04000254 RID: 596
	private AverageVelocity averageVelocity = new AverageVelocity();

	// Token: 0x04000255 RID: 597
	private bool wasWakeOn = true;

	// Token: 0x04000256 RID: 598
	private bool wasEngineSprayOn = true;

	// Token: 0x04000257 RID: 599
	private bool wasEngineIdleOn = true;

	// Token: 0x04000258 RID: 600
	private Material causticsMaterial;

	// Token: 0x04000259 RID: 601
	private Color causticsColor;

	// Token: 0x0400025A RID: 602
	private float causticsAlpha = 1f;

	// Token: 0x0400025B RID: 603
	protected float currentEngineRotation;

	// Token: 0x0400025C RID: 604
	protected float propellerRotationSpeed;

	// Token: 0x0400025D RID: 605
	[Header("Audio")]
	public BlendedSoundLoops engineLoops;

	// Token: 0x0400025E RID: 606
	public BlendedSoundLoops waterLoops;

	// Token: 0x0400025F RID: 607
	public SoundDefinition engineStartSoundDef;

	// Token: 0x04000260 RID: 608
	public SoundDefinition engineStopSoundDef;

	// Token: 0x04000261 RID: 609
	public SoundDefinition movementSplashAccentSoundDef;

	// Token: 0x04000262 RID: 610
	public SoundDefinition engineSteerSoundDef;

	// Token: 0x04000263 RID: 611
	public GameObjectRef pushLandEffect;

	// Token: 0x04000264 RID: 612
	public GameObjectRef pushWaterEffect;

	// Token: 0x04000265 RID: 613
	public float waterSpeedDivisor = 10f;

	// Token: 0x04000266 RID: 614
	public float turnPitchModScale = -0.25f;

	// Token: 0x04000267 RID: 615
	public float tiltPitchModScale = 0.3f;

	// Token: 0x04000268 RID: 616
	public float splashAccentFrequencyMin = 1f;

	// Token: 0x04000269 RID: 617
	public float splashAccentFrequencyMax = 10f;

	// Token: 0x0400026A RID: 618
	private float directionalPitchModScale;

	// Token: 0x0400026B RID: 619
	private float vol;

	// Token: 0x0400026C RID: 620
	private float pitch;

	// Token: 0x0400026D RID: 621
	private float speed;

	// Token: 0x0400026E RID: 622
	private bool wasEngineOn;

	// Token: 0x0400026F RID: 623
	private float nextSplashAccent;

	// Token: 0x04000270 RID: 624
	private int prevSteering;

	// Token: 0x060004CB RID: 1227 RVA: 0x0003CC6C File Offset: 0x0003AE6C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("MotorRowboat.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_FuelStorage", 0.1f))
			{
				if (this.Menu_FuelStorage_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_FuelStorage.show = true;
					this.__menuOption_Menu_FuelStorage.showDisabled = false;
					this.__menuOption_Menu_FuelStorage.longUseOnly = false;
					this.__menuOption_Menu_FuelStorage.order = -80;
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
			using (TimeWarning.New("Menu_Push", 0.1f))
			{
				if (this.Menu_Push_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Push.show = true;
					this.__menuOption_Menu_Push.showDisabled = false;
					this.__menuOption_Menu_Push.longUseOnly = false;
					this.__menuOption_Menu_Push.order = -100;
					this.__menuOption_Menu_Push.icon = "player_carry";
					this.__menuOption_Menu_Push.desc = "push_desc";
					this.__menuOption_Menu_Push.title = "push";
					if (this.__menuOption_Menu_Push.function == null)
					{
						this.__menuOption_Menu_Push.function = new Action<BasePlayer>(this.Menu_Push);
					}
					list.Add(this.__menuOption_Menu_Push);
				}
			}
			using (TimeWarning.New("Menu_StartEngine", 0.1f))
			{
				if (this.Menu_StartEngine_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_StartEngine.show = true;
					this.__menuOption_Menu_StartEngine.showDisabled = false;
					this.__menuOption_Menu_StartEngine.longUseOnly = false;
					this.__menuOption_Menu_StartEngine.order = -90;
					this.__menuOption_Menu_StartEngine.icon = "gear";
					this.__menuOption_Menu_StartEngine.desc = "startboatengine_desc";
					this.__menuOption_Menu_StartEngine.title = "startboatengine";
					if (this.__menuOption_Menu_StartEngine.function == null)
					{
						this.__menuOption_Menu_StartEngine.function = new Action<BasePlayer>(this.Menu_StartEngine);
					}
					list.Add(this.__menuOption_Menu_StartEngine);
				}
			}
			using (TimeWarning.New("Menu_StopEngine", 0.1f))
			{
				if (this.Menu_StopEngine_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_StopEngine.show = true;
					this.__menuOption_Menu_StopEngine.showDisabled = false;
					this.__menuOption_Menu_StopEngine.longUseOnly = false;
					this.__menuOption_Menu_StopEngine.order = -90;
					this.__menuOption_Menu_StopEngine.icon = "close";
					this.__menuOption_Menu_StopEngine.desc = "stopboatengine_desc";
					this.__menuOption_Menu_StopEngine.title = "stopboatengine";
					if (this.__menuOption_Menu_StopEngine.function == null)
					{
						this.__menuOption_Menu_StopEngine.function = new Action<BasePlayer>(this.Menu_StopEngine);
					}
					list.Add(this.__menuOption_Menu_StopEngine);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060004CC RID: 1228 RVA: 0x0003D00C File Offset: 0x0003B20C
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_FuelStorage_ShowIf(LocalPlayer.Entity) || this.Menu_Push_ShowIf(LocalPlayer.Entity) || this.Menu_StartEngine_ShowIf(LocalPlayer.Entity) || this.Menu_StopEngine_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0003D05C File Offset: 0x0003B25C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MotorRowboat.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0003D0A0 File Offset: 0x0003B2A0
	public override void UpdatePlayerModel(BasePlayer player)
	{
		if (this.engineLeftHandPosition == null)
		{
			return;
		}
		if (player.modelState.poseType == 5)
		{
			player.playerModel.leftHandTargetPosition = this.engineLeftHandPosition.position;
			player.playerModel.leftHandTargetRotation = this.engineLeftHandPosition.rotation;
			return;
		}
		player.playerModel.leftHandTargetPosition = Vector3.zero;
		player.playerModel.leftHandTargetRotation = Quaternion.identity;
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x000063C1 File Offset: 0x000045C1
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		this.UpdateEffects();
		this.UpdateWake();
		this.UpdateSounds();
		this.UpdateEngineRotation();
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0003D118 File Offset: 0x0003B318
	public void UpdateWake()
	{
		if (TerrainMeta.WaterMap == null)
		{
			return;
		}
		float height = WaterSystem.GetHeight(this.boatRear.position);
		Vector3 position = this.boatRear.position;
		position.y = height + 0.075f;
		Vector3 vector = base.transform.InverseTransformPoint(position);
		if (this.wakeEffect != null)
		{
			this.wakeEffect.gameObject.transform.localPosition = vector;
		}
		float num = Vector3.Dot(WaterSystem.GetNormal(this.boatRear.position), Vector3.up);
		bool flag = this.boatRear.position.y < height;
		bool flag2 = num >= 0.98f;
		bool flag3 = flag && flag2 && base.HasFlag(BaseEntity.Flags.Reserved2);
		if (this.wakeEffect != null && flag3 != this.wasWakeOn)
		{
			if (flag3)
			{
				this.wakeEffect.Play();
			}
			else
			{
				this.wakeEffect.Stop();
			}
		}
		bool flag4 = flag && base.HasFlag(BaseEntity.Flags.Reserved2);
		if (flag4 != this.wasEngineSprayOn)
		{
			if (flag4)
			{
				this.engineEffectThrottle.Play();
			}
			else
			{
				this.engineEffectThrottle.Stop();
			}
		}
		bool flag5 = flag && base.HasFlag(BaseEntity.Flags.Reserved1) && !flag4;
		Vector3 localPosition = this.engineEffectIdle.gameObject.transform.localPosition;
		localPosition.y = vector.y;
		this.engineEffectIdle.gameObject.transform.localPosition = localPosition;
		if (flag5 != this.wasEngineIdleOn)
		{
			if (flag5)
			{
				this.engineEffectIdle.Play();
			}
			else
			{
				this.engineEffectIdle.Stop();
			}
		}
		this.wasEngineIdleOn = flag5;
		this.wasWakeOn = flag3;
		this.wasEngineSprayOn = flag4;
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0003D2E0 File Offset: 0x0003B4E0
	public void UpdateEffects()
	{
		this.causticsProjector.enabled = (MainCamera.Distance(this) < 50f && TerrainMeta.WaterMap != null);
		if (!this.causticsProjector.enabled)
		{
			return;
		}
		float num = 1f;
		float num2 = Mathf.InverseLerp(2f, 3f, this.averageVelocity.Speed);
		Vector3 normalized = (MainCamera.position - base.transform.position).normalized;
		float num3 = Vector3.Dot(base.transform.up, normalized);
		num3 = Mathf.InverseLerp(0.7f, 0.8f, num3);
		bool flag = WaterSystem.GetHeight(this.causticsDepthTest.transform.position) >= this.causticsDepthTest.transform.position.y;
		num *= 1f - num2;
		num *= 1f - num3;
		if (!flag || base.IsFlipped())
		{
			num = 0f;
		}
		this.causticsAlpha = Mathf.MoveTowards(this.causticsAlpha, num, Time.deltaTime * 2f);
		Color color = this.causticsColor;
		color *= this.causticsAlpha;
		this.causticsMaterial.SetColor("_Color", color);
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0003D420 File Offset: 0x0003B620
	public override void InitializeClientEffects()
	{
		base.InitializeClientEffects();
		this.causticsMaterial = new Material(this.causticsProjector.material);
		this.causticsColor = this.causticsMaterial.GetColor("_Color");
		this.causticsProjector.material = this.causticsMaterial;
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void ShutdownClientEffects()
	{
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0003D470 File Offset: 0x0003B670
	public virtual void UpdateEngineRotation()
	{
		float num = 0f;
		if (base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			num += 30f;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved4))
		{
			num -= 30f;
		}
		this.currentEngineRotation = Mathf.Lerp(this.currentEngineRotation, num, Time.deltaTime * 4f);
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

	// Token: 0x060004D5 RID: 1237 RVA: 0x000063E4 File Offset: 0x000045E4
	public override void SetNetworkPosition(Vector3 vPos)
	{
		base.SetNetworkPosition(vPos);
		this.averageVelocity.Record(vPos);
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x000063F9 File Offset: 0x000045F9
	public override bool MountMenuVisible()
	{
		return base.healthFraction > 0f && !this.LookingAtEngine(LocalPlayer.Entity) && !base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00006425 File Offset: 0x00004625
	public override bool DisplayHealthInfo(BasePlayer player)
	{
		return this.ShowHealthInfo && base.healthFraction <= 0.75f && base.healthFraction > 0f;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0000644B File Offset: 0x0000464B
	[BaseEntity.Menu.Description("push_desc", "Push this object")]
	[BaseEntity.Menu("push", "Push", Order = -100)]
	[BaseEntity.Menu.Icon("player_carry")]
	[BaseEntity.Menu.ShowIf("Menu_Push_ShowIf")]
	public void Menu_Push(BasePlayer player)
	{
		base.ServerRPC("RPC_WantsPush");
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00006458 File Offset: 0x00004658
	public bool Menu_Push_ShowIf(BasePlayer player)
	{
		return !player.isMounted && player.IsOnGround() && base.healthFraction > 0f && this.ShowPushMenu();
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0003D564 File Offset: 0x0003B764
	public virtual bool ShowPushMenu()
	{
		BasePlayer entity = LocalPlayer.Entity;
		Ray ray = new Ray(entity.transform.position + Vector3.up * (0.25f + entity.GetRadius()), Vector3.down);
		RaycastHit hit;
		if (!base.IsFlipped() && Physics.SphereCast(ray, entity.GetRadius() * 0.95f, ref hit, 4f, 8192))
		{
			BaseEntity entity2 = hit.GetEntity();
			if (entity2 != null && entity2.EqualNetID(this))
			{
				return false;
			}
		}
		return base.HasFlag(BaseEntity.Flags.Reserved7) && (LocalPlayer.Entity.WaterFactor() <= 0.6f || base.IsFlipped());
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0000647F File Offset: 0x0000467F
	public virtual bool LookingAtEngine(BasePlayer player)
	{
		return !(player == null) && !(player.lookingAtCollider == null) && player.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x000064AA File Offset: 0x000046AA
	public virtual bool LookingAtFuelArea(BasePlayer player)
	{
		return this.LookingAtEngine(player);
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x000064B3 File Offset: 0x000046B3
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu.ShowIf("Menu_StartEngine_ShowIf")]
	[BaseEntity.Menu("startboatengine", "Start Engine", Order = -90)]
	[BaseEntity.Menu.Description("startboatengine_desc", "Start the boat engine")]
	public void Menu_StartEngine(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_EngineToggle", true);
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x000064C1 File Offset: 0x000046C1
	public bool Menu_StartEngine_ShowIf(BasePlayer player)
	{
		return !base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved6) && !base.IsFlipped() && this.LookingAtEngine(player) && player.isMounted && !base.InDryDock();
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00006501 File Offset: 0x00004701
	[BaseEntity.Menu.Description("stopboatengine_desc", "Stop the boat engine")]
	[BaseEntity.Menu.ShowIf("Menu_StopEngine_ShowIf")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu("stopboatengine", "Stop Engine", Order = -90)]
	public void Menu_StopEngine(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_EngineToggle", false);
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0000650F File Offset: 0x0000470F
	public bool Menu_StopEngine_ShowIf(BasePlayer player)
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1) && !base.IsFlipped() && this.LookingAtEngine(player) && player.isMounted;
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x00003059 File Offset: 0x00001259
	[BaseEntity.Menu.Icon("open")]
	[BaseEntity.Menu.ShowIf("Menu_FuelStorage_ShowIf")]
	[BaseEntity.Menu.Description("fuelstorage_desc", "Open the fuel storage")]
	[BaseEntity.Menu("Fuel Storage", "Fuel Storage", Order = -80)]
	public void Menu_FuelStorage(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenFuel");
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x00006537 File Offset: 0x00004737
	public bool Menu_FuelStorage_ShowIf(BasePlayer player)
	{
		return !base.IsFlipped() && this.LookingAtFuelArea(player) && player.isMounted;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0003D614 File Offset: 0x0003B814
	public override void OnSignal(BaseEntity.Signal signal, string arg)
	{
		base.OnSignal(signal, arg);
		float hardness;
		if (signal == BaseEntity.Signal.PhysImpact && float.TryParse(arg, ref hardness))
		{
			PhysicsEffects component = base.gameObject.GetComponent<PhysicsEffects>();
			if (component != null)
			{
				component.PlayImpactSound(hardness);
			}
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0003D654 File Offset: 0x0003B854
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.motorBoat != null)
		{
			this.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0003D6AC File Offset: 0x0003B8AC
	public void UpdateSounds()
	{
		bool flag = base.HasFlag(BaseEntity.Flags.Reserved1);
		bool flag2 = base.HasFlag(BaseEntity.Flags.Reserved2);
		bool shouldPlay = base.HasFlag(BaseEntity.Flags.Reserved5);
		int num = 0;
		if (base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			num++;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved4))
		{
			num--;
		}
		float num2 = flag2 ? 1f : 0f;
		if (flag && !this.wasEngineOn && this.engineStartSoundDef != null && this.engineRotate != null)
		{
			SoundManager.PlayOneshot(this.engineStartSoundDef, this.engineRotate.gameObject, false, default(Vector3));
		}
		if (!flag && this.wasEngineOn && this.engineStopSoundDef != null && this.engineRotate != null)
		{
			SoundManager.PlayOneshot(this.engineStopSoundDef, this.engineRotate.gameObject, false, default(Vector3));
		}
		Vector3 rhs = Vector3.Cross(Vector3.up, base.transform.right);
		float num3 = Vector3.Dot(this.averageVelocity.Average.normalized, rhs);
		float value = Vector3.Dot(base.transform.forward, Vector3.up);
		num3 = Mathf.Clamp01(-num3);
		this.directionalPitchModScale = Mathf.Lerp(this.directionalPitchModScale, Mathf.Clamp01(this.averageVelocity.Average.magnitude / 5f), Time.deltaTime * 4f);
		float num4 = Mathf.InverseLerp(1f, 0.8f, num3) * this.turnPitchModScale;
		float num5 = Mathf.InverseLerp(0f, 2f, value) * this.tiltPitchModScale;
		this.pitch = num2 + (num4 + num5) * this.directionalPitchModScale;
		this.speed = this.averageVelocity.Average.magnitude / this.waterSpeedDivisor;
		this.engineLoops.blend = this.pitch;
		this.waterLoops.blend = this.speed;
		this.engineLoops.shouldPlay = flag;
		this.waterLoops.shouldPlay = shouldPlay;
		if (Time.time > this.nextSplashAccent)
		{
			if (this.speed > 1f)
			{
				SoundManager.PlayOneshot(this.movementSplashAccentSoundDef, base.gameObject, false, default(Vector3));
			}
			this.nextSplashAccent = Time.time + Random.Range(this.splashAccentFrequencyMin, this.splashAccentFrequencyMax);
		}
		if (num != this.prevSteering && this.engineRotate != null)
		{
			SoundManager.PlayOneshot(this.engineSteerSoundDef, this.engineRotate.gameObject, false, default(Vector3));
		}
		this.prevSteering = num;
		this.wasEngineOn = flag;
	}
}
