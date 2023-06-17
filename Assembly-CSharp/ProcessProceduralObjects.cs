using System;
using System.Collections.Generic;

// Token: 0x02000531 RID: 1329
public class ProcessProceduralObjects : ProceduralComponent
{
	// Token: 0x06001E0B RID: 7691 RVA: 0x000A48C8 File Offset: 0x000A2AC8
	public override void Process(uint seed)
	{
		List<ProceduralObject> proceduralObjects = SingletonComponent<WorldSetup>.Instance.ProceduralObjects;
		if (!World.Cached)
		{
			for (int i = 0; i < proceduralObjects.Count; i++)
			{
				ProceduralObject proceduralObject = proceduralObjects[i];
				if (proceduralObject)
				{
					proceduralObject.Process();
				}
			}
		}
		proceduralObjects.Clear();
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06001E0C RID: 7692 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
