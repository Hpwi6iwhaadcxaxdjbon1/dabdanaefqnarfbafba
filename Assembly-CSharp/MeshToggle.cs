using System;
using UnityEngine;

// Token: 0x02000714 RID: 1812
public class MeshToggle : MonoBehaviour
{
	// Token: 0x0400239A RID: 9114
	public Mesh[] RendererMeshes;

	// Token: 0x0400239B RID: 9115
	public Mesh[] ColliderMeshes;

	// Token: 0x060027BA RID: 10170 RVA: 0x000CD9E4 File Offset: 0x000CBBE4
	public void SwitchRenderer(int index)
	{
		if (this.RendererMeshes.Length == 0)
		{
			return;
		}
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.RendererMeshes[Mathf.Clamp(index, 0, this.RendererMeshes.Length - 1)];
	}

	// Token: 0x060027BB RID: 10171 RVA: 0x000CDA2C File Offset: 0x000CBC2C
	public void SwitchRenderer(float factor)
	{
		int index = Mathf.RoundToInt(factor * (float)this.RendererMeshes.Length);
		this.SwitchRenderer(index);
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x000CDA54 File Offset: 0x000CBC54
	public void SwitchCollider(int index)
	{
		if (this.ColliderMeshes.Length == 0)
		{
			return;
		}
		MeshCollider component = base.GetComponent<MeshCollider>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.ColliderMeshes[Mathf.Clamp(index, 0, this.ColliderMeshes.Length - 1)];
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x000CDA9C File Offset: 0x000CBC9C
	public void SwitchCollider(float factor)
	{
		int index = Mathf.RoundToInt(factor * (float)this.ColliderMeshes.Length);
		this.SwitchCollider(index);
	}

	// Token: 0x060027BE RID: 10174 RVA: 0x0001F06D File Offset: 0x0001D26D
	public void SwitchAll(int index)
	{
		this.SwitchRenderer(index);
		this.SwitchCollider(index);
	}

	// Token: 0x060027BF RID: 10175 RVA: 0x0001F07D File Offset: 0x0001D27D
	public void SwitchAll(float factor)
	{
		this.SwitchRenderer(factor);
		this.SwitchCollider(factor);
	}
}
