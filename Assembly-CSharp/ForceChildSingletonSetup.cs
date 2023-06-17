using System;
using UnityEngine;

// Token: 0x020006FA RID: 1786
public class ForceChildSingletonSetup : MonoBehaviour
{
	// Token: 0x06002752 RID: 10066 RVA: 0x000CC79C File Offset: 0x000CA99C
	[ComponentHelp("Any child objects of this object that contain SingletonComponents will be registered - even if they're not enabled")]
	private void Awake()
	{
		SingletonComponent[] componentsInChildren = base.GetComponentsInChildren<SingletonComponent>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Setup();
		}
	}
}
