using System;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class EnvSync : PointEntity
{
	// Token: 0x04001053 RID: 4179
	private const float syncInterval = 5f;

	// Token: 0x04001054 RID: 4180
	private const float syncIntervalInv = 0.2f;

	// Token: 0x04001055 RID: 4181
	public static float EngineTimeClient;

	// Token: 0x04001056 RID: 4182
	public static float EngineTimeServer;

	// Token: 0x04001057 RID: 4183
	private TimeSpan timeSpan = TimeSpan.Zero;

	// Token: 0x060013DE RID: 5086 RVA: 0x0007C5A4 File Offset: 0x0007A7A4
	protected void Update()
	{
		if (this.timeSpan == TimeSpan.Zero)
		{
			return;
		}
		DateTime dateTime = TOD_Sky.Instance.Cycle.DateTime;
		TimeSpan timeSpan = this.timeSpan;
		if (this.timeSpan.TotalMinutes > -60.0 && this.timeSpan.TotalMinutes < 60.0)
		{
			timeSpan = TimeSpan.FromHours(this.timeSpan.TotalHours * (double)Time.deltaTime * 0.20000000298023224);
		}
		dateTime += timeSpan;
		this.timeSpan -= timeSpan;
		TOD_Sky.Instance.Cycle.DateTime = dateTime;
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x0007C654 File Offset: 0x0007A854
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.environment == null)
		{
			return;
		}
		EnvSync.EngineTimeClient = Time.realtimeSinceStartup;
		EnvSync.EngineTimeServer = info.msg.environment.engineTime;
		if (!TOD_Sky.Instance)
		{
			return;
		}
		DateTime dateTime = DateTime.FromBinary(info.msg.environment.dateTime);
		this.timeSpan = dateTime - TOD_Sky.Instance.Cycle.DateTime;
		if (!SingletonComponent<Climate>.Instance)
		{
			return;
		}
		SingletonComponent<Climate>.Instance.Overrides.Clouds = info.msg.environment.clouds;
		SingletonComponent<Climate>.Instance.Overrides.Fog = info.msg.environment.fog;
		SingletonComponent<Climate>.Instance.Overrides.Wind = info.msg.environment.wind;
		SingletonComponent<Climate>.Instance.Overrides.Rain = info.msg.environment.rain;
	}
}
