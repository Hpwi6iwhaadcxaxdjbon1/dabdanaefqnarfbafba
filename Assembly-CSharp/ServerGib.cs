using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020002F4 RID: 756
public class ServerGib : BaseCombatEntity
{
	// Token: 0x040010E5 RID: 4325
	public GameObject _gibSource;

	// Token: 0x040010E6 RID: 4326
	public string _gibName;

	// Token: 0x040010E7 RID: 4327
	public PhysicMaterial physicsMaterial;

	// Token: 0x040010E8 RID: 4328
	private MeshCollider meshCollider;

	// Token: 0x040010E9 RID: 4329
	private MeshFilter meshFilter;

	// Token: 0x040010EA RID: 4330
	private MeshRenderer meshRenderer;

	// Token: 0x040010EB RID: 4331
	private bool initialized;

	// Token: 0x06001407 RID: 5127 RVA: 0x0001115D File Offset: 0x0000F35D
	public MeshCollider GetCollider()
	{
		return this.meshCollider;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x00011165 File Offset: 0x0000F365
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.servergib != null)
		{
			this.ClientSetGib(info.msg.servergib.gibName);
		}
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x0007D36C File Offset: 0x0007B56C
	public void ClientSetGib(string newgibname)
	{
		if (this.initialized)
		{
			return;
		}
		this._gibName = newgibname;
		Transform transform = this._gibSource.transform.Find(this._gibName);
		MeshFilter component = transform.GetComponent<MeshFilter>();
		MeshRenderer component2 = transform.GetComponent<MeshRenderer>();
		this.PhysicsInit(component.sharedMesh);
		this.VisualsInit(component.sharedMesh, component2.sharedMaterials);
		this.initialized = true;
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0000964D File Offset: 0x0000784D
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x00011191 File Offset: 0x0000F391
	public void VisualsInit(Mesh mesh, Material[] materials)
	{
		this.meshFilter = base.gameObject.AddComponent<MeshFilter>();
		this.meshFilter.sharedMesh = mesh;
		this.meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		this.meshRenderer.sharedMaterials = materials;
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x0007D3D4 File Offset: 0x0007B5D4
	public virtual void PhysicsInit(Mesh mesh)
	{
		this.meshCollider = base.gameObject.AddComponent<MeshCollider>();
		this.meshCollider.sharedMesh = mesh;
		this.meshCollider.convex = true;
		this.meshCollider.material = this.physicsMaterial;
		if (base.isClient)
		{
			base.gameObject.AddComponent<ColliderInfo>().SetFlag(ColliderInfo.Flags.Opaque, false);
		}
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		rigidbody.mass = Mathf.Clamp(this.meshCollider.bounds.size.magnitude * this.meshCollider.bounds.size.magnitude * 20f, 10f, 2000f);
		rigidbody.interpolation = 1;
		if (base.isServer)
		{
			rigidbody.drag = 0.1f;
			rigidbody.angularDrag = 0.1f;
		}
		base.gameObject.layer = LayerMask.NameToLayer("Default");
		if (base.isClient)
		{
			rigidbody.isKinematic = true;
		}
	}
}
