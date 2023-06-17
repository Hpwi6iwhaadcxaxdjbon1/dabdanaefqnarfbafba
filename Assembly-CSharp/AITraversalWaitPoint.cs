using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class AITraversalWaitPoint : MonoBehaviour
{
	// Token: 0x040008D6 RID: 2262
	public float nextFreeTime;

	// Token: 0x06000C27 RID: 3111 RVA: 0x0000B659 File Offset: 0x00009859
	public bool Occupied()
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0000B668 File Offset: 0x00009868
	public void Occupy(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}
}
