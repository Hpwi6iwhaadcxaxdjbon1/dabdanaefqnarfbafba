using System;
using UnityEngine;

// Token: 0x020003AF RID: 943
public class LevelInfo : SingletonComponent<LevelInfo>
{
	// Token: 0x04001472 RID: 5234
	public string shortName;

	// Token: 0x04001473 RID: 5235
	public string displayName;

	// Token: 0x04001474 RID: 5236
	[TextArea]
	public string description;

	// Token: 0x04001475 RID: 5237
	[Tooltip("A background image to be shown when loading the map")]
	public Texture2D image;

	// Token: 0x04001476 RID: 5238
	[Space(10f)]
	[Tooltip("You should incrememnt this version when you make changes to the map that will invalidate old saves")]
	public int version = 1;
}
