using System;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class AnimationEventForward : MonoBehaviour
{
	// Token: 0x04000C05 RID: 3077
	public GameObject targetObject;

	// Token: 0x06000EE6 RID: 3814 RVA: 0x0000D6DA File Offset: 0x0000B8DA
	public void Event(string type)
	{
		this.targetObject.SendMessage(type);
	}
}
