using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using GameTips;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000080 RID: 128
public class BuildingBlock : StabilityEntity
{
	// Token: 0x04000509 RID: 1289
	[NonSerialized]
	public Construction blockDefinition;

	// Token: 0x0400050A RID: 1290
	private static Vector3[] outsideLookupOffsets = new Vector3[]
	{
		new Vector3(0f, 1f, 0f).normalized,
		new Vector3(1f, 1f, 0f).normalized,
		new Vector3(-1f, 1f, 0f).normalized,
		new Vector3(0f, 1f, 1f).normalized,
		new Vector3(0f, 1f, -1f).normalized
	};

	// Token: 0x0400050B RID: 1291
	private bool forceSkinRefresh;

	// Token: 0x0400050C RID: 1292
	private int modelState;

	// Token: 0x0400050D RID: 1293
	private int lastModelState;

	// Token: 0x0400050E RID: 1294
	public BuildingGrade.Enum grade;

	// Token: 0x0400050F RID: 1295
	private BuildingGrade.Enum lastGrade = BuildingGrade.Enum.None;

	// Token: 0x04000510 RID: 1296
	private ConstructionSkin currentSkin;

	// Token: 0x04000511 RID: 1297
	private DeferredAction skinChange;

	// Token: 0x04000512 RID: 1298
	private MeshRenderer placeholderRenderer;

	// Token: 0x04000513 RID: 1299
	private MeshCollider placeholderCollider;

	// Token: 0x04000514 RID: 1300
	private static Material HighlightMaterial;

	// Token: 0x04000515 RID: 1301
	public static BuildingBlock.UpdateSkinWorkQueue updateSkinQueueClient = new BuildingBlock.UpdateSkinWorkQueue();

	// Token: 0x060007F8 RID: 2040 RVA: 0x0004A28C File Offset: 0x0004848C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BuildingBlock.OnRpcMessage", 0.1f))
		{
			if (rpc == 1307514308U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: RefreshSkin ");
				}
				using (TimeWarning.New("RefreshSkin", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RefreshSkin(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in RefreshSkin", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x00008792 File Offset: 0x00006992
	protected override void ClientInit(Entity info)
	{
		this.blockDefinition = PrefabAttribute.client.Find<Construction>(this.prefabID);
		base.ClientInit(info);
		this.UpdateGrade();
		this.UpdateSkin(false);
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x000087BE File Offset: 0x000069BE
	public override void PostNetworkUpdate()
	{
		this.UpdateGrade();
		this.UpdateSkin(false);
		base.PostNetworkUpdate();
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0004A3A8 File Offset: 0x000485A8
	public List<Option> GetBuildMenu(BasePlayer player)
	{
		List<Option> result = new List<Option>();
		this.GradeChangingOptions(ref result, player);
		this.RotationOptions(ref result, player);
		this.DemolishOptions(ref result, player);
		return result;
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x000087D3 File Offset: 0x000069D3
	[BaseEntity.RPC_Client]
	private void RefreshSkin(BaseEntity.RPCMessage msg)
	{
		if (this.currentSkin)
		{
			this.currentSkin.Refresh(this);
		}
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x000087EE File Offset: 0x000069EE
	public override void DoDestroyEffects(BaseNetworkable.DestroyMode mode, Message msg)
	{
		if (mode == BaseNetworkable.DestroyMode.Gib && this.currentSkin != null)
		{
			base.gameObject.CustomGib();
		}
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool ShouldLerp()
	{
		return false;
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool NeedsCrosshair()
	{
		return false;
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0000880D File Offset: 0x00006A0D
	public override void ResetState()
	{
		base.ResetState();
		this.blockDefinition = null;
		this.forceSkinRefresh = false;
		this.modelState = 0;
		this.lastModelState = 0;
		this.grade = BuildingGrade.Enum.Twigs;
		this.lastGrade = BuildingGrade.Enum.None;
		this.DestroySkin();
		this.UpdatePlaceholder(true);
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0000884C File Offset: 0x00006A4C
	public override void InitShared()
	{
		base.InitShared();
		this.placeholderRenderer = base.GetComponent<MeshRenderer>();
		this.placeholderCollider = base.GetComponent<MeshCollider>();
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0000886C File Offset: 0x00006A6C
	public override void PostInitShared()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
		this.grade = this.currentGrade.gradeBase.type;
		base.PostInitShared();
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x00005DD3 File Offset: 0x00003FD3
	public override void DestroyShared()
	{
		base.DestroyShared();
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x000088A0 File Offset: 0x00006AA0
	public override string Categorize()
	{
		return "building";
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x00005B85 File Offset: 0x00003D85
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0004A3D8 File Offset: 0x000485D8
	public override bool IsOutside()
	{
		float num = 50f;
		Vector3 a = base.PivotPoint();
		for (int i = 0; i < BuildingBlock.outsideLookupOffsets.Length; i++)
		{
			Vector3 a2 = BuildingBlock.outsideLookupOffsets[i];
			Vector3 origin = a + a2 * num;
			if (!Physics.Raycast(new Ray(origin, -a2), num - 0.5f, 2097152))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x000088A7 File Offset: 0x00006AA7
	private bool CanDemolish(BasePlayer player)
	{
		return this.IsDemolishable() && this.HasDemolishPrivilege(player);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x000088BA File Offset: 0x00006ABA
	private bool IsDemolishable()
	{
		return Server.pve || base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x000088D3 File Offset: 0x00006AD3
	private bool HasDemolishPrivilege(BasePlayer player)
	{
		return player.IsBuildingAuthed(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x000088F7 File Offset: 0x00006AF7
	private void Demolish(BasePlayer player)
	{
		if (!this.CanDemolish(player))
		{
			return;
		}
		base.ServerRPC("DoDemolish");
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0004A444 File Offset: 0x00048644
	private void DemolishOptions(ref List<Option> options, BasePlayer player)
	{
		if (this.blockDefinition.grades == null)
		{
			return;
		}
		Option option = default(Option);
		option.title = "demolish";
		option.icon = "demolish";
		option.desc = "demolish_desc";
		option.show = this.CanDemolish(player);
		option.function = new Action<BasePlayer>(this.Demolish);
		option.order = 1;
		options.Add(option);
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0000890E File Offset: 0x00006B0E
	public void SetConditionalModel(int state)
	{
		this.modelState = state;
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x00008917 File Offset: 0x00006B17
	public bool GetConditionalModel(int index)
	{
		return (this.modelState & 1 << index) != 0;
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x0600080E RID: 2062 RVA: 0x0004A4BC File Offset: 0x000486BC
	public ConstructionGrade currentGrade
	{
		get
		{
			ConstructionGrade constructionGrade = this.GetGrade(this.grade);
			if (constructionGrade != null)
			{
				return constructionGrade;
			}
			for (int i = 0; i < this.blockDefinition.grades.Length; i++)
			{
				if (this.blockDefinition.grades[i] != null)
				{
					return this.blockDefinition.grades[i];
				}
			}
			Debug.LogWarning("Building block grade not found: " + this.grade);
			return null;
		}
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0004A538 File Offset: 0x00048738
	private ConstructionGrade GetGrade(BuildingGrade.Enum iGrade)
	{
		if (this.grade >= (BuildingGrade.Enum)this.blockDefinition.grades.Length)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Grade out of range ",
				base.gameObject,
				" ",
				this.grade,
				" / ",
				this.blockDefinition.grades.Length
			}));
			return this.blockDefinition.defaultGrade;
		}
		return this.blockDefinition.grades[(int)iGrade];
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00008929 File Offset: 0x00006B29
	private bool CanChangeToGrade(BuildingGrade.Enum iGrade, BasePlayer player)
	{
		return this.HasUpgradePrivilege(iGrade, player) && !this.IsUpgradeBlocked();
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0004A5C8 File Offset: 0x000487C8
	private bool HasUpgradePrivilege(BuildingGrade.Enum iGrade, BasePlayer player)
	{
		return iGrade != this.grade && iGrade < (BuildingGrade.Enum)this.blockDefinition.grades.Length && iGrade >= BuildingGrade.Enum.Twigs && iGrade >= this.grade && !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0004A628 File Offset: 0x00048828
	private bool IsUpgradeBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnUpgrade)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.client.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, volumes, ~(1 << base.gameObject.layer));
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0004A684 File Offset: 0x00048884
	private bool CanAffordUpgrade(BuildingGrade.Enum iGrade, BasePlayer player)
	{
		foreach (ItemAmount itemAmount in this.GetGrade(iGrade).costToBuild)
		{
			if ((float)player.inventory.GetAmount(itemAmount.itemid) < itemAmount.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0004A6F8 File Offset: 0x000488F8
	public void SetGrade(BuildingGrade.Enum iGradeID)
	{
		if (this.blockDefinition.grades == null || iGradeID >= (BuildingGrade.Enum)this.blockDefinition.grades.Length)
		{
			Debug.LogError("Tried to set to undefined grade! " + this.blockDefinition.fullName, base.gameObject);
			return;
		}
		this.grade = iGradeID;
		this.grade = this.currentGrade.gradeBase.type;
		this.UpdateGrade();
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x00008940 File Offset: 0x00006B40
	private void UpdateGrade()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x00008958 File Offset: 0x00006B58
	private void UpgradeToGrade(BuildingGrade.Enum i, BasePlayer player)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (!this.CanChangeToGrade(i, player))
		{
			return;
		}
		HowToHammerUpgrade.UpgradeHappened();
		base.ServerRPC<int>("DoUpgradeToGrade", (int)i);
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0004A768 File Offset: 0x00048968
	private void GradeChangingOptions(ref List<Option> options, BasePlayer player)
	{
		if (this.blockDefinition.grades != null)
		{
			for (int i = 0; i < this.blockDefinition.grades.Length; i++)
			{
				ConstructionGrade constructionGrade = this.blockDefinition.grades[i];
				if (!(constructionGrade == null) && i != 0)
				{
					BuildingGrade.Enum iNewGrade = (BuildingGrade.Enum)i;
					Option option = default(Option);
					option.title = constructionGrade.gradeBase.upgradeMenu.name.token;
					option.desc = constructionGrade.gradeBase.upgradeMenu.description.token;
					option.iconSprite = constructionGrade.gradeBase.upgradeMenu.icon;
					option.order = constructionGrade.gradeBase.upgradeMenu.order;
					option.show = true;
					if (i < (int)this.grade)
					{
						option.requirements = Translate.Get("grade_is_lower", null);
						option.showDisabled = true;
					}
					else if (i == (int)this.grade)
					{
						option.requirements = Translate.Get("grade_is_current", null);
						option.showDisabled = true;
					}
					else if (!this.HasUpgradePrivilege(iNewGrade, player))
					{
						option.requirements = Translate.Get("building_blocked", null);
						option.showDisabled = true;
					}
					else if (this.IsUpgradeBlocked())
					{
						option.requirements = Translate.Get("upgrade_blocked", null);
						option.showDisabled = true;
					}
					else
					{
						option.requirements = LocalPlayer.BuildItemRequiredString(constructionGrade.costToBuild);
						option.showDisabled = !this.CanAffordUpgrade(iNewGrade, player);
						option.function = delegate(BasePlayer ply)
						{
							this.UpgradeToGrade(iNewGrade, ply);
						};
					}
					options.Add(option);
				}
			}
		}
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0000897F File Offset: 0x00006B7F
	private bool NeedsSkinChange()
	{
		return this.currentSkin == null || this.forceSkinRefresh || this.lastGrade != this.grade || this.lastModelState != this.modelState;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0004A930 File Offset: 0x00048B30
	public void UpdateSkin(bool force = false)
	{
		if (force)
		{
			this.forceSkinRefresh = true;
		}
		if (!this.NeedsSkinChange())
		{
			return;
		}
		if (this.cachedStability <= 0f || base.isServer)
		{
			this.ChangeSkin();
			return;
		}
		if (!this.skinChange)
		{
			this.skinChange = new DeferredAction(this, new Action(this.ChangeSkin), ActionPriority.Medium);
		}
		if (!this.skinChange.Idle)
		{
			return;
		}
		this.skinChange.Invoke();
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x000089B8 File Offset: 0x00006BB8
	private void DestroySkin()
	{
		if (this.currentSkin != null)
		{
			this.currentSkin.Destroy(this);
			this.currentSkin = null;
		}
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x000089DB File Offset: 0x00006BDB
	private void UpdatePlaceholder(bool state)
	{
		if (this.placeholderRenderer)
		{
			this.placeholderRenderer.enabled = state;
		}
		if (this.placeholderCollider)
		{
			this.placeholderCollider.enabled = state;
		}
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0004A9AC File Offset: 0x00048BAC
	private void ChangeSkin()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		ConstructionGrade currentGrade = this.currentGrade;
		if (currentGrade.skinObject.isValid)
		{
			this.ChangeSkin(currentGrade.skinObject);
			return;
		}
		foreach (ConstructionGrade constructionGrade in this.blockDefinition.grades)
		{
			if (constructionGrade.skinObject.isValid)
			{
				this.ChangeSkin(constructionGrade.skinObject);
				return;
			}
		}
		Debug.LogWarning("No skins found for " + base.gameObject);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0004AA30 File Offset: 0x00048C30
	private void ChangeSkin(GameObjectRef prefab)
	{
		bool flag = this.lastGrade != this.grade;
		this.lastGrade = this.grade;
		if (flag)
		{
			if (this.currentSkin == null)
			{
				this.UpdatePlaceholder(false);
			}
			else
			{
				this.DestroySkin();
			}
			GameObject gameObject = base.gameManager.CreatePrefab(prefab.resourcePath, base.transform, true);
			this.currentSkin = gameObject.GetComponent<ConstructionSkin>();
			Model component = this.currentSkin.GetComponent<Model>();
			base.SetModel(component);
			Assert.IsTrue(this.model == component, "Didn't manage to set model successfully!");
		}
		bool flag2 = this.lastModelState != this.modelState;
		this.lastModelState = this.modelState;
		if (flag || flag2 || this.forceSkinRefresh)
		{
			this.currentSkin.Refresh(this);
			this.forceSkinRefresh = false;
		}
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00008A0F File Offset: 0x00006C0F
	public override bool ShouldBlockProjectiles()
	{
		return this.grade > BuildingGrade.Enum.Twigs;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00008A1A File Offset: 0x00006C1A
	public override float MaxHealth()
	{
		return this.currentGrade.maxHealth;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x00008A27 File Offset: 0x00006C27
	public override List<ItemAmount> BuildCost()
	{
		return this.currentGrade.costToBuild;
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0004AB08 File Offset: 0x00048D08
	public void DrawHighlight()
	{
		foreach (MeshRenderer meshRenderer in base.GetComponentsInChildren<MeshRenderer>())
		{
			if (meshRenderer.gameObject.layer == base.gameObject.layer)
			{
				this.Highlight(meshRenderer);
			}
		}
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0004AB50 File Offset: 0x00048D50
	public void Highlight(MeshRenderer renderer)
	{
		MeshFilter component = renderer.GetComponent<MeshFilter>();
		if (component == null)
		{
			return;
		}
		if (component.sharedMesh == null)
		{
			return;
		}
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetFloat("_AlphaTestRef", 0.4f + Mathf.Sin(UnityEngine.Time.time) * 0.3f);
		Vector3 b = Vector3.zero;
		if (BuildingBlock.HighlightMaterial == null)
		{
			BuildingBlock.HighlightMaterial = FileSystem.Load<Material>("Assets/Content/Materials/guide_highlight.mat", true);
		}
		if (MainCamera.isValid)
		{
			b = (MainCamera.position - base.transform.position).normalized * 0.003f;
		}
		for (int i = 0; i < component.sharedMesh.subMeshCount; i++)
		{
			string name = component.sharedMesh.name;
			if (!name.EndsWith("LOD1") && !name.EndsWith("LOD2") && !name.EndsWith("LOD3") && !name.EndsWith("collision"))
			{
				UnityEngine.Graphics.DrawMesh(component.sharedMesh, renderer.transform.position + b, renderer.transform.rotation, BuildingBlock.HighlightMaterial, base.gameObject.layer, null, i, materialPropertyBlock, false, true);
			}
		}
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00008A34 File Offset: 0x00006C34
	private bool CanRotate(BasePlayer player)
	{
		return this.IsRotatable() && this.HasRotationPrivilege(player) && !this.IsRotationBlocked();
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x00008A52 File Offset: 0x00006C52
	private bool IsRotatable()
	{
		return this.blockDefinition.grades != null && this.blockDefinition.canRotate && base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0004AC98 File Offset: 0x00048E98
	private bool IsRotationBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnRotate)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.client.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, volumes, ~(1 << base.gameObject.layer));
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x00008A82 File Offset: 0x00006C82
	private bool HasRotationPrivilege(BasePlayer player)
	{
		return !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x00008AA9 File Offset: 0x00006CA9
	private void Rotation(BasePlayer player)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (!this.CanRotate(player))
		{
			return;
		}
		base.ServerRPC("DoRotation");
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0004ACF4 File Offset: 0x00048EF4
	private void RotationOptions(ref List<Option> options, BasePlayer player)
	{
		if (this.blockDefinition.grades == null)
		{
			return;
		}
		if (!this.blockDefinition.canRotate)
		{
			return;
		}
		Option option = default(Option);
		option.title = "rotate";
		option.icon = "rotate";
		option.desc = "rotate_building_desc";
		option.order = 101;
		option.show = this.IsRotatable();
		if (!this.HasRotationPrivilege(player))
		{
			option.requirements = Translate.Get("building_blocked", null);
			option.showDisabled = true;
		}
		else if (this.IsRotationBlocked())
		{
			option.requirements = Translate.Get("rotation_blocked", null);
			option.showDisabled = true;
		}
		else
		{
			option.showDisabled = false;
			option.function = new Action<BasePlayer>(this.Rotation);
		}
		options.Add(option);
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0004ADCC File Offset: 0x00048FCC
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.buildingBlock != null)
		{
			this.SetConditionalModel(info.msg.buildingBlock.model);
			this.SetGrade((BuildingGrade.Enum)info.msg.buildingBlock.grade);
		}
	}

	// Token: 0x02000081 RID: 129
	public static class BlockFlags
	{
		// Token: 0x04000516 RID: 1302
		public const BaseEntity.Flags CanRotate = BaseEntity.Flags.Reserved1;

		// Token: 0x04000517 RID: 1303
		public const BaseEntity.Flags CanDemolish = BaseEntity.Flags.Reserved2;
	}

	// Token: 0x02000082 RID: 130
	public class UpdateSkinWorkQueue : ObjectWorkQueue<BuildingBlock>
	{
		// Token: 0x0600082C RID: 2092 RVA: 0x00008AD8 File Offset: 0x00006CD8
		protected override void RunJob(BuildingBlock entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.UpdateSkin(true);
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00008AEB File Offset: 0x00006CEB
		protected override bool ShouldAdd(BuildingBlock entity)
		{
			return entity.IsValid();
		}
	}
}
