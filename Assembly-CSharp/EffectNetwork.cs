using System;
using ConVar;
using Network;

// Token: 0x02000277 RID: 631
public static class EffectNetwork
{
	// Token: 0x04000EBE RID: 3774
	private static Effect effect = new Effect();

	// Token: 0x06001232 RID: 4658 RVA: 0x0007788C File Offset: 0x00075A8C
	public static void OnReceivedEffect(Message msg)
	{
		EffectNetwork.effect.Clear();
		EffectData.Deserialize(msg.read, EffectNetwork.effect, false);
		if (!EffectNetwork.effect.NetworkConstruct())
		{
			return;
		}
		if (LocalPlayer.Entity && LocalPlayer.Entity.userID == EffectNetwork.effect.source && ConVar.Client.prediction)
		{
			return;
		}
		EffectLibrary.Run(EffectNetwork.effect);
		EffectNetwork.effect.Clear();
	}
}
