using System;
using UnityEngine;

// Token: 0x020006CB RID: 1739
public class TooltipContainer : MonoBehaviour
{
	// Token: 0x040022B2 RID: 8882
	private RectTransform Source;

	// Token: 0x06002698 RID: 9880 RVA: 0x000CABA0 File Offset: 0x000C8DA0
	private void Update()
	{
		if (this.Source == null || !this.Source.gameObject.activeInHierarchy)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3[] array = new Vector3[4];
		this.Source.GetLocalCorners(array);
		Vector3 vector = this.Source.TransformPoint(Vector3.Lerp(array[1], array[2], 0.5f));
		(base.transform as RectTransform).pivot = new Vector2(0.5f, 0f);
		mousePosition.x = vector.x;
		mousePosition.y = vector.y + 8f;
		if (mousePosition.y > (float)Screen.height - (base.transform as RectTransform).rect.height)
		{
			Vector3 vector2 = this.Source.TransformPoint(Vector3.Lerp(array[0], array[3], 0.5f));
			(base.transform as RectTransform).pivot = new Vector2(0.5f, 1f);
			mousePosition.x = vector2.x;
			mousePosition.y = vector2.y - 8f;
		}
		float num = (base.transform as RectTransform).sizeDelta.x * 0.5f;
		mousePosition.x = Mathf.Clamp(mousePosition.x, num + 8f, (float)Screen.width - num - 8f);
		if (mousePosition.y < 8f)
		{
			mousePosition.y = 8f;
		}
		base.transform.position = mousePosition;
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000CAD4C File Offset: 0x000C8F4C
	public void SetSourceRect(RectTransform source)
	{
		this.Source = source;
		this.Update();
		base.transform.localScale = Vector3.zero;
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.scale(base.gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(27);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.2f);
	}
}
