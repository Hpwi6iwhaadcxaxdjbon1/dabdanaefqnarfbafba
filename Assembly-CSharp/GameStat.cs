using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000627 RID: 1575
public class GameStat : MonoBehaviour
{
	// Token: 0x04001F42 RID: 8002
	public float refreshTime = 5f;

	// Token: 0x04001F43 RID: 8003
	public Text title;

	// Token: 0x04001F44 RID: 8004
	public Text globalStat;

	// Token: 0x04001F45 RID: 8005
	public Text localStat;

	// Token: 0x04001F46 RID: 8006
	private long globalValue;

	// Token: 0x04001F47 RID: 8007
	private long localValue;

	// Token: 0x04001F48 RID: 8008
	private long oldGlobalValue;

	// Token: 0x04001F49 RID: 8009
	private long oldLocalValue;

	// Token: 0x04001F4A RID: 8010
	private float secondsSinceRefresh;

	// Token: 0x04001F4B RID: 8011
	private float secondsUntilUpdate;

	// Token: 0x04001F4C RID: 8012
	private float secondsUntilChange;

	// Token: 0x04001F4D RID: 8013
	public GameStat.Stat[] stats;

	// Token: 0x04001F4E RID: 8014
	private GameStat.Stat[] unshownStats;

	// Token: 0x04001F4F RID: 8015
	private GameStat.Stat stat;

	// Token: 0x06002330 RID: 9008 RVA: 0x0001BE36 File Offset: 0x0001A036
	private void Awake()
	{
		this.Refresh();
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000BAF58 File Offset: 0x000B9158
	public void Refresh()
	{
		if (this.stats == null)
		{
			return;
		}
		if (Client.Steam == null)
		{
			return;
		}
		if (this.secondsUntilChange <= 0f)
		{
			if (this.unshownStats == null || this.unshownStats.Length == 0)
			{
				this.unshownStats = Enumerable.ToArray<GameStat.Stat>(this.stats);
			}
			this.stat = this.stats[Random.Range(0, this.stats.Length)];
			this.unshownStats = Enumerable.ToArray<GameStat.Stat>(Enumerable.Where<GameStat.Stat>(this.unshownStats, (GameStat.Stat x) => x.statName != this.stat.statName));
			this.secondsUntilChange = 60f;
			this.localValue = 0L;
			this.globalValue = 0L;
		}
		this.title.text = this.stat.statTitle;
		this.oldLocalValue = this.localValue;
		this.oldGlobalValue = this.globalValue;
		this.localValue = (long)Client.Steam.Stats.GetInt(this.stat.statName);
		this.globalValue = Client.Steam.Stats.GetGlobalInt(this.stat.statName);
		this.secondsSinceRefresh = this.refreshTime;
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000BB07C File Offset: 0x000B927C
	public void UpdateText(double delta)
	{
		this.globalStat.text = string.Format("{0:N0}", (double)this.oldGlobalValue + (double)(this.globalValue - this.oldGlobalValue) * delta);
		this.localStat.text = string.Format("{0:N0}", (double)this.oldLocalValue + (double)(this.localValue - this.oldLocalValue) * delta);
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000BB0F0 File Offset: 0x000B92F0
	private void Update()
	{
		if (this.secondsUntilUpdate <= 0f || this.oldGlobalValue == 0L)
		{
			this.UpdateText((double)Mathf.InverseLerp(this.refreshTime, 0f, this.secondsSinceRefresh));
			this.secondsUntilUpdate = Random.Range(0f, 0.5f);
		}
		this.secondsSinceRefresh -= Time.deltaTime;
		this.secondsUntilUpdate -= Time.deltaTime;
		this.secondsUntilChange -= Time.deltaTime;
		if (this.secondsSinceRefresh <= 0f)
		{
			this.Refresh();
			this.secondsSinceRefresh = this.refreshTime;
		}
	}

	// Token: 0x02000628 RID: 1576
	[Serializable]
	public struct Stat
	{
		// Token: 0x04001F50 RID: 8016
		public string statName;

		// Token: 0x04001F51 RID: 8017
		public string statTitle;
	}
}
