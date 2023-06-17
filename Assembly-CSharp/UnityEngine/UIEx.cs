using System;
using UnityEngine.UI;

namespace UnityEngine
{
	// Token: 0x0200083B RID: 2107
	public static class UIEx
	{
		// Token: 0x06002DAC RID: 11692 RVA: 0x000E56A8 File Offset: 0x000E38A8
		public static Vector2 Unpivot(this RectTransform rect, Vector2 localPos)
		{
			localPos.x += rect.pivot.x * rect.rect.width;
			localPos.y += rect.pivot.y * rect.rect.height;
			return localPos;
		}

		// Token: 0x06002DAD RID: 11693 RVA: 0x000E5700 File Offset: 0x000E3900
		public static void CenterOnPosition(this ScrollRect scrollrect, Vector2 pos)
		{
			RectTransform rectTransform = scrollrect.transform as RectTransform;
			Vector2 vector = new Vector2(scrollrect.content.localScale.x, scrollrect.content.localScale.y);
			pos.x *= vector.x;
			pos.y *= vector.y;
			Vector2 vector2 = new Vector2(scrollrect.content.rect.width * vector.x - rectTransform.rect.width, scrollrect.content.rect.height * vector.y - rectTransform.rect.height);
			pos.x = pos.x / vector2.x + scrollrect.content.pivot.x;
			pos.y = pos.y / vector2.y + scrollrect.content.pivot.y;
			if (scrollrect.movementType != null)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			scrollrect.normalizedPosition = pos;
		}
	}
}
