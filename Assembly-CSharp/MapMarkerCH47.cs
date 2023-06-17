using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class MapMarkerCH47 : MapMarker
{
	// Token: 0x04000819 RID: 2073
	private GameObject createdMarker;

	// Token: 0x06000B8E RID: 2958 RVA: 0x0000B0D1 File Offset: 0x000092D1
	public override void SetupUIMarker(GameObject marker)
	{
		base.SetupUIMarker(marker);
		this.createdMarker = marker;
		this.Update();
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00059820 File Offset: 0x00057A20
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		if (this.createdMarker == null)
		{
			return;
		}
		Vector2 v = MapInterface.WorldPosToImagePos(base.transform.position);
		this.createdMarker.GetComponent<RectTransform>().localPosition = v;
		if (!base.GetParentEntity())
		{
			return;
		}
		Vector3 forward = base.GetParentEntity().transform.forward;
		forward.y = 0f;
		forward.Normalize();
		this.createdMarker.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(forward.x, -forward.z) * 57.29578f + 180f);
	}
}
