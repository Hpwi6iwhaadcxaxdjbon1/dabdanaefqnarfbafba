using System;
using UnityEngine;

// Token: 0x02000613 RID: 1555
public class ConvarToggleChildren : MonoBehaviour
{
	// Token: 0x04001F1D RID: 7965
	public string ConvarName;

	// Token: 0x04001F1E RID: 7966
	public string ConvarEnabled = "True";

	// Token: 0x04001F1F RID: 7967
	private bool state;

	// Token: 0x04001F20 RID: 7968
	private ConsoleSystem.Command Command;

	// Token: 0x060022E4 RID: 8932 RVA: 0x000BA560 File Offset: 0x000B8760
	protected void Awake()
	{
		this.Command = ConsoleSystem.Index.Client.Find(this.ConvarName);
		if (this.Command == null)
		{
			this.Command = ConsoleSystem.Index.Server.Find(this.ConvarName);
		}
		if (this.Command != null)
		{
			this.SetState(this.Command.String == this.ConvarEnabled);
		}
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x000BA5BC File Offset: 0x000B87BC
	protected void Update()
	{
		if (this.Command != null)
		{
			bool flag = this.Command.String == this.ConvarEnabled;
			if (this.state != flag)
			{
				this.SetState(flag);
			}
		}
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x000BA5F8 File Offset: 0x000B87F8
	private void SetState(bool newState)
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(newState);
		}
		this.state = newState;
	}
}
