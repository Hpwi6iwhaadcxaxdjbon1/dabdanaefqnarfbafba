using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class CargoShip : BaseEntity
{
	// Token: 0x0400002B RID: 43
	public int targetNodeIndex = -1;

	// Token: 0x0400002C RID: 44
	public GameObject wakeParent;

	// Token: 0x0400002D RID: 45
	public GameObjectRef scientistTurretPrefab;

	// Token: 0x0400002E RID: 46
	public Transform[] scientistSpawnPoints;

	// Token: 0x0400002F RID: 47
	public List<Transform> crateSpawns;

	// Token: 0x04000030 RID: 48
	public GameObjectRef lockedCratePrefab;

	// Token: 0x04000031 RID: 49
	public GameObjectRef militaryCratePrefab;

	// Token: 0x04000032 RID: 50
	public GameObjectRef eliteCratePrefab;

	// Token: 0x04000033 RID: 51
	public GameObjectRef junkCratePrefab;

	// Token: 0x04000034 RID: 52
	public Transform waterLine;

	// Token: 0x04000035 RID: 53
	public Transform rudder;

	// Token: 0x04000036 RID: 54
	public Transform propeller;

	// Token: 0x04000037 RID: 55
	public GameObjectRef escapeBoatPrefab;

	// Token: 0x04000038 RID: 56
	public Transform escapeBoatPoint;

	// Token: 0x04000039 RID: 57
	public GameObject radiation;

	// Token: 0x0400003A RID: 58
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x0400003B RID: 59
	public GameObject hornOrigin;

	// Token: 0x0400003C RID: 60
	public SoundDefinition hornDef;

	// Token: 0x0400003D RID: 61
	public CargoShipSounds cargoShipSounds;

	// Token: 0x0400003E RID: 62
	public GameObject[] layouts;

	// Token: 0x0600003E RID: 62 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool PhysicsDriven()
	{
		return true;
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00002F0E File Offset: 0x0000110E
	public void UpdateLayoutFromFlags()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			this.layouts[0].SetActive(true);
			return;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			this.layouts[1].SetActive(true);
		}
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002F47 File Offset: 0x00001147
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		this.propeller.Rotate(Vector3.right, 180f * UnityEngine.Time.deltaTime, Space.Self);
		this.cargoShipSounds.UpdateSounds();
	}

	// Token: 0x06000041 RID: 65 RVA: 0x000277C8 File Offset: 0x000259C8
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.UpdateLayoutFromFlags();
		float height = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		Vector3 position = this.wakeParent.transform.position;
		this.wakeParent.transform.position = new Vector3(position.x, height, position.z);
		this.cargoShipSounds.InitSounds();
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00027838 File Offset: 0x00025A38
	[BaseEntity.RPC_Client]
	public void DoHornSound(BaseEntity.RPCMessage msg)
	{
		SoundManager.PlayOneshot(this.hornDef, this.hornOrigin, false, default(Vector3));
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00027864 File Offset: 0x00025A64
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CargoShip.OnRpcMessage", 0.1f))
		{
			if (rpc == 1778023151U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: DoHornSound ");
				}
				using (TimeWarning.New("DoHornSound", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoHornSound(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in DoHornSound", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}
