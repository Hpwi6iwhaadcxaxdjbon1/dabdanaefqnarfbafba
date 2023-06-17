using System;

// Token: 0x0200034E RID: 846
public class DummySwitch : IOEntity
{
	// Token: 0x04001307 RID: 4871
	public string listenString = "";

	// Token: 0x04001308 RID: 4872
	public float duration = -1f;

	// Token: 0x0600160B RID: 5643 RVA: 0x0000464B File Offset: 0x0000284B
	public override bool WantsPower()
	{
		return base.IsOn();
	}
}
