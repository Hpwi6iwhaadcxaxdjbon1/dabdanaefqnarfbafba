using System;
using ConVar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006EA RID: 1770
public class DragMe : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x04002306 RID: 8966
	public static DragMe dragging;

	// Token: 0x04002307 RID: 8967
	public static GameObject dragIcon;

	// Token: 0x04002308 RID: 8968
	public static object data;

	// Token: 0x04002309 RID: 8969
	[NonSerialized]
	public string dragType = "generic";

	// Token: 0x06002718 RID: 10008 RVA: 0x000CBB3C File Offset: 0x000C9D3C
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		this.OnEndDrag(eventData);
		IDraggable component = base.GetComponent<IDraggable>();
		if (component == null)
		{
			return;
		}
		DragMe.data = component.GetDragData();
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] OnBeginDrag - " + DragMe.data);
		}
		if (DragMe.data == null)
		{
			return;
		}
		DragMe.dragging = this;
		this.dragType = component.GetDragType();
		Canvas dragOverlayCanvas = UIRootScaled.DragOverlayCanvas;
		if (dragOverlayCanvas)
		{
			DragMe.dragIcon = new GameObject("Drag Icon");
			DragMe.dragIcon.transform.SetParent(dragOverlayCanvas.transform, false);
			DragMe.dragIcon.transform.SetAsLastSibling();
			CanvasGroup canvasGroup = DragMe.dragIcon.AddComponent<CanvasGroup>();
			canvasGroup.interactable = false;
			canvasGroup.alpha = 0.8f;
			canvasGroup.blocksRaycasts = false;
			Image image = DragMe.dragIcon.AddComponent<Image>();
			image.rectTransform.sizeDelta = new Vector2(64f, 64f);
			image.sprite = component.GetDragSprite();
		}
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] OnBeginDrag - dragging " + base.gameObject);
		}
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x000CBC50 File Offset: 0x000C9E50
	public virtual void OnDrag(PointerEventData eventData)
	{
		if (DragMe.dragIcon == null)
		{
			return;
		}
		Vector3 position = default(Vector3);
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(base.transform as RectTransform, eventData.position, eventData.pressEventCamera, ref position))
		{
			DragMe.dragIcon.transform.position = position;
		}
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x000CBCA4 File Offset: 0x000C9EA4
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] OnEndDrag " + base.gameObject);
		}
		DragMe.dragging = null;
		DragMe.data = null;
		if (DragMe.dragIcon)
		{
			GameManager.Destroy(DragMe.dragIcon, 0f);
		}
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x000CBCF4 File Offset: 0x000C9EF4
	internal DropMe FindTarget(GameObject obj)
	{
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] FindTarget GameObject = " + obj);
		}
		if (obj == null)
		{
			if (ConVar.Client.debugdragdrop)
			{
				Debug.Log("[dragdrop] FindTarget = null");
			}
			return null;
		}
		DropMe dropMe = obj.GetComponent<DropMe>();
		if (dropMe && dropMe.Accepts(this))
		{
			if (ConVar.Client.debugdragdrop)
			{
				Debug.Log("[dragdrop] FindTarget = " + dropMe.gameObject);
			}
			return dropMe;
		}
		dropMe = obj.GetComponentInParent<DropMe>();
		if (dropMe && dropMe.Accepts(this))
		{
			if (ConVar.Client.debugdragdrop)
			{
				Debug.Log("[dragdrop] FindTarget = " + dropMe.gameObject);
			}
			return dropMe;
		}
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] FindTarget = null ");
		}
		return null;
	}
}
