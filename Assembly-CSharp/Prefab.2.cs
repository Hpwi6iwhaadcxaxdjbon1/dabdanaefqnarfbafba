using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003DD RID: 989
public class Prefab : IComparable<Prefab>
{
	// Token: 0x0400153B RID: 5435
	public uint ID;

	// Token: 0x0400153C RID: 5436
	public string Name;

	// Token: 0x0400153D RID: 5437
	public GameObject Object;

	// Token: 0x0400153E RID: 5438
	public GameManager Manager;

	// Token: 0x0400153F RID: 5439
	public PrefabAttribute.Library Attribute;

	// Token: 0x04001540 RID: 5440
	public PrefabParameters Parameters;

	// Token: 0x060018C0 RID: 6336 RVA: 0x00014B39 File Offset: 0x00012D39
	public Prefab(string name, GameObject prefab, GameManager manager, PrefabAttribute.Library attribute)
	{
		this.ID = StringPool.Get(name);
		this.Name = name;
		this.Object = prefab;
		this.Manager = manager;
		this.Attribute = attribute;
		this.Parameters = prefab.GetComponent<PrefabParameters>();
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x00014B76 File Offset: 0x00012D76
	public static implicit operator GameObject(Prefab prefab)
	{
		return prefab.Object;
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x0008DD28 File Offset: 0x0008BF28
	public int CompareTo(Prefab that)
	{
		if (that == null)
		{
			return 1;
		}
		PrefabPriority prefabPriority = (this.Parameters != null) ? this.Parameters.Priority : PrefabPriority.Default;
		return ((that.Parameters != null) ? that.Parameters.Priority : PrefabPriority.Default).CompareTo(prefabPriority);
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x0008DD88 File Offset: 0x0008BF88
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		TerrainAnchor[] anchors = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, mode, filter);
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x0008DDC0 File Offset: 0x0008BFC0
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainAnchor[] anchors = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, filter);
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x0008DDF8 File Offset: 0x0008BFF8
	public bool ApplyTerrainChecks(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainCheck[] anchors = this.Attribute.FindAll<TerrainCheck>(this.ID);
		return this.Object.transform.ApplyTerrainChecks(anchors, pos, rot, scale, filter);
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x0008DE30 File Offset: 0x0008C030
	public bool ApplyTerrainFilters(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainFilter[] filters = this.Attribute.FindAll<TerrainFilter>(this.ID);
		return this.Object.transform.ApplyTerrainFilters(filters, pos, rot, scale, filter);
	}

	// Token: 0x060018C7 RID: 6343 RVA: 0x0008DE68 File Offset: 0x0008C068
	public void ApplyTerrainModifiers(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainModifier[] modifiers = this.Attribute.FindAll<TerrainModifier>(this.ID);
		this.Object.transform.ApplyTerrainModifiers(modifiers, pos, rot, scale);
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x0008DE9C File Offset: 0x0008C09C
	public void ApplyTerrainPlacements(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainPlacement[] placements = this.Attribute.FindAll<TerrainPlacement>(this.ID);
		this.Object.transform.ApplyTerrainPlacements(placements, pos, rot, scale);
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x0008DED0 File Offset: 0x0008C0D0
	public bool ApplyWaterChecks(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		WaterCheck[] anchors = this.Attribute.FindAll<WaterCheck>(this.ID);
		return this.Object.transform.ApplyWaterChecks(anchors, pos, rot, scale);
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x0008DF04 File Offset: 0x0008C104
	public void ApplyDecorComponents(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		DecorComponent[] components = this.Attribute.FindAll<DecorComponent>(this.ID);
		this.Object.transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x00014B7E File Offset: 0x00012D7E
	public bool CheckEnvironmentVolumes(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		return this.Object.transform.CheckEnvironmentVolumes(pos, rot, scale, type);
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x00014B95 File Offset: 0x00012D95
	public GameObject Spawn(Transform transform)
	{
		return this.Manager.CreatePrefab(this.Name, transform, true);
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x00014BAA File Offset: 0x00012DAA
	public GameObject Spawn(Vector3 pos, Quaternion rot)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, true);
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x00014BC0 File Offset: 0x00012DC0
	public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, scale, true);
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x00014BD7 File Offset: 0x00012DD7
	public BaseEntity SpawnEntity(Vector3 pos, Quaternion rot)
	{
		return this.Manager.CreateEntity(this.Name, pos, rot, true);
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x0008DF38 File Offset: 0x0008C138
	public static Prefab<T> Load<T>(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string text = StringPool.Get(id);
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x0008DF7C File Offset: 0x0008C17C
	public static Prefab[] Load(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		Prefab[] array2 = new Prefab[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array[i];
			GameObject prefab = manager.FindPrefab(text);
			array2[i] = new Prefab(text, prefab, manager, attribute);
		}
		return array2;
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x00014BED File Offset: 0x00012DED
	public static Prefab<T>[] Load<T>(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		return Prefab.Load<T>(Prefab.FindPrefabNames(folder, useProbabilities), manager, attribute);
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x0008DFE4 File Offset: 0x0008C1E4
	public static Prefab<T>[] Load<T>(string[] names, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		Prefab<T>[] array = new Prefab<T>[names.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = names[i];
			GameObject gameObject = manager.FindPrefab(text);
			T component = gameObject.GetComponent<T>();
			array[i] = new Prefab<T>(text, gameObject, component, manager, attribute);
		}
		return array;
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x0008E040 File Offset: 0x0008C240
	public static Prefab LoadRandom(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject prefab = manager.FindPrefab(text);
		return new Prefab(text, prefab, manager, attribute);
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x0008E098 File Offset: 0x0008C298
	public static Prefab<T> LoadRandom<T>(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x060018D6 RID: 6358 RVA: 0x00014C07 File Offset: 0x00012E07
	public static PrefabAttribute.Library DefaultAttribute
	{
		get
		{
			return PrefabAttribute.client;
		}
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x060018D7 RID: 6359 RVA: 0x00014C0E File Offset: 0x00012E0E
	public static GameManager DefaultManager
	{
		get
		{
			return GameManager.client;
		}
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x0008E0F8 File Offset: 0x0008C2F8
	private static string[] FindPrefabNames(string strPrefab, bool useProbabilities = false)
	{
		strPrefab = strPrefab.TrimEnd(new char[]
		{
			'/'
		}).ToLower();
		GameObject[] array = FileSystem.LoadPrefabs(strPrefab + "/");
		List<string> list = new List<string>(array.Length);
		foreach (GameObject gameObject in array)
		{
			string text = strPrefab + "/" + gameObject.name.ToLower() + ".prefab";
			if (!useProbabilities)
			{
				list.Add(text);
			}
			else
			{
				PrefabParameters component = gameObject.GetComponent<PrefabParameters>();
				int num = component ? component.Count : 1;
				for (int j = 0; j < num; j++)
				{
					list.Add(text);
				}
			}
		}
		list.Sort();
		return list.ToArray();
	}
}
