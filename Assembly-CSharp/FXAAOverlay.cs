using System;
using ConVar;

// Token: 0x020001D2 RID: 466
public class FXAAOverlay : ImageEffectLayer
{
	// Token: 0x04000C0D RID: 3085
	public FXAA fxaa;

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0000D708 File Offset: 0x0000B908
	public void Awake()
	{
		this.sortOrder = -2;
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0000D6F1 File Offset: 0x0000B8F1
	public override void Start()
	{
		base.Start();
		base.layerEnabled = false;
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x0000D712 File Offset: 0x0000B912
	protected void Update()
	{
		base.layerEnabled = (Effects.antialiasing == 1);
	}
}
