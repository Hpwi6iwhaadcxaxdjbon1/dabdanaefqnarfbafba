using System;
using Network;

// Token: 0x020000A6 RID: 166
public class Locker : StorageContainer
{
	// Token: 0x04000615 RID: 1557
	public GameObjectRef equipSound;

	// Token: 0x06000965 RID: 2405 RVA: 0x000504B8 File Offset: 0x0004E6B8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Locker.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00004723 File Offset: 0x00002923
	public bool IsEquipping()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x000097CA File Offset: 0x000079CA
	public void EquipFromIndex(int index)
	{
		if (index < 0 || index > 2)
		{
			return;
		}
		base.ServerRPC<int>("RPC_Equip", index);
	}

	// Token: 0x020000A7 RID: 167
	public static class LockerFlags
	{
		// Token: 0x04000616 RID: 1558
		public const BaseEntity.Flags IsEquipping = BaseEntity.Flags.Reserved1;
	}
}
