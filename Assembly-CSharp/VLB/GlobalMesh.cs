using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020007D4 RID: 2004
	public static class GlobalMesh
	{
		// Token: 0x040027A4 RID: 10148
		private static Mesh ms_Mesh;

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06002BB1 RID: 11185 RVA: 0x000DFC30 File Offset: 0x000DDE30
		public static Mesh mesh
		{
			get
			{
				if (GlobalMesh.ms_Mesh == null)
				{
					GlobalMesh.ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
					GlobalMesh.ms_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				}
				return GlobalMesh.ms_Mesh;
			}
		}
	}
}
