using System;
using Network;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class BaseLauncher : BaseProjectile
{
	// Token: 0x06000719 RID: 1817 RVA: 0x00045EE4 File Offset: 0x000440E4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLauncher.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00045F28 File Offset: 0x00044128
	public override void LaunchProjectile()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		BaseEntity baseEntity = ownerPlayer.GetParentEntity();
		if (baseEntity == null)
		{
			baseEntity = ownerPlayer.GetMounted();
		}
		Vector3 vector = ownerPlayer.eyes.position;
		Vector3 vector2 = ownerPlayer.eyes.BodyForward();
		if (baseEntity != null)
		{
			vector = baseEntity.transform.InverseTransformPoint(vector);
			vector2 = baseEntity.transform.InverseTransformDirection(vector2);
		}
		bool arg = baseEntity != null;
		base.ServerRPC<Vector3, Vector3, bool>("SV_Launch", vector, vector2, arg);
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00045FB0 File Offset: 0x000441B0
	public override void OnFrame()
	{
		base.OnFrame();
		if (this.viewModel && this.viewModel.instance)
		{
			SwapRPG component = this.viewModel.instance.GetComponent<SwapRPG>();
			if (component)
			{
				component.UpdateAmmoType(this.primaryMagazine.ammoType);
			}
		}
	}
}
