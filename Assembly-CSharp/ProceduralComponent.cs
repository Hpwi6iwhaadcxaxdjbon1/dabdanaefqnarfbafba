using System;
using UnityEngine;

// Token: 0x020004FA RID: 1274
public abstract class ProceduralComponent : MonoBehaviour
{
	// Token: 0x040019EB RID: 6635
	[InspectorFlags]
	public ProceduralComponent.Realm Mode = (ProceduralComponent.Realm)3;

	// Token: 0x040019EC RID: 6636
	public string Description = "Procedural Component";

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06001D91 RID: 7569 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool RunOnCache
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x00017B9F File Offset: 0x00015D9F
	public bool ShouldRun()
	{
		return (!World.Cached || this.RunOnCache) && (this.Mode & ProceduralComponent.Realm.Client) != (ProceduralComponent.Realm)0;
	}

	// Token: 0x06001D93 RID: 7571
	public abstract void Process(uint seed);

	// Token: 0x020004FB RID: 1275
	public enum Realm
	{
		// Token: 0x040019EE RID: 6638
		Client = 1,
		// Token: 0x040019EF RID: 6639
		Server
	}
}
