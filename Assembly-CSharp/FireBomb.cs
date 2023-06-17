using System;
using UnityEngine;

// Token: 0x0200027D RID: 637
public class FireBomb : MonoBehaviour, IClientComponent
{
	// Token: 0x04000EE1 RID: 3809
	public GameObject fireParticle;

	// Token: 0x04000EE2 RID: 3810
	public float bombRadius;

	// Token: 0x04000EE3 RID: 3811
	public float particleDuration;

	// Token: 0x04000EE4 RID: 3812
	public float emitDuration;

	// Token: 0x04000EE5 RID: 3813
	private float particleSpawnRadius;

	// Token: 0x04000EE6 RID: 3814
	private float emitIntervalTime;

	// Token: 0x04000EE7 RID: 3815
	private float emitStartTime;

	// Token: 0x04000EE8 RID: 3816
	private float nextEmitTime;

	// Token: 0x06001246 RID: 4678 RVA: 0x00077FC0 File Offset: 0x000761C0
	private void Start()
	{
		this.emitStartTime = Time.time;
		this.emitIntervalTime = this.emitDuration / 5f;
		this.nextEmitTime = Time.time + this.emitIntervalTime;
		this.particleSpawnRadius = 0f;
		this.CreateParticleEffect();
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x00078010 File Offset: 0x00076210
	private void Update()
	{
		float num = Time.time - this.emitStartTime;
		if (num > this.emitDuration + this.particleDuration * 1.25f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (num <= this.emitDuration)
		{
			float num2 = Mathf.Clamp(num / this.emitDuration, 0.2f, 1f);
			this.particleSpawnRadius = this.bombRadius * num2;
			if (this.nextEmitTime <= Time.time)
			{
				this.nextEmitTime = Time.time + this.emitIntervalTime;
				this.CreateParticleEffect();
				this.emitIntervalTime *= 0.7f;
			}
		}
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x000780B4 File Offset: 0x000762B4
	private void CreateParticleEffect()
	{
		Vector3 vector = Random.insideUnitSphere;
		vector.y = 0f;
		vector *= this.particleSpawnRadius;
		vector += base.transform.position;
		vector.y += this.bombRadius;
		RaycastHit raycastHit;
		Vector3 position;
		if (Physics.Raycast(vector, -Vector3.up, ref raycastHit, 100f, 8454144))
		{
			position = raycastHit.point;
		}
		else
		{
			position = vector;
		}
		Object.Instantiate<GameObject>(this.fireParticle, position, Quaternion.LookRotation(Vector3.up));
	}
}
