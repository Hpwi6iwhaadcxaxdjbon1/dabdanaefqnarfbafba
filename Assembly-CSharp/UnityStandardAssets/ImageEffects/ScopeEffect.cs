using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	// Token: 0x0200081A RID: 2074
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Other/Scope Overlay")]
	public class ScopeEffect : PostEffectsBase, IImageEffect
	{
		// Token: 0x040028B7 RID: 10423
		public Material overlayMaterial;

		// Token: 0x06002D1D RID: 11549 RVA: 0x00002D44 File Offset: 0x00000F44
		public override bool CheckResources()
		{
			return true;
		}

		// Token: 0x06002D1E RID: 11550 RVA: 0x000232A8 File Offset: 0x000214A8
		public bool IsActive()
		{
			return base.enabled && this.CheckResources();
		}

		// Token: 0x06002D1F RID: 11551 RVA: 0x000232BA File Offset: 0x000214BA
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.overlayMaterial.SetVector("_Screen", new Vector2((float)Screen.width, (float)Screen.height));
			Graphics.Blit(source, destination, this.overlayMaterial);
		}
	}
}
