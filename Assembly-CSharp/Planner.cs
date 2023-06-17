using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using GameTips;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class Planner : HeldEntity
{
	// Token: 0x04000630 RID: 1584
	private Vector3 rotationOffset = Vector3.zero;

	// Token: 0x04000631 RID: 1585
	[NonSerialized]
	public Construction currentConstruction;

	// Token: 0x04000632 RID: 1586
	public BaseEntity[] buildableList;

	// Token: 0x04000633 RID: 1587
	internal Planner.Guide guide = new Planner.Guide();

	// Token: 0x0600098E RID: 2446 RVA: 0x00050FA0 File Offset: 0x0004F1A0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Planner.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0000999B File Offset: 0x00007B9B
	public override void OnFrame()
	{
		base.OnFrame();
		this.UpdateGuide();
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00050FE4 File Offset: 0x0004F1E4
	public override void OnDeploy()
	{
		base.OnDeploy();
		if (this.isTypeDeployable)
		{
			ItemModDeployable modDeployable = this.GetModDeployable();
			if (modDeployable)
			{
				this.buildableList = new BaseEntity[]
				{
					modDeployable.entityPrefab.Get().GetComponent<BaseEntity>()
				};
			}
		}
		if (this.currentConstruction == null)
		{
			this.SetDefaultPlan();
		}
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x000099A9 File Offset: 0x00007BA9
	public override void OnHolstered()
	{
		base.OnHolstered();
		this.CloseGuide();
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00051044 File Offset: 0x0004F244
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
		{
			this.DoBuild();
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY))
		{
			this.OpenComponentMenu();
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.RELOAD))
		{
			Vector3 vector = Vector3.zero;
			if (this.currentConstruction && this.currentConstruction.canRotate)
			{
				vector = this.currentConstruction.rotationAmount;
			}
			this.rotationOffset.x = Mathf.Repeat(this.rotationOffset.x + vector.x, 360f);
			this.rotationOffset.y = Mathf.Repeat(this.rotationOffset.y + vector.y, 360f);
			this.rotationOffset.z = Mathf.Repeat(this.rotationOffset.z + vector.z, 360f);
		}
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0005115C File Offset: 0x0004F35C
	private void OpenComponentMenu()
	{
		if (this.isTypeDeployable)
		{
			return;
		}
		HowToOpenBuildOptions.BuildOptionChanged();
		ContextMenuUI.Start(ContextMenuUI.MenuType.RightClick);
		BaseEntity[] array = this.buildableList;
		for (int i = 0; i < array.Length; i++)
		{
			BaseEntity baseEntity = array[i];
			BaseEntity baseEntity2 = baseEntity;
			Construction construction = PrefabAttribute.client.Find<Construction>(baseEntity2.prefabID);
			if (construction == null)
			{
				Debug.Log("Missing Construction Prefab or something: " + baseEntity2.prefabID);
			}
			else
			{
				string requirements = LocalPlayer.BuildItemRequiredString(construction.defaultGrade.costToBuild);
				ContextMenuUI.AddOption(construction.info.name.token, construction.info.description.token, construction.info.icon, delegate(BasePlayer ply)
				{
					this.SwitchConstruction(construction);
				}, construction.info.order, false, construction == this.currentConstruction, requirements);
			}
		}
		ContextMenuUI.End();
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x000099B7 File Offset: 0x00007BB7
	private void SwitchConstruction(Construction c)
	{
		this.currentConstruction = c;
		this.rotationOffset = Vector3.zero;
		this.UpdateGuide();
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00051280 File Offset: 0x0004F480
	private void DoBuild()
	{
		if (this.currentConstruction == null)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.CanBuild() && !this.currentConstruction.canBypassBuildingPermission)
		{
			return;
		}
		if (this.guide == null)
		{
			return;
		}
		if (!this.guide.IsValid())
		{
			return;
		}
		if (!this.guide.lastPlacement.valid)
		{
			ownerPlayer.ChatMessage(Construction.lastPlacementError);
			return;
		}
		if (!this.isTypeDeployable)
		{
			Analytics.PlacedBlocks++;
		}
		using (CreateBuilding createBuilding = Pool.Get<CreateBuilding>())
		{
			if (this.guide.lastPlacement.entity != null && this.guide.lastPlacement.entity.net != null)
			{
				createBuilding.entity = this.guide.lastPlacement.entity.net.ID;
			}
			if (this.guide.lastPlacement.socket != null)
			{
				createBuilding.socket = StringPool.Get(this.guide.lastPlacement.socket.socketName);
				if (createBuilding.socket == 0U)
				{
					Debug.Log("Socket name not pooled! " + this.guide.lastPlacement.socket.socketName);
					return;
				}
			}
			createBuilding.blockID = this.currentConstruction.prefabID;
			createBuilding.ray = this.guide.lastPlacement.ray;
			createBuilding.onterrain = this.guide.lastPlacement.onTerrain;
			createBuilding.position = this.guide.lastPlacement.position;
			createBuilding.normal = this.guide.lastPlacement.normal;
			createBuilding.rotation = this.guide.lastPlacement.rotation;
			if (createBuilding.entity > 0U)
			{
				createBuilding.position = this.guide.lastPlacement.entity.transform.InverseTransformPoint(createBuilding.position);
				createBuilding.normal = this.guide.lastPlacement.entity.transform.InverseTransformDirection(createBuilding.normal);
				createBuilding.rotation = Quaternion.Inverse(this.guide.lastPlacement.entity.transform.rotation) * createBuilding.rotation;
			}
			base.ServerRPC<CreateBuilding>("DoPlace", createBuilding);
		}
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x000099D1 File Offset: 0x00007BD1
	private void SetDefaultPlan()
	{
		this.currentConstruction = PrefabAttribute.client.Find<Construction>(this.buildableList[0].prefabID);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x00051504 File Offset: 0x0004F704
	public override bool NeedsCrosshair()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		return modDeployable && modDeployable.showCrosshair;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0004D2D8 File Offset: 0x0004B4D8
	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		if (ownerItemDefinition == null)
		{
			return null;
		}
		return ownerItemDefinition.GetComponent<ItemModDeployable>();
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0005152C File Offset: 0x0004F72C
	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (modDeployable == null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x0600099A RID: 2458 RVA: 0x000099F0 File Offset: 0x00007BF0
	public bool isTypeDeployable
	{
		get
		{
			return this.GetModDeployable() != null;
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00051554 File Offset: 0x0004F754
	private void UpdateGuide()
	{
		if (this.currentConstruction == null)
		{
			this.CloseGuide();
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		Construction.Target target = default(Construction.Target);
		target.ray = ownerPlayer.eyes.BodyRay();
		target.player = ownerPlayer;
		this.FillPlacement(ref target, this.currentConstruction);
		this.guide.Update(ref target, this.currentConstruction);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x000099FE File Offset: 0x00007BFE
	private void CloseGuide()
	{
		this.guide.Shutdown();
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x000515CC File Offset: 0x0004F7CC
	internal void FillPlacement(ref Construction.Target target, Construction component)
	{
		if (component.socketHandle)
		{
			component.socketHandle.AdjustTarget(ref target, component.maxplaceDistance);
		}
		this.FindAppropriateHandle(ref target, component);
		if (target.valid)
		{
			return;
		}
		this.FindTerrainPlacement(ref target, component);
		bool valid = target.valid;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0005161C File Offset: 0x0004F81C
	internal void FindTerrainPlacement(ref Construction.Target target, Construction component)
	{
		RaycastHit raycastHit;
		if (GamePhysics.Trace(target.ray, 0f, out raycastHit, component.maxplaceDistance + 0f, 161546496, 1))
		{
			target.position = target.ray.origin + target.ray.direction * raycastHit.distance;
			target.normal = raycastHit.normal;
			target.rotation = this.rotationOffset;
			target.onTerrain = true;
			target.valid = true;
			if (raycastHit.collider)
			{
				target.entity = raycastHit.collider.gameObject.ToBaseEntity();
				return;
			}
		}
		else
		{
			target.position = target.ray.origin + target.ray.direction * component.maxplaceDistance;
			target.normal = Vector3.up;
			target.rotation = this.rotationOffset;
			target.onTerrain = component.canPlaceAtMaxDistance;
			target.valid = component.canPlaceAtMaxDistance;
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00051724 File Offset: 0x0004F924
	internal bool FindAppropriateHandle(ref Construction.Target target, Construction component)
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		BasePlayer player = target.player;
		Ray ray = target.ray;
		float num = float.MaxValue;
		target.valid = false;
		Vis.Entities<BaseEntity>(ray.origin, component.maxplaceDistance * 2f, list, 18874625, 2);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.isServer)
			{
				Construction construction = PrefabAttribute.client.Find<Construction>(baseEntity.prefabID);
				if (!(construction == null))
				{
					foreach (Socket_Base socket_Base in construction.allSockets)
					{
						RaycastHit raycastHit;
						if (socket_Base.female && !socket_Base.femaleDummy && socket_Base.GetSelectBounds(baseEntity.transform.position, baseEntity.transform.rotation).Trace(ray, ref raycastHit, float.PositiveInfinity) && raycastHit.distance >= 1f && raycastHit.distance <= num && !baseEntity.IsOccupied(socket_Base))
						{
							Construction.Target target2 = new Construction.Target
							{
								socket = socket_Base,
								entity = baseEntity,
								ray = ray,
								valid = true,
								player = player,
								rotation = this.rotationOffset
							};
							if (component.HasMaleSockets(target2))
							{
								target = target2;
								num = raycastHit.distance;
							}
						}
					}
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return target.valid;
	}

	// Token: 0x020000AE RID: 174
	public class Guide
	{
		// Token: 0x04000634 RID: 1588
		public GameObject guideObject;

		// Token: 0x04000635 RID: 1589
		public Construction.Target lastPlacement;

		// Token: 0x04000636 RID: 1590
		private Construction component;

		// Token: 0x04000637 RID: 1591
		private Material goodMat;

		// Token: 0x04000638 RID: 1592
		private Material neutralMat;

		// Token: 0x04000639 RID: 1593
		private Material badMat;

		// Token: 0x0400063A RID: 1594
		private bool wasTransparent = true;

		// Token: 0x060009A1 RID: 2465 RVA: 0x00009A29 File Offset: 0x00007C29
		public bool IsValid()
		{
			return this.guideObject != null;
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x000518E8 File Offset: 0x0004FAE8
		private void UpdateComponent(Construction c)
		{
			if (this.guideObject)
			{
				GameManager.Destroy(this.guideObject, 0f);
				this.guideObject = null;
			}
			this.component = c;
			if (this.component == null)
			{
				return;
			}
			this.guideObject = this.component.CreateGuideSkin();
			if (this.guideObject != null)
			{
				this.BecomeInvalid();
			}
			this.lastPlacement.valid = false;
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00051960 File Offset: 0x0004FB60
		public void Update(ref Construction.Target placement, Construction currentComponent)
		{
			if (this.guideObject == null || this.component == null || currentComponent == null || currentComponent.fullName != this.component.fullName)
			{
				this.UpdateComponent(currentComponent);
			}
			if (this.guideObject == null)
			{
				return;
			}
			Construction.lastPlacementError = "No Error";
			Vector3 position = this.guideObject.transform.position;
			Quaternion rotation = this.guideObject.transform.rotation;
			Vector3 direction = placement.ray.direction;
			direction.y = 0f;
			direction.Normalize();
			this.guideObject.transform.position = placement.ray.origin + placement.ray.direction * currentComponent.maxplaceDistance;
			this.guideObject.transform.rotation = Quaternion.Euler(placement.rotation) * Quaternion.LookRotation(direction);
			bool flag = currentComponent.UpdatePlacement(this.guideObject.transform, this.component, ref placement);
			if (Input.GetKey(KeyCode.KeypadDivide))
			{
				Debug.Log("Planner.Guide update: " + Construction.lastPlacementError);
			}
			bool flag2 = WaterLevel.Test(this.guideObject.transform.position + new Vector3(0f, currentComponent.bounds.min.y, 0f));
			this.UpdateGuideTransparency(!flag2);
			if (MainCamera.mainCamera)
			{
				this.guideObject.transform.position = this.guideObject.transform.position + (MainCamera.position - this.guideObject.transform.position).normalized * 0.05f;
			}
			if (flag)
			{
				if (placement.inBuildingPrivilege)
				{
					if (!this.lastPlacement.valid || !this.lastPlacement.inBuildingPrivilege)
					{
						this.BecomeNeutral();
					}
				}
				else if (!this.lastPlacement.valid || this.lastPlacement.inBuildingPrivilege)
				{
					this.BecomeValid();
				}
				this.lastPlacement = placement;
				return;
			}
			if (!this.lastPlacement.valid)
			{
				return;
			}
			if (Vector3.Distance(position, this.guideObject.transform.position) < 0.25f && currentComponent.UpdatePlacement(this.guideObject.transform, this.component, ref this.lastPlacement))
			{
				this.guideObject.transform.position = position;
				this.guideObject.transform.rotation = rotation;
				return;
			}
			this.lastPlacement.valid = false;
			this.BecomeInvalid();
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00009A37 File Offset: 0x00007C37
		public void BecomeValid()
		{
			MaterialReplacement.ReplaceRecursive(this.guideObject, this.CacheGoodMaterial());
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00009A4A File Offset: 0x00007C4A
		public void BecomeNeutral()
		{
			MaterialReplacement.ReplaceRecursive(this.guideObject, this.CacheNeutralMaterial());
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00009A5D File Offset: 0x00007C5D
		public void BecomeInvalid()
		{
			MaterialReplacement.ReplaceRecursive(this.guideObject, this.CacheBadMaterial());
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00051C1C File Offset: 0x0004FE1C
		public void Shutdown()
		{
			if (this.guideObject != null)
			{
				GameManager.Destroy(this.guideObject, 0f);
				this.guideObject = null;
			}
			this.ReleaseMaterialInstance(ref this.goodMat);
			this.ReleaseMaterialInstance(ref this.neutralMat);
			this.ReleaseMaterialInstance(ref this.badMat);
			this.wasTransparent = true;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00009A70 File Offset: 0x00007C70
		private Material CacheMaterialInstance(Material asset, ref Material inst)
		{
			if (inst == null)
			{
				inst = Object.Instantiate<Material>(asset);
			}
			return inst;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00009A86 File Offset: 0x00007C86
		private void ReleaseMaterialInstance(ref Material mat)
		{
			if (mat != null)
			{
				Object.Destroy(mat);
				mat = null;
			}
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00009A9C File Offset: 0x00007C9C
		private Material CacheGoodMaterial()
		{
			return this.CacheMaterialInstance(FileSystem.Load<Material>("Assets/Content/materials/guide_good.mat", true), ref this.goodMat);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00009AB5 File Offset: 0x00007CB5
		private Material CacheNeutralMaterial()
		{
			return this.CacheMaterialInstance(FileSystem.Load<Material>("Assets/Content/materials/guide_neutral.mat", true), ref this.neutralMat);
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00009ACE File Offset: 0x00007CCE
		private Material CacheBadMaterial()
		{
			return this.CacheMaterialInstance(FileSystem.Load<Material>("Assets/Content/materials/guide_bad.mat", true), ref this.badMat);
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x00051C7C File Offset: 0x0004FE7C
		private void UpdateGuideTransparency(bool transparent)
		{
			if (this.wasTransparent != transparent)
			{
				this.CacheGoodMaterial().renderQueue = (transparent ? 3000 : 2500);
				this.CacheNeutralMaterial().renderQueue = (transparent ? 3000 : 2500);
				this.CacheBadMaterial().renderQueue = (transparent ? 3000 : 2500);
				this.wasTransparent = transparent;
			}
		}
	}
}
