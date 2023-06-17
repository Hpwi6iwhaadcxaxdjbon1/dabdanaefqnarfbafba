using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000626 RID: 1574
public class FPSText : MonoBehaviour
{
	// Token: 0x04001F40 RID: 8000
	public Text text;

	// Token: 0x04001F41 RID: 8001
	private Stopwatch fpsTimer = Stopwatch.StartNew();

	// Token: 0x0600232E RID: 9006 RVA: 0x000BAEE4 File Offset: 0x000B90E4
	protected void Update()
	{
		if (this.fpsTimer.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		this.text.enabled = true;
		this.fpsTimer.Reset();
		this.fpsTimer.Start();
		string text = Performance.current.frameRate + " FPS";
		this.text.text = text;
	}
}
