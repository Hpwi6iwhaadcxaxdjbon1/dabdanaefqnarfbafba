using System;
using UnityEngine;

// Token: 0x02000250 RID: 592
public class ObjectSpam : MonoBehaviour
{
	// Token: 0x04000E56 RID: 3670
	public GameObject source;

	// Token: 0x04000E57 RID: 3671
	public int amount = 1000;

	// Token: 0x04000E58 RID: 3672
	public float radius;

	// Token: 0x060011A4 RID: 4516 RVA: 0x00074F90 File Offset: 0x00073190
	private void Start()
	{
		for (int i = 0; i < this.amount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.source);
			gameObject.transform.position = base.transform.position + Vector3Ex.Range(-this.radius, this.radius);
			gameObject.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
		}
	}
}
