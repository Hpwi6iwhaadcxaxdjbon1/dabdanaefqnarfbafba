using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class Ragdoll : BaseMonoBehaviour
{
	// Token: 0x04000D8F RID: 3471
	public Transform eyeTransform;

	// Token: 0x04000D90 RID: 3472
	public Transform centerBone;

	// Token: 0x04000D91 RID: 3473
	public Rigidbody primaryBody;

	// Token: 0x04000D92 RID: 3474
	public PhysicMaterial physicMaterial;

	// Token: 0x04000D93 RID: 3475
	public SpringJoint corpseJoint;

	// Token: 0x04000D94 RID: 3476
	public GameObject GibEffect;

	// Token: 0x04000D95 RID: 3477
	[NonSerialized]
	public Rigidbody pinTo;

	// Token: 0x04000D96 RID: 3478
	private LODGroup _lodGroup;

	// Token: 0x04000D97 RID: 3479
	private ArticulatedOccludee occludee;

	// Token: 0x04000D98 RID: 3480
	protected bool IsSetup;

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060010A5 RID: 4261 RVA: 0x0000EA70 File Offset: 0x0000CC70
	public LODGroup lodGroup
	{
		get
		{
			if (this._lodGroup == null)
			{
				this._lodGroup = base.GetComponent<LODGroup>();
			}
			return this._lodGroup;
		}
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x000704A0 File Offset: 0x0006E6A0
	public void Awake()
	{
		this.occludee = base.GetComponent<ArticulatedOccludee>();
		this.RagdollSetup();
		base.InvokeRepeating(new Action(this.UpdateLODBounds), 2f, Random.Range(9f, 10f));
		if (this.occludee != null && this.occludee.enabled)
		{
			this.occludee.Invoke(new Action(this.occludee.TriggerUpdateVisibilityBounds), Random.Range(0.1f, 0.2f));
			this.occludee.ProcessVisibility(this.lodGroup);
		}
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x00070540 File Offset: 0x0006E740
	private void RagdollSetup()
	{
		if (this.IsSetup)
		{
			return;
		}
		this.IsSetup = true;
		List<Joint> list = Pool.GetList<Joint>();
		List<CharacterJoint> list2 = Pool.GetList<CharacterJoint>();
		List<ConfigurableJoint> list3 = Pool.GetList<ConfigurableJoint>();
		List<Rigidbody> list4 = Pool.GetList<Rigidbody>();
		base.GetComponentsInChildren<Joint>(true, list);
		base.GetComponentsInChildren<CharacterJoint>(true, list2);
		base.GetComponentsInChildren<ConfigurableJoint>(true, list3);
		base.GetComponentsInChildren<Rigidbody>(true, list4);
		foreach (Joint joint in list)
		{
			joint.enablePreprocessing = false;
		}
		foreach (CharacterJoint characterJoint in list2)
		{
			characterJoint.enableProjection = true;
		}
		foreach (ConfigurableJoint configurableJoint in list3)
		{
			configurableJoint.projectionMode = 1;
		}
		foreach (Rigidbody rigidbody in list4)
		{
			Physics.ApplyRagdoll(rigidbody);
			rigidbody.interpolation = 1;
			rigidbody.angularDrag = 1f;
			rigidbody.drag = 1f;
			rigidbody.detectCollisions = true;
			if (rigidbody.mass < 1f)
			{
				rigidbody.mass = 1f;
			}
			rigidbody.velocity = Random.onUnitSphere * 5f;
			rigidbody.angularVelocity = Random.onUnitSphere * 5f;
		}
		Pool.FreeList<Joint>(ref list);
		Pool.FreeList<CharacterJoint>(ref list2);
		Pool.FreeList<ConfigurableJoint>(ref list3);
		Pool.FreeList<Rigidbody>(ref list4);
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x0007071C File Offset: 0x0006E91C
	public void CopyBonesFrom(Model otherModel)
	{
		this.RagdollSetup();
		if (otherModel == null)
		{
			return;
		}
		if (base.transform == null)
		{
			return;
		}
		if (otherModel.boneTransforms.Length == 0)
		{
			return;
		}
		SkeletonScale componentInParent = otherModel.GetComponentInParent<SkeletonScale>();
		if (componentInParent)
		{
			componentInParent.Reset();
		}
		Model component = base.GetComponent<Model>();
		if (component)
		{
			component.SyncBones(otherModel);
			RagdollInteritable.Inherit(otherModel, component);
		}
		this.UpdateLODBounds();
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x0007078C File Offset: 0x0006E98C
	public void MoveRigidbodiesToRoot()
	{
		Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].transform.SetParent(base.transform, true);
		}
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x000707C4 File Offset: 0x0006E9C4
	public void CreatePinJoint()
	{
		if (this.corpseJoint != null)
		{
			return;
		}
		this.corpseJoint = this.primaryBody.gameObject.AddComponent<SpringJoint>();
		this.corpseJoint.connectedBody = this.pinTo;
		this.corpseJoint.autoConfigureConnectedAnchor = false;
		this.corpseJoint.anchor = Vector3.zero;
		this.corpseJoint.connectedAnchor = Vector3.zero;
		this.corpseJoint.minDistance = 0f;
		this.corpseJoint.maxDistance = 1f;
		this.corpseJoint.damper = 1000f;
		this.corpseJoint.spring = 5000f;
		this.corpseJoint.enableCollision = false;
		this.corpseJoint.enablePreprocessing = false;
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x0007088C File Offset: 0x0006EA8C
	private void UpdateLODBounds()
	{
		if (this.lodGroup != null && (this.occludee == null || this.occludee.IsVisible))
		{
			this.lodGroup.RecalculateBounds();
		}
		this.centerBone;
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x000708DC File Offset: 0x0006EADC
	public void SetRagdollSkin(int iSkin)
	{
		AnimalSkin component = base.GetComponent<AnimalSkin>();
		if (component != null)
		{
			component.ChangeSkin(iSkin);
		}
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x00070900 File Offset: 0x0006EB00
	public void OnGibbed()
	{
		if (this.GibEffect == null)
		{
			return;
		}
		this.GibEffect.SetActive(true);
		this.GibEffect.transform.SetParent(null, true);
		foreach (ParticleSystem particleSystem in this.GibEffect.GetComponentsInChildren<ParticleSystem>())
		{
			particleSystem.Simulate(0.1f);
			particleSystem.Play();
		}
	}
}
