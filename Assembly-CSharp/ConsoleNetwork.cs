using System;
using Network;
using UnityEngine;

// Token: 0x02000291 RID: 657
public static class ConsoleNetwork
{
	// Token: 0x06001296 RID: 4758 RVA: 0x0000FF84 File Offset: 0x0000E184
	internal static void Init()
	{
		ConsoleSystem.OnSendToServer = new Func<string, bool>(ConsoleNetwork.ClientRunOnServer);
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x000793B0 File Offset: 0x000775B0
	public static bool ClientRunOnServer(string strCommand)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return false;
		}
		Net.cl.write.Start();
		Net.cl.write.PacketID(12);
		Net.cl.write.String(strCommand);
		Net.cl.write.Send(new SendInfo(Net.cl.Connection));
		return true;
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x0000FF97 File Offset: 0x0000E197
	internal static void OnConsoleMessageFromServer(Message packet)
	{
		Debug.Log(packet.read.String());
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x00079424 File Offset: 0x00077624
	internal static void OnConsoleCommandfromServer(Message packet)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet().FromServer(), packet.read.String(), Array.Empty<object>());
	}
}
