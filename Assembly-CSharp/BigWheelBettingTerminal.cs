using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class BigWheelBettingTerminal : StorageContainer
{
	// Token: 0x040004AE RID: 1198
	public BigWheelGame bigWheel;

	// Token: 0x040004AF RID: 1199
	public Vector3 seatedPlayerOffset = Vector3.forward;

	// Token: 0x040004B0 RID: 1200
	public float offsetCheckRadius = 0.4f;

	// Token: 0x040004B1 RID: 1201
	public SoundDefinition winSound;

	// Token: 0x040004B2 RID: 1202
	public SoundDefinition loseSound;

	// Token: 0x040004B3 RID: 1203
	public float nextSpinTime;

	// Token: 0x060007CB RID: 1995 RVA: 0x00048DB8 File Offset: 0x00046FB8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0.1f))
		{
			if (rpc == 2394178227U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: SetTimeUntilNextSpin ");
				}
				using (TimeWarning.New("SetTimeUntilNextSpin", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage timeUntilNextSpin = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetTimeUntilNextSpin(timeUntilNextSpin);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in SetTimeUntilNextSpin", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 3184631126U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: WinOrLoseSound ");
				}
				using (TimeWarning.New("WinOrLoseSound", 0.1f))
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
							this.WinOrLoseSound(msg2);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in WinOrLoseSound", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00008576 File Offset: 0x00006776
	public new void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.seatedPlayerOffset), this.offsetCheckRadius);
		base.OnDrawGizmos();
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00049000 File Offset: 0x00047200
	public bool IsPlayerValid(BasePlayer player)
	{
		if (!player.isMounted)
		{
			return false;
		}
		Vector3 vector = base.transform.TransformPoint(this.seatedPlayerOffset);
		return Vector3Ex.Distance2D(player.transform.position, vector) <= this.offsetCheckRadius;
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x000085A4 File Offset: 0x000067A4
	public override bool ShouldShowLootMenus()
	{
		return this.IsPlayerValid(LocalPlayer.Entity) && base.ShouldShowLootMenus();
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x000085BB File Offset: 0x000067BB
	private Vector3 SoundPosition()
	{
		return base.transform.position + Vector3.up * 1.2f;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x000085DC File Offset: 0x000067DC
	public void WinSound()
	{
		SoundManager.PlayOneshot(this.winSound, base.gameObject, false, this.SoundPosition());
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x000085F7 File Offset: 0x000067F7
	public void LoseSound()
	{
		SoundManager.PlayOneshot(this.loseSound, base.gameObject, false, this.SoundPosition());
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x00008612 File Offset: 0x00006812
	[BaseEntity.RPC_Client]
	public void SetTimeUntilNextSpin(BaseEntity.RPCMessage msg)
	{
		this.nextSpinTime = UnityEngine.Time.realtimeSinceStartup + msg.read.Float();
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00049048 File Offset: 0x00047248
	[BaseEntity.RPC_Client]
	public void WinOrLoseSound(BaseEntity.RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			base.Invoke(new Action(this.WinSound), Random.Range(0f, 0.25f));
			return;
		}
		base.Invoke(new Action(this.LoseSound), Random.Range(0f, 0.25f));
	}
}
