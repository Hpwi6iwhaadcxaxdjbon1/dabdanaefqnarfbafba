using System;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class ScientistNPC : HumanNPC
{
	// Token: 0x04000884 RID: 2180
	public GameObjectRef[] RadioChatterEffects;

	// Token: 0x04000885 RID: 2181
	public GameObjectRef[] DeathEffects;

	// Token: 0x04000886 RID: 2182
	public Vector2 IdleChatterRepeatRange = new Vector2(10f, 15f);

	// Token: 0x04000887 RID: 2183
	public ScientistNPC.RadioChatterType radioChatterType;

	// Token: 0x0200012E RID: 302
	public enum RadioChatterType
	{
		// Token: 0x04000889 RID: 2185
		NONE,
		// Token: 0x0400088A RID: 2186
		Idle,
		// Token: 0x0400088B RID: 2187
		Alert
	}
}
