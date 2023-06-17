using System;
using UnityEngine;

// Token: 0x020001EF RID: 495
public struct MeshRendererInstance
{
	// Token: 0x04000C85 RID: 3205
	public Renderer renderer;

	// Token: 0x04000C86 RID: 3206
	public OBB bounds;

	// Token: 0x04000C87 RID: 3207
	public Vector3 position;

	// Token: 0x04000C88 RID: 3208
	public Quaternion rotation;

	// Token: 0x04000C89 RID: 3209
	public Vector3 scale;

	// Token: 0x04000C8A RID: 3210
	public MeshCache.Data data;

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000F94 RID: 3988 RVA: 0x0000DDAA File Offset: 0x0000BFAA
	// (set) Token: 0x06000F95 RID: 3989 RVA: 0x0000DDB7 File Offset: 0x0000BFB7
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
