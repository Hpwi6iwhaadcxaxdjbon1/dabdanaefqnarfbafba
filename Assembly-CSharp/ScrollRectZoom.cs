using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006B8 RID: 1720
public class ScrollRectZoom : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x04002272 RID: 8818
	public ScrollRectEx scrollRect;

	// Token: 0x04002273 RID: 8819
	public float zoom = 1f;

	// Token: 0x04002274 RID: 8820
	public bool smooth = true;

	// Token: 0x04002275 RID: 8821
	public float max = 1.5f;

	// Token: 0x04002276 RID: 8822
	public float min = 0.5f;

	// Token: 0x04002277 RID: 8823
	public float velocity;

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06002644 RID: 9796 RVA: 0x0001DCD4 File Offset: 0x0001BED4
	public RectTransform rectTransform
	{
		get
		{
			return this.scrollRect.transform as RectTransform;
		}
	}

	// Token: 0x06002645 RID: 9797 RVA: 0x0001DCE6 File Offset: 0x0001BEE6
	private void OnEnable()
	{
		this.SetZoom(this.zoom);
	}

	// Token: 0x06002646 RID: 9798 RVA: 0x0001DCF4 File Offset: 0x0001BEF4
	public void OnScroll(PointerEventData data)
	{
		this.velocity += data.scrollDelta.y * 0.001f;
		this.velocity = Mathf.Clamp(this.velocity, -1f, 1f);
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x0001DD2F File Offset: 0x0001BF2F
	private void Update()
	{
		this.velocity = Mathf.Lerp(this.velocity, 0f, Time.deltaTime * 10f);
		this.SetZoom(this.zoom + this.velocity);
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000C9D38 File Offset: 0x000C7F38
	private void SetZoom(float z)
	{
		z = Mathf.Clamp(z, this.min, this.max);
		if (z == this.zoom)
		{
			return;
		}
		this.zoom = z;
		Rect rect = (this.scrollRect.transform as RectTransform).rect;
		Vector2 vector = this.scrollRect.content.rect.size * this.zoom;
		Vector2 normalizedPosition = this.scrollRect.normalizedPosition;
		if (vector.x < rect.width)
		{
			this.zoom = rect.width / this.scrollRect.content.rect.size.x;
		}
		if (vector.y < rect.height)
		{
			this.zoom = rect.height / this.scrollRect.content.rect.size.y;
		}
		this.scrollRect.content.localScale = Vector3.one * this.zoom;
		this.scrollRect.normalizedPosition = normalizedPosition;
	}
}
