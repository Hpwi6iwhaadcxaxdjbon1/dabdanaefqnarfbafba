using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x0200062F RID: 1583
public class ImagePainter : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
	// Token: 0x04001F71 RID: 8049
	public ImagePainter.OnDrawingEvent onDrawing = new ImagePainter.OnDrawingEvent();

	// Token: 0x04001F72 RID: 8050
	public MonoBehaviour redirectRightClick;

	// Token: 0x04001F73 RID: 8051
	[Tooltip("Spacing scale will depend on your texel size, tweak to what's right.")]
	public float spacingScale = 1f;

	// Token: 0x04001F74 RID: 8052
	internal Brush brush;

	// Token: 0x04001F75 RID: 8053
	internal ImagePainter.PointerState[] pointerState = new ImagePainter.PointerState[]
	{
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState()
	};

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06002357 RID: 9047 RVA: 0x0000B1D8 File Offset: 0x000093D8
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000BBA94 File Offset: 0x000B9C94
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == 1)
		{
			return;
		}
		Vector2 position;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, ref position);
		this.DrawAt(position, eventData.button);
		this.pointerState[eventData.button].isDown = true;
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x0001BF75 File Offset: 0x0001A175
	public virtual void OnPointerUp(PointerEventData eventData)
	{
		this.pointerState[eventData.button].isDown = false;
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000BBAE8 File Offset: 0x000B9CE8
	public virtual void OnDrag(PointerEventData eventData)
	{
		if (eventData.button == 1)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnDrag", eventData);
			}
			return;
		}
		Vector2 position;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, ref position);
		this.DrawAt(position, eventData.button);
	}

	// Token: 0x0600235B RID: 9051 RVA: 0x0001BF8A File Offset: 0x0001A18A
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == 1)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnBeginDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x0001BFB4 File Offset: 0x0001A1B4
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button == 1)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnEndDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x0001BFDE File Offset: 0x0001A1DE
	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (eventData.button == 1)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnInitializePotentialDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000BBB44 File Offset: 0x000B9D44
	private void DrawAt(Vector2 position, PointerEventData.InputButton button)
	{
		if (this.brush == null)
		{
			return;
		}
		ImagePainter.PointerState pointerState = this.pointerState[button];
		Vector2 vector = this.rectTransform.Unpivot(position);
		if (pointerState.isDown)
		{
			Vector2 vector2 = pointerState.lastPos - vector;
			Vector2 normalized = vector2.normalized;
			for (float num = 0f; num < vector2.magnitude; num += Mathf.Max(this.brush.spacing, 1f) * Mathf.Max(this.spacingScale, 0.1f))
			{
				this.onDrawing.Invoke(vector + num * normalized, this.brush);
			}
			pointerState.lastPos = vector;
			return;
		}
		this.onDrawing.Invoke(vector, this.brush);
		pointerState.lastPos = vector;
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x0001C008 File Offset: 0x0001A208
	public void UpdateBrush(Brush brush)
	{
		this.brush = brush;
	}

	// Token: 0x02000630 RID: 1584
	[Serializable]
	public class OnDrawingEvent : UnityEvent<Vector2, Brush>
	{
	}

	// Token: 0x02000631 RID: 1585
	internal class PointerState
	{
		// Token: 0x04001F76 RID: 8054
		public Vector2 lastPos;

		// Token: 0x04001F77 RID: 8055
		public bool isDown;
	}
}
