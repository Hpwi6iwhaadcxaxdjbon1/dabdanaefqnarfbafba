using System;

// Token: 0x020003F2 RID: 1010
public class SaveRestore : SingletonComponent<SaveRestore>
{
	// Token: 0x04001571 RID: 5489
	public static bool IsSaving;

	// Token: 0x04001572 RID: 5490
	public bool timedSave = true;

	// Token: 0x04001573 RID: 5491
	public int timedSavePause;

	// Token: 0x04001574 RID: 5492
	public static DateTime SaveCreatedTime;
}
