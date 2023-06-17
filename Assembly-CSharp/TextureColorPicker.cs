using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x0200022E RID: 558
public class TextureColorPicker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
	// Token: 0x04000DCD RID: 3533
	public Texture2D texture;

	// Token: 0x04000DCE RID: 3534
	public TextureColorPicker.onColorSelectedEvent onColorSelected = new TextureColorPicker.onColorSelectedEvent();

	// Token: 0x060010E1 RID: 4321 RVA: 0x0000EC46 File Offset: 0x0000CE46
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		this.OnDrag(eventData);
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x00071D90 File Offset: 0x0006FF90
	public virtual void OnDrag(PointerEventData eventData)
	{
		RectTransform rectTransform = base.transform as RectTransform;
		Vector2 vector = default(Vector2);
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, ref vector))
		{
			vector.x += rectTransform.rect.width * 0.5f;
			vector.y += rectTransform.rect.height * 0.5f;
			vector.x /= rectTransform.rect.width;
			vector.y /= rectTransform.rect.height;
			Color pixel = this.texture.GetPixel((int)(vector.x * (float)this.texture.width), (int)(vector.y * (float)this.texture.height));
			this.onColorSelected.Invoke(pixel);
		}
	}

	// Token: 0x0200022F RID: 559
	[Serializable]
	public class onColorSelectedEvent : UnityEvent<Color>
	{
	}
}
