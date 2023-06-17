using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000296 RID: 662
public class Profile
{
	// Token: 0x04000F53 RID: 3923
	public Stopwatch watch = new Stopwatch();

	// Token: 0x04000F54 RID: 3924
	public string category;

	// Token: 0x04000F55 RID: 3925
	public string name;

	// Token: 0x04000F56 RID: 3926
	public float warnTime;

	// Token: 0x060012B1 RID: 4785 RVA: 0x00010050 File Offset: 0x0000E250
	public Profile(string cat, string nam, float WarnTime = 1f)
	{
		this.category = cat;
		this.name = nam;
		this.warnTime = WarnTime;
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x00010078 File Offset: 0x0000E278
	public void Start()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000797D8 File Offset: 0x000779D8
	public void Stop()
	{
		this.watch.Stop();
		if ((float)this.watch.Elapsed.Seconds > this.warnTime)
		{
			Debug.Log(string.Concat(new object[]
			{
				this.category,
				".",
				this.name,
				": Took ",
				this.watch.Elapsed.Seconds,
				" seconds"
			}));
		}
	}
}
