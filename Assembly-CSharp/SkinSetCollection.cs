using System;
using UnityEngine;

// Token: 0x020005EB RID: 1515
[CreateAssetMenu(menuName = "Rust/Skin Set Collection")]
public class SkinSetCollection : ScriptableObject
{
	// Token: 0x04001E4C RID: 7756
	public SkinSet[] Skins;

	// Token: 0x0600222C RID: 8748 RVA: 0x0001B1A5 File Offset: 0x000193A5
	public int GetIndex(float MeshNumber)
	{
		return Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)this.Skins.Length), 0, this.Skins.Length - 1);
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x0001B1C7 File Offset: 0x000193C7
	public SkinSet Get(float MeshNumber)
	{
		return this.Skins[this.GetIndex(MeshNumber)];
	}
}
