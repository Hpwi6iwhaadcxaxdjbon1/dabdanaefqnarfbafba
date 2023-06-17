using System;
using ConVar;

// Token: 0x020001D6 RID: 470
public class SharpenAndVignetteOverlay : ImageEffectLayer
{
	// Token: 0x04000C1F RID: 3103
	public CC_SharpenAndVignette sharpenAndVignette;

	// Token: 0x06000F05 RID: 3845 RVA: 0x0000D749 File Offset: 0x0000B949
	public void Awake()
	{
		this.sortOrder = -1;
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0000D6F1 File Offset: 0x0000B8F1
	public override void Start()
	{
		base.Start();
		base.layerEnabled = false;
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x0000D752 File Offset: 0x0000B952
	protected void Update()
	{
		base.layerEnabled = (Effects.sharpen || Effects.vignet);
		this.sharpenAndVignette.applySharpen = Effects.sharpen;
		this.sharpenAndVignette.applyVignette = Effects.vignet;
	}
}
