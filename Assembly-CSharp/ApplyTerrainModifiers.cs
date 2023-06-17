using System;
using UnityEngine;

// Token: 0x02000539 RID: 1337
public class ApplyTerrainModifiers : MonoBehaviour
{
	// Token: 0x06001E26 RID: 7718 RVA: 0x000A5A88 File Offset: 0x000A3C88
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainModifier[] modifiers = null;
		if (component.isClient)
		{
			modifiers = PrefabAttribute.client.FindAll<TerrainModifier>(component.prefabID);
		}
		base.transform.ApplyTerrainModifiers(modifiers);
		GameManager.Destroy(this, 0f);
	}
}
