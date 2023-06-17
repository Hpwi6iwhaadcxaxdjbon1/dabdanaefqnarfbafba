using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007EF RID: 2031
	public class HowToOpenMap : BaseTip
	{
		// Token: 0x0400282F RID: 10287
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_openmap", "Press [+map] to open the MAP");

		// Token: 0x06002C5A RID: 11354 RVA: 0x00022951 File Offset: 0x00020B51
		public override Translate.Phrase GetPhrase()
		{
			return HowToOpenMap.Phrase;
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06002C5B RID: 11355 RVA: 0x00022958 File Offset: 0x00020B58
		public int MapOpenedTimes
		{
			get
			{
				return Analytics.MapOpened;
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06002C5C RID: 11356 RVA: 0x0002295F File Offset: 0x00020B5F
		public float MapOpenedSecondsAgo
		{
			get
			{
				return Time.realtimeSinceStartup - MapInterface.LastOpened;
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06002C5D RID: 11357 RVA: 0x0002296C File Offset: 0x00020B6C
		public override bool ShouldShow
		{
			get
			{
				return this.MapOpenedTimes <= 3 && this.MapOpenedSecondsAgo >= 120f;
			}
		}
	}
}
