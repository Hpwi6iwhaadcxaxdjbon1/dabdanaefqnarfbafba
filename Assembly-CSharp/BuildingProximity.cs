using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class BuildingProximity : PrefabAttribute
{
	// Token: 0x04000B30 RID: 2864
	private const float check_radius = 2f;

	// Token: 0x04000B31 RID: 2865
	private const float check_forgiveness = 0.01f;

	// Token: 0x04000B32 RID: 2866
	private const float foundation_width = 3f;

	// Token: 0x04000B33 RID: 2867
	private const float foundation_extents = 1.5f;

	// Token: 0x06000E24 RID: 3620 RVA: 0x000637C0 File Offset: 0x000619C0
	public static bool Check(BasePlayer player, Construction construction, Vector3 position, Quaternion rotation)
	{
		OBB obb;
		obb..ctor(position, rotation, construction.bounds);
		float radius = obb.extents.magnitude + 2f;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(obb.position, radius, list, 2097152, 2);
		uint num = 0U;
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock buildingBlock = list[i];
			Construction blockDefinition = buildingBlock.blockDefinition;
			Vector3 position2 = buildingBlock.transform.position;
			Quaternion rotation2 = buildingBlock.transform.rotation;
			BuildingProximity.ProximityInfo proximity = BuildingProximity.GetProximity(construction, position, rotation, blockDefinition, position2, rotation2);
			BuildingProximity.ProximityInfo proximity2 = BuildingProximity.GetProximity(blockDefinition, position2, rotation2, construction, position, rotation);
			BuildingProximity.ProximityInfo proximityInfo = default(BuildingProximity.ProximityInfo);
			proximityInfo.hit = (proximity.hit || proximity2.hit);
			proximityInfo.connection = (proximity.connection || proximity2.connection);
			if (proximity.sqrDist <= proximity2.sqrDist)
			{
				proximityInfo.line = proximity.line;
				proximityInfo.sqrDist = proximity.sqrDist;
			}
			else
			{
				proximityInfo.line = proximity2.line;
				proximityInfo.sqrDist = proximity2.sqrDist;
			}
			if (proximityInfo.connection)
			{
				BuildingManager.Building building = buildingBlock.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (dominatingBuildingPrivilege != null)
					{
						if (!construction.canBypassBuildingPermission && !dominatingBuildingPrivilege.IsAuthed(player))
						{
							Construction.lastPlacementError = "Cannot attach to unauthorized building";
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
						if (num == 0U)
						{
							num = building.ID;
						}
						else if (num != building.ID)
						{
							Construction.lastPlacementError = "Cannot connect two buildings with cupboards";
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
					}
				}
			}
			if (proximityInfo.hit)
			{
				Vector3 vector = proximityInfo.line.point1 - proximityInfo.line.point0;
				if (Mathf.Abs(vector.y) <= 1.49f && Vector3Ex.Magnitude2D(vector) <= 1.49f)
				{
					if (Input.GetKey(KeyCode.KeypadDivide))
					{
						DDraw.Line(proximityInfo.line.point0, proximityInfo.line.point1, Color.red, 0.1f, true, true);
						DDraw.Sphere(proximityInfo.line.point0, 0.1f, Color.red, 0.1f, true);
						DDraw.Sphere(proximityInfo.line.point1, 0.1f, Color.red, 0.1f, true);
					}
					Construction.lastPlacementError = "Not enough space";
					Pool.FreeList<BuildingBlock>(ref list);
					return true;
				}
			}
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return false;
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x00063A68 File Offset: 0x00061C68
	private static BuildingProximity.ProximityInfo GetProximity(Construction construction1, Vector3 position1, Quaternion rotation1, Construction construction2, Vector3 position2, Quaternion rotation2)
	{
		BuildingProximity.ProximityInfo proximityInfo = default(BuildingProximity.ProximityInfo);
		proximityInfo.hit = false;
		proximityInfo.connection = false;
		proximityInfo.line = default(Line);
		proximityInfo.sqrDist = float.MaxValue;
		for (int i = 0; i < construction1.allSockets.Length; i++)
		{
			ConstructionSocket constructionSocket = construction1.allSockets[i] as ConstructionSocket;
			if (!(constructionSocket == null))
			{
				for (int j = 0; j < construction2.allSockets.Length; j++)
				{
					Socket_Base socket = construction2.allSockets[j];
					if (constructionSocket.CanConnect(position1, rotation1, socket, position2, rotation2))
					{
						proximityInfo.connection = true;
						return proximityInfo;
					}
				}
			}
		}
		if (!proximityInfo.connection && construction1.allProximities.Length != 0)
		{
			for (int k = 0; k < construction1.allSockets.Length; k++)
			{
				ConstructionSocket constructionSocket2 = construction1.allSockets[k] as ConstructionSocket;
				if (!(constructionSocket2 == null) && constructionSocket2.socketType == ConstructionSocket.Type.Wall)
				{
					Vector3 selectPivot = constructionSocket2.GetSelectPivot(position1, rotation1);
					for (int l = 0; l < construction2.allProximities.Length; l++)
					{
						Vector3 selectPivot2 = construction2.allProximities[l].GetSelectPivot(position2, rotation2);
						Line line;
						line..ctor(selectPivot, selectPivot2);
						float sqrMagnitude = (line.point1 - line.point0).sqrMagnitude;
						if (sqrMagnitude < proximityInfo.sqrDist)
						{
							proximityInfo.hit = true;
							proximityInfo.line = line;
							proximityInfo.sqrDist = sqrMagnitude;
						}
					}
				}
			}
		}
		return proximityInfo;
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x0000D0DB File Offset: 0x0000B2DB
	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + rotation * this.worldPosition;
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x0000D0EF File Offset: 0x0000B2EF
	protected override Type GetIndexedType()
	{
		return typeof(BuildingProximity);
	}

	// Token: 0x0200019B RID: 411
	private struct ProximityInfo
	{
		// Token: 0x04000B34 RID: 2868
		public bool hit;

		// Token: 0x04000B35 RID: 2869
		public bool connection;

		// Token: 0x04000B36 RID: 2870
		public Line line;

		// Token: 0x04000B37 RID: 2871
		public float sqrDist;
	}
}
