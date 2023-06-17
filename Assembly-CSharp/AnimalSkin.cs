using System;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class AnimalSkin : MonoBehaviour, IClientComponent
{
	// Token: 0x04000BD9 RID: 3033
	public SkinnedMeshRenderer[] animalMesh;

	// Token: 0x04000BDA RID: 3034
	public AnimalMultiSkin[] animalSkins;

	// Token: 0x04000BDB RID: 3035
	private Model model;

	// Token: 0x04000BDC RID: 3036
	public bool dontRandomizeOnStart;

	// Token: 0x06000ECD RID: 3789 RVA: 0x00066A3C File Offset: 0x00064C3C
	private void Start()
	{
		this.model = base.gameObject.GetComponent<Model>();
		if (!this.dontRandomizeOnStart)
		{
			int iSkin = Mathf.FloorToInt((float)Random.Range(0, this.animalSkins.Length));
			this.ChangeSkin(iSkin);
		}
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x00066A80 File Offset: 0x00064C80
	public void ChangeSkin(int iSkin)
	{
		if (this.animalSkins.Length == 0)
		{
			return;
		}
		iSkin = Mathf.Clamp(iSkin, 0, this.animalSkins.Length - 1);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.animalMesh)
		{
			Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			if (sharedMaterials != null)
			{
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					sharedMaterials[j] = this.animalSkins[iSkin].multiSkin[j];
				}
				skinnedMeshRenderer.sharedMaterials = sharedMaterials;
			}
		}
		if (this.model != null)
		{
			this.model.skin = iSkin;
		}
	}
}
