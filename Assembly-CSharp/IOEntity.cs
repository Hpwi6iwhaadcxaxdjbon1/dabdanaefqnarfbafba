using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200009F RID: 159
public class IOEntity : BaseCombatEntity
{
	// Token: 0x040005EF RID: 1519
	[Header("IOEntity")]
	public Transform debugOrigin;

	// Token: 0x040005F0 RID: 1520
	public ItemDefinition sourceItem;

	// Token: 0x040005F1 RID: 1521
	[Help("How many miliseconds to budget for processing io entities per server frame")]
	[ServerVar]
	public static float framebudgetms = 1f;

	// Token: 0x040005F2 RID: 1522
	[ServerVar]
	public static float responsetime = 0.1f;

	// Token: 0x040005F3 RID: 1523
	[ServerVar]
	public static int backtracking = 8;

	// Token: 0x040005F4 RID: 1524
	public const BaseEntity.Flags Flag_ShortCircuit = BaseEntity.Flags.Reserved7;

	// Token: 0x040005F5 RID: 1525
	public const BaseEntity.Flags Flag_HasPower = BaseEntity.Flags.Reserved8;

	// Token: 0x040005F6 RID: 1526
	public IOEntity.IOSlot[] inputs;

	// Token: 0x040005F7 RID: 1527
	public IOEntity.IOSlot[] outputs;

	// Token: 0x040005F8 RID: 1528
	public IOEntity.IOType ioType;

	// Token: 0x040005F9 RID: 1529
	[NonSerialized]
	public int client_powerin;

	// Token: 0x040005FA RID: 1530
	[NonSerialized]
	public int client_powerout;

	// Token: 0x040005FB RID: 1531
	[NonSerialized]
	public bool client_extradata_dirty = true;

	// Token: 0x040005FC RID: 1532
	private Action requestAdditionalDataAction;

	// Token: 0x06000933 RID: 2355 RVA: 0x0004F7F0 File Offset: 0x0004D9F0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IOEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 4227434274U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: Client_ReceiveAdditionalData ");
				}
				using (TimeWarning.New("Client_ReceiveAdditionalData", 0.1f))
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
							this.Client_ReceiveAdditionalData(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in Client_ReceiveAdditionalData", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x000095D8 File Offset: 0x000077D8
	public override void ResetState()
	{
		this.client_powerin = 0;
		this.client_powerout = 0;
		this.client_extradata_dirty = true;
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x000095EF File Offset: 0x000077EF
	public string GetDisplayName()
	{
		if (this.sourceItem != null)
		{
			return this.sourceItem.displayName.translated;
		}
		return base.ShortPrefabName;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool IsRootEntity()
	{
		return false;
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool WantsPower()
	{
		return true;
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00005D70 File Offset: 0x00003F70
	public bool IsPowered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x0004F90C File Offset: 0x0004DB0C
	public bool IsConnectedTo(IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < this.inputs.Length)
		{
			IOEntity.IOSlot ioslot = this.inputs[slot];
			if (ioslot.mainPowerSlot)
			{
				IOEntity ioentity = ioslot.connectedTo.Get(true);
				if (ioentity != null)
				{
					if (ioentity == entity)
					{
						return true;
					}
					if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x0004F96C File Offset: 0x0004DB6C
	public bool IsConnectedTo(IOEntity entity, int depth, bool defaultReturn = false)
	{
		if (depth > 0)
		{
			for (int i = 0; i < this.inputs.Length; i++)
			{
				IOEntity.IOSlot ioslot = this.inputs[i];
				if (ioslot.mainPowerSlot)
				{
					IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						if (ioentity == entity)
						{
							return true;
						}
						if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return defaultReturn;
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00009616 File Offset: 0x00007816
	public void RequestAdditionalData()
	{
		if (LocalPlayer.Entity.lookingAtEntity != this)
		{
			return;
		}
		base.ServerRPC("Server_RequestData");
		this.client_extradata_dirty = true;
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x0004F9D8 File Offset: 0x0004DBD8
	[BaseEntity.RPC_Client]
	private void Client_ReceiveAdditionalData(BaseEntity.RPCMessage msg)
	{
		int first = msg.read.Int32();
		int second = msg.read.Int32();
		float third = msg.read.Float();
		float fourth = msg.read.Float();
		this.client_extradata_dirty = false;
		this.ProcessAdditionalData(first, second, third, fourth);
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0000963D File Offset: 0x0000783D
	public virtual void ProcessAdditionalData(int first, int second, float third, float fourth)
	{
		this.client_powerin = first;
		this.client_powerout = second;
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x0004FA28 File Offset: 0x0004DC28
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		if (!info.fromDisk && info.msg.ioEntity.inputs != null)
		{
			int count = info.msg.ioEntity.inputs.Count;
			if (this.inputs.Length != count)
			{
				this.inputs = new IOEntity.IOSlot[count];
			}
			for (int i = 0; i < count; i++)
			{
				if (this.inputs[i] == null)
				{
					this.inputs[i] = new IOEntity.IOSlot();
				}
				IOEntity.IOConnection ioconnection = info.msg.ioEntity.inputs[i];
				this.inputs[i].connectedTo = new IOEntity.IORef();
				this.inputs[i].connectedTo.entityRef.uid = ioconnection.connectedID;
				if (base.isClient)
				{
					this.inputs[i].connectedTo.InitClient();
				}
				this.inputs[i].connectedToSlot = ioconnection.connectedToSlot;
				this.inputs[i].niceName = ioconnection.niceName;
				this.inputs[i].type = (IOEntity.IOType)ioconnection.type;
			}
		}
		if (info.msg.ioEntity.outputs != null)
		{
			if (!info.fromDisk && base.isClient)
			{
				IOEntity.IOSlot[] array = this.outputs;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Clear();
				}
			}
			int count2 = info.msg.ioEntity.outputs.Count;
			if (this.outputs.Length != count2 && count2 > 0)
			{
				IOEntity.IOSlot[] array2 = this.outputs;
				this.outputs = new IOEntity.IOSlot[count2];
				for (int k = 0; k < array2.Length; k++)
				{
					if (k < count2)
					{
						this.outputs[k] = array2[k];
					}
				}
			}
			for (int l = 0; l < count2; l++)
			{
				if (this.outputs[l] == null)
				{
					this.outputs[l] = new IOEntity.IOSlot();
				}
				IOEntity.IOConnection ioconnection2 = info.msg.ioEntity.outputs[l];
				this.outputs[l].connectedTo = new IOEntity.IORef();
				this.outputs[l].connectedTo.entityRef.uid = ioconnection2.connectedID;
				if (base.isClient)
				{
					this.outputs[l].connectedTo.InitClient();
				}
				this.outputs[l].connectedToSlot = ioconnection2.connectedToSlot;
				this.outputs[l].niceName = ioconnection2.niceName;
				this.outputs[l].type = (IOEntity.IOType)ioconnection2.type;
				if (info.fromDisk || base.isClient)
				{
					List<IOEntity.IOConnection.LineVec> linePointList = ioconnection2.linePointList;
					if (this.outputs[l].linePoints == null || this.outputs[l].linePoints.Length != linePointList.Count)
					{
						this.outputs[l].linePoints = new Vector3[linePointList.Count];
					}
					for (int m = 0; m < linePointList.Count; m++)
					{
						this.outputs[l].linePoints[m] = linePointList[m].vec;
					}
					if (base.isClient)
					{
						this.outputs[l].UpdateLines();
					}
				}
			}
		}
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x0004FD88 File Offset: 0x0004DF88
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (LocalPlayer.Entity != null && LocalPlayer.Entity.lookingAtEntity == this)
		{
			if (this.requestAdditionalDataAction == null)
			{
				this.requestAdditionalDataAction = new Action(this.RequestAdditionalData);
			}
			if (!base.IsInvoking(this.requestAdditionalDataAction))
			{
				base.Invoke(this.requestAdditionalDataAction, 0.5f);
			}
		}
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0000964D File Offset: 0x0000784D
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0004FDF4 File Offset: 0x0004DFF4
	protected override void DoClientDestroy()
	{
		IOEntity.IOSlot[] array = this.outputs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CleanupLines();
		}
		base.DoClientDestroy();
	}

	// Token: 0x020000A0 RID: 160
	public enum IOType
	{
		// Token: 0x040005FE RID: 1534
		Electric,
		// Token: 0x040005FF RID: 1535
		Fluidic,
		// Token: 0x04000600 RID: 1536
		Kinetic,
		// Token: 0x04000601 RID: 1537
		Generic
	}

	// Token: 0x020000A1 RID: 161
	[Serializable]
	public class IORef
	{
		// Token: 0x04000602 RID: 1538
		public EntityRef entityRef;

		// Token: 0x04000603 RID: 1539
		public IOEntity ioEnt;

		// Token: 0x06000944 RID: 2372 RVA: 0x0004FE24 File Offset: 0x0004E024
		public void Init()
		{
			if (this.ioEnt != null && !this.entityRef.IsValid(true))
			{
				this.entityRef.Set(this.ioEnt);
			}
			if (this.entityRef.IsValid(true))
			{
				this.ioEnt = this.entityRef.Get(true).GetComponent<IOEntity>();
			}
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x00009681 File Offset: 0x00007881
		public void InitClient()
		{
			if (this.entityRef.IsValid(false) && this.ioEnt == null)
			{
				this.ioEnt = this.entityRef.Get(false).GetComponent<IOEntity>();
			}
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x000096B6 File Offset: 0x000078B6
		public IOEntity Get(bool isServer = true)
		{
			if (this.ioEnt == null && this.entityRef.IsValid(isServer))
			{
				this.ioEnt = this.entityRef.Get(isServer).GetComponent<IOEntity>();
			}
			return this.ioEnt;
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x000096F1 File Offset: 0x000078F1
		public void Clear()
		{
			this.ioEnt = null;
			this.entityRef.Set(null);
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x00009706 File Offset: 0x00007906
		public void Set(IOEntity newIOEnt)
		{
			this.entityRef.Set(newIOEnt);
		}
	}

	// Token: 0x020000A2 RID: 162
	[Serializable]
	public class IOSlot
	{
		// Token: 0x04000604 RID: 1540
		public string niceName;

		// Token: 0x04000605 RID: 1541
		public IOEntity.IOType type;

		// Token: 0x04000606 RID: 1542
		public IOEntity.IORef connectedTo;

		// Token: 0x04000607 RID: 1543
		public int connectedToSlot;

		// Token: 0x04000608 RID: 1544
		public Vector3[] linePoints;

		// Token: 0x04000609 RID: 1545
		public ClientIOLine line;

		// Token: 0x0400060A RID: 1546
		public Vector3 handlePosition;

		// Token: 0x0400060B RID: 1547
		public bool rootConnectionsOnly;

		// Token: 0x0400060C RID: 1548
		public bool mainPowerSlot;

		// Token: 0x0600094A RID: 2378 RVA: 0x00009714 File Offset: 0x00007914
		public void OnDestroy()
		{
			this.CleanupLines();
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x0000971C File Offset: 0x0000791C
		public void Clear()
		{
			this.connectedTo.Clear();
			this.connectedToSlot = 0;
			if (this.line != null)
			{
				this.line.SetVisible(false);
			}
			this.linePoints = null;
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x00009751 File Offset: 0x00007951
		public void CleanupLines()
		{
			if (this.line != null)
			{
				this.line.Clear();
				GameManager.client.Retire(this.line.gameObject);
				this.line = null;
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0004FE84 File Offset: 0x0004E084
		public void UpdateLines()
		{
			if (this.linePoints.Length != 0)
			{
				if (this.line == null)
				{
					this.line = WireTool.CreateLine(this.linePoints[0]);
				}
				if (this.line)
				{
					this.line.SetPositions(this.linePoints);
					this.line.SetVisible(true);
					return;
				}
			}
			else if (this.line != null)
			{
				this.line.SetVisible(false);
			}
		}
	}
}
