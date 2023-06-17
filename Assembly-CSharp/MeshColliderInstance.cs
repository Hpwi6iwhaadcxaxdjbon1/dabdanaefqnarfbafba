using System;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public struct MeshColliderInstance
{
	// Token: 0x04000C61 RID: 3169
	public Transform transform;

	// Token: 0x04000C62 RID: 3170
	public Rigidbody rigidbody;

	// Token: 0x04000C63 RID: 3171
	public Collider collider;

	// Token: 0x04000C64 RID: 3172
	public OBB bounds;

	// Token: 0x04000C65 RID: 3173
	public Vector3 position;

	// Token: 0x04000C66 RID: 3174
	public Quaternion rotation;

	// Token: 0x04000C67 RID: 3175
	public Vector3 scale;

	// Token: 0x04000C68 RID: 3176
	public MeshCache.Data data;

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000F78 RID: 3960 RVA: 0x0000DCA9 File Offset: 0x0000BEA9
	// (set) Token: 0x06000F79 RID: 3961 RVA: 0x0000DCB6 File Offset: 0x0000BEB6
	public Mesh mesh
	{
		get
		{
			return this.data.mesh;
		}
		set
		{
			this.data = MeshCache.Get(value);
		}
	}
}
