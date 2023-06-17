using System;
using Network;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class InstrumentTool : HeldEntity
{
	// Token: 0x040005EE RID: 1518
	public GameObjectRef[] soundEffect = new GameObjectRef[2];

	// Token: 0x0600092F RID: 2351 RVA: 0x0004F710 File Offset: 0x0004D910
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("InstrumentTool.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0004F754 File Offset: 0x0004D954
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
		{
			float y = ownerPlayer.eyes.BodyForward().y;
			this.PlayNote(0, y);
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Attack, "");
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY))
		{
			float y2 = ownerPlayer.eyes.BodyForward().y;
			this.PlayNote(1, y2);
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Alt_Attack, "");
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x0000958A File Offset: 0x0000778A
	private void PlayNote(byte i, float y)
	{
		base.ServerRPC<byte, float>("SVPlayNote", i, y);
		EffectLibrary.Run(new Effect(this.soundEffect[(int)i].resourcePath, this, 0U, Vector3.zero, Vector3.forward, null)
		{
			scale = y
		});
	}
}
