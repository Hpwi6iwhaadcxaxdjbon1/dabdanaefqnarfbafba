using System;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000393 RID: 915
public class GameManager
{
	// Token: 0x04001410 RID: 5136
	public static GameManager client = new GameManager(true, false);

	// Token: 0x04001411 RID: 5137
	internal PrefabPreProcess preProcessed;

	// Token: 0x04001412 RID: 5138
	internal PrefabPoolCollection pool;

	// Token: 0x04001413 RID: 5139
	private bool Clientside;

	// Token: 0x04001414 RID: 5140
	private bool Serverside;

	// Token: 0x0600174C RID: 5964 RVA: 0x000138F3 File Offset: 0x00011AF3
	public void Reset()
	{
		this.pool.Clear();
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x00013900 File Offset: 0x00011B00
	public GameManager(bool clientside, bool serverside)
	{
		this.Clientside = clientside;
		this.Serverside = serverside;
		this.preProcessed = new PrefabPreProcess(clientside, serverside, false);
		this.pool = new PrefabPoolCollection();
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x00089F78 File Offset: 0x00088178
	public GameObject FindPrefab(uint prefabID)
	{
		string text = StringPool.Get(prefabID);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return this.FindPrefab(text);
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x0001392F File Offset: 0x00011B2F
	public GameObject FindPrefab(BaseEntity ent)
	{
		if (ent == null)
		{
			return null;
		}
		return this.FindPrefab(ent.PrefabName);
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x00089FA0 File Offset: 0x000881A0
	public GameObject FindPrefab(string strPrefab)
	{
		GameObject gameObject = this.preProcessed.Find(strPrefab);
		if (gameObject != null)
		{
			return gameObject;
		}
		gameObject = FileSystem.LoadPrefab(strPrefab);
		if (gameObject == null)
		{
			return null;
		}
		this.preProcessed.Process(strPrefab, gameObject);
		return this.preProcessed.Find(strPrefab);
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x00089FF4 File Offset: 0x000881F4
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject)
		{
			gameObject.transform.localScale = scale;
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x0008A02C File Offset: 0x0008822C
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x0008A058 File Offset: 0x00088258
	public GameObject CreatePrefab(string strPrefab, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, Vector3.zero, Quaternion.identity);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x0008A08C File Offset: 0x0008828C
	public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, parent.position, parent.rotation);
		if (gameObject)
		{
			gameObject.transform.SetParent(parent, false);
			gameObject.Identity();
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x0008A0D4 File Offset: 0x000882D4
	public BaseEntity CreateEntity(string strPrefab, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), bool startActive = true)
	{
		if (string.IsNullOrEmpty(strPrefab))
		{
			return null;
		}
		GameObject gameObject = this.CreatePrefab(strPrefab, pos, rot, startActive);
		if (gameObject == null)
		{
			return null;
		}
		BaseEntity component = gameObject.GetComponent<BaseEntity>();
		if (!component)
		{
			Debug.LogError("CreateEntity called on a prefab that isn't an entity! " + strPrefab);
			Object.Destroy(gameObject);
			return null;
		}
		return component;
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x0008A12C File Offset: 0x0008832C
	private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
	{
		if (!StringEx.IsLower(strPrefab))
		{
			Debug.LogWarning("Converting prefab name to lowercase: " + strPrefab);
			strPrefab = strPrefab.ToLower();
		}
		GameObject gameObject = this.FindPrefab(strPrefab);
		if (!gameObject)
		{
			Debug.LogError("Couldn't find prefab \"" + strPrefab + "\"");
			return null;
		}
		GameObject gameObject2 = this.pool.Pop(StringPool.Get(strPrefab), pos, rot);
		if (gameObject2 == null)
		{
			gameObject2 = Facepunch.Instantiate.GameObject(gameObject, pos, rot);
			gameObject2.name = strPrefab;
		}
		else
		{
			gameObject2.transform.localScale = gameObject.transform.localScale;
		}
		if (this.Clientside && !this.Serverside && gameObject2.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject2, Rust.Client.EntityScene);
		}
		return gameObject2;
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x00013948 File Offset: 0x00011B48
	public static void Destroy(Component component, float delay = 0f)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		Object.Destroy(component, delay);
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x00013973 File Offset: 0x00011B73
	public static void Destroy(GameObject instance, float delay = 0f)
	{
		if (!instance)
		{
			return;
		}
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		Object.Destroy(instance, delay);
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x000139A7 File Offset: 0x00011BA7
	public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		Object.DestroyImmediate(component, allowDestroyingAssets);
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x000139D2 File Offset: 0x00011BD2
	public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
	{
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		Object.DestroyImmediate(instance, allowDestroyingAssets);
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x0008A1F4 File Offset: 0x000883F4
	public void Retire(GameObject instance)
	{
		if (!instance)
		{
			return;
		}
		using (TimeWarning.New("GameManager.Retire", 0.1f))
		{
			if (instance.GetComponent<BaseEntity>().IsValid())
			{
				Debug.LogError("Trying to retire an entity without killing it first: " + instance.name);
			}
			if (!Application.isQuitting && Pool.enabled && instance.SupportsPooling())
			{
				this.pool.Push(instance);
			}
			else
			{
				Object.Destroy(instance);
			}
		}
	}
}
