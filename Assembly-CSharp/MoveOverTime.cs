using System;
using UnityEngine;

// Token: 0x02000479 RID: 1145
public class MoveOverTime : MonoBehaviour
{
	// Token: 0x040017A2 RID: 6050
	[Range(-10f, 10f)]
	public float speed = 1f;

	// Token: 0x040017A3 RID: 6051
	public Vector3 position;

	// Token: 0x040017A4 RID: 6052
	public Vector3 rotation;

	// Token: 0x040017A5 RID: 6053
	public Vector3 scale;

	// Token: 0x06001AB9 RID: 6841 RVA: 0x000938A0 File Offset: 0x00091AA0
	private void Update()
	{
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles + this.rotation * this.speed * Time.deltaTime);
		base.transform.localScale += this.scale * this.speed * Time.deltaTime;
		base.transform.localPosition += this.position * this.speed * Time.deltaTime;
	}
}
