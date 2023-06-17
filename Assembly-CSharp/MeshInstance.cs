using System;
using UnityEngine;

// Token: 0x020001EC RID: 492
public struct MeshInstance
{
	// Token: 0x04000C79 RID: 3193
	public Vector3 position;

	// Token: 0x04000C7A RID: 3194
	public Quaternion rotation;

	// Token: 0x04000C7B RID: 3195
	public Vector3 scale;

	// Token: 0x04000C7C RID: 3196
	public MeshCache.Data data;

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000F8A RID: 3978 RVA: 0x0000DD87 File Offset: 0x0000BF87
	// (set) Token: 0x06000F8B RID: 3979 RVA: 0x0000DD94 File Offset: 0x0000BF94
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
