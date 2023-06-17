using System;
using UnityEngine;

// Token: 0x02000722 RID: 1826
public class UpdateHandler : SingletonComponent<UpdateHandler>
{
	// Token: 0x040023CB RID: 9163
	private ListHashSet<UpdateBehaviour> list = new ListHashSet<UpdateBehaviour>(1024);

	// Token: 0x060027F8 RID: 10232 RVA: 0x000CE2BC File Offset: 0x000CC4BC
	protected void Update()
	{
		UpdateBehaviour[] buffer = this.list.Values.Buffer;
		int count = this.list.Count;
		float time = Time.time;
		for (int i = 0; i < count; i++)
		{
			UpdateBehaviour updateBehaviour = buffer[i];
			if (time >= updateBehaviour.nextUpdate)
			{
				updateBehaviour.nextUpdate = time;
				updateBehaviour.DeltaUpdate(time - updateBehaviour.lastUpdate);
				updateBehaviour.lastUpdate = time;
			}
		}
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x0001F2A0 File Offset: 0x0001D4A0
	public static void Add(UpdateBehaviour behaviour)
	{
		if (!SingletonComponent<UpdateHandler>.Instance)
		{
			UpdateHandler.CreateInstance();
		}
		SingletonComponent<UpdateHandler>.Instance.list.Add(behaviour);
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x0001F2C3 File Offset: 0x0001D4C3
	public static void Remove(UpdateBehaviour behaviour)
	{
		if (SingletonComponent<UpdateHandler>.Instance)
		{
			SingletonComponent<UpdateHandler>.Instance.list.Remove(behaviour);
		}
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x0001F2E2 File Offset: 0x0001D4E2
	private static void CreateInstance()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "UpdateHandler";
		gameObject.AddComponent<UpdateHandler>();
		Object.DontDestroyOnLoad(gameObject);
	}
}
