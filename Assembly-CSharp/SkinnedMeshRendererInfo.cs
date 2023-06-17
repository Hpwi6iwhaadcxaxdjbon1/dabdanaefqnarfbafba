using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001F7 RID: 503
public class SkinnedMeshRendererInfo : ComponentInfo<SkinnedMeshRenderer>
{
	// Token: 0x04000C9A RID: 3226
	public ShadowCastingMode shadows;

	// Token: 0x04000C9B RID: 3227
	public Material material;

	// Token: 0x04000C9C RID: 3228
	public Mesh mesh;

	// Token: 0x04000C9D RID: 3229
	public Bounds bounds;

	// Token: 0x04000C9E RID: 3230
	public Mesh cachedMesh;

	// Token: 0x04000C9F RID: 3231
	public SkinnedMeshRendererCache.RigInfo cachedRig;

	// Token: 0x04000CA0 RID: 3232
	private Transform root;

	// Token: 0x04000CA1 RID: 3233
	private Transform[] bones;

	// Token: 0x06000FAB RID: 4011 RVA: 0x0006B130 File Offset: 0x00069330
	public override void Reset()
	{
		if (this.component == null)
		{
			return;
		}
		this.component.shadowCastingMode = this.shadows;
		if (this.material)
		{
			this.component.sharedMaterial = this.material;
		}
		this.component.sharedMesh = this.mesh;
		this.component.localBounds = this.bounds;
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x0006B1A0 File Offset: 0x000693A0
	public override void Setup()
	{
		this.shadows = this.component.shadowCastingMode;
		this.material = this.component.sharedMaterial;
		this.mesh = this.component.sharedMesh;
		this.bounds = this.component.localBounds;
		this.RefreshCache();
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0006B1F8 File Offset: 0x000693F8
	private void RefreshCache()
	{
		if (this.cachedMesh != this.component.sharedMesh)
		{
			if (this.cachedMesh != null)
			{
				SkinnedMeshRendererCache.Add(this.cachedMesh, this.cachedRig);
			}
			this.cachedMesh = this.component.sharedMesh;
			this.cachedRig = SkinnedMeshRendererCache.Get(this.component);
		}
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0006B260 File Offset: 0x00069460
	public void RemapBones(SkinnedMultiMesh multiMesh)
	{
		this.RefreshCache();
		if (this.cachedRig.root == null || this.cachedRig.bones == null)
		{
			Debug.LogWarning("Invalid rig cache. Skipping bone remapping.");
			return;
		}
		this.root = multiMesh.FindBone(this.cachedRig.root);
		if (this.bones == null || this.bones.Length != this.cachedRig.bones.Length)
		{
			this.bones = new Transform[this.cachedRig.bones.Length];
		}
		for (int i = 0; i < this.bones.Length; i++)
		{
			this.bones[i] = multiMesh.FindBone(this.cachedRig.bones[i]);
		}
		this.component.rootBone = this.root;
		this.component.bones = this.bones;
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0006B338 File Offset: 0x00069538
	public void BuildRig()
	{
		this.RefreshCache();
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		base.transform.position = Vector3.zero;
		Transform[] array = new Transform[this.cachedRig.transforms.Length];
		for (int i = 0; i < this.cachedRig.transforms.Length; i++)
		{
			GameObject gameObject = new GameObject(this.cachedRig.bones[i]);
			gameObject.transform.position = this.cachedRig.transforms[i].MultiplyPoint(Vector3.zero);
			gameObject.transform.rotation = Quaternion.LookRotation(this.cachedRig.transforms[i].GetColumn(2), this.cachedRig.transforms[i].GetColumn(1));
			gameObject.transform.SetParent(base.transform, true);
			array[i] = gameObject.transform;
		}
		GameObject gameObject2 = new GameObject("root");
		gameObject2.transform.position = this.cachedRig.rootTransform.MultiplyPoint(Vector3.zero);
		gameObject2.transform.rotation = Quaternion.LookRotation(this.cachedRig.rootTransform.GetColumn(2), this.cachedRig.rootTransform.GetColumn(1));
		gameObject2.transform.SetParent(base.transform, true);
		this.component.rootBone = gameObject2.transform;
		this.component.bones = array;
		base.transform.rotation = rotation;
		base.transform.position = position;
	}
}
