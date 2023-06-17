using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000345 RID: 837
public class CH47LandingZone : MonoBehaviour
{
	// Token: 0x040012F0 RID: 4848
	public float lastDropTime;

	// Token: 0x040012F1 RID: 4849
	private static List<CH47LandingZone> landingZones = new List<CH47LandingZone>();

	// Token: 0x060015ED RID: 5613 RVA: 0x00012869 File Offset: 0x00010A69
	public void Awake()
	{
		if (!CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Add(this);
		}
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x00085D9C File Offset: 0x00083F9C
	public static CH47LandingZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47LandingZone result = null;
		foreach (CH47LandingZone ch47LandingZone in CH47LandingZone.landingZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47LandingZone.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = ch47LandingZone;
			}
		}
		return result;
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x00012883 File Offset: 0x00010A83
	public void OnDestroy()
	{
		if (CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Remove(this);
		}
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x0001289E File Offset: 0x00010A9E
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x000128AC File Offset: 0x00010AAC
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x00085E10 File Offset: 0x00084010
	public void OnDrawGizmos()
	{
		Color magenta = Color.magenta;
		magenta.a = 0.25f;
		Gizmos.color = magenta;
		GizmosUtil.DrawCircleY(base.transform.position, 6f);
		magenta.a = 1f;
		Gizmos.color = magenta;
		GizmosUtil.DrawWireCircleY(base.transform.position, 6f);
	}
}
