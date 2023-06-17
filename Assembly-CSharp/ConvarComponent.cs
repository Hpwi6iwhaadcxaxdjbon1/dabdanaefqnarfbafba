using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000703 RID: 1795
public class ConvarComponent : MonoBehaviour
{
	// Token: 0x0400235A RID: 9050
	public bool runOnServer = true;

	// Token: 0x0400235B RID: 9051
	public bool runOnClient = true;

	// Token: 0x0400235C RID: 9052
	public List<ConvarComponent.ConvarEvent> List = new List<ConvarComponent.ConvarEvent>();

	// Token: 0x06002774 RID: 10100 RVA: 0x000CCE3C File Offset: 0x000CB03C
	protected void OnEnable()
	{
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnEnable();
		}
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000CCE98 File Offset: 0x000CB098
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnDisable();
		}
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x0001EC51 File Offset: 0x0001CE51
	private bool ShouldRun()
	{
		return this.runOnClient;
	}

	// Token: 0x02000704 RID: 1796
	[Serializable]
	public class ConvarEvent
	{
		// Token: 0x0400235D RID: 9053
		public string convar;

		// Token: 0x0400235E RID: 9054
		public string on;

		// Token: 0x0400235F RID: 9055
		public MonoBehaviour component;

		// Token: 0x04002360 RID: 9056
		internal ConsoleSystem.Command cmd;

		// Token: 0x06002778 RID: 10104 RVA: 0x000CCEFC File Offset: 0x000CB0FC
		public void OnEnable()
		{
			this.cmd = ConsoleSystem.Index.Client.Find(this.convar);
			if (this.cmd == null)
			{
				this.cmd = ConsoleSystem.Index.Server.Find(this.convar);
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged += new Action<ConsoleSystem.Command>(this.cmd_OnValueChanged);
			this.cmd_OnValueChanged(this.cmd);
		}

		// Token: 0x06002779 RID: 10105 RVA: 0x000CCF60 File Offset: 0x000CB160
		private void cmd_OnValueChanged(ConsoleSystem.Command obj)
		{
			if (this.component == null)
			{
				return;
			}
			bool flag = obj.String == this.on;
			if (this.component.enabled == flag)
			{
				return;
			}
			this.component.enabled = flag;
		}

		// Token: 0x0600277A RID: 10106 RVA: 0x0001EC7F File Offset: 0x0001CE7F
		public void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged -= new Action<ConsoleSystem.Command>(this.cmd_OnValueChanged);
		}
	}
}
