using System;
using UnityEngine;

// Token: 0x02000580 RID: 1408
[ExecuteInEditMode]
public class DeferredDecal : MonoBehaviour
{
	// Token: 0x04001C24 RID: 7204
	public bool StickyGizmos;

	// Token: 0x04001C25 RID: 7205
	public Mesh mesh;

	// Token: 0x04001C26 RID: 7206
	public Material material;

	// Token: 0x04001C27 RID: 7207
	public DeferredDecalQueue queue;

	// Token: 0x04001C28 RID: 7208
	private bool cached;

	// Token: 0x04001C29 RID: 7209
	private Matrix4x4 localToWorldMatrix;

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06002032 RID: 8242 RVA: 0x000197D2 File Offset: 0x000179D2
	public Matrix4x4 matrix
	{
		get
		{
			if (this.cached)
			{
				return this.localToWorldMatrix;
			}
			this.cached = (base.transform.parent == null);
			this.localToWorldMatrix = base.transform.localToWorldMatrix;
			return this.localToWorldMatrix;
		}
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x00019811 File Offset: 0x00017A11
	protected void OnEnable()
	{
		DeferredDecalSystem.AddDecal(this);
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x00019819 File Offset: 0x00017A19
	protected void OnDisable()
	{
		DeferredDecalSystem.RemoveDecal(this);
		this.cached = false;
	}
}
