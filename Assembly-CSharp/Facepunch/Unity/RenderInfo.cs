using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace Facepunch.Unity
{
	// Token: 0x020008A6 RID: 2214
	public static class RenderInfo
	{
		// Token: 0x06002FD0 RID: 12240 RVA: 0x000EB858 File Offset: 0x000E9A58
		public static void GenerateReport()
		{
			Renderer[] array = Object.FindObjectsOfType<Renderer>();
			List<RenderInfo.RendererInstance> list = new List<RenderInfo.RendererInstance>();
			Renderer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RenderInfo.RendererInstance rendererInstance = RenderInfo.RendererInstance.From(array2[i]);
				list.Add(rendererInstance);
			}
			string text = string.Format(Application.dataPath + "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
			string text2 = JsonConvert.SerializeObject(list, 1);
			File.WriteAllText(text, text2);
			string text3 = Application.streamingAssetsPath + "/RenderInfo.exe";
			string text4 = "\"" + text + "\"";
			Debug.Log("Launching " + text3 + " " + text4);
			Process.Start(text3, text4);
		}

		// Token: 0x020008A7 RID: 2215
		public struct RendererInstance
		{
			// Token: 0x04002A8B RID: 10891
			public bool IsVisible;

			// Token: 0x04002A8C RID: 10892
			public bool CastShadows;

			// Token: 0x04002A8D RID: 10893
			public bool Enabled;

			// Token: 0x04002A8E RID: 10894
			public bool RecieveShadows;

			// Token: 0x04002A8F RID: 10895
			public float Size;

			// Token: 0x04002A90 RID: 10896
			public float Distance;

			// Token: 0x04002A91 RID: 10897
			public int BoneCount;

			// Token: 0x04002A92 RID: 10898
			public int MaterialCount;

			// Token: 0x04002A93 RID: 10899
			public int VertexCount;

			// Token: 0x04002A94 RID: 10900
			public int TriangleCount;

			// Token: 0x04002A95 RID: 10901
			public int SubMeshCount;

			// Token: 0x04002A96 RID: 10902
			public int BlendShapeCount;

			// Token: 0x04002A97 RID: 10903
			public string RenderType;

			// Token: 0x04002A98 RID: 10904
			public string MeshName;

			// Token: 0x04002A99 RID: 10905
			public string ObjectName;

			// Token: 0x04002A9A RID: 10906
			public string EntityName;

			// Token: 0x04002A9B RID: 10907
			public uint EntityId;

			// Token: 0x04002A9C RID: 10908
			public bool UpdateWhenOffscreen;

			// Token: 0x04002A9D RID: 10909
			public int ParticleCount;

			// Token: 0x06002FD1 RID: 12241 RVA: 0x000EB90C File Offset: 0x000E9B0C
			public static RenderInfo.RendererInstance From(Renderer renderer)
			{
				RenderInfo.RendererInstance result = default(RenderInfo.RendererInstance);
				result.IsVisible = renderer.isVisible;
				result.CastShadows = (renderer.shadowCastingMode > ShadowCastingMode.Off);
				result.RecieveShadows = renderer.receiveShadows;
				result.Enabled = (renderer.enabled && renderer.gameObject.activeInHierarchy);
				result.Size = renderer.bounds.size.magnitude;
				result.Distance = Vector3.Distance(renderer.bounds.center, Camera.main.transform.position);
				result.MaterialCount = renderer.sharedMaterials.Length;
				result.RenderType = renderer.GetType().Name;
				BaseEntity baseEntity = renderer.gameObject.ToBaseEntity();
				if (baseEntity)
				{
					result.EntityName = baseEntity.PrefabName;
					if (baseEntity.net != null)
					{
						result.EntityId = baseEntity.net.ID;
					}
				}
				else
				{
					result.ObjectName = renderer.transform.GetRecursiveName("");
				}
				if (renderer is MeshRenderer)
				{
					result.BoneCount = 0;
					MeshFilter component = renderer.GetComponent<MeshFilter>();
					if (component)
					{
						result.ReadMesh(component.sharedMesh);
					}
				}
				if (renderer is SkinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					result.ReadMesh(skinnedMeshRenderer.sharedMesh);
					result.UpdateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				}
				if (renderer is ParticleSystemRenderer)
				{
					ParticleSystem component2 = renderer.GetComponent<ParticleSystem>();
					if (component2)
					{
						result.MeshName = component2.name;
						result.ParticleCount = component2.particleCount;
					}
				}
				return result;
			}

			// Token: 0x06002FD2 RID: 12242 RVA: 0x000EBAB4 File Offset: 0x000E9CB4
			public void ReadMesh(Mesh mesh)
			{
				if (mesh == null)
				{
					this.MeshName = "<NULL>";
					return;
				}
				this.VertexCount = mesh.vertexCount;
				this.SubMeshCount = mesh.subMeshCount;
				this.BlendShapeCount = mesh.blendShapeCount;
				this.MeshName = mesh.name;
			}
		}
	}
}
