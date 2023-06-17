using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000731 RID: 1841
public class InstancingBuffer
{
	// Token: 0x040023CF RID: 9167
	private BufferList<Matrix4x4> matrices;

	// Token: 0x040023D0 RID: 9168
	private Mesh mesh;

	// Token: 0x040023D1 RID: 9169
	private Material material;

	// Token: 0x040023D2 RID: 9170
	private int submeshIndex;

	// Token: 0x040023D3 RID: 9171
	private int shaderPass;

	// Token: 0x06002829 RID: 10281 RVA: 0x000CF278 File Offset: 0x000CD478
	public InstancingBuffer(InstancingKey key, int capacity)
	{
		this.mesh = key.mesh;
		this.material = key.material;
		this.submeshIndex = key.submeshIndex;
		this.shaderPass = key.shaderPass;
		this.matrices = new BufferList<Matrix4x4>(capacity);
	}

	// Token: 0x0600282A RID: 10282 RVA: 0x0001F4DF File Offset: 0x0001D6DF
	public InstancingBuffer(Mesh mesh, Material material, int submeshIndex, int shaderPass, int capacity)
	{
		this.mesh = mesh;
		this.material = material;
		this.submeshIndex = submeshIndex;
		this.shaderPass = shaderPass;
		this.matrices = new BufferList<Matrix4x4>(capacity);
	}

	// Token: 0x0600282B RID: 10283 RVA: 0x000CF2C8 File Offset: 0x000CD4C8
	public void Apply(CommandBuffer buf, bool instancing, MaterialPropertyBlock block)
	{
		if (instancing && SystemInfo.supportsInstancing)
		{
			buf.DrawMeshInstanced(this.mesh, this.submeshIndex, this.material, this.shaderPass, this.matrices.Buffer, Mathf.Min(this.matrices.Count, 1023), block);
			return;
		}
		Matrix4x4[] buffer = this.matrices.Buffer;
		int count = this.matrices.Count;
		for (int i = 0; i < count; i++)
		{
			buf.DrawMesh(this.mesh, buffer[i], this.material, this.submeshIndex, this.shaderPass);
		}
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x0001F511 File Offset: 0x0001D711
	public void Clear()
	{
		this.matrices.Clear();
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x0001F51E File Offset: 0x0001D71E
	public void Add(Matrix4x4 matrix)
	{
		this.matrices.Add(matrix);
	}
}
