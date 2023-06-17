using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000299 RID: 665
public class BaseCorpse : BaseCombatEntity
{
	// Token: 0x04000F5B RID: 3931
	public GameObjectRef prefabRagdoll;

	// Token: 0x04000F5C RID: 3932
	public BaseEntity parentEnt;

	// Token: 0x04000F5D RID: 3933
	[NonSerialized]
	internal ResourceDispenser resourceDispenser;

	// Token: 0x04000F5E RID: 3934
	internal GameObject ragdollObject;

	// Token: 0x060012B8 RID: 4792 RVA: 0x00079864 File Offset: 0x00077A64
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		Rigidbody pinTo = this.SetupRigidBody();
		if (this.prefabRagdoll.isValid)
		{
			this.ragdollObject = base.gameManager.CreatePrefab(this.prefabRagdoll.resourcePath, base.transform.position, base.transform.rotation, false);
			Ragdoll component = this.ragdollObject.GetComponent<Ragdoll>();
			if (component == null)
			{
				this.ragdollObject.SetActive(true);
				if (this.parentEnt != null && info.createdThisFrame)
				{
					this.parentEnt.OnBecameRagdoll(null);
				}
				return;
			}
			SkinnedMultiMesh component2 = this.ragdollObject.GetComponent<SkinnedMultiMesh>();
			if (component2 != null)
			{
				component2.BuildBoneDictionary();
			}
			component.pinTo = pinTo;
			if (this.parentEnt != null && info.createdThisFrame)
			{
				Model model = this.parentEnt.GetModel();
				component.CopyBonesFrom(model);
				this.parentEnt.OnBecameRagdoll(component);
				if (model.GetSkin() != -1)
				{
					component.SetRagdollSkin(model.GetSkin());
				}
			}
			this.ragdollObject.SetActive(true);
			PlayerModel[] componentsInChildren = component.GetComponentsInChildren<PlayerModel>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Rebuild(true);
			}
			this.model = this.ragdollObject.GetComponent<Model>();
			BaseEntityChild.Setup(this.ragdollObject, this);
			this.ragdollObject.AddComponent<BaseEntityChild>();
			component.MoveRigidbodiesToRoot();
		}
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x000799D4 File Offset: 0x00077BD4
	public override void ClientOnEnable()
	{
		base.ClientOnEnable();
		if (this.ragdollObject != null)
		{
			Ragdoll component = this.ragdollObject.GetComponent<Ragdoll>();
			if (component)
			{
				component.CreatePinJoint();
			}
		}
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x00079A10 File Offset: 0x00077C10
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		if (this.ragdollObject)
		{
			Ragdoll component = this.ragdollObject.GetComponent<Ragdoll>();
			if (component)
			{
				component.OnGibbed();
				this.ragdollObject.BroadcastOnParentDestroying();
			}
			GameManager.Destroy(this.ragdollObject, 0f);
		}
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x00079A68 File Offset: 0x00077C68
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (base.isClient && this.ragdollObject && this.ragdollObject.transform.parent != base.transform.parent)
		{
			this.ragdollObject.transform.parent = base.transform.parent;
		}
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x00079AD0 File Offset: 0x00077CD0
	private Rigidbody SetupRigidBody()
	{
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		if (rigidbody == null)
		{
			Debug.LogError("[BaseCorpse] already has a RigidBody defined - and it shouldn't!" + base.gameObject.name);
			return null;
		}
		rigidbody.mass = 10f;
		rigidbody.useGravity = true;
		rigidbody.drag = 0.5f;
		rigidbody.collisionDetectionMode = 0;
		if (base.isClient)
		{
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}
		return rigidbody;
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x00010098 File Offset: 0x0000E298
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.corpse != null)
		{
			this.Load(info.msg.corpse);
		}
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x000100BF File Offset: 0x0000E2BF
	private void Load(Corpse corpse)
	{
		bool isServer = base.isServer;
		if (base.isClient)
		{
			this.parentEnt = (BaseNetworkable.clientEntities.Find(corpse.parentID) as BaseEntity);
		}
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00079B4C File Offset: 0x00077D4C
	public override void OnAttacked(HitInfo info)
	{
		List<Rigidbody> list = Pool.GetList<Rigidbody>();
		Vis.Components<Rigidbody>(info.HitPositionWorld, 0.5f, list, 512, 2);
		foreach (Rigidbody rigidbody in list)
		{
			rigidbody.AddForceAtPosition(info.attackNormal * 1f, info.HitPositionWorld, 1);
		}
		Pool.FreeList<Rigidbody>(ref list);
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x000100EB File Offset: 0x0000E2EB
	public override string Categorize()
	{
		return "corpse";
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x060012C1 RID: 4801 RVA: 0x000100F2 File Offset: 0x0000E2F2
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}
}
