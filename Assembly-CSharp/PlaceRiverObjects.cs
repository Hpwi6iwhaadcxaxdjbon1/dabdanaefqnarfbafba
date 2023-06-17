using System;
using UnityEngine.Serialization;

// Token: 0x0200052F RID: 1327
public class PlaceRiverObjects : ProceduralComponent
{
	// Token: 0x04001A64 RID: 6756
	public PathList.BasicObject[] Start;

	// Token: 0x04001A65 RID: 6757
	public PathList.BasicObject[] End;

	// Token: 0x04001A66 RID: 6758
	[FormerlySerializedAs("RiversideObjects")]
	public PathList.SideObject[] Side;

	// Token: 0x04001A67 RID: 6759
	[FormerlySerializedAs("RiverObjects")]
	public PathList.PathObject[] Path;

	// Token: 0x06001E07 RID: 7687 RVA: 0x000A4658 File Offset: 0x000A2858
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers)
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
			foreach (PathList.PathObject obj4 in this.Path)
			{
				pathList.SpawnAlong(ref seed, obj4);
			}
			foreach (PathList.SideObject obj5 in this.Side)
			{
				pathList.SpawnSide(ref seed, obj5);
			}
			foreach (PathList.BasicObject obj6 in this.End)
			{
				pathList.SpawnEnd(ref seed, obj6);
			}
			pathList.ResetTrims();
		}
	}
}
