using System;
using Rust;
using UnityEngine;

// Token: 0x02000721 RID: 1825
public abstract class UpdateBehaviour : MonoBehaviour
{
	// Token: 0x040023C9 RID: 9161
	internal float lastUpdate;

	// Token: 0x040023CA RID: 9162
	internal float nextUpdate;

	// Token: 0x060027F2 RID: 10226
	public abstract void DeltaUpdate(float deltaTime);

	// Token: 0x060027F3 RID: 10227 RVA: 0x0001F24D File Offset: 0x0001D44D
	protected void Sleep(float seconds)
	{
		this.nextUpdate = Time.time + seconds;
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x0001F25C File Offset: 0x0001D45C
	protected void SleepAccumulative(float seconds)
	{
		this.nextUpdate += seconds;
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x0001F26C File Offset: 0x0001D46C
	protected virtual void OnEnable()
	{
		this.lastUpdate = Time.time - Time.deltaTime;
		this.nextUpdate = Time.time;
		UpdateHandler.Add(this);
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x0001F290 File Offset: 0x0001D490
	protected virtual void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		UpdateHandler.Remove(this);
	}
}
