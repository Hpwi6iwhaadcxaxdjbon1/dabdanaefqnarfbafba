using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200058A RID: 1418
[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public class DeferredExtensionMesh : MonoBehaviour
{
	// Token: 0x04001C6B RID: 7275
	public SubsurfaceProfile subsurfaceProfile;

	// Token: 0x04001C6C RID: 7276
	private bool isVisible;

	// Token: 0x04001C6D RID: 7277
	private List<DeferredExtensionMesh.MaterialLink> materialLinks = new List<DeferredExtensionMesh.MaterialLink>();

	// Token: 0x04001C6E RID: 7278
	private MaterialPropertyBlock _block;

	// Token: 0x04001C6F RID: 7279
	private Renderer _renderer;

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06002069 RID: 8297 RVA: 0x000B091C File Offset: 0x000AEB1C
	private MaterialPropertyBlock block
	{
		get
		{
			return this._block = ((this._block == null) ? new MaterialPropertyBlock() : this._block);
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x0600206A RID: 8298 RVA: 0x000B0948 File Offset: 0x000AEB48
	private Renderer renderer
	{
		get
		{
			return this._renderer = ((this._renderer == null) ? base.GetComponent<Renderer>() : this._renderer);
		}
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x00019A38 File Offset: 0x00017C38
	private void OnEnable()
	{
		this.PrepareMaterials(this.renderer.sharedMaterials);
		this.UpdatePropertyBlock();
		DeferredExtension.Register(this);
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x00019A57 File Offset: 0x00017C57
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		DeferredExtension.Unregister(this);
		this.CleanupMaterials();
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x00019A6D File Offset: 0x00017C6D
	public void UpdatePropertyBlock()
	{
		if (this.subsurfaceProfile != null)
		{
			this.block.SetFloat("_SubsurfaceProfile", (float)this.subsurfaceProfile.Id);
		}
		this.renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x00019AAA File Offset: 0x00017CAA
	private void OnBecameVisible()
	{
		this.isVisible = (this.materialLinks.Count > 0);
		if (this.isVisible)
		{
			DeferredExtension.BecameVisible(this);
		}
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x00019ACE File Offset: 0x00017CCE
	private void OnBecameInvisible()
	{
		if (this.isVisible)
		{
			DeferredExtension.BecameInvisible(this);
			this.isVisible = false;
		}
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x00019AE5 File Offset: 0x00017CE5
	private void CleanupMaterials()
	{
		if (this.materialLinks != null)
		{
			this.materialLinks.Clear();
		}
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000B097C File Offset: 0x000AEB7C
	private void PrepareMaterials(Material[] mats)
	{
		this.CleanupMaterials();
		for (int i = 0; i < mats.Length; i++)
		{
			Material material = mats[i];
			if (material != null)
			{
				int num = -1;
				for (int j = 0; j < material.passCount; j++)
				{
					if (material.GetPassName(j).ToUpper() == "DEFERRED_EXTENDED")
					{
						num = j;
						break;
					}
				}
				if (num >= 0)
				{
					this.materialLinks.Add(new DeferredExtensionMesh.MaterialLink(material, i, num));
				}
			}
		}
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x000B09F0 File Offset: 0x000AEBF0
	public void AddToCommandBuffer(CommandBuffer cb)
	{
		for (int i = 0; i < this.materialLinks.Count; i++)
		{
			DeferredExtensionMesh.MaterialLink materialLink = this.materialLinks[i];
			cb.DrawRenderer(this.renderer, materialLink.material, materialLink.submeshIndex, materialLink.passIndex);
		}
	}

	// Token: 0x0200058B RID: 1419
	public struct MaterialLink
	{
		// Token: 0x04001C70 RID: 7280
		public Material material;

		// Token: 0x04001C71 RID: 7281
		public int submeshIndex;

		// Token: 0x04001C72 RID: 7282
		public int passIndex;

		// Token: 0x06002074 RID: 8308 RVA: 0x00019B0D File Offset: 0x00017D0D
		public MaterialLink(Material material, int submeshIndex, int passIndex)
		{
			this.material = material;
			this.submeshIndex = submeshIndex;
			this.passIndex = passIndex;
		}
	}
}
