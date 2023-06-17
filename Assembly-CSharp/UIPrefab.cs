using System;
using Facepunch;
using UnityEngine;

// Token: 0x020006DF RID: 1759
public class UIPrefab : MonoBehaviour
{
	// Token: 0x040022F7 RID: 8951
	public GameObject prefabSource;

	// Token: 0x040022F8 RID: 8952
	internal GameObject createdGameObject;

	// Token: 0x060026F2 RID: 9970 RVA: 0x000CB95C File Offset: 0x000C9B5C
	private void Awake()
	{
		if (this.prefabSource == null)
		{
			return;
		}
		if (this.createdGameObject != null)
		{
			return;
		}
		this.createdGameObject = Facepunch.Instantiate.GameObject(this.prefabSource, null);
		this.createdGameObject.name = this.prefabSource.name;
		this.createdGameObject.transform.SetParent(base.transform, false);
		this.createdGameObject.Identity();
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x0001E698 File Offset: 0x0001C898
	public void SetVisible(bool visible)
	{
		if (this.createdGameObject == null)
		{
			return;
		}
		if (this.createdGameObject.activeSelf == visible)
		{
			return;
		}
		this.createdGameObject.SetActive(visible);
	}
}
