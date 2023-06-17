using System;
using ConVar;
using UnityEngine;

// Token: 0x02000396 RID: 918
public class Gibbable : MonoBehaviour, IClientComponent
{
	// Token: 0x04001420 RID: 5152
	public GameObject gibSource;

	// Token: 0x04001421 RID: 5153
	public GameObject materialSource;

	// Token: 0x04001422 RID: 5154
	public bool copyMaterialBlock = true;

	// Token: 0x04001423 RID: 5155
	public PhysicMaterial physicsMaterial;

	// Token: 0x04001424 RID: 5156
	public GameObjectRef fxPrefab;

	// Token: 0x04001425 RID: 5157
	public bool spawnFxPrefab = true;

	// Token: 0x04001426 RID: 5158
	[Tooltip("If enabled, gibs will spawn even though we've hit a gib limit")]
	public bool important;

	// Token: 0x04001427 RID: 5159
	public float explodeScale;

	// Token: 0x04001428 RID: 5160
	private static MaterialPropertyBlock propertyBlock;

	// Token: 0x04001429 RID: 5161
	public float scaleOverride = 1f;

	// Token: 0x06001768 RID: 5992 RVA: 0x0008A488 File Offset: 0x00088688
	public void CreateGibs(Transform parent)
	{
		if (!this.important && Gib.gibCount > Effects.maxgibs)
		{
			return;
		}
		if (MainCamera.Distance(base.transform.position) > 500f)
		{
			return;
		}
		if (this.gibSource)
		{
			if (Gibbable.propertyBlock == null)
			{
				Gibbable.propertyBlock = new MaterialPropertyBlock();
			}
			MeshRenderer meshRenderer = (this.materialSource != null) ? this.materialSource.GetComponent<MeshRenderer>() : base.GetComponent<MeshRenderer>();
			if (meshRenderer)
			{
				Gibbable.propertyBlock.Clear();
				meshRenderer.GetPropertyBlock(Gibbable.propertyBlock);
			}
			foreach (MeshRenderer meshRenderer2 in this.gibSource.GetComponentsInChildren<MeshRenderer>(true))
			{
				Gib gib = Gib.Create(meshRenderer2.GetComponent<MeshFilter>().sharedMesh, (meshRenderer != null) ? meshRenderer.sharedMaterials : meshRenderer2.sharedMaterials, base.transform.localToWorldMatrix.MultiplyPoint(meshRenderer2.transform.localPosition), base.transform.rotation * meshRenderer2.transform.localRotation);
				gib.transform.localScale = Vector3.one * this.scaleOverride;
				gib.transform.parent = parent;
				if (this.explodeScale == 0f)
				{
					gib.CreatePhysics(this.physicsMaterial);
				}
				else
				{
					Vector3 vel = meshRenderer2.transform.localPosition.normalized * this.explodeScale;
					Vector3 angVel = Vector3Ex.Range(-10f, 10f);
					gib.CreatePhysics(this.physicsMaterial, vel, angVel);
				}
				if (this.copyMaterialBlock && !Gibbable.propertyBlock.isEmpty)
				{
					gib.meshRenderer.SetPropertyBlock(Gibbable.propertyBlock);
				}
			}
		}
		if (this.spawnFxPrefab)
		{
			EffectLibrary.CreateEffect(this.fxPrefab.isValid ? this.fxPrefab.resourcePath : Gib.GetEffect(this.physicsMaterial), base.transform.position, base.transform.rotation);
		}
	}
}
