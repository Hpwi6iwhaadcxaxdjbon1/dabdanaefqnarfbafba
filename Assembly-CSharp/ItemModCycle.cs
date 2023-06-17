using System;
using UnityEngine;

// Token: 0x02000455 RID: 1109
public class ItemModCycle : ItemMod
{
	// Token: 0x04001729 RID: 5929
	public ItemMod[] actions;

	// Token: 0x0400172A RID: 5930
	public float timeBetweenCycles = 1f;

	// Token: 0x0400172B RID: 5931
	public float timerStart;

	// Token: 0x0400172C RID: 5932
	public bool onlyAdvanceTimerWhenPass;

	// Token: 0x06001A58 RID: 6744 RVA: 0x00015C83 File Offset: 0x00013E83
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null", base.gameObject);
		}
	}
}
