using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000730 RID: 1840
public class InstancingContainer
{
	// Token: 0x040023CD RID: 9165
	private int capacity;

	// Token: 0x040023CE RID: 9166
	private ListDictionary<InstancingKey, InstancingBuffer> buffers;

	// Token: 0x06002823 RID: 10275 RVA: 0x0001F4A3 File Offset: 0x0001D6A3
	public InstancingContainer(int capacity = 128)
	{
		this.capacity = capacity;
		this.buffers = new ListDictionary<InstancingKey, InstancingBuffer>(8);
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x0001F4BE File Offset: 0x0001D6BE
	public void Free()
	{
		this.buffers.Clear();
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x000CF1A8 File Offset: 0x000CD3A8
	public void Clear()
	{
		int count = this.buffers.Values.Count;
		InstancingBuffer[] buffer = this.buffers.Values.Buffer;
		for (int i = 0; i < count; i++)
		{
			buffer[i].Clear();
		}
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000CF1EC File Offset: 0x000CD3EC
	public InstancingBuffer Get(Mesh mesh, Material material, int submeshIndex = 0, int shaderPass = -1)
	{
		InstancingKey instancingKey = new InstancingKey(mesh, submeshIndex, material, shaderPass);
		InstancingBuffer instancingBuffer;
		if (!this.buffers.TryGetValue(instancingKey, ref instancingBuffer))
		{
			instancingBuffer = new InstancingBuffer(instancingKey, this.capacity);
			this.buffers.Add(instancingKey, instancingBuffer);
		}
		return instancingBuffer;
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x0001F4CB File Offset: 0x0001D6CB
	public void Add(Mesh mesh, Material material, Matrix4x4 matrix, int submeshIndex = 0, int shaderPass = -1)
	{
		this.Get(mesh, material, submeshIndex, shaderPass).Add(matrix);
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x000CF230 File Offset: 0x000CD430
	public void Apply(CommandBuffer buf, bool instancing, MaterialPropertyBlock block = null)
	{
		int count = this.buffers.Values.Count;
		InstancingBuffer[] buffer = this.buffers.Values.Buffer;
		for (int i = 0; i < count; i++)
		{
			buffer[i].Apply(buf, instancing, block);
		}
	}
}
