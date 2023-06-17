using System;
using UnityEngine;

// Token: 0x02000374 RID: 884
public class CreateEffect : MonoBehaviour
{
	// Token: 0x04001387 RID: 4999
	public GameObjectRef EffectToCreate;

	// Token: 0x0600169A RID: 5786 RVA: 0x00013189 File Offset: 0x00011389
	public void OnEnable()
	{
		Effect.client.Run(this.EffectToCreate.resourcePath, base.transform.position, base.transform.up, base.transform.forward);
	}
}
