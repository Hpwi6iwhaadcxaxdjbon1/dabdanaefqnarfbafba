using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200075E RID: 1886
public struct Timing
{
	// Token: 0x0400243E RID: 9278
	private Stopwatch sw;

	// Token: 0x0400243F RID: 9279
	private string name;

	// Token: 0x06002926 RID: 10534 RVA: 0x0001FFF5 File Offset: 0x0001E1F5
	public static Timing Start(string name)
	{
		return new Timing(name);
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000D2508 File Offset: 0x000D0708
	public void End()
	{
		if (this.sw.Elapsed.TotalSeconds > 0.30000001192092896)
		{
			Debug.Log("[" + this.sw.Elapsed.TotalSeconds.ToString("0.0") + "s] " + this.name);
		}
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x0001FFFD File Offset: 0x0001E1FD
	public Timing(string name)
	{
		this.sw = Stopwatch.StartNew();
		this.name = name;
	}
}
