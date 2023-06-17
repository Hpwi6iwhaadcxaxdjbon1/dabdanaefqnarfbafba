using System;
using UnityEngine;

// Token: 0x02000715 RID: 1813
public class MoveForward : MonoBehaviour
{
	// Token: 0x0400239C RID: 9116
	public float Speed = 2f;

	// Token: 0x060027C1 RID: 10177 RVA: 0x0001F08D File Offset: 0x0001D28D
	protected void Update()
	{
		base.GetComponent<Rigidbody>().velocity = this.Speed * base.transform.forward;
	}
}
