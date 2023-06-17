using System;

// Token: 0x02000100 RID: 256
public class ItemModXMasTreeDecoration : ItemMod
{
	// Token: 0x04000793 RID: 1939
	public ItemModXMasTreeDecoration.xmasFlags flagsToChange;

	// Token: 0x02000101 RID: 257
	public enum xmasFlags
	{
		// Token: 0x04000795 RID: 1941
		pineCones = 128,
		// Token: 0x04000796 RID: 1942
		candyCanes = 256,
		// Token: 0x04000797 RID: 1943
		gingerbreadMen = 512,
		// Token: 0x04000798 RID: 1944
		Tinsel = 1024,
		// Token: 0x04000799 RID: 1945
		Balls = 2048,
		// Token: 0x0400079A RID: 1946
		Star = 16384,
		// Token: 0x0400079B RID: 1947
		Lights = 32768
	}
}
