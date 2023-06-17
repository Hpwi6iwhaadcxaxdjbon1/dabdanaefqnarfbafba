using System;
using UnityEngine;

// Token: 0x02000713 RID: 1811
public class MaterialSetup : MonoBehaviour, IClientComponent
{
	// Token: 0x04002398 RID: 9112
	public bool destroy = true;

	// Token: 0x04002399 RID: 9113
	public MaterialConfig config;

	// Token: 0x060027B7 RID: 10167 RVA: 0x0001F043 File Offset: 0x0001D243
	protected void OnEnable()
	{
		this.Setup();
		if (this.destroy)
		{
			GameManager.Destroy(this, 0f);
		}
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x000CD9A0 File Offset: 0x000CBBA0
	private void Setup()
	{
		Renderer component = base.GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = this.config.GetMaterialPropertyBlock(component.sharedMaterial, base.transform.position, base.transform.lossyScale);
		component.SetPropertyBlock(materialPropertyBlock);
	}
}
