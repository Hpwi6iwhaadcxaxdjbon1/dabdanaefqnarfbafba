using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class BaseHelicopterVehicle : BaseVehicle
{
	// Token: 0x040012CE RID: 4814
	[Header("Helicopter")]
	public Rigidbody rigidBody;

	// Token: 0x040012CF RID: 4815
	public float engineThrustMax;

	// Token: 0x040012D0 RID: 4816
	public Vector3 torqueScale;

	// Token: 0x040012D1 RID: 4817
	public Transform com;

	// Token: 0x040012D2 RID: 4818
	[Header("Effects")]
	public Transform[] GroundPoints;

	// Token: 0x040012D3 RID: 4819
	public Transform[] GroundEffects;

	// Token: 0x040012D4 RID: 4820
	public GameObjectRef explosionEffect;

	// Token: 0x040012D5 RID: 4821
	public GameObjectRef fireBall;

	// Token: 0x040012D6 RID: 4822
	public GameObjectRef impactEffectSmall;

	// Token: 0x040012D7 RID: 4823
	public GameObjectRef impactEffectLarge;

	// Token: 0x060015DB RID: 5595 RVA: 0x00085C74 File Offset: 0x00083E74
	public virtual void UpdateEffects()
	{
		for (int i = 0; i < this.GroundEffects.Length; i++)
		{
			Component component = this.GroundPoints[i];
			Transform transform = this.GroundEffects[i];
			RaycastHit raycastHit;
			if (Physics.Raycast(component.transform.position, -base.transform.up, ref raycastHit, 80f, 8388608))
			{
				transform.gameObject.GetComponent<ParticleSystem>().enableEmission = true;
				transform.transform.position = raycastHit.point + new Vector3(0f, 0.5f, 0f);
			}
			else
			{
				transform.gameObject.GetComponent<ParticleSystem>().enableEmission = false;
			}
		}
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x00012740 File Offset: 0x00010940
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		base.InvokeRepeating(new Action(this.UpdateEffects), 0f, 0.015f);
	}

	// Token: 0x02000340 RID: 832
	public class HelicopterInputState
	{
		// Token: 0x040012D8 RID: 4824
		public float throttle;

		// Token: 0x040012D9 RID: 4825
		public float roll;

		// Token: 0x040012DA RID: 4826
		public float yaw;

		// Token: 0x040012DB RID: 4827
		public float pitch;

		// Token: 0x040012DC RID: 4828
		public bool groundControl;

		// Token: 0x060015DE RID: 5598 RVA: 0x00012766 File Offset: 0x00010966
		public void Reset()
		{
			this.throttle = 0f;
			this.roll = 0f;
			this.yaw = 0f;
			this.pitch = 0f;
			this.groundControl = false;
		}
	}
}
