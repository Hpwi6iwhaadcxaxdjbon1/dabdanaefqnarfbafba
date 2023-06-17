using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200062B RID: 1579
public class HostileNote : MonoBehaviour, IClientComponent
{
	// Token: 0x04001F5C RID: 8028
	public CanvasGroup warnGroup;

	// Token: 0x04001F5D RID: 8029
	public CanvasGroup group;

	// Token: 0x04001F5E RID: 8030
	public CanvasGroup timerGroup;

	// Token: 0x04001F5F RID: 8031
	public Text timerText;

	// Token: 0x04001F60 RID: 8032
	public static float unhostileTime;

	// Token: 0x04001F61 RID: 8033
	public static float weaponDrawnDuration;

	// Token: 0x04001F62 RID: 8034
	public Color warnColor;

	// Token: 0x04001F63 RID: 8035
	public Color hostileColor;

	// Token: 0x06002341 RID: 9025 RVA: 0x000BB4B8 File Offset: 0x000B96B8
	public void Update()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (!entity)
		{
			return;
		}
		float num = (Time.realtimeSinceStartup < HostileNote.unhostileTime && entity.InSafeZone()) ? 1f : 0f;
		if (this.group.alpha != num)
		{
			this.group.alpha = Mathf.MoveTowards(this.group.alpha, num, Time.deltaTime * 4f);
		}
		if (this.group.alpha > 0f)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)(HostileNote.unhostileTime - Time.realtimeSinceStartup));
			this.timerText.text = timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2");
		}
		float num2 = (entity.InSafeZone() && num == 0f && HostileNote.weaponDrawnDuration > 0f) ? 1f : 0f;
		if (this.warnGroup.alpha != num2)
		{
			this.warnGroup.alpha = Mathf.MoveTowards(this.warnGroup.alpha, num2, Time.deltaTime * 4f);
		}
		if (this.warnGroup.alpha > 0f)
		{
			TimeSpan timeSpan2 = TimeSpan.FromSeconds((double)(5f - HostileNote.weaponDrawnDuration));
			this.timerText.text = timeSpan2.Minutes.ToString("D2") + ":" + timeSpan2.Seconds.ToString("D2");
		}
		float num3 = Mathf.Max(this.warnGroup.alpha, this.group.alpha);
		if (this.timerGroup.alpha != num3)
		{
			this.timerGroup.alpha = num3;
		}
		if (this.timerGroup.alpha > 0f)
		{
			this.timerText.color = ((this.warnGroup.alpha > 0f) ? this.warnColor : this.hostileColor);
		}
	}
}
