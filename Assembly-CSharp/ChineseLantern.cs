using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class ChineseLantern : BaseFuelLightSource
{
	// Token: 0x04000758 RID: 1880
	public Transform pivotRotator;

	// Token: 0x04000759 RID: 1881
	public float swaySpeed = 1f;

	// Token: 0x0400075A RID: 1882
	public float swayDistance = 0.25f;

	// Token: 0x0400075B RID: 1883
	public float lerpSpeed = 2f;

	// Token: 0x0400075C RID: 1884
	private float lookupIndex;

	// Token: 0x06000B05 RID: 2821 RVA: 0x0000AB48 File Offset: 0x00008D48
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.lookupIndex = Random.Range(0f, 50f);
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00056D70 File Offset: 0x00054F70
	public void Update()
	{
		this.lookupIndex += Time.deltaTime;
		float num = this.lookupIndex * this.swaySpeed;
		float num2 = Mathf.PerlinNoise(num, 0f);
		float num3 = Mathf.PerlinNoise(0f, num + 150f);
		Vector3 normalized = (base.transform.position + Vector3.down * 2f + new Vector3(num2 - 0.5f, 0f, num3 - 0.5f).normalized * this.swayDistance - this.pivotRotator.position).normalized;
		Quaternion b = QuaternionEx.LookRotationForcedUp(this.pivotRotator.forward, -normalized);
		this.pivotRotator.rotation = Quaternion.Lerp(this.pivotRotator.rotation, b, Time.deltaTime * this.lerpSpeed);
	}
}
