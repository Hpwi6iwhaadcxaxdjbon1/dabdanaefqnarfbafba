using System;
using ConVar;
using UnityEngine;

// Token: 0x02000629 RID: 1577
public class FPSGraph : Graph
{
	// Token: 0x06002336 RID: 9014 RVA: 0x000BB19C File Offset: 0x000B939C
	public void Refresh()
	{
		base.enabled = (FPS.graph > 0);
		this.Area.width = (float)(this.Resolution = Mathf.Clamp(FPS.graph, 0, Screen.width));
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x0001BE69 File Offset: 0x0001A069
	protected void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x0001BE71 File Offset: 0x0001A071
	protected override float GetValue()
	{
		return 1f / UnityEngine.Time.deltaTime;
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x0001BE7E File Offset: 0x0001A07E
	protected override Color GetColor(float value)
	{
		if (value < 10f)
		{
			return Color.red;
		}
		if (value >= 30f)
		{
			return Color.green;
		}
		return Color.yellow;
	}
}
