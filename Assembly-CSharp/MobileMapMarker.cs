using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class MobileMapMarker : MapMarker
{
	// Token: 0x0400081A RID: 2074
	private GameObject createdMarker;

	// Token: 0x06000B92 RID: 2962 RVA: 0x0000B0E7 File Offset: 0x000092E7
	public override void SetupUIMarker(GameObject marker)
	{
		base.SetupUIMarker(marker);
		this.createdMarker = marker;
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x000598DC File Offset: 0x00057ADC
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
		base.GetParentEntity();
	}
}
