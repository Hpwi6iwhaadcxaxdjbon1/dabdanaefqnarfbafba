using System;
using UnityEngine;

// Token: 0x02000707 RID: 1799
public class IgnoreCollision : MonoBehaviour
{
	// Token: 0x04002363 RID: 9059
	public Collider collider;

	// Token: 0x06002781 RID: 10113 RVA: 0x0001ED37 File Offset: 0x0001CF37
	protected void OnTriggerEnter(Collider other)
	{
		Debug.Log("IgnoreCollision: " + this.collider.gameObject.name + " + " + other.gameObject.name);
		Physics.IgnoreCollision(other, this.collider, true);
	}
}
