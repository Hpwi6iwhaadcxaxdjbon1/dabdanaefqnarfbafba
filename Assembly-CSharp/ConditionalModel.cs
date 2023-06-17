using System;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class ConditionalModel : PrefabAttribute
{
	// Token: 0x04000B38 RID: 2872
	public GameObjectRef prefab;

	// Token: 0x04000B39 RID: 2873
	public bool onClient = true;

	// Token: 0x04000B3A RID: 2874
	public bool onServer = true;

	// Token: 0x04000B3B RID: 2875
	[NonSerialized]
	public ModelConditionTest[] conditions;

	// Token: 0x06000E29 RID: 3625 RVA: 0x0000D0FB File Offset: 0x0000B2FB
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.conditions = base.GetComponentsInChildren<ModelConditionTest>(true);
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x00063BEC File Offset: 0x00061DEC
	public bool RunTests(BaseEntity parent)
	{
		for (int i = 0; i < this.conditions.Length; i++)
		{
			if (!this.conditions[i].DoTest(parent))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x00063C20 File Offset: 0x00061E20
	public GameObject InstantiateSkin(BaseEntity parent)
	{
		if (!this.onClient && base.isClient)
		{
			return null;
		}
		GameObject gameObject = this.gameManager.CreatePrefab(this.prefab.resourcePath, parent.transform, false);
		if (gameObject)
		{
			gameObject.transform.localPosition = this.worldPosition;
			gameObject.transform.localRotation = this.worldRotation;
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x0000D117 File Offset: 0x0000B317
	protected override Type GetIndexedType()
	{
		return typeof(ConditionalModel);
	}
}
