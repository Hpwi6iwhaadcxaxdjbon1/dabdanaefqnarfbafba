using System;
using Network;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class Deployer : HeldEntity
{
	// Token: 0x0400056F RID: 1391
	private string placementError;

	// Token: 0x06000897 RID: 2199 RVA: 0x0004D294 File Offset: 0x0004B494
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Deployer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0004D2D8 File Offset: 0x0004B4D8
	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		if (ownerItemDefinition == null)
		{
			return null;
		}
		return ownerItemDefinition.GetComponent<ItemModDeployable>();
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0004D300 File Offset: 0x0004B500
	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (modDeployable == null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x00008FA1 File Offset: 0x000071A1
	public override void OnDeploy()
	{
		base.OnDeploy();
		DeployGuide.Start(this.GetDeployable());
		this.UpdateGuide();
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0004D328 File Offset: 0x0004B528
	private void UpdateGuide()
	{
		if (DeployGuide.Update())
		{
			return;
		}
		Deployable deployable = this.GetDeployable();
		if (deployable == null)
		{
			return;
		}
		if (deployable.toSlot)
		{
			this.UpdateGuide_Slot(base.GetOwnerPlayer(), deployable);
			return;
		}
		this.UpdateGuide_Regular(base.GetOwnerPlayer(), deployable);
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0004D374 File Offset: 0x0004B574
	private void UpdateGuide_Slot(BasePlayer player, Deployable deployable)
	{
		BaseEntity lookingAtEntity = player.lookingAtEntity;
		if (!lookingAtEntity.IsValid() || !lookingAtEntity.HasSlot(deployable.slot) || lookingAtEntity.GetSlot(deployable.slot) != null)
		{
			DeployGuide.SetValid(false);
			return;
		}
		Transform slotAnchor = lookingAtEntity.GetSlotAnchor(deployable.slot);
		DeployGuide.Place(slotAnchor.position, slotAnchor.rotation);
		if (!player.CanBuild())
		{
			DeployGuide.SetValid(false);
			return;
		}
		DeployGuide.SetValid(true);
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0004D3EC File Offset: 0x0004B5EC
	private void UpdateGuide_Regular(BasePlayer player, Deployable deployable)
	{
		Ray ray = player.eyes.BodyRay();
		DeployGuide.Place(ray.origin + ray.direction * 8f, default(Quaternion));
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(ray, 0f, out raycastHit, 8f, 1235288065, 0))
		{
			DeployGuide.SetValid(false);
			return;
		}
		Quaternion rot = Quaternion.LookRotation(raycastHit.normal, ray.direction) * Quaternion.Euler(90f, 0f, 0f);
		DeployGuide.Place(raycastHit.point, rot);
		if (!this.CheckPlacement(deployable, ray, 8f))
		{
			DeployGuide.SetValid(false);
			return;
		}
		if (!player.CanBuild())
		{
			DeployGuide.SetValid(false);
			return;
		}
		DeployGuide.SetValid(true);
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x00008FBA File Offset: 0x000071BA
	public override void OnHolstered()
	{
		base.OnHolstered();
		DeployGuide.End();
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x00008FC7 File Offset: 0x000071C7
	public override void OnFrame()
	{
		base.OnFrame();
		this.UpdateGuide();
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0004D4B8 File Offset: 0x0004B6B8
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		this.UpdateGuide();
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
		{
			Ray ray = ownerPlayer.eyes.BodyRay();
			if (!ownerPlayer.CanBuild())
			{
				return;
			}
			Deployable deployable = this.GetDeployable();
			if (deployable == null)
			{
				return;
			}
			if (deployable.toSlot)
			{
				BaseEntity lookingAtEntity = ownerPlayer.lookingAtEntity;
				if (!lookingAtEntity.IsValid())
				{
					return;
				}
				base.ServerRPC<Ray, uint>("DoDeploy", ray, lookingAtEntity.net.ID);
				DeployGuide.HideFor(0.5f);
				return;
			}
			else
			{
				if (!this.CheckPlacement(deployable, ray, 8f))
				{
					Debug.Log("Player couldn't place item because of: " + this.placementError);
					return;
				}
				base.ServerRPC<Ray, int>("DoDeploy", ray, 0);
				DeployGuide.HideFor(0.5f);
			}
		}
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00008FD5 File Offset: 0x000071D5
	public Quaternion GetDeployedRotation(Vector3 normal, Vector3 placeDir)
	{
		return Quaternion.LookRotation(normal, placeDir) * Quaternion.Euler(90f, 0f, 0f);
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0004D598 File Offset: 0x0004B798
	public bool IsPlacementAngleAcceptable(Vector3 pos, Quaternion rot)
	{
		Vector3 lhs = rot * Vector3.up;
		return Mathf.Acos(Vector3.Dot(lhs, Vector3.up)) <= 0.61086524f;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0004D5CC File Offset: 0x0004B7CC
	public bool CheckPlacement(Deployable deployable, Ray ray, float fDistance)
	{
		using (TimeWarning.New("Deploy.CheckPlacement", 0.1f))
		{
			RaycastHit raycastHit;
			if (!Physics.Raycast(ray, ref raycastHit, fDistance, 1235288065))
			{
				this.placementError = "Nothing to place on";
				return false;
			}
			DeployVolume[] volumes = PrefabAttribute.client.FindAll<DeployVolume>(deployable.prefabID);
			Vector3 point = raycastHit.point;
			Quaternion deployedRotation = this.GetDeployedRotation(raycastHit.normal, ray.direction);
			if (DeployVolume.Check(point, deployedRotation, volumes, -1))
			{
				this.placementError = "Not enough space";
				return false;
			}
			if (!this.IsPlacementAngleAcceptable(raycastHit.point, deployedRotation))
			{
				this.placementError = "Angle too steep";
				return false;
			}
		}
		return true;
	}
}
