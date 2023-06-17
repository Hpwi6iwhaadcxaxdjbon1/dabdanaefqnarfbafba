using System;
using ConVar;
using UnityEngine;

// Token: 0x020006F7 RID: 1783
public abstract class BaseMonoBehaviour : FacepunchBehaviour
{
	// Token: 0x06002746 RID: 10054 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool IsDebugging()
	{
		return false;
	}

	// Token: 0x06002747 RID: 10055 RVA: 0x0001E9F1 File Offset: 0x0001CBF1
	public virtual string GetLogColor()
	{
		return "yellow";
	}

	// Token: 0x06002748 RID: 10056 RVA: 0x000CC650 File Offset: 0x000CA850
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x06002749 RID: 10057 RVA: 0x000CC6C0 File Offset: 0x000CA8C0
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1, object arg2)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1, arg2);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000CC734 File Offset: 0x000CA934
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			str,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x020006F8 RID: 1784
	public enum LogEntryType
	{
		// Token: 0x04002336 RID: 9014
		General,
		// Token: 0x04002337 RID: 9015
		Network,
		// Token: 0x04002338 RID: 9016
		Hierarchy,
		// Token: 0x04002339 RID: 9017
		Serialization
	}
}
