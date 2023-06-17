using System;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public interface IDraggable
{
	// Token: 0x0600271D RID: 10013
	object GetDragData();

	// Token: 0x0600271E RID: 10014
	string GetDragType();

	// Token: 0x0600271F RID: 10015
	Sprite GetDragSprite();
}
