using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200074C RID: 1868
public class MeshColliderBatch : MeshBatch
{
	// Token: 0x04002403 RID: 9219
	private Vector3 position;

	// Token: 0x04002404 RID: 9220
	private UnityEngine.Mesh meshBatch;

	// Token: 0x04002405 RID: 9221
	private MeshCollider meshCollider;

	// Token: 0x04002406 RID: 9222
	private MeshColliderData meshData;

	// Token: 0x04002407 RID: 9223
	private MeshColliderGroup meshGroup;

	// Token: 0x04002408 RID: 9224
	private MeshColliderLookup meshLookup;

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x0600289A RID: 10394 RVA: 0x0001F857 File Offset: 0x0001DA57
	public override int VertexCapacity
	{
		get
		{
			return Batching.collider_capacity;
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x0600289B RID: 10395 RVA: 0x0001F85E File Offset: 0x0001DA5E
	public override int VertexCutoff
	{
		get
		{
			return Batching.collider_vertices;
		}
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x0001F865 File Offset: 0x0001DA65
	public Transform LookupTransform(int triangleIndex)
	{
		return this.meshLookup.Get(triangleIndex).transform;
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x0001F878 File Offset: 0x0001DA78
	public Rigidbody LookupRigidbody(int triangleIndex)
	{
		return this.meshLookup.Get(triangleIndex).rigidbody;
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x0001F88B File Offset: 0x0001DA8B
	public Collider LookupCollider(int triangleIndex)
	{
		return this.meshLookup.Get(triangleIndex).collider;
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000D06C0 File Offset: 0x000CE8C0
	public void LookupColliders<T>(Vector3 position, float distance, List<T> list) where T : Collider
	{
		List<MeshColliderLookup.LookupEntry> data = this.meshLookup.src.data;
		float num = distance * distance;
		for (int i = 0; i < data.Count; i++)
		{
			MeshColliderLookup.LookupEntry lookupEntry = data[i];
			if (lookupEntry.collider && (lookupEntry.bounds.ClosestPoint(position) - position).sqrMagnitude <= num)
			{
				list.Add(lookupEntry.collider as T);
			}
		}
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000D0740 File Offset: 0x000CE940
	public void LookupColliders<T>(OBB bounds, List<T> list) where T : Collider
	{
		List<MeshColliderLookup.LookupEntry> data = this.meshLookup.src.data;
		for (int i = 0; i < data.Count; i++)
		{
			MeshColliderLookup.LookupEntry lookupEntry = data[i];
			if (lookupEntry.collider && lookupEntry.bounds.Intersects(bounds))
			{
				list.Add(lookupEntry.collider as T);
			}
		}
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x0001F89E File Offset: 0x0001DA9E
	protected void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.meshData = new MeshColliderData();
		this.meshGroup = new MeshColliderGroup();
		this.meshLookup = new MeshColliderLookup();
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000D07AC File Offset: 0x000CE9AC
	public void Setup(Vector3 position, LayerMask layer, PhysicMaterial material)
	{
		base.transform.position = position;
		this.position = position;
		base.gameObject.layer = layer;
		this.meshCollider.sharedMaterial = material;
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000D07EC File Offset: 0x000CE9EC
	public void Add(MeshColliderInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x0001F8CD File Offset: 0x0001DACD
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x0001F8E5 File Offset: 0x0001DAE5
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x0001F8FD File Offset: 0x0001DAFD
	protected override void RefreshMesh()
	{
		this.meshLookup.dst.Clear();
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup, this.meshLookup);
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x0001F931 File Offset: 0x0001DB31
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshLookup.Apply();
		this.meshData.Apply(this.meshBatch);
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x000D083C File Offset: 0x000CEA3C
	protected override void ToggleMesh(bool state)
	{
		if (!Application.isLoading)
		{
			List<MeshColliderLookup.LookupEntry> data = this.meshLookup.src.data;
			for (int i = 0; i < data.Count; i++)
			{
				Collider collider = data[i].collider;
				if (collider)
				{
					collider.enabled = !state;
				}
			}
		}
		if (state)
		{
			if (this.meshCollider)
			{
				this.meshCollider.sharedMesh = this.meshBatch;
				this.meshCollider.enabled = false;
				this.meshCollider.enabled = true;
				return;
			}
		}
		else if (this.meshCollider)
		{
			this.meshCollider.sharedMesh = null;
			this.meshCollider.enabled = false;
		}
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000D08F0 File Offset: 0x000CEAF0
	protected override void OnPooled()
	{
		if (this.meshCollider)
		{
			this.meshCollider.sharedMesh = null;
		}
		if (this.meshBatch)
		{
			AssetPool.Free(ref this.meshBatch);
		}
		this.meshData.Free();
		this.meshGroup.Free();
		this.meshLookup.src.Clear();
		this.meshLookup.dst.Clear();
	}
}
