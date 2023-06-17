using System;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class SirenLight : IOEntity
{
	// Token: 0x040006DA RID: 1754
	public GameObject lightObj;

	// Token: 0x040006DB RID: 1755
	public float speed;

	// Token: 0x06000A77 RID: 2679 RVA: 0x00055474 File Offset: 0x00053674
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		this.lightObj.SetActive(base.IsPowered());
		if (base.IsPowered())
		{
			this.lightObj.transform.Rotate(Vector3.up, 360f * this.speed * Time.deltaTime);
		}
	}
}
