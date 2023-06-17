using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000591 RID: 1425
[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public class DeferredMeshDecal : MonoBehaviour, IClientComponent
{
	// Token: 0x04001C7F RID: 7295
	private bool isVisible;

	// Token: 0x04001C80 RID: 7296
	private Renderer renderer;

	// Token: 0x04001C81 RID: 7297
	private List<DeferredMeshDecal.MaterialLink> materialLinks = new List<DeferredMeshDecal.MaterialLink>();

	// Token: 0x04001C82 RID: 7298
	private List<DeferredMeshDecal.InstanceData> instanceData = new List<DeferredMeshDecal.InstanceData>();

	// Token: 0x0600208D RID: 8333 RVA: 0x00019C75 File Offset: 0x00017E75
	private void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.PrepareLinks(this.renderer.sharedMaterials);
		DeferredMeshDecalRenderer.Register(this);
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x00019C9A File Offset: 0x00017E9A
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		DeferredMeshDecalRenderer.Unregister(this);
		this.CleanupLinks();
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x00019CB0 File Offset: 0x00017EB0
	private void OnBecameVisible()
	{
		this.isVisible = (this.materialLinks.Count > 0);
		if (this.isVisible)
		{
			DeferredMeshDecalRenderer.BecameVisible(this);
		}
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x00019CD4 File Offset: 0x00017ED4
	private void OnBecameInvisible()
	{
		if (this.isVisible)
		{
			DeferredMeshDecalRenderer.BecameInvisible(this);
			this.isVisible = false;
		}
	}

	// Token: 0x06002091 RID: 8337 RVA: 0x000B11F0 File Offset: 0x000AF3F0
	private void CleanupLinks()
	{
		if (this.materialLinks != null)
		{
			for (int i = 0; i < this.materialLinks.Count; i++)
			{
				this.materialLinks[i].replacement.Release();
			}
			this.materialLinks.Clear();
		}
		if (this.instanceData != null)
		{
			this.instanceData.Clear();
		}
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x000B1250 File Offset: 0x000AF450
	private void PrepareLinks(Material[] mats)
	{
		this.CleanupLinks();
		for (int i = 0; i < mats.Length; i++)
		{
			Material material = mats[i];
			if (material != null && string.Equals(material.GetTag("MeshDecal", false), "True", 5))
			{
				DeferredMeshDecal.MaterialReplacement replacement = DeferredMeshDecal.MaterialReplacement.Allocate(material);
				this.materialLinks.Add(new DeferredMeshDecal.MaterialLink(replacement, i));
			}
		}
		foreach (DeferredMeshDecal.MaterialLink materialLink in this.materialLinks)
		{
			this.instanceData.Add(new DeferredMeshDecal.InstanceData(this.renderer, materialLink.replacement.material, materialLink.submeshIndex));
		}
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x000B131C File Offset: 0x000AF51C
	public bool IsLinkedToMaterial(Material mat)
	{
		for (int i = 0; i < this.materialLinks.Count; i++)
		{
			if (mat == this.materialLinks[i].replacement.reference)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x000B1360 File Offset: 0x000AF560
	public void AddToCommandBuffer(CommandBuffer cb)
	{
		for (int i = 0; i < this.materialLinks.Count; i++)
		{
			DeferredMeshDecal.MaterialLink materialLink = this.materialLinks[i];
			cb.DrawRenderer(this.renderer, materialLink.replacement.material, materialLink.submeshIndex, 0);
		}
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x00019CEB File Offset: 0x00017EEB
	public List<DeferredMeshDecal.InstanceData> GetInstanceData()
	{
		return this.instanceData;
	}

	// Token: 0x02000592 RID: 1426
	public class MaterialReplacement
	{
		// Token: 0x04001C83 RID: 7299
		public Material reference;

		// Token: 0x04001C84 RID: 7300
		public Material material;

		// Token: 0x04001C85 RID: 7301
		private int refCount;

		// Token: 0x04001C86 RID: 7302
		private static Dictionary<Material, DeferredMeshDecal.MaterialReplacement> pool = new Dictionary<Material, DeferredMeshDecal.MaterialReplacement>();

		// Token: 0x06002097 RID: 8343 RVA: 0x000B13B0 File Offset: 0x000AF5B0
		private MaterialReplacement(Material reference)
		{
			this.reference = reference;
			this.material = new Material(Shader.Find("Hidden/" + reference.shader.name))
			{
				hideFlags = HideFlags.DontSave
			};
			this.material.name = reference.name;
			this.material.CopyPropertiesFromMaterial(reference);
			this.material.enableInstancing = reference.enableInstancing;
			this.refCount = 1;
		}

		// Token: 0x06002098 RID: 8344 RVA: 0x000B142C File Offset: 0x000AF62C
		public static DeferredMeshDecal.MaterialReplacement Allocate(Material reference)
		{
			DeferredMeshDecal.MaterialReplacement materialReplacement;
			if (!DeferredMeshDecal.MaterialReplacement.pool.TryGetValue(reference, ref materialReplacement))
			{
				materialReplacement = new DeferredMeshDecal.MaterialReplacement(reference);
				DeferredMeshDecal.MaterialReplacement.pool.Add(reference, materialReplacement);
			}
			else
			{
				materialReplacement.refCount++;
			}
			return materialReplacement;
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000B146C File Offset: 0x000AF66C
		public void Release()
		{
			this.refCount--;
			if (this.refCount == 0)
			{
				Object.Destroy(this.material);
				DeferredMeshDecal.MaterialReplacement.pool.Remove(this.reference);
				this.reference = null;
				this.material = null;
			}
		}
	}

	// Token: 0x02000593 RID: 1427
	public struct MaterialLink
	{
		// Token: 0x04001C87 RID: 7303
		public DeferredMeshDecal.MaterialReplacement replacement;

		// Token: 0x04001C88 RID: 7304
		public int submeshIndex;

		// Token: 0x0600209B RID: 8347 RVA: 0x00019D1D File Offset: 0x00017F1D
		public MaterialLink(DeferredMeshDecal.MaterialReplacement replacement, int submeshIndex)
		{
			this.replacement = replacement;
			this.submeshIndex = submeshIndex;
		}
	}

	// Token: 0x02000594 RID: 1428
	public class InstanceData
	{
		// Token: 0x04001C89 RID: 7305
		private Transform transform;

		// Token: 0x04001C8A RID: 7306
		private Material material;

		// Token: 0x04001C8B RID: 7307
		private Mesh mesh;

		// Token: 0x04001C8C RID: 7308
		private int submeshIndex;

		// Token: 0x04001C8D RID: 7309
		private int hash;

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x0600209C RID: 8348 RVA: 0x00019D2D File Offset: 0x00017F2D
		public Material Material
		{
			get
			{
				return this.material;
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x0600209D RID: 8349 RVA: 0x00019D35 File Offset: 0x00017F35
		public Mesh Mesh
		{
			get
			{
				return this.mesh;
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x0600209E RID: 8350 RVA: 0x00019D3D File Offset: 0x00017F3D
		public int SubmeshIndex
		{
			get
			{
				return this.submeshIndex;
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x0600209F RID: 8351 RVA: 0x00019D45 File Offset: 0x00017F45
		public Matrix4x4 LocalToWorld
		{
			get
			{
				return this.transform.localToWorldMatrix;
			}
		}

		// Token: 0x060020A0 RID: 8352 RVA: 0x000B14BC File Offset: 0x000AF6BC
		public InstanceData(Renderer renderer, Material material, int submeshIndex)
		{
			this.transform = renderer.transform;
			this.material = material;
			if (renderer.GetType() == typeof(SkinnedMeshRenderer))
			{
				this.mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
			}
			else
			{
				this.mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
			}
			this.submeshIndex = submeshIndex;
			this.hash = this.GenerateHashCode();
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x00019D52 File Offset: 0x00017F52
		private int GenerateHashCode()
		{
			return ((17 * 31 + this.material.GetHashCode()) * 31 + this.mesh.GetHashCode()) * 31 + this.submeshIndex.GetHashCode();
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x000B1530 File Offset: 0x000AF730
		public override bool Equals(object obj)
		{
			DeferredMeshDecal.InstanceData instanceData = obj as DeferredMeshDecal.InstanceData;
			return instanceData.material == this.material && instanceData.mesh == this.mesh && instanceData.submeshIndex == this.submeshIndex;
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x00019D83 File Offset: 0x00017F83
		public override int GetHashCode()
		{
			return this.hash;
		}
	}
}
