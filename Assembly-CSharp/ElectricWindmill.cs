using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class ElectricWindmill : IOEntity
{
	// Token: 0x04000744 RID: 1860
	public Animator animator;

	// Token: 0x04000745 RID: 1861
	public int maxPowerGeneration = 100;

	// Token: 0x04000746 RID: 1862
	public Transform vaneRot;

	// Token: 0x04000747 RID: 1863
	public SoundDefinition wooshSound;

	// Token: 0x04000748 RID: 1864
	public Transform wooshOrigin;

	// Token: 0x04000749 RID: 1865
	private float serverWindSpeed;

	// Token: 0x0400074A RID: 1866
	private float lastServerTime;

	// Token: 0x0400074B RID: 1867
	protected static int speedIndex = Animator.StringToHash("speed");

	// Token: 0x0400074C RID: 1868
	public float targetSpeed;

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00056A84 File Offset: 0x00054C84
	public float GetWindSpeedScale()
	{
		float num = Time.time / 600f;
		float num2 = base.transform.position.x / 512f;
		float num3 = base.transform.position.z / 512f;
		float num4 = Mathf.PerlinNoise(num2 + num, num3 + num * 0.1f);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		float num5 = base.transform.position.y - height;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		return Mathf.Clamp01(Mathf.InverseLerp(0f, 50f, num5) * 0.5f + num4);
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00056B34 File Offset: 0x00054D34
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && base.isClient && info.msg.ioEntity != null)
		{
			this.lastServerTime = info.msg.ioEntity.genericFloat1;
			this.serverWindSpeed = info.msg.ioEntity.genericFloat2;
		}
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00056B94 File Offset: 0x00054D94
	public Vector3 GetWindAimDir(float time)
	{
		float num = time / 3600f * 360f;
		int num2 = 10;
		Vector3 vector = new Vector3(Mathf.Sin(num * 0.017453292f) * (float)num2, 0f, Mathf.Cos(num * 0.017453292f) * (float)num2);
		return vector.normalized;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00056BE4 File Offset: 0x00054DE4
	public void Woosh()
	{
		SoundManager.PlayOneshot(this.wooshSound, this.wooshOrigin.gameObject, false, default(Vector3));
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00056C14 File Offset: 0x00054E14
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		float b = this.serverWindSpeed;
		this.targetSpeed = Mathf.Lerp(this.targetSpeed, b, Time.deltaTime * 0.25f);
		this.animator.SetFloat(ElectricWindmill.speedIndex, this.targetSpeed);
		Vector3 windAimDir = this.GetWindAimDir(this.lastServerTime);
		this.vaneRot.transform.rotation = Quaternion.Lerp(this.vaneRot.transform.rotation, Quaternion.LookRotation(windAimDir), Time.deltaTime * 0.5f);
	}
}
