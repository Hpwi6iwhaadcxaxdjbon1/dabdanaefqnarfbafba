using System;
using UnityEngine;

namespace Painting
{
	// Token: 0x020007E8 RID: 2024
	[Serializable]
	public class Brush
	{
		// Token: 0x04002823 RID: 10275
		public float spacing;

		// Token: 0x04002824 RID: 10276
		public Vector2 brushSize;

		// Token: 0x04002825 RID: 10277
		public Texture2D texture;

		// Token: 0x04002826 RID: 10278
		public Color color;

		// Token: 0x04002827 RID: 10279
		public bool erase;
	}
}
