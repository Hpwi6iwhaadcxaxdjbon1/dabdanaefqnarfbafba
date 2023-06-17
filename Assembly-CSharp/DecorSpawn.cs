using System;
using System.Collections;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020004AD RID: 1197
public class DecorSpawn : MonoBehaviour, IClientComponent
{
	// Token: 0x0400189C RID: 6300
	public static List<DecorSpawn> Instances = new List<DecorSpawn>();

	// Token: 0x0400189D RID: 6301
	private static bool Enabled = true;

	// Token: 0x0400189E RID: 6302
	public SpawnFilter Filter;

	// Token: 0x0400189F RID: 6303
	public string ResourceFolder = string.Empty;

	// Token: 0x040018A0 RID: 6304
	public uint Seed;

	// Token: 0x040018A1 RID: 6305
	public float ObjectCutoff = 0.2f;

	// Token: 0x040018A2 RID: 6306
	public float ObjectTapering = 0.2f;

	// Token: 0x040018A3 RID: 6307
	public int ObjectsPerPatch = 10;

	// Token: 0x040018A4 RID: 6308
	public float ClusterRadius = 2f;

	// Token: 0x040018A5 RID: 6309
	public int ClusterSizeMin = 1;

	// Token: 0x040018A6 RID: 6310
	public int ClusterSizeMax = 10;

	// Token: 0x040018A7 RID: 6311
	public int PatchCount = 8;

	// Token: 0x040018A8 RID: 6312
	public int PatchSize = 100;

	// Token: 0x040018A9 RID: 6313
	public bool LOD = true;

	// Token: 0x040018AA RID: 6314
	internal Transform Anchor;

	// Token: 0x040018AB RID: 6315
	internal Prefab[] Prefabs;

	// Token: 0x040018AC RID: 6316
	private DecorPatch[] patches;

	// Token: 0x06001BBE RID: 7102 RVA: 0x00016BB7 File Offset: 0x00014DB7
	public static void SetEnabled(bool b)
	{
		DecorSpawn.Enabled = b;
		DecorSpawn.RemoveAll(true);
		DecorSpawn.RefreshAll(true);
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x0009976C File Offset: 0x0009796C
	public static void RefreshAll(bool force = false)
	{
		foreach (DecorSpawn decorSpawn in DecorSpawn.Instances)
		{
			decorSpawn.Refresh(force);
		}
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000997BC File Offset: 0x000979BC
	public static void RemoveAll(bool force = false)
	{
		foreach (DecorSpawn decorSpawn in DecorSpawn.Instances)
		{
			decorSpawn.Remove(force);
		}
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x00016BCB File Offset: 0x00014DCB
	public void Remove(bool force = false)
	{
		this.FreePatches();
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x0009980C File Offset: 0x00097A0C
	public void Refresh(bool force = false)
	{
		this.Anchor = MainCamera.mainCamera.transform;
		if (!this.Anchor)
		{
			return;
		}
		if (!DecorSpawn.Enabled)
		{
			return;
		}
		this.InitPatches();
		if (this.patches == null)
		{
			return;
		}
		foreach (DecorPatch decorPatch in this.patches)
		{
			if (decorPatch.Shift() || force)
			{
				decorPatch.Spawn();
			}
		}
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00016BD3 File Offset: 0x00014DD3
	[ContextMenu("Refresh All")]
	private void RefreshAll_ContextMenu()
	{
		DecorSpawn.RemoveAll(true);
		DecorSpawn.RefreshAll(true);
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00016BE1 File Offset: 0x00014DE1
	[ContextMenu("Refresh")]
	private void Refresh_ContextMenu()
	{
		this.Remove(true);
		this.Refresh(true);
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x00016BF1 File Offset: 0x00014DF1
	protected void OnEnable()
	{
		DecorSpawn.Instances.Add(this);
		base.StartCoroutine(this.UpdateCoroutine());
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x00016C0B File Offset: 0x00014E0B
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		DecorSpawn.Instances.Remove(this);
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x00099878 File Offset: 0x00097A78
	private void InitPatches()
	{
		if (this.Prefabs == null)
		{
			this.Prefabs = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
			if (this.Prefabs == null || this.Prefabs.Length == 0)
			{
				Debug.LogError("Empty decor folder: " + this.ResourceFolder);
				return;
			}
		}
		if (this.patches == null)
		{
			this.patches = new DecorPatch[this.PatchCount * this.PatchCount];
			for (int i = 0; i < this.PatchCount; i++)
			{
				for (int j = 0; j < this.PatchCount; j++)
				{
					this.patches[j * this.PatchCount + i] = new DecorPatch(this, i, j);
				}
			}
		}
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x00099930 File Offset: 0x00097B30
	private void FreePatches()
	{
		if (this.patches == null)
		{
			return;
		}
		DecorPatch[] array = this.patches;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DestroyInstances();
		}
		this.patches = null;
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x00016C21 File Offset: 0x00014E21
	private IEnumerator UpdateCoroutine()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			int i = 0;
			while (this.patches != null)
			{
				if (i >= this.patches.Length)
				{
					break;
				}
				DecorPatch decorPatch = this.patches[i];
				if (decorPatch.Shift())
				{
					decorPatch.Spawn();
					yield return CoroutineEx.waitForEndOfFrame;
				}
				else
				{
					yield return CoroutineEx.waitForEndOfFrame;
				}
				int num = i;
				i = num + 1;
			}
		}
		yield break;
	}
}
