using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200060C RID: 1548
public class ConnectionScreen : SingletonComponent<ConnectionScreen>
{
	// Token: 0x04001EFC RID: 7932
	public Text statusText;

	// Token: 0x04001EFD RID: 7933
	public GameObject disconnectButton;

	// Token: 0x04001EFE RID: 7934
	public GameObject retryButton;

	// Token: 0x04001EFF RID: 7935
	public ServerBrowserInfo browserInfo;

	// Token: 0x04001F00 RID: 7936
	public UnityEvent onShowConnectionScreen = new UnityEvent();

	// Token: 0x04001F01 RID: 7937
	internal ServerList.Server currentServer;

	// Token: 0x060022B8 RID: 8888 RVA: 0x0001B881 File Offset: 0x00019A81
	public static void Show()
	{
		if (!SingletonComponent<ConnectionScreen>.Instance)
		{
			return;
		}
		SingletonComponent<ConnectionScreen>.Instance.onShowConnectionScreen.Invoke();
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000B999C File Offset: 0x000B7B9C
	public static void UpdateServer(ServerList.Server server)
	{
		if (!SingletonComponent<ConnectionScreen>.Instance)
		{
			return;
		}
		SingletonComponent<ConnectionScreen>.Instance.currentServer = server;
		SingletonComponent<ConnectionScreen>.Instance.browserInfo.Open(server);
		SingletonComponent<ConnectionScreen>.Instance.disconnectButton.SetActive(true);
		SingletonComponent<ConnectionScreen>.Instance.retryButton.SetActive(false);
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000B99F4 File Offset: 0x000B7BF4
	public static void FailedWith(string str)
	{
		if (!SingletonComponent<ConnectionScreen>.Instance)
		{
			return;
		}
		SingletonComponent<ConnectionScreen>.Instance.statusText.text = "Connection Failed: " + str;
		SingletonComponent<ConnectionScreen>.Instance.disconnectButton.SetActive(false);
		SingletonComponent<ConnectionScreen>.Instance.retryButton.SetActive(true);
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000B9A48 File Offset: 0x000B7C48
	public static void FailedWithException(Exception e)
	{
		if (!SingletonComponent<ConnectionScreen>.Instance)
		{
			return;
		}
		SingletonComponent<ConnectionScreen>.Instance.statusText.text = "Exception: " + e.Message;
		SingletonComponent<ConnectionScreen>.Instance.disconnectButton.SetActive(false);
		SingletonComponent<ConnectionScreen>.Instance.retryButton.SetActive(true);
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x0001B89F File Offset: 0x00019A9F
	public static void SetStatus(string status)
	{
		if (!SingletonComponent<ConnectionScreen>.Instance)
		{
			return;
		}
		SingletonComponent<ConnectionScreen>.Instance.statusText.text = status;
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000B9AA4 File Offset: 0x000B7CA4
	public static void DisconnectReason(string str)
	{
		if (!SingletonComponent<ConnectionScreen>.Instance)
		{
			return;
		}
		if (!SingletonComponent<ConnectionScreen>.Instance.statusText)
		{
			return;
		}
		if (!SingletonComponent<ConnectionScreen>.Instance.disconnectButton)
		{
			return;
		}
		if (!SingletonComponent<ConnectionScreen>.Instance.retryButton)
		{
			return;
		}
		SingletonComponent<ConnectionScreen>.Instance.statusText.text = "Disconnected: " + str;
		SingletonComponent<ConnectionScreen>.Instance.disconnectButton.SetActive(false);
		SingletonComponent<ConnectionScreen>.Instance.retryButton.SetActive(true);
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000B9B30 File Offset: 0x000B7D30
	public void RetryConnection()
	{
		if (this.currentServer != null && this.currentServer.Address != null)
		{
			ConnectionScreen.SetStatus("reconnecting");
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "client.connect", new object[]
			{
				this.currentServer.Address.ToString() + ":" + this.currentServer.ConnectionPort
			});
			SingletonComponent<ConnectionScreen>.Instance.disconnectButton.SetActive(true);
			SingletonComponent<ConnectionScreen>.Instance.retryButton.SetActive(false);
			return;
		}
		ConnectionScreen.SetStatus("reconnect_error_no_address");
	}
}
