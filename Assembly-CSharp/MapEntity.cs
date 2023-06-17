using System;
using Network;

// Token: 0x020000A8 RID: 168
public class MapEntity : HeldEntity
{
	// Token: 0x04000617 RID: 1559
	[NonSerialized]
	public uint[] fogImages = new uint[1];

	// Token: 0x04000618 RID: 1560
	[NonSerialized]
	public uint[] paintImages = new uint[144];

	// Token: 0x06000969 RID: 2409 RVA: 0x000504FC File Offset: 0x0004E6FC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MapEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00050540 File Offset: 0x0004E740
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mapEntity != null)
		{
			if (info.msg.mapEntity.fogImages.Count == this.fogImages.Length)
			{
				this.fogImages = info.msg.mapEntity.fogImages.ToArray();
			}
			if (info.msg.mapEntity.paintImages.Count == this.paintImages.Length)
			{
				this.paintImages = info.msg.mapEntity.paintImages.ToArray();
			}
		}
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x000097E1 File Offset: 0x000079E1
	public void PaintImageUpdate(int id, uint hash, byte[] data)
	{
		this.paintImages[id] = hash;
		base.ServerRPC<byte, byte, uint, uint, byte[]>("ImageUpdate", 1, (byte)id, hash, (uint)data.Length, data);
	}
}
