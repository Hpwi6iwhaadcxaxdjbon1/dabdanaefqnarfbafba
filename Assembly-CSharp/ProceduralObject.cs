using System;
using UnityEngine;

// Token: 0x02000532 RID: 1330
public abstract class ProceduralObject : MonoBehaviour
{
	// Token: 0x06001E0E RID: 7694 RVA: 0x00017DE6 File Offset: 0x00015FE6
	protected void Awake()
	{
		if (SingletonComponent<WorldSetup>.Instance == null)
		{
			return;
		}
		if (SingletonComponent<WorldSetup>.Instance.ProceduralObjects == null)
		{
			Debug.LogError("WorldSetup.Instance.ProceduralObjects is null.", this);
			return;
		}
		SingletonComponent<WorldSetup>.Instance.ProceduralObjects.Add(this);
	}

	// Token: 0x06001E0F RID: 7695
	public abstract void Process();
}
