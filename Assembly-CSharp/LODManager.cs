using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020003C5 RID: 965
public class LODManager : SingletonComponent<LODManager>
{
	// Token: 0x040014E8 RID: 5352
	public float MaxMilliseconds = 1f;

	// Token: 0x040014E9 RID: 5353
	private ListHashSet<ILOD> members = new ListHashSet<ILOD>(8);

	// Token: 0x040014EA RID: 5354
	private Stopwatch watch = Stopwatch.StartNew();

	// Token: 0x040014EB RID: 5355
	private int offset;

	// Token: 0x06001859 RID: 6233 RVA: 0x0008CDF0 File Offset: 0x0008AFF0
	protected void LateUpdate()
	{
		this.watch.Reset();
		this.watch.Start();
		for (int i = 0; i < this.members.Count; i++)
		{
			int num = (i + this.offset) % this.members.Count;
			this.members.Values[num].ChangeLOD();
			if (this.watch.Elapsed.TotalMilliseconds > (double)this.MaxMilliseconds)
			{
				this.offset = num + 1;
				return;
			}
		}
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x0001459A File Offset: 0x0001279A
	public static void Add(ILOD component, Transform transform)
	{
		if (!SingletonComponent<LODManager>.Instance)
		{
			return;
		}
		SingletonComponent<LODManager>.Instance.members.Add(component);
		component.ChangeLOD();
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x000145BF File Offset: 0x000127BF
	public static void Remove(ILOD component, Transform transform)
	{
		if (!SingletonComponent<LODManager>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LODManager>.Instance.members.Remove(component))
		{
			Debug.LogError("Removing component from LOD cell it does not belong to. Did it move after wake?", transform);
		}
	}
}
