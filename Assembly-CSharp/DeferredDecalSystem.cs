using System;
using ConVar;
using UnityEngine.Rendering;

// Token: 0x02000583 RID: 1411
public class DeferredDecalSystem
{
	// Token: 0x04001C33 RID: 7219
	internal static bool IsDirty = false;

	// Token: 0x04001C34 RID: 7220
	internal const int QueueSize = 2;

	// Token: 0x04001C35 RID: 7221
	internal static ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[] DiffuseDecals = new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[]
	{
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8),
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8)
	};

	// Token: 0x04001C36 RID: 7222
	internal static ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[] SpecularDecals = new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[]
	{
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8),
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8)
	};

	// Token: 0x04001C37 RID: 7223
	internal static ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[] NormalsDecals = new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[]
	{
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8),
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8)
	};

	// Token: 0x04001C38 RID: 7224
	internal static ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[] EmissionDecals = new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[]
	{
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8),
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8)
	};

	// Token: 0x04001C39 RID: 7225
	internal static ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[] CombinedDecals = new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>[]
	{
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8),
		new ListDictionary<InstancingKey, ListHashSet<DeferredDecal>>(8)
	};

	// Token: 0x04001C3A RID: 7226
	internal static int DiffusePass = 0;

	// Token: 0x04001C3B RID: 7227
	internal static int SpecularPass = 1;

	// Token: 0x04001C3C RID: 7228
	internal static int NormalsPass = 0;

	// Token: 0x04001C3D RID: 7229
	internal static int EmissionPass = 2;

	// Token: 0x04001C3E RID: 7230
	internal static int CombinedPass = 0;

	// Token: 0x04001C3F RID: 7231
	internal static RenderTargetIdentifier[] DiffuseRenderTarget = new RenderTargetIdentifier[]
	{
		BuiltinRenderTextureType.GBuffer0,
		BuiltinRenderTextureType.CameraTarget
	};

	// Token: 0x04001C40 RID: 7232
	internal static RenderTargetIdentifier[] SpecularRenderTarget = new RenderTargetIdentifier[]
	{
		BuiltinRenderTextureType.GBuffer1
	};

	// Token: 0x04001C41 RID: 7233
	internal static RenderTargetIdentifier[] NormalsRenderTarget = new RenderTargetIdentifier[]
	{
		BuiltinRenderTextureType.GBuffer2
	};

	// Token: 0x04001C42 RID: 7234
	internal static RenderTargetIdentifier[] EmissionRenderTarget = new RenderTargetIdentifier[]
	{
		BuiltinRenderTextureType.CameraTarget
	};

	// Token: 0x04001C43 RID: 7235
	internal static RenderTargetIdentifier[] CombinedRenderTarget = new RenderTargetIdentifier[]
	{
		BuiltinRenderTextureType.GBuffer0,
		BuiltinRenderTextureType.GBuffer2,
		BuiltinRenderTextureType.CameraTarget
	};

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x0600203E RID: 8254 RVA: 0x000AF9B4 File Offset: 0x000ADBB4
	public static bool IsEmpty
	{
		get
		{
			for (int i = 0; i < 2; i++)
			{
				if (DeferredDecalSystem.DiffuseDecals[i].Count > 0)
				{
					return false;
				}
				if (DeferredDecalSystem.SpecularDecals[i].Count > 0)
				{
					return false;
				}
				if (DeferredDecalSystem.NormalsDecals[i].Count > 0)
				{
					return false;
				}
				if (DeferredDecalSystem.EmissionDecals[i].Count > 0)
				{
					return false;
				}
				if (DeferredDecalSystem.CombinedDecals[i].Count > 0)
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000AFA24 File Offset: 0x000ADC24
	public static void Clear()
	{
		for (int i = 0; i < 2; i++)
		{
			DeferredDecalSystem.DiffuseDecals[i].Clear();
			DeferredDecalSystem.SpecularDecals[i].Clear();
			DeferredDecalSystem.NormalsDecals[i].Clear();
			DeferredDecalSystem.EmissionDecals[i].Clear();
			DeferredDecalSystem.CombinedDecals[i].Clear();
		}
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000AFA7C File Offset: 0x000ADC7C
	private static ListHashSet<DeferredDecal> GetList(DeferredDecal decal, ListDictionary<InstancingKey, ListHashSet<DeferredDecal>> dict, int pass)
	{
		InstancingKey instancingKey = new InstancingKey(decal.mesh, 0, decal.material, pass);
		ListHashSet<DeferredDecal> listHashSet;
		if (!dict.TryGetValue(instancingKey, ref listHashSet))
		{
			listHashSet = new ListHashSet<DeferredDecal>(Decal.capacity);
			dict.Add(instancingKey, listHashSet);
		}
		return listHashSet;
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x000AFAC0 File Offset: 0x000ADCC0
	public static void AddDecal(DeferredDecal decal)
	{
		if (!decal.material)
		{
			return;
		}
		bool flag = decal.material.IsKeywordEnabled("_DIFFUSE_ON");
		bool flag2 = decal.material.IsKeywordEnabled("_SPECGLOSS_ON");
		bool flag3 = decal.material.IsKeywordEnabled("_NORMALS_ON");
		bool flag4 = decal.material.IsKeywordEnabled("_EMISSION_ON");
		int queue = (int)decal.queue;
		if (flag && flag3)
		{
			DeferredDecalSystem.GetList(decal, DeferredDecalSystem.CombinedDecals[queue], DeferredDecalSystem.CombinedPass).Add(decal);
		}
		else if (flag)
		{
			DeferredDecalSystem.GetList(decal, DeferredDecalSystem.DiffuseDecals[queue], DeferredDecalSystem.DiffusePass).Add(decal);
		}
		else if (flag3)
		{
			DeferredDecalSystem.GetList(decal, DeferredDecalSystem.NormalsDecals[queue], DeferredDecalSystem.NormalsPass).Add(decal);
		}
		if (flag2)
		{
			DeferredDecalSystem.GetList(decal, DeferredDecalSystem.SpecularDecals[queue], DeferredDecalSystem.SpecularPass).Add(decal);
		}
		if (flag4)
		{
			DeferredDecalSystem.GetList(decal, DeferredDecalSystem.EmissionDecals[queue], DeferredDecalSystem.EmissionPass).Add(decal);
		}
		DeferredDecalSystem.IsDirty = true;
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000AFBB8 File Offset: 0x000ADDB8
	public static void RemoveDecal(DeferredDecal decal)
	{
		int queue = (int)decal.queue;
		DeferredDecalSystem.GetList(decal, DeferredDecalSystem.DiffuseDecals[queue], DeferredDecalSystem.DiffusePass).Remove(decal);
		DeferredDecalSystem.GetList(decal, DeferredDecalSystem.SpecularDecals[queue], DeferredDecalSystem.SpecularPass).Remove(decal);
		DeferredDecalSystem.GetList(decal, DeferredDecalSystem.NormalsDecals[queue], DeferredDecalSystem.NormalsPass).Remove(decal);
		DeferredDecalSystem.GetList(decal, DeferredDecalSystem.EmissionDecals[queue], DeferredDecalSystem.EmissionPass).Remove(decal);
		DeferredDecalSystem.GetList(decal, DeferredDecalSystem.CombinedDecals[queue], DeferredDecalSystem.CombinedPass).Remove(decal);
		DeferredDecalSystem.IsDirty = true;
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x00019859 File Offset: 0x00017A59
	public static void RefreshDecal(DeferredDecal decal)
	{
		DeferredDecalSystem.RemoveDecal(decal);
		DeferredDecalSystem.AddDecal(decal);
	}
}
