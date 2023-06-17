using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class MapMarker : BaseEntity
{
	// Token: 0x040010A3 RID: 4259
	public static List<MapMarker> mapMarkers = new List<MapMarker>();

	// Token: 0x040010A4 RID: 4260
	public GameObject markerObj;

	// Token: 0x060013F2 RID: 5106 RVA: 0x00010FA9 File Offset: 0x0000F1A9
	public static void Init()
	{
		if (MapMarker.mapMarkers != null)
		{
			MapMarker.mapMarkers.Clear();
		}
		MapMarker.mapMarkers = new List<MapMarker>();
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void SetupUIMarker(GameObject marker)
	{
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x00010FC6 File Offset: 0x0000F1C6
	public static void AddMarker(MapMarker marker)
	{
		if (!MapMarker.mapMarkers.Contains(marker))
		{
			MapMarker.mapMarkers.Add(marker);
		}
		MapMarker.UpdateInterface();
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x00010FE5 File Offset: 0x0000F1E5
	public static void RemoveMarker(MapMarker marker)
	{
		if (MapMarker.mapMarkers.Contains(marker))
		{
			MapMarker.mapMarkers.Remove(marker);
		}
		MapMarker.UpdateInterface();
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x00011005 File Offset: 0x0000F205
	public override void InitShared()
	{
		if (base.isClient)
		{
			MapMarker.AddMarker(this);
		}
		base.InitShared();
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x0001101B File Offset: 0x0000F21B
	public override void DestroyShared()
	{
		if (base.isClient)
		{
			MapMarker.RemoveMarker(this);
		}
		base.DestroyShared();
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x00011031 File Offset: 0x0000F231
	public static void UpdateInterface()
	{
		if (MapInterface.IsOpen)
		{
			SingletonComponent<MapInterface>.Instance.MarkersDirty();
		}
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x00011044 File Offset: 0x0000F244
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		MapMarker.UpdateInterface();
	}
}
