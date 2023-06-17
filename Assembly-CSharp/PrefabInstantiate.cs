using System;
using UnityEngine;

// Token: 0x020003DF RID: 991
public class PrefabInstantiate : MonoBehaviour, IClientComponent
{
	// Token: 0x04001545 RID: 5445
	public GameObjectRef Prefab;

	// Token: 0x060018DB RID: 6363 RVA: 0x00014C21 File Offset: 0x00012E21
	protected void Awake()
	{
		GameManager.client.CreatePrefab(this.Prefab.resourcePath, base.transform, true);
	}
}
