using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using VLB;

// Token: 0x020003E2 RID: 994
public class PrefabPreProcess : IPrefabProcessor
{
	// Token: 0x0400154E RID: 5454
	public static Type[] clientsideOnlyTypes = new Type[]
	{
		typeof(IClientComponent),
		typeof(ImageEffectLayer),
		typeof(NGSS_Directional),
		typeof(VolumetricDustParticles),
		typeof(VolumetricLightBeam),
		typeof(Cloth),
		typeof(MeshFilter),
		typeof(Renderer),
		typeof(AudioLowPassFilter),
		typeof(AudioSource),
		typeof(AudioListener),
		typeof(ParticleSystemRenderer),
		typeof(ParticleSystem),
		typeof(ParticleEmitFromParentObject),
		typeof(Light),
		typeof(LODGroup),
		typeof(Animator),
		typeof(AnimationEvents),
		typeof(PlayerVoiceSpeaker),
		typeof(PlayerVoiceRecorder),
		typeof(ParticleScaler),
		typeof(PostEffectsBase),
		typeof(TOD_ImageEffect),
		typeof(Tree),
		typeof(Projector),
		typeof(HttpImage),
		typeof(EventTrigger),
		typeof(StandaloneInputModule),
		typeof(UIBehaviour),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasGroup),
		typeof(GraphicRaycaster)
	};

	// Token: 0x0400154F RID: 5455
	public static Type[] serversideOnlyTypes = new Type[]
	{
		typeof(IServerComponent),
		typeof(NavMeshObstacle)
	};

	// Token: 0x04001550 RID: 5456
	public bool isClientside;

	// Token: 0x04001551 RID: 5457
	public bool isServerside;

	// Token: 0x04001552 RID: 5458
	public bool isBundling;

	// Token: 0x04001553 RID: 5459
	internal Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x04001554 RID: 5460
	private List<Component> destroyList = new List<Component>();

	// Token: 0x04001555 RID: 5461
	private List<GameObject> cleanupList = new List<GameObject>();

	// Token: 0x060018DE RID: 6366 RVA: 0x0008E1B8 File Offset: 0x0008C3B8
	public PrefabPreProcess(bool clientside, bool serverside, bool bundling = false)
	{
		this.isClientside = clientside;
		this.isServerside = serverside;
		this.isBundling = bundling;
	}

	// Token: 0x060018DF RID: 6367 RVA: 0x0008E208 File Offset: 0x0008C408
	public GameObject Find(string strPrefab)
	{
		GameObject gameObject;
		if (!this.prefabList.TryGetValue(strPrefab, ref gameObject))
		{
			return null;
		}
		if (gameObject == null)
		{
			this.prefabList.Remove(strPrefab);
			return null;
		}
		return gameObject;
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x0008E240 File Offset: 0x0008C440
	public bool NeedsProcessing(GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return false;
		}
		if (PrefabPreProcess.HasComponents<IPrefabPreProcess>(go.transform))
		{
			return true;
		}
		if (PrefabPreProcess.HasComponents<IPrefabPostProcess>(go.transform))
		{
			return true;
		}
		if (PrefabPreProcess.HasComponents<IEditorComponent>(go.transform))
		{
			return true;
		}
		if (!this.isClientside)
		{
			if (Enumerable.Any<Type>(PrefabPreProcess.clientsideOnlyTypes, (Type type) => PrefabPreProcess.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (PrefabPreProcess.HasComponents<IClientComponentEx>(go.transform))
			{
				return true;
			}
		}
		if (!this.isServerside)
		{
			if (Enumerable.Any<Type>(PrefabPreProcess.serversideOnlyTypes, (Type type) => PrefabPreProcess.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (PrefabPreProcess.HasComponents<IServerComponentEx>(go.transform))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x0008E318 File Offset: 0x0008C518
	public void ProcessObject(string name, GameObject go, bool resetLocalTransform = true)
	{
		if (!this.isClientside)
		{
			foreach (Type t in PrefabPreProcess.clientsideOnlyTypes)
			{
				this.DestroyComponents(t, go, this.isClientside, this.isServerside);
			}
			foreach (IClientComponentEx clientComponentEx in PrefabPreProcess.FindComponents<IClientComponentEx>(go.transform))
			{
				clientComponentEx.PreClientComponentCull(this);
			}
		}
		if (!this.isServerside)
		{
			foreach (Type t2 in PrefabPreProcess.serversideOnlyTypes)
			{
				this.DestroyComponents(t2, go, this.isClientside, this.isServerside);
			}
			foreach (IServerComponentEx serverComponentEx in PrefabPreProcess.FindComponents<IServerComponentEx>(go.transform))
			{
				serverComponentEx.PreServerComponentCull(this);
			}
		}
		this.DestroyComponents(typeof(IEditorComponent), go, this.isClientside, this.isServerside);
		if (resetLocalTransform)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
		}
		List<Transform> list = PrefabPreProcess.FindComponents<Transform>(go.transform);
		list.Reverse();
		foreach (IPrefabPreProcess prefabPreProcess in PrefabPreProcess.FindComponents<IPrefabPreProcess>(go.transform))
		{
			prefabPreProcess.PreProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
		foreach (Transform transform in list)
		{
			if (transform && transform.gameObject)
			{
				if (this.isServerside && transform.gameObject.CompareTag("Server Cull"))
				{
					this.RemoveComponents(transform.gameObject);
					this.NominateForDeletion(transform.gameObject);
				}
				if (this.isClientside)
				{
					bool flag = transform.gameObject.CompareTag("Client Cull");
					bool flag2 = transform != go.transform && transform.gameObject.GetComponent<BaseEntity>() != null;
					if (flag || flag2)
					{
						this.RemoveComponents(transform.gameObject);
						this.NominateForDeletion(transform.gameObject);
					}
				}
			}
		}
		this.RunCleanupQueue();
		foreach (IPrefabPostProcess prefabPostProcess in PrefabPreProcess.FindComponents<IPrefabPostProcess>(go.transform))
		{
			prefabPostProcess.PostProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x0008E61C File Offset: 0x0008C81C
	public void Process(string name, GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return;
		}
		GameObject hierarchyGroup = this.GetHierarchyGroup();
		GameObject gameObject = go;
		go = Instantiate.GameObject(gameObject, hierarchyGroup.transform);
		go.name = gameObject.name;
		if (this.NeedsProcessing(go))
		{
			this.ProcessObject(name, go, true);
		}
		this.AddPrefab(name, go);
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x00014C56 File Offset: 0x00012E56
	public GameObject GetHierarchyGroup()
	{
		if (this.isClientside && this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Generic", false, true);
		}
		if (this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Server", false, true);
		}
		return HierarchyUtil.GetRoot("PrefabPreProcess - Client", false, true);
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x00014C96 File Offset: 0x00012E96
	public void AddPrefab(string name, GameObject go)
	{
		go.SetActive(false);
		this.prefabList.Add(name, go);
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x0008E674 File Offset: 0x0008C874
	private void DestroyComponents(Type t, GameObject go, bool client, bool server)
	{
		List<Component> list = new List<Component>();
		PrefabPreProcess.FindComponents(go.transform, list, t);
		list.Reverse();
		foreach (Component component in list)
		{
			RealmedRemove component2 = component.GetComponent<RealmedRemove>();
			if (!(component2 != null) || component2.ShouldDelete(component, client, server))
			{
				if (!component.gameObject.CompareTag("persist"))
				{
					this.NominateForDeletion(component.gameObject);
				}
				Object.DestroyImmediate(component, true);
			}
		}
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x00014CAC File Offset: 0x00012EAC
	private static bool ShouldExclude(Transform transform)
	{
		return transform.GetComponent<BaseEntity>() != null;
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x0008E718 File Offset: 0x0008C918
	private static bool HasComponents<T>(Transform transform)
	{
		if (transform.GetComponent<T>() != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!PrefabPreProcess.ShouldExclude(transform2) && PrefabPreProcess.HasComponents<T>(transform2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x0008E78C File Offset: 0x0008C98C
	private static bool HasComponents(Transform transform, Type t)
	{
		if (transform.GetComponent(t) != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!PrefabPreProcess.ShouldExclude(transform2) && PrefabPreProcess.HasComponents(transform2, t))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x0008E804 File Offset: 0x0008CA04
	public static List<T> FindComponents<T>(Transform transform)
	{
		List<T> list = new List<T>();
		PrefabPreProcess.FindComponents<T>(transform, list);
		return list;
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x0008E820 File Offset: 0x0008CA20
	public static void FindComponents<T>(Transform transform, List<T> list)
	{
		list.AddRange(transform.GetComponents<T>());
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!PrefabPreProcess.ShouldExclude(transform2))
			{
				PrefabPreProcess.FindComponents<T>(transform2, list);
			}
		}
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x0008E888 File Offset: 0x0008CA88
	public static List<Component> FindComponents(Transform transform, Type t)
	{
		List<Component> list = new List<Component>();
		PrefabPreProcess.FindComponents(transform, list, t);
		return list;
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x0008E8A4 File Offset: 0x0008CAA4
	public static void FindComponents(Transform transform, List<Component> list, Type t)
	{
		list.AddRange(transform.GetComponents(t));
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!PrefabPreProcess.ShouldExclude(transform2))
			{
				PrefabPreProcess.FindComponents(transform2, list, t);
			}
		}
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x00014CBF File Offset: 0x00012EBF
	public void RemoveComponent(Component c)
	{
		if (c == null)
		{
			return;
		}
		this.destroyList.Add(c);
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x0008E910 File Offset: 0x0008CB10
	public void RemoveComponents(GameObject gameObj)
	{
		foreach (Component component in gameObj.GetComponents<Component>())
		{
			if (!(component is Transform))
			{
				this.destroyList.Add(component);
			}
		}
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x00014CD7 File Offset: 0x00012ED7
	public void NominateForDeletion(GameObject gameObj)
	{
		this.cleanupList.Add(gameObj);
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x0008E94C File Offset: 0x0008CB4C
	private void RunCleanupQueue()
	{
		foreach (Component obj in this.destroyList)
		{
			Object.DestroyImmediate(obj, true);
		}
		this.destroyList.Clear();
		foreach (GameObject go in this.cleanupList)
		{
			this.DoCleanup(go);
		}
		this.cleanupList.Clear();
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x0008E9F8 File Offset: 0x0008CBF8
	private void DoCleanup(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		if (go.GetComponentsInChildren<Component>(true).Length > 1)
		{
			return;
		}
		Transform parent = go.transform.parent;
		if (parent == null)
		{
			return;
		}
		if (parent.name.StartsWith("PrefabPreProcess - "))
		{
			return;
		}
		Object.DestroyImmediate(go, true);
	}
}
