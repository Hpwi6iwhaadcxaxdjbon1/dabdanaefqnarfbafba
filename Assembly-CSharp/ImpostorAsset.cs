using System;
using UnityEngine;

// Token: 0x0200059C RID: 1436
public class ImpostorAsset : ScriptableObject
{
	// Token: 0x04001CAE RID: 7342
	public ImpostorAsset.TextureEntry[] textures;

	// Token: 0x04001CAF RID: 7343
	public Vector2 size;

	// Token: 0x04001CB0 RID: 7344
	public Vector2 pivot;

	// Token: 0x04001CB1 RID: 7345
	public Mesh mesh;

	// Token: 0x060020D0 RID: 8400 RVA: 0x000B1C2C File Offset: 0x000AFE2C
	public Texture2D FindTexture(string name)
	{
		foreach (ImpostorAsset.TextureEntry textureEntry in this.textures)
		{
			if (textureEntry.name == name)
			{
				return textureEntry.texture;
			}
		}
		return null;
	}

	// Token: 0x0200059D RID: 1437
	[Serializable]
	public class TextureEntry
	{
		// Token: 0x04001CB2 RID: 7346
		public string name;

		// Token: 0x04001CB3 RID: 7347
		public Texture2D texture;

		// Token: 0x060020D2 RID: 8402 RVA: 0x0001A0C3 File Offset: 0x000182C3
		public TextureEntry(string name, Texture2D texture)
		{
			this.name = name;
			this.texture = texture;
		}
	}
}
