using System;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200067C RID: 1660
public class LifeInfographicStat : MonoBehaviour
{
	// Token: 0x040020FA RID: 8442
	public LifeInfographicStat.DataType dataSource;

	// Token: 0x0600250A RID: 9482 RVA: 0x000C37D0 File Offset: 0x000C19D0
	private string GetText()
	{
		LifeInfographic componentInParent = base.GetComponentInParent<LifeInfographic>();
		if (componentInParent && componentInParent.life != null)
		{
			PlayerLifeStory life = componentInParent.life;
			switch (this.dataSource)
			{
			case LifeInfographicStat.DataType.AliveTime_Short:
				return NumberExtensions.FormatSeconds((long)life.secondsAlive);
			case LifeInfographicStat.DataType.SleepingTime_Short:
				return NumberExtensions.FormatSeconds((long)life.secondsSleeping);
			case LifeInfographicStat.DataType.KillerName:
				if (Global.streamermode)
				{
					return RandomUsernames.Get(life.deathInfo.attackerSteamID);
				}
				if (life.deathInfo == null)
				{
					return "";
				}
				return life.deathInfo.attackerName;
			case LifeInfographicStat.DataType.KillerWeapon:
				if (life.deathInfo == null)
				{
					return "";
				}
				return life.deathInfo.inflictorName;
			case LifeInfographicStat.DataType.AliveTime_Long:
				return NumberExtensions.FormatSecondsLong((long)life.secondsAlive);
			}
		}
		return this.dataSource.ToString();
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x0001D0E9 File Offset: 0x0001B2E9
	private void OnEnable()
	{
		this.InfographicUpdated();
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x000C38AC File Offset: 0x000C1AAC
	private void InfographicUpdated()
	{
		Text component = base.GetComponent<Text>();
		if (component)
		{
			component.text = this.GetText();
		}
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x0001D0E9 File Offset: 0x0001B2E9
	private void OnValidate()
	{
		this.InfographicUpdated();
	}

	// Token: 0x0200067D RID: 1661
	public enum DataType
	{
		// Token: 0x040020FC RID: 8444
		None,
		// Token: 0x040020FD RID: 8445
		AliveTime_Short,
		// Token: 0x040020FE RID: 8446
		SleepingTime_Short,
		// Token: 0x040020FF RID: 8447
		KillerName,
		// Token: 0x04002100 RID: 8448
		KillerWeapon,
		// Token: 0x04002101 RID: 8449
		AliveTime_Long
	}
}
