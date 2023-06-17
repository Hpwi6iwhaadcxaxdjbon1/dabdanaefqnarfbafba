using System;
using UnityEngine.Serialization;

// Token: 0x0200052E RID: 1326
public class PlacePowerlineObjects : ProceduralComponent
{
	// Token: 0x04001A60 RID: 6752
	public PathList.BasicObject[] Start;

	// Token: 0x04001A61 RID: 6753
	public PathList.BasicObject[] End;

	// Token: 0x04001A62 RID: 6754
	public PathList.SideObject[] Side;

	// Token: 0x04001A63 RID: 6755
	[FormerlySerializedAs("PowerlineObjects")]
	public PathList.PathObject[] Path;

	// Token: 0x06001E05 RID: 7685 RVA: 0x000A4520 File Offset: 0x000A2720
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Powerlines)
		{
			foreach (PathList.BasicObject obj in this.Start)
			{
				pathList.TrimStart(obj);
			}
			foreach (PathList.BasicObject obj2 in this.End)
			{
				pathList.TrimEnd(obj2);
			}
			foreach (PathList.BasicObject obj3 in this.Start)
			{
				pathList.SpawnStart(ref seed, obj3);
			}
			foreach (PathList.BasicObject obj4 in this.End)
			{
				pathList.SpawnEnd(ref seed, obj4);
			}
			foreach (PathList.PathObject obj5 in this.Path)
			{
				pathList.SpawnAlong(ref seed, obj5);
			}
			foreach (PathList.SideObject obj6 in this.Side)
			{
				pathList.SpawnSide(ref seed, obj6);
			}
			pathList.ResetTrims();
		}
	}
}
