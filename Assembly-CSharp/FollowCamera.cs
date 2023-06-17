using System;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class FollowCamera : MonoBehaviour, IClientComponent
{
	// Token: 0x06000FDB RID: 4059 RVA: 0x0000E06C File Offset: 0x0000C26C
	private void LateUpdate()
	{
		if (MainCamera.mainCamera == null)
		{
			return;
		}
		base.transform.position = MainCamera.position;
	}
}
