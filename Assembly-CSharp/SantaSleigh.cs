using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class SantaSleigh : BaseEntity
{
	// Token: 0x04000663 RID: 1635
	public GameObjectRef prefabDrop;

	// Token: 0x04000664 RID: 1636
	public SpawnFilter filter;

	// Token: 0x04000665 RID: 1637
	public Transform dropOrigin;

	// Token: 0x04000666 RID: 1638
	[ServerVar]
	public static float altitudeAboveTerrain = 50f;

	// Token: 0x04000667 RID: 1639
	[ServerVar]
	public static float desiredAltitude = 60f;

	// Token: 0x04000668 RID: 1640
	public Light bigLight;

	// Token: 0x04000669 RID: 1641
	public SoundPlayer hohoho;

	// Token: 0x0400066A RID: 1642
	public float hohohospacing = 4f;

	// Token: 0x0400066B RID: 1643
	public float hohoho_additional_spacing = 2f;

	// Token: 0x060009E8 RID: 2536 RVA: 0x00052F20 File Offset: 0x00051120
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SantaSleigh.OnRpcMessage", 0.1f))
		{
			if (rpc == 1273609064U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: ClientPlayHoHoHo ");
				}
				using (TimeWarning.New("ClientPlayHoHoHo", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							this.ClientPlayHoHoHo();
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in ClientPlayHoHoHo", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool PhysicsDriven()
	{
		return true;
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00009D45 File Offset: 0x00007F45
	[BaseEntity.RPC_Client]
	public void ClientPlayHoHoHo()
	{
		this.hohoho.PlayOneshot();
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00053004 File Offset: 0x00051204
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		Vector3 position = base.transform.position;
		position.y = height + this.bigLight.range * 0.5f;
		this.bigLight.transform.position = position;
	}
}
