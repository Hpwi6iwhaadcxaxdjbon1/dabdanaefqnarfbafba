using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001A3 RID: 419
[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
	// Token: 0x04000B67 RID: 2919
	public Mesh mesh;

	// Token: 0x04000B68 RID: 2920
	public Material material;

	// Token: 0x04000B69 RID: 2921
	public bool renderer;

	// Token: 0x04000B6A RID: 2922
	public bool collider;

	// Token: 0x06000E45 RID: 3653 RVA: 0x0006474C File Offset: 0x0006294C
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (clientside)
		{
			if (this.renderer)
			{
				Object component = rootObj.GetComponent<MeshFilter>();
				MeshRenderer meshRenderer = rootObj.GetComponent<MeshRenderer>();
				if (!component)
				{
					rootObj.AddComponent<MeshFilter>().sharedMesh = this.mesh;
				}
				if (!meshRenderer)
				{
					meshRenderer = rootObj.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = this.material;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				}
			}
			if (this.collider && !rootObj.GetComponent<MeshCollider>())
			{
				rootObj.AddComponent<MeshCollider>().sharedMesh = this.mesh;
			}
		}
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0000D1C6 File Offset: 0x0000B3C6
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionPlaceholder);
	}
}
