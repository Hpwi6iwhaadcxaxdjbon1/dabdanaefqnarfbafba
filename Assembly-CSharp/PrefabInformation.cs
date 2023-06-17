using System;
using UnityEngine;

// Token: 0x020003DE RID: 990
public class PrefabInformation : PrefabAttribute
{
	// Token: 0x04001541 RID: 5441
	public ItemDefinition associatedItemDefinition;

	// Token: 0x04001542 RID: 5442
	public Translate.Phrase title;

	// Token: 0x04001543 RID: 5443
	public Translate.Phrase description;

	// Token: 0x04001544 RID: 5444
	public Sprite sprite;

	// Token: 0x060018D9 RID: 6361 RVA: 0x00014C15 File Offset: 0x00012E15
	protected override Type GetIndexedType()
	{
		return typeof(PrefabInformation);
	}
}
