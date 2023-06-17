using System;
using UnityEngine.Serialization;

// Token: 0x02000530 RID: 1328
public class PlaceRoadObjects : ProceduralComponent
{
	// Token: 0x04001A68 RID: 6760
	public PathList.BasicObject[] Start;

	// Token: 0x04001A69 RID: 6761
	public PathList.BasicObject[] End;

	// Token: 0x04001A6A RID: 6762
	[FormerlySerializedAs("RoadsideObjects")]
	public PathList.SideObject[] Side;

	// Token: 0x04001A6B RID: 6763
	[FormerlySerializedAs("RoadObjects")]
	public PathList.PathObject[] Path;

	// Token: 0x06001E09 RID: 7689 RVA: 0x000A4790 File Offset: 0x000A2990
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads)
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
