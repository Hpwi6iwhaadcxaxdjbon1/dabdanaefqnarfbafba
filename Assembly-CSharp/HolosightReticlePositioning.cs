using System;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class HolosightReticlePositioning : MonoBehaviour
{
	// Token: 0x04000828 RID: 2088
	public IronsightAimPoint aimPoint;

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000BA9 RID: 2985 RVA: 0x0000B1D8 File Offset: 0x000093D8
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0000B1E5 File Offset: 0x000093E5
	private void Update()
	{
		if (MainCamera.isValid)
		{
			this.UpdatePosition(MainCamera.mainCamera);
		}
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x00059E40 File Offset: 0x00058040
	private void UpdatePosition(Camera cam)
	{
		Vector3 position = this.aimPoint.targetPoint.transform.position;
		Vector2 vector = RectTransformUtility.WorldToScreenPoint(cam, position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform.parent as RectTransform, vector, cam, ref vector);
		vector.x /= (this.rectTransform.parent as RectTransform).rect.width * 0.5f;
		vector.y /= (this.rectTransform.parent as RectTransform).rect.height * 0.5f;
		this.rectTransform.anchoredPosition = vector;
	}
}
