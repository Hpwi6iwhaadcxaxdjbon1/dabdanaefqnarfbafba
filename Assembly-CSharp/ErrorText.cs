using System;
using System.Diagnostics;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000625 RID: 1573
public class ErrorText : MonoBehaviour
{
	// Token: 0x04001F3D RID: 7997
	public Text text;

	// Token: 0x04001F3E RID: 7998
	public int maxLength = 1024;

	// Token: 0x04001F3F RID: 7999
	private Stopwatch stopwatch;

	// Token: 0x06002329 RID: 9001 RVA: 0x0001BDE2 File Offset: 0x00019FE2
	public void OnEnable()
	{
		Output.OnMessage += new Action<string, string, LogType>(this.CaptureLog);
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x0001BDF5 File Offset: 0x00019FF5
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		Output.OnMessage -= new Action<string, string, LogType>(this.CaptureLog);
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000BADE8 File Offset: 0x000B8FE8
	internal void CaptureLog(string error, string stacktrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		Text text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			error,
			"\n",
			stacktrace,
			"\n\n"
		});
		if (this.text.text.Length > this.maxLength)
		{
			this.text.text = this.text.text.Substring(this.text.text.Length - this.maxLength, this.maxLength);
		}
		this.stopwatch = Stopwatch.StartNew();
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000BAE98 File Offset: 0x000B9098
	protected void Update()
	{
		if (this.stopwatch != null && this.stopwatch.Elapsed.TotalSeconds > 30.0)
		{
			this.text.text = string.Empty;
			this.stopwatch = null;
		}
	}
}
