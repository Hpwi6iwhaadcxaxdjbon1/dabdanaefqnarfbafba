using System;
using UnityEngine;

// Token: 0x02000732 RID: 1842
public struct InstancingKey : IEquatable<InstancingKey>
{
	// Token: 0x040023D4 RID: 9172
	public Mesh mesh;

	// Token: 0x040023D5 RID: 9173
	public int submeshIndex;

	// Token: 0x040023D6 RID: 9174
	public Material material;

	// Token: 0x040023D7 RID: 9175
	public int shaderPass;

	// Token: 0x0600282E RID: 10286 RVA: 0x0001F52C File Offset: 0x0001D72C
	public InstancingKey(Mesh mesh, int submeshIndex, Material material, int shaderPass)
	{
		this.mesh = mesh;
		this.submeshIndex = submeshIndex;
		this.material = material;
		this.shaderPass = shaderPass;
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x0001F54B File Offset: 0x0001D74B
	public override int GetHashCode()
	{
		return this.mesh.GetHashCode() ^ this.material.GetHashCode();
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x0001F564 File Offset: 0x0001D764
	public override bool Equals(object other)
	{
		return other is InstancingKey && this.Equals((InstancingKey)other);
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x000CF368 File Offset: 0x000CD568
	public bool Equals(InstancingKey other)
	{
		return this.mesh == other.mesh && this.submeshIndex == other.submeshIndex && this.material == other.material && this.shaderPass == other.shaderPass;
	}
}
