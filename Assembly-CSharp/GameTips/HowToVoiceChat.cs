using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007F6 RID: 2038
	public class HowToVoiceChat : BaseTip
	{
		// Token: 0x0400283A RID: 10298
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_voice", "Press [+voice] to voice chat to other players");

		// Token: 0x06002C85 RID: 11397 RVA: 0x00022BDC File Offset: 0x00020DDC
		public override Translate.Phrase GetPhrase()
		{
			return HowToVoiceChat.Phrase;
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06002C86 RID: 11398 RVA: 0x00022BE3 File Offset: 0x00020DE3
		public float SecondsSpeaking
		{
			get
			{
				return Analytics.SecondsSpeaking;
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06002C87 RID: 11399 RVA: 0x000E1668 File Offset: 0x000DF868
		public override bool ShouldShow
		{
			get
			{
				if (this.SecondsSpeaking >= 2f)
				{
					return false;
				}
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				bool result = false;
				List<BasePlayer> list = Pool.GetList<BasePlayer>();
				Vis.Entities<BasePlayer>(LocalPlayer.Entity.transform.position, 15f, list, 16384, 2);
				foreach (BasePlayer basePlayer in list)
				{
					if (!(basePlayer == LocalPlayer.Entity) && !basePlayer.IsSleeping() && !basePlayer.IsDead() && !basePlayer.IsWounded() && !basePlayer.IsDucked() && basePlayer.voiceSpeaker.IsSpeaking() && (double)Vector3.Dot(LocalPlayer.Entity.eyes.HeadForward(), (basePlayer.transform.position - LocalPlayer.Entity.transform.position).normalized) >= 0.6)
					{
						result = true;
						break;
					}
				}
				Pool.FreeList<BasePlayer>(ref list);
				return result;
			}
		}
	}
}
