using System;

// Token: 0x02000524 RID: 1316
public class GenerateWireMeshes : ProceduralComponent
{
	// Token: 0x06001DF4 RID: 7668 RVA: 0x00017D0B File Offset: 0x00015F0B
	public override void Process(uint seed)
	{
		TerrainMeta.Path.CreateWires();
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x06001DF5 RID: 7669 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
