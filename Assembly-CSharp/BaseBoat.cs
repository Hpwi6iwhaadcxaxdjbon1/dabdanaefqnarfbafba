using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class BaseBoat : BaseVehicle
{
	// Token: 0x040012AA RID: 4778
	public float engineThrust = 10f;

	// Token: 0x040012AB RID: 4779
	public float steeringScale = 0.1f;

	// Token: 0x040012AC RID: 4780
	public float gasPedal;

	// Token: 0x040012AD RID: 4781
	public float steering;

	// Token: 0x040012AE RID: 4782
	public Rigidbody myRigidBody;

	// Token: 0x040012AF RID: 4783
	public Transform thrustPoint;

	// Token: 0x040012B0 RID: 4784
	public Transform centerOfMass;

	// Token: 0x040012B1 RID: 4785
	public Buoyancy buoyancy;

	// Token: 0x040012B2 RID: 4786
	public GameObject clientCollider;

	// Token: 0x040012B3 RID: 4787
	public GameObject serverCollider;

	// Token: 0x060015C2 RID: 5570 RVA: 0x000125BA File Offset: 0x000107BA
	public bool InDryDock()
	{
		return base.GetParentEntity() != null;
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x000125C8 File Offset: 0x000107C8
	public override float MaxVelocity()
	{
		return 25f;
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool PhysicsDriven()
	{
		return true;
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x000851A4 File Offset: 0x000833A4
	protected override void ClientInit(Entity info)
	{
		if (this.clientCollider)
		{
			this.clientCollider.gameObject.SetActive(true);
		}
		if (this.serverCollider)
		{
			this.serverCollider.gameObject.SetActive(false);
		}
		base.ClientInit(info);
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x000125CF File Offset: 0x000107CF
	public virtual bool EngineInWater()
	{
		return TerrainMeta.WaterMap.GetHeight(this.thrustPoint.position) > this.thrustPoint.position.y;
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x000125F8 File Offset: 0x000107F8
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		if (TerrainMeta.WaterMap.GetHeight(player.eyes.position) >= player.eyes.position.y)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x00012631 File Offset: 0x00010831
	public bool IsFlipped()
	{
		return Vector3.Dot(Vector3.up, base.transform.up) <= 0f;
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x000851F4 File Offset: 0x000833F4
	public static float GetWaterDepth(Vector3 pos)
	{
		if (Application.isPlaying && !(TerrainMeta.WaterMap == null))
		{
			return TerrainMeta.WaterMap.GetDepth(pos);
		}
		RaycastHit raycastHit;
		if (!Physics.Raycast(pos, Vector3.down, ref raycastHit, 100f, 8388608))
		{
			return 100f;
		}
		return raycastHit.distance;
	}
}
