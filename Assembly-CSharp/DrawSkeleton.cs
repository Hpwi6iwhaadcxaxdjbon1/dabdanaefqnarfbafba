using System;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class DrawSkeleton : MonoBehaviour
{
	// Token: 0x0600118A RID: 4490 RVA: 0x0000F613 File Offset: 0x0000D813
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawSkeleton.DrawTransform(base.transform);
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x00074B00 File Offset: 0x00072D00
	private static void DrawTransform(Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Gizmos.DrawLine(t.position, t.GetChild(i).position);
			DrawSkeleton.DrawTransform(t.GetChild(i));
		}
	}
}
