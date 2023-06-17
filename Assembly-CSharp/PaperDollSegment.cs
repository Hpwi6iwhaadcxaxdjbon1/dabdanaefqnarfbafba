using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200066A RID: 1642
public class PaperDollSegment : BaseMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x0400208B RID: 8331
	public static HitArea selectedAreas;

	// Token: 0x0400208C RID: 8332
	[InspectorFlags]
	public HitArea area;

	// Token: 0x0400208D RID: 8333
	public Image overlayImg;

	// Token: 0x06002490 RID: 9360 RVA: 0x0001CD41 File Offset: 0x0001AF41
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.SetSegmentVisible(true);
		PaperDollSegment.selectedAreas = this.area;
		GlobalMessages.OnClothingChanged();
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x0001CD5A File Offset: 0x0001AF5A
	public void OnPointerExit(PointerEventData eventData)
	{
		this.SetSegmentVisible(false);
		PaperDollSegment.selectedAreas = (HitArea)0;
		GlobalMessages.OnClothingChanged();
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000C13C4 File Offset: 0x000BF5C4
	public void SetSegmentVisible(bool vis)
	{
		Color color = this.overlayImg.color;
		color.a = (vis ? 0f : 0f);
		this.overlayImg.color = color;
	}
}
