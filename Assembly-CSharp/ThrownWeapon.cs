using System;
using Network;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class ThrownWeapon : AttackEntity
{
	// Token: 0x04000679 RID: 1657
	[Header("Throw Weapon")]
	public GameObjectRef prefabToThrow;

	// Token: 0x0400067A RID: 1658
	public float maxThrowVelocity = 10f;

	// Token: 0x0400067B RID: 1659
	public float tumbleVelocity;

	// Token: 0x0400067C RID: 1660
	public Vector3 overrideAngle = Vector3.zero;

	// Token: 0x06000A08 RID: 2568 RVA: 0x00053654 File Offset: 0x00051854
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ThrownWeapon.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x00053698 File Offset: 0x00051898
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (!this.IsFullyDeployed())
		{
			return;
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY) && !base.HasAttackCooldown())
		{
			Vector3 arg = ownerPlayer.eyes.position;
			RaycastHit raycastHit;
			if (!Physics.Raycast(ownerPlayer.eyes.position, ownerPlayer.eyes.BodyForward(), ref raycastHit, 0.25f, 1269916433))
			{
				arg = ownerPlayer.eyes.position + ownerPlayer.eyes.BodyForward() * 0.25f;
			}
			base.ServerRPC<Vector3, Vector3, float>("DoThrow", arg, ownerPlayer.eyes.BodyForward(), 1f);
			this.viewModel.Play("throw");
			base.StartAttackCooldown(this.repeatDelay);
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Throw, "");
		}
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00009EC3 File Offset: 0x000080C3
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
	}
}
