using System;
using Network;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class PagerEntity : BaseEntity, IRFObject
{
	// Token: 0x0400062A RID: 1578
	public static BaseEntity.Flags Flag_Silent = BaseEntity.Flags.Reserved1;

	// Token: 0x0400062B RID: 1579
	private int frequency = 55;

	// Token: 0x0400062C RID: 1580
	public float beepRepeat = 2f;

	// Token: 0x0400062D RID: 1581
	public GameObjectRef pagerEffect;

	// Token: 0x0400062E RID: 1582
	public GameObjectRef silentEffect;

	// Token: 0x0400062F RID: 1583
	private bool wasOn;

	// Token: 0x06000983 RID: 2435 RVA: 0x00050EEC File Offset: 0x0004F0EC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PagerEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x000098AA File Offset: 0x00007AAA
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x000098B2 File Offset: 0x00007AB2
	public void OnParentDestroying()
	{
		if (base.isServer)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x000098C8 File Offset: 0x00007AC8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00050F30 File Offset: 0x0004F130
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (this.wasOn != base.IsOn())
		{
			if (base.IsOn())
			{
				this.BeginAlarm();
			}
			else
			{
				this.EndAlarm();
			}
			LocalPlayer.OnInventoryChanged();
		}
		this.wasOn = base.IsOn();
		LocalPlayer.OnItemAmountChanged();
		if (SelectedItem.item != null && SelectedItem.item.info.GetComponent<ItemModRFListener>() != null)
		{
			SelectedItem.UpdateItem();
		}
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x000098F4 File Offset: 0x00007AF4
	public void Beep()
	{
		Effect.client.Run(base.HasFlag(PagerEntity.Flag_Silent) ? this.silentEffect.resourcePath : this.pagerEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero);
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x0000992C File Offset: 0x00007B2C
	public void BeginAlarm()
	{
		if (base.IsInvoking(new Action(this.Beep)))
		{
			return;
		}
		base.InvokeRepeating(new Action(this.Beep), 0f, this.beepRepeat);
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00009960 File Offset: 0x00007B60
	public void EndAlarm()
	{
		base.CancelInvoke(new Action(this.Beep));
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x00006C27 File Offset: 0x00004E27
	public void ClientSetFrequency(int newFreq)
	{
		base.ServerRPC<int>("ServerSetFrequency", newFreq);
	}
}
