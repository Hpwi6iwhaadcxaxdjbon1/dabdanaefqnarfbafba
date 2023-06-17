using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class MeshColliderLookup
{
	// Token: 0x04000C69 RID: 3177
	public MeshColliderLookup.LookupGroup src = new MeshColliderLookup.LookupGroup();

	// Token: 0x04000C6A RID: 3178
	public MeshColliderLookup.LookupGroup dst = new MeshColliderLookup.LookupGroup();

	// Token: 0x06000F7A RID: 3962 RVA: 0x0006A038 File Offset: 0x00068238
	public void Apply()
	{
		MeshColliderLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x0000DCC4 File Offset: 0x0000BEC4
	public void Add(MeshColliderInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x0000DCD2 File Offset: 0x0000BED2
	public MeshColliderLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x020001E8 RID: 488
	public class LookupGroup
	{
		// Token: 0x04000C6B RID: 3179
		public List<MeshColliderLookup.LookupEntry> data = new List<MeshColliderLookup.LookupEntry>();

		// Token: 0x04000C6C RID: 3180
		public List<int> indices = new List<int>();

		// Token: 0x06000F7E RID: 3966 RVA: 0x0000DCFE File Offset: 0x0000BEFE
		public void Clear()
		{
			this.data.Clear();
			this.indices.Clear();
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0006A06C File Offset: 0x0006826C
		public void Add(MeshColliderInstance instance)
		{
			this.data.Add(new MeshColliderLookup.LookupEntry(instance));
			int num = this.data.Count - 1;
			int num2 = instance.data.triangles.Length / 3;
			for (int i = 0; i < num2; i++)
			{
				this.indices.Add(num);
			}
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0000DD16 File Offset: 0x0000BF16
		public MeshColliderLookup.LookupEntry Get(int index)
		{
			return this.data[this.indices[index]];
		}
	}

	// Token: 0x020001E9 RID: 489
	public struct LookupEntry
	{
		// Token: 0x04000C6D RID: 3181
		public Transform transform;

		// Token: 0x04000C6E RID: 3182
		public Rigidbody rigidbody;

		// Token: 0x04000C6F RID: 3183
		public Collider collider;

		// Token: 0x04000C70 RID: 3184
		public OBB bounds;

		// Token: 0x06000F82 RID: 3970 RVA: 0x0000DD4D File Offset: 0x0000BF4D
		public LookupEntry(MeshColliderInstance instance)
		{
			this.transform = instance.transform;
			this.rigidbody = instance.rigidbody;
			this.collider = instance.collider;
			this.bounds = instance.bounds;
		}
	}
}
