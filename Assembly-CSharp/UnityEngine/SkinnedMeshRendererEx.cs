using System;

namespace UnityEngine
{
	// Token: 0x02000837 RID: 2103
	public static class SkinnedMeshRendererEx
	{
		// Token: 0x06002D8D RID: 11661 RVA: 0x000E4D84 File Offset: 0x000E2F84
		public static Transform FindRig(this SkinnedMeshRenderer renderer)
		{
			Transform parent = renderer.transform.parent;
			Transform transform = renderer.rootBone;
			while (transform != null && transform.parent != null && transform.parent != parent)
			{
				transform = transform.parent;
			}
			return transform;
		}
	}
}
