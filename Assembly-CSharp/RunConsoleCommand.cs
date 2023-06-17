using System;
using UnityEngine;

// Token: 0x0200071E RID: 1822
public class RunConsoleCommand : MonoBehaviour
{
	// Token: 0x060027E7 RID: 10215 RVA: 0x0001F1E8 File Offset: 0x0001D3E8
	public void ClientRun(string command)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, command, Array.Empty<object>());
	}
}
