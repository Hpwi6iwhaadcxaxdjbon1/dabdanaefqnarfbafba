using System;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class NPCFootstepEffects : BaseFootstepEffect
{
	// Token: 0x04000EFD RID: 3837
	public string impactEffectDirectory = "footstep/stag";

	// Token: 0x04000EFE RID: 3838
	public Transform frontLeftFoot;

	// Token: 0x04000EFF RID: 3839
	public Transform frontRightFoot;

	// Token: 0x04000F00 RID: 3840
	public Transform backLeftFoot;

	// Token: 0x04000F01 RID: 3841
	public Transform backRightFoot;

	// Token: 0x0600125E RID: 4702 RVA: 0x0000FD0E File Offset: 0x0000DF0E
	private void FrontLeftFootstep()
	{
		this.Footstep(this.frontLeftFoot.position);
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x0000FD21 File Offset: 0x0000DF21
	private void FrontRightFootstep()
	{
		this.Footstep(this.frontRightFoot.position);
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x0000FD34 File Offset: 0x0000DF34
	private void BackLeftFootstep()
	{
		this.Footstep(this.backLeftFoot.position);
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x0000FD47 File Offset: 0x0000DF47
	private void BackRightFootstep()
	{
		this.Footstep(this.backRightFoot.position);
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x000788E0 File Offset: 0x00076AE0
	private void Footstep(Vector3 vFootPos)
	{
		BaseFootstepEffect.GroundInfo groundInfo = base.GetGroundInfo(vFootPos, base.transform.forward, false);
		GameObject gameObject = groundInfo.SpawnEffect(this.impactEffectDirectory);
		if (gameObject != null)
		{
			this.SetupFootstep(gameObject);
		}
		groundInfo.SpawnDecal(this.impactEffectDirectory);
		groundInfo.SpawnDisplacement("npc");
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x0007893C File Offset: 0x00076B3C
	private void SetupFootstep(GameObject effect)
	{
		FootstepSound.Hardness hardness = FootstepSound.Hardness.Medium;
		effect.GetComponent<FootstepSound>().PlayFootstep(hardness, false, base.gameObject, false);
	}
}
