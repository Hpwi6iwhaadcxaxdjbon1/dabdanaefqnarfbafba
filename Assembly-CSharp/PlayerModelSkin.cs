using System;
using UnityEngine;

// Token: 0x02000218 RID: 536
public class PlayerModelSkin : MonoBehaviour
{
	// Token: 0x06001066 RID: 4198 RVA: 0x0000E7A0 File Offset: 0x0000C9A0
	public void Setup(SkinSetCollection skin, float materialNum, float meshNum)
	{
		SkinSet skinSet = skin.Get(meshNum);
		if (skinSet == null)
		{
			Debug.LogError("Skin.Get returned a NULL skin");
		}
		skinSet.Process(base.gameObject, materialNum);
	}
}
