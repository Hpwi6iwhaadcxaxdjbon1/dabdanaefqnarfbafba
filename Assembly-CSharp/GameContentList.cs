using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000260 RID: 608
public class GameContentList : MonoBehaviour
{
	// Token: 0x04000E76 RID: 3702
	public GameContentList.ResourceType resourceType;

	// Token: 0x04000E77 RID: 3703
	public List<Object> foundObjects;

	// Token: 0x02000261 RID: 609
	public enum ResourceType
	{
		// Token: 0x04000E79 RID: 3705
		Audio,
		// Token: 0x04000E7A RID: 3706
		Textures,
		// Token: 0x04000E7B RID: 3707
		Models
	}
}
