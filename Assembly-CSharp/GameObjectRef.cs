using System;
using Facepunch;
using UnityEngine;

// Token: 0x0200076F RID: 1903
[Serializable]
public class GameObjectRef : ResourceRef<GameObject>
{
	// Token: 0x0600296F RID: 10607 RVA: 0x000203DF File Offset: 0x0001E5DF
	public GameObject Instantiate(Transform parent = null)
	{
		return Facepunch.Instantiate.GameObject(base.Get(), parent);
	}
}
