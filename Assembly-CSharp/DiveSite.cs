using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class DiveSite : JunkPile
{
	// Token: 0x0400075D RID: 1885
	public Transform bobber;

	// Token: 0x06000B08 RID: 2824 RVA: 0x00056E68 File Offset: 0x00055068
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		if (this.bobber)
		{
			Vector3 position = this.bobber.transform.position;
			position.y = WaterSystem.GetHeight(position);
			this.bobber.transform.position = position;
		}
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x00056EB8 File Offset: 0x000550B8
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		if (this.bobber)
		{
			Vector3 position = this.bobber.transform.position;
			position.y = WaterSystem.GetHeight(position) + Mathf.Sin(Time.time * 3f) * 0.075f;
			this.bobber.transform.position = position;
		}
	}
}
