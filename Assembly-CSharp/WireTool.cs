using System;
using System.Collections.Generic;
using Facepunch;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class WireTool : HeldEntity
{
	// Token: 0x0400069C RID: 1692
	public static float maxWireLength = 30f;

	// Token: 0x0400069D RID: 1693
	private const int maxLineNodes = 16;

	// Token: 0x0400069E RID: 1694
	public GameObjectRef plugEffect;

	// Token: 0x0400069F RID: 1695
	public GameObjectRef ioLine;

	// Token: 0x040006A0 RID: 1696
	public WireTool.PendingPlug_t pending;

	// Token: 0x040006A1 RID: 1697
	private const float lineThickness = 0.02f;

	// Token: 0x040006A2 RID: 1698
	private bool wantsCrosshair;

	// Token: 0x040006A3 RID: 1699
	private ClientIOLine pendingLine;

	// Token: 0x040006A4 RID: 1700
	private float nextClearSendTime;

	// Token: 0x040006A5 RID: 1701
	private float remainingDist;

	// Token: 0x040006A6 RID: 1702
	private bool validSurface;

	// Token: 0x040006A7 RID: 1703
	private float clearProgress;

	// Token: 0x040006A8 RID: 1704
	public Sprite InputSprite;

	// Token: 0x040006A9 RID: 1705
	public Sprite OutputSprite;

	// Token: 0x040006AA RID: 1706
	public Sprite ClearSprite;

	// Token: 0x06000A1F RID: 2591 RVA: 0x0005405C File Offset: 0x0005225C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WireTool.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0000A01A File Offset: 0x0000821A
	public void ClearPendingPlug()
	{
		this.pending.ent = null;
		this.pending.index = -1;
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0000A034 File Offset: 0x00008234
	public bool HasPendingPlug()
	{
		return this.pending.ent != null && this.pending.index != -1;
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0000A05C File Offset: 0x0000825C
	public bool PendingPlugIsInput()
	{
		return this.pending.ent != null && this.pending.index != -1 && this.pending.input;
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0000A08C File Offset: 0x0000828C
	public bool PendingPlugIsOutput()
	{
		return this.pending.ent != null && this.pending.index != -1 && !this.pending.input;
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0000A0BF File Offset: 0x000082BF
	public static bool CanPlayerUseWires(BasePlayer player)
	{
		return player.CanBuild() && !GamePhysics.CheckSphere(player.eyes.position, 0.1f, 536870912, 2);
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x0000A0E9 File Offset: 0x000082E9
	public bool PendingPlugRoot()
	{
		return this.pending.ent != null && this.pending.ent.IsRootEntity();
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0000A110 File Offset: 0x00008310
	public bool HasPendingWire()
	{
		return this.pendingLine != null;
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0000A11E File Offset: 0x0000831E
	public float GetPendingLengthRemaining()
	{
		if (this.pendingLine == null)
		{
			return WireTool.maxWireLength;
		}
		return this.remainingDist;
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x000540A0 File Offset: 0x000522A0
	public int GetWireClicksRemaining()
	{
		if (this.pendingLine == null)
		{
			return 0;
		}
		int num = this.pendingLine._line.positionCount;
		num--;
		if (num < 0)
		{
			num = 0;
		}
		if (num > 16)
		{
			num = 16;
		}
		return 16 - num;
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0000A13A File Offset: 0x0000833A
	public bool ValidSurface()
	{
		return this.validSurface;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0000A142 File Offset: 0x00008342
	public void ClientRequestClear(IOEntity ent, int slot, bool input)
	{
		if (Time.time < this.nextClearSendTime)
		{
			return;
		}
		this.nextClearSendTime = Time.time + 0.2f;
		base.ServerRPC<uint, int, bool>("RequestClear", ent.net.ID, slot, input);
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x000540E4 File Offset: 0x000522E4
	public IOEntity GetLookingAtIOEnt()
	{
		if (LocalPlayer.Entity == null)
		{
			return null;
		}
		BaseEntity lookingAtEntity = LocalPlayer.Entity.lookingAtEntity;
		if (lookingAtEntity == null)
		{
			return null;
		}
		return lookingAtEntity.GetComponent<IOEntity>();
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0000A17B File Offset: 0x0000837B
	public override void OnHolster()
	{
		this.ClearPendingPlug();
		this.EndLine(true);
		this.clearProgress = 0f;
		PowerBar.Instance.SetProgress(0f);
		PowerBar.Instance.SetVisible(false);
		base.OnHolster();
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0000A1B5 File Offset: 0x000083B5
	public override void OnDeploy()
	{
		base.OnDeploy();
		this.ClearPendingPlug();
		this.clearProgress = 0f;
		this.EndLine(true);
		PowerBar.Instance.SetProgress(0f);
		PowerBar.Instance.SetVisible(false);
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0000A1EF File Offset: 0x000083EF
	public bool WantsInputList()
	{
		return !this.WantsOutputList();
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0000A1FA File Offset: 0x000083FA
	public bool WantsOutputList()
	{
		return !this.HasPendingPlug();
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0000A205 File Offset: 0x00008405
	public void ClearClicked()
	{
		if (this.GetLookingAtIOEnt())
		{
			base.ServerRPC<uint>("TryClear", this.GetLookingAtIOEnt().net.ID);
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0005411C File Offset: 0x0005231C
	public bool IsSlotOccupied(IOEntity ent, bool input, int index)
	{
		if (input && index < ent.inputs.Length)
		{
			return ent.inputs[index].connectedTo.Get(true) != null;
		}
		return input || index >= ent.outputs.Length || ent.outputs[index].connectedTo.Get(true) != null;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0005417C File Offset: 0x0005237C
	public void ClientAttemptConnection(bool input, int index)
	{
		IOEntity lookingAtIOEnt = this.GetLookingAtIOEnt();
		if (!WireTool.CanPlayerUseWires(LocalPlayer.Entity))
		{
			return;
		}
		if (lookingAtIOEnt == null)
		{
			return;
		}
		if (this.HasPendingPlug() && input == this.pending.input)
		{
			return;
		}
		if (this.GetPendingLengthRemaining() <= 0f)
		{
			return;
		}
		if (this.IsSlotOccupied(lookingAtIOEnt, input, index))
		{
			return;
		}
		this.viewModel.Trigger("attack");
		Vector3 vector = lookingAtIOEnt.debugOrigin.transform.position;
		if (!this.HasPendingPlug())
		{
			this.pending.ent = lookingAtIOEnt;
			this.pending.index = index;
			this.pending.input = input;
			Vector3 vector2 = input ? this.pending.ent.inputs[index].handlePosition : this.pending.ent.outputs[index].handlePosition;
			vector2 = this.pending.ent.transform.TransformPoint(vector2);
			vector = vector2;
			this.BeginLine(vector2);
		}
		else
		{
			int num = input ? index : this.pending.index;
			int num2 = (!input) ? index : this.pending.index;
			uint num3 = input ? lookingAtIOEnt.net.ID : this.pending.ent.net.ID;
			uint num4 = (!input) ? lookingAtIOEnt.net.ID : this.pending.ent.net.ID;
			Vector3 vector3 = input ? lookingAtIOEnt.inputs[index].handlePosition : lookingAtIOEnt.outputs[index].handlePosition;
			vector3 = lookingAtIOEnt.transform.TransformPoint(vector3);
			this.pendingLine.AddPosition(vector3);
			vector = vector3;
			this.SendPendingLine(num3, num, num4, num2);
			base.ServerRPC<uint, int, uint, int>("MakeConnection", num3, num, num4, num2);
			this.EndLine(true);
			this.ClearPendingPlug();
		}
		Vector3 normalized = (vector - lookingAtIOEnt.transform.position).normalized;
		Effect.client.Run(this.plugEffect.resourcePath, vector, normalized, default(Vector3));
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00054398 File Offset: 0x00052598
	public void BeginLine(Vector3 startPos)
	{
		this.EndLine(true);
		this.pendingLine = base.gameManager.CreatePrefab("assets/prefabs/tools/wire/clientioline.prefab", this.pending.ent.debugOrigin.transform.position, Quaternion.identity, true).GetComponent<ClientIOLine>();
		this.pendingLine.AddPosition(startPos);
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x000543F4 File Offset: 0x000525F4
	public static ClientIOLine CreateLine(Vector3 spawnPos)
	{
		GameObject gameObject = GameManager.client.CreatePrefab("assets/prefabs/tools/wire/clientioline.prefab", spawnPos, Quaternion.identity, true);
		if (gameObject != null)
		{
			return gameObject.GetComponent<ClientIOLine>();
		}
		return null;
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0000A22F File Offset: 0x0000842F
	public void EndLine(bool destroy = false)
	{
		if (destroy && this.pendingLine != null)
		{
			Object.Destroy(this.pendingLine.gameObject);
		}
		this.pendingLine = null;
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0005442C File Offset: 0x0005262C
	public void SendPendingLine(uint inputID, int inputIndex, uint outputID, int outputIndex)
	{
		if (this.pendingLine != null)
		{
			List<Vector3> list = Pool.GetList<Vector3>();
			Vector3[] array = new Vector3[this.pendingLine._line.positionCount];
			this.pendingLine._line.GetPositions(array);
			foreach (Vector3 vector in array)
			{
				list.Add(vector);
			}
			if (!this.ValidateLine(list))
			{
				return;
			}
			base.ServerRPCList<Vector3, uint, int, uint, int>("AddLine", list, inputID, inputIndex, outputID, outputIndex);
			Pool.FreeList<Vector3>(ref list);
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0000A259 File Offset: 0x00008459
	public override bool NeedsCrosshair()
	{
		return this.wantsCrosshair;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x000544B8 File Offset: 0x000526B8
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer entity = LocalPlayer.Entity;
		IOEntity lookingAtIOEnt = this.GetLookingAtIOEnt();
		this.wantsCrosshair = (lookingAtIOEnt != null);
		if (this.pendingLine != null)
		{
			Vector3 vector = LocalPlayer.Entity.eyes.position;
			bool flag = false;
			RaycastHit raycastHit;
			if (Physics.Raycast(entity.eyes.HeadRay(), ref raycastHit, 3f, 1218519041))
			{
				vector = raycastHit.point + raycastHit.normal * 0.02f;
				flag = true;
			}
			else
			{
				vector = entity.eyes.position + entity.eyes.BodyForward() * 3f;
			}
			this.validSurface = flag;
			Vector3 lastPlacedNodePosition = this.pendingLine.GetLastPlacedNodePosition();
			float length = this.pendingLine.GetLength();
			float num = Vector3.Distance(lastPlacedNodePosition, vector);
			this.remainingDist = WireTool.maxWireLength - (length + num);
			if (this.remainingDist < 0f)
			{
				vector = lastPlacedNodePosition + (vector - lastPlacedNodePosition).normalized * (WireTool.maxWireLength - length);
				this.remainingDist = 0f;
				flag = false;
			}
			bool flag2 = true;
			Vector3 normalized = (vector - lastPlacedNodePosition).normalized;
			float d = Vector3.Distance(vector, lastPlacedNodePosition);
			vector = lastPlacedNodePosition + normalized * d;
			DDraw.Sphere(vector, 0.01f, (flag && flag2) ? Color.green : Color.red, Time.deltaTime, false);
			DDraw.Line(lastPlacedNodePosition, vector, (flag && flag2) ? Color.green : Color.red, Time.deltaTime, false, true);
			if (entity.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
			{
				if (LookAtIOEnt.GetSelectedIndex() != -1 && (!this.HasPendingPlug() || LookAtIOEnt.SelectedIsInput() != this.pending.input))
				{
					this.ClientAttemptConnection(LookAtIOEnt.SelectedIsInput(), LookAtIOEnt.GetSelectedIndex());
					return;
				}
				if (flag && flag2)
				{
					if (this.GetWireClicksRemaining() <= 0)
					{
						return;
					}
					this.pendingLine.AddPosition(vector);
					this.pendingLine.SetLastNodePosition(vector);
					Effect.client.Run(this.plugEffect.resourcePath, vector, -LocalPlayer.Entity.eyes.BodyForward(), default(Vector3));
				}
			}
		}
		if (entity.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY) && LookAtIOEnt.GetSelectedIndex() != -1 && (!this.HasPendingPlug() || LookAtIOEnt.SelectedIsInput() != this.pending.input))
		{
			this.ClientAttemptConnection(LookAtIOEnt.SelectedIsInput(), LookAtIOEnt.GetSelectedIndex());
			return;
		}
		if (this.viewModel)
		{
			this.viewModel.SetBool("validplug", lookingAtIOEnt);
			this.viewModel.SetBool("male", !LookAtIOEnt.SelectedIsInput());
		}
		bool flag3 = false;
		if (lookingAtIOEnt != null && LookAtIOEnt.GetSelectedIndex() != -1)
		{
			if (LookAtIOEnt.SelectedIsInput())
			{
				flag3 = (LookAtIOEnt.GetSelectedIndex() < lookingAtIOEnt.inputs.Length && lookingAtIOEnt.inputs[LookAtIOEnt.GetSelectedIndex()].connectedTo.Get(true) != null);
			}
			else
			{
				flag3 = (LookAtIOEnt.GetSelectedIndex() < lookingAtIOEnt.outputs.Length && lookingAtIOEnt.outputs[LookAtIOEnt.GetSelectedIndex()].connectedTo.Get(true) != null);
			}
		}
		if (lookingAtIOEnt && entity.input.state.IsDown(BUTTON.FIRE_SECONDARY) && flag3)
		{
			this.clearProgress += Time.deltaTime * 2f;
			this.clearProgress = Mathf.Clamp(this.clearProgress, 0f, 1f);
			if (this.clearProgress == 1f)
			{
				this.ClientRequestClear(lookingAtIOEnt, LookAtIOEnt.GetSelectedIndex(), LookAtIOEnt.SelectedIsInput());
				return;
			}
		}
		else
		{
			if (!lookingAtIOEnt && entity.input.state.IsDown(BUTTON.FIRE_SECONDARY) && !flag3 && this.HasPendingPlug() && this.HasPendingWire())
			{
				this.ClearPendingPlug();
				this.EndLine(true);
				return;
			}
			this.clearProgress = 0f;
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0000A261 File Offset: 0x00008461
	public float GetClearProgress()
	{
		return this.clearProgress;
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x000548CC File Offset: 0x00052ACC
	public bool ValidateLine(List<Vector3> lineList)
	{
		if (lineList.Count < 2)
		{
			return false;
		}
		Vector3 a = lineList[0];
		float num = 0f;
		for (int i = 1; i < lineList.Count; i++)
		{
			Vector3 vector = lineList[i];
			num += Vector3.Distance(a, vector);
			if (num > WireTool.maxWireLength)
			{
				return false;
			}
			a = vector;
		}
		return true;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00054924 File Offset: 0x00052B24
	private void AddIOOption(IOEntity.IOSlot slot, List<Option> opts, bool input, int index, int order = 0)
	{
		bool flag = false;
		if (input && slot.rootConnectionsOnly && this.HasPendingPlug() && this.pending.ent != null && !this.pending.ent.IsRootEntity())
		{
			flag = true;
		}
		opts.Add(new Option
		{
			title = slot.niceName,
			desc = (flag ? "Must be root entity" : slot.niceName),
			iconSprite = (input ? this.InputSprite : this.OutputSprite),
			order = order,
			show = true,
			showDisabled = (slot.connectedTo.entityRef.Get(false) != null || flag),
			function = delegate(BasePlayer ply)
			{
				this.ClientAttemptConnection(input, index);
			}
		});
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00054A24 File Offset: 0x00052C24
	public void AddClearConnections(List<Option> opts)
	{
		opts.Add(new Option
		{
			title = "Clear",
			desc = "Clear All connections",
			iconSprite = this.ClearSprite,
			order = Mathf.CeilToInt((float)opts.Count * 0.5f),
			show = true,
			showDisabled = false,
			function = delegate(BasePlayer ply)
			{
				this.ClearClicked();
			}
		});
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x00054AA4 File Offset: 0x00052CA4
	public List<Option> GetIOOptions(IOEntity source)
	{
		List<Option> list = new List<Option>();
		if (this.WantsOutputList())
		{
			for (int i = 0; i < source.outputs.Length; i++)
			{
				IOEntity.IOSlot slot = source.outputs[i];
				this.AddIOOption(slot, list, false, i, i);
			}
		}
		else if (this.WantsInputList())
		{
			for (int j = 0; j < source.inputs.Length; j++)
			{
				IOEntity.IOSlot slot2 = source.inputs[j];
				this.AddIOOption(slot2, list, true, j, j);
			}
		}
		this.AddClearConnections(list);
		return list;
	}

	// Token: 0x020000C0 RID: 192
	public struct PendingPlug_t
	{
		// Token: 0x040006AB RID: 1707
		public IOEntity ent;

		// Token: 0x040006AC RID: 1708
		public bool input;

		// Token: 0x040006AD RID: 1709
		public int index;

		// Token: 0x040006AE RID: 1710
		public GameObject tempLine;
	}
}
