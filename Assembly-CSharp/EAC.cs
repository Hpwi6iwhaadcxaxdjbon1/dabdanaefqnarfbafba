using System;
using System.Collections;
using System.IO;
using EasyAntiCheat.Client;
using EasyAntiCheat.Client.Hydra;
using Network;
using UnityEngine;

// Token: 0x02000200 RID: 512
public static class EAC
{
	// Token: 0x04000CA9 RID: 3241
	public static bool isLoading = false;

	// Token: 0x04000CAA RID: 3242
	public static string lastError = "";

	// Token: 0x06000FC7 RID: 4039 RVA: 0x0000DFDA File Offset: 0x0000C1DA
	public static void Encrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		Runtime.NetProtect.ProtectMessage(src, (long)srcOffset, dst, (long)dstOffset);
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x0000DFEE File Offset: 0x0000C1EE
	public static void Decrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		Runtime.NetProtect.UnprotectMessage(src, (long)srcOffset, dst, (long)dstOffset);
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x0000E002 File Offset: 0x0000C202
	private static void OnLaunchProgress(object sender, LoadProgressEventArgs eventArgs)
	{
		SingletonComponent<Bootstrap>.Instance.messageString = string.Format("Loading EasyAntiCheat {0}%", eventArgs.Progress);
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x0006B810 File Offset: 0x00069A10
	private static void OnLaunchCompleted(object sender, LoadCompletedEventArgs eventArgs)
	{
		EAC.isLoading = false;
		if (eventArgs.Status == null)
		{
			SingletonComponent<Bootstrap>.Instance.messageString = "EasyAntiCheat Loaded";
			return;
		}
		SingletonComponent<Bootstrap>.Instance.ThrowError(string.Format("EasyAntiCheat Error: ({1}) {0}", eventArgs.Message, eventArgs.Status));
		EAC.lastError = eventArgs.Message;
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x0006B870 File Offset: 0x00069A70
	private static void OnStateChanged(object sender, StateChangedEventArgs eventArgs)
	{
		StateChangeType type = eventArgs.Type;
		if (type == 1)
		{
			if (Net.cl != null && Net.cl.IsConnected())
			{
				Net.cl.Disconnect("Integrity Error: " + eventArgs.Message, true);
			}
			Debug.LogError("[EAC] Game Client Violation: " + eventArgs.Message);
			return;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"[EAC] Unhandled game callback:",
			eventArgs.Type,
			" - ",
			eventArgs.Message
		}));
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x0006B904 File Offset: 0x00069B04
	private static void SendToServer(byte[] message, int messageLength)
	{
		if (Net.cl.write.Start())
		{
			Net.cl.write.PacketID(22);
			Net.cl.write.UInt32((uint)messageLength);
			Net.cl.write.Write(message, 0, messageLength);
			Net.cl.write.Send(new SendInfo(Net.cl.Connection));
		}
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0000E023 File Offset: 0x0000C223
	public static IEnumerator DoStartup()
	{
		if (Runtime.IsActive())
		{
			DebugEx.Log("Loading EasyAntiCheat", StackTraceLogType.None);
			EAC.isLoading = true;
			Runtime.Initialize(new EventHandler<StateChangedEventArgs>(EAC.OnStateChanged), new EventHandler<LoadCompletedEventArgs>(EAC.OnLaunchCompleted), new EventHandler<LoadProgressEventArgs>(EAC.OnLaunchProgress));
			while (EAC.isLoading)
			{
				yield return CoroutineEx.waitForEndOfFrame;
			}
			DebugEx.Log("EasyAntiCheat Loaded", StackTraceLogType.None);
		}
		if (EAC.lastError.Length > 0)
		{
			yield return CoroutineEx.waitForSeconds(5f);
			Application.Quit();
		}
		yield break;
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0000E02B File Offset: 0x0000C22B
	public static void DoShutdown()
	{
		EAC.lastError = "Shut Down";
		Runtime.Release();
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x0006B974 File Offset: 0x00069B74
	public static void DoUpdate()
	{
		if (Net.cl != null && Net.cl.IsConnected())
		{
			byte[] message;
			int messageLength;
			while (Runtime.PopNetworkMessage(ref message, ref messageLength))
			{
				EAC.SendToServer(message, messageLength);
			}
		}
		Runtime.PollStatus();
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x0000E03C File Offset: 0x0000C23C
	public static void OnJoinServer()
	{
		Runtime.ConnectionReset();
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x0000E03C File Offset: 0x0000C23C
	public static void OnLeaveServer()
	{
		Runtime.ConnectionReset();
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x0006B9B0 File Offset: 0x00069BB0
	public static void OnMessageReceived(Message message)
	{
		MemoryStream memoryStream = message.read.MemoryStreamWithSize();
		Runtime.PushNetworkMessage(memoryStream.GetBuffer(), (int)memoryStream.Length);
	}
}
