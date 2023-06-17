using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public static class SkinnedMeshRendererCache
{
	// Token: 0x04000C92 RID: 3218
	public static Dictionary<Mesh, SkinnedMeshRendererCache.RigInfo> dictionary = new Dictionary<Mesh, SkinnedMeshRendererCache.RigInfo>();

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0000DE9D File Offset: 0x0000C09D
	public static void Add(Mesh mesh, SkinnedMeshRendererCache.RigInfo info)
	{
		if (!SkinnedMeshRendererCache.dictionary.ContainsKey(mesh))
		{
			SkinnedMeshRendererCache.dictionary.Add(mesh, info);
		}
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x0006AFFC File Offset: 0x000691FC
	public static SkinnedMeshRendererCache.RigInfo Get(SkinnedMeshRenderer renderer)
	{
		SkinnedMeshRendererCache.RigInfo rigInfo;
		if (!SkinnedMeshRendererCache.dictionary.TryGetValue(renderer.sharedMesh, ref rigInfo))
		{
			rigInfo = new SkinnedMeshRendererCache.RigInfo();
			Transform rootBone = renderer.rootBone;
			Transform[] bones = renderer.bones;
			if (rootBone == null)
			{
				Debug.LogWarning("Renderer without a valid root bone: " + renderer.name + " " + renderer.sharedMesh.name, renderer.gameObject);
				return rigInfo;
			}
			renderer.transform.position = Vector3.zero;
			renderer.transform.rotation = Quaternion.identity;
			renderer.transform.localScale = Vector3.one;
			rigInfo.root = rootBone.name;
			rigInfo.bones = Enumerable.ToArray<string>(Enumerable.Select<Transform, string>(bones, (Transform x) => x.name));
			rigInfo.transforms = Enumerable.ToArray<Matrix4x4>(Enumerable.Select<Transform, Matrix4x4>(bones, (Transform x) => x.transform.localToWorldMatrix));
			rigInfo.rootTransform = renderer.rootBone.transform.localToWorldMatrix;
			SkinnedMeshRendererCache.dictionary.Add(renderer.sharedMesh, rigInfo);
		}
		return rigInfo;
	}

	// Token: 0x020001F5 RID: 501
	[Serializable]
	public class RigInfo
	{
		// Token: 0x04000C93 RID: 3219
		public string root;

		// Token: 0x04000C94 RID: 3220
		public string[] bones;

		// Token: 0x04000C95 RID: 3221
		public Matrix4x4[] transforms;

		// Token: 0x04000C96 RID: 3222
		public Matrix4x4 rootTransform;
	}
}
