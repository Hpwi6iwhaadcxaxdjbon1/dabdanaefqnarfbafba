using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class MapMarkerGenericRadius : MapMarker
{
	// Token: 0x04000619 RID: 1561
	public float radius;

	// Token: 0x0400061A RID: 1562
	public Color color1;

	// Token: 0x0400061B RID: 1563
	public Color color2;

	// Token: 0x0400061C RID: 1564
	public float alpha;

	// Token: 0x0400061D RID: 1565
	private UIMapGenericRadius cachedUIMarker;

	// Token: 0x0600096D RID: 2413 RVA: 0x000505D8 File Offset: 0x0004E7D8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MapMarkerGenericRadius.OnRpcMessage", 0.1f))
		{
			if (rpc == 1743973926U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: MarkerUpdate ");
				}
				using (TimeWarning.New("MarkerUpdate", 0.1f))
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
							this.MarkerUpdate(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in MarkerUpdate", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000506F4 File Offset: 0x0004E8F4
	[BaseEntity.RPC_Client]
	public void MarkerUpdate(BaseEntity.RPCMessage msg)
	{
		Vector3 vector = msg.read.Vector3();
		float a = msg.read.Float();
		Vector3 vector2 = msg.read.Vector3();
		float totalAlpha = msg.read.Float();
		float num = msg.read.Float();
		float a2 = (vector2.x == -1f) ? 0f : 1f;
		this.radius = num;
		this.color1 = new Color(vector.x, vector.y, vector.z, a);
		this.color2 = new Color(vector2.x, vector2.y, vector2.z, a2);
		this.alpha = totalAlpha;
		if (this.cachedUIMarker)
		{
			this.cachedUIMarker.SetRadius(this.radius, false);
			this.cachedUIMarker.UpdateColors(new Color(vector.x, vector.y, vector.z, a), new Color(vector2.x, vector2.y, vector2.z, a2), totalAlpha);
		}
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x00050804 File Offset: 0x0004EA04
	public override void SetupUIMarker(GameObject marker)
	{
		if (marker == null)
		{
			return;
		}
		UIMapGenericRadius component = marker.GetComponent<UIMapGenericRadius>();
		if (component)
		{
			component.SetRadius(this.radius, true);
			component.UpdateColors(this.color1, this.color2, this.alpha);
			this.cachedUIMarker = component;
		}
	}
}
