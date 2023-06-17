using System;
using UnityEngine;

// Token: 0x0200063E RID: 1598
public class UIUnderlay : SingletonComponent<UIUnderlay>
{
	// Token: 0x04001FBC RID: 8124
	public GameObject damageDirectional;

	// Token: 0x060023A6 RID: 9126 RVA: 0x000BD188 File Offset: 0x000BB388
	public void DirectionalDamage(Vector3 pos)
	{
		if (SingletonComponent<MainCamera>.Instance == null)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.damageDirectional);
		gameObject.transform.SetParent(base.transform, false);
		Vector3 vector = SingletonComponent<MainCamera>.Instance.transform.worldToLocalMatrix.MultiplyPoint(pos);
		vector.y = 0f;
		vector.Normalize();
		RectTransform rectTransform = base.transform as RectTransform;
		RectTransform rectTransform2 = gameObject.transform as RectTransform;
		rectTransform2.anchoredPosition = this.RectProjectEdge(rectTransform.rect, new Vector2(vector.x, vector.z));
		rectTransform2.localRotation = Quaternion.Euler(0f, 0f, (float)Random.Range(0, 360));
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000BD248 File Offset: 0x000BB448
	private Vector2 RectProjectEdge(Rect r, Vector2 v)
	{
		v = v.normalized * r.size.magnitude * 2f;
		v += r.center;
		v.x = Mathf.Clamp(v.x, r.xMin, r.xMax);
		v.y = Mathf.Clamp(v.y, r.yMin, r.yMax);
		return v;
	}
}
