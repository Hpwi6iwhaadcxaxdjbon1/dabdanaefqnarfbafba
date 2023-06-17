using System;

// Token: 0x0200050D RID: 1293
public class GeneratePowerlineTopology : ProceduralComponent
{
	// Token: 0x06001DBF RID: 7615 RVA: 0x000A1F40 File Offset: 0x000A0140
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Powerlines)
		{
			pathList.Path.RecalculateTangents();
		}
	}
}
