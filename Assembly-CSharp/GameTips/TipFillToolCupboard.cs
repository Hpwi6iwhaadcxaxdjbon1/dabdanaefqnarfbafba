using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x020007FB RID: 2043
	public class TipFillToolCupboard : BaseTip
	{
		// Token: 0x04002843 RID: 10307
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_fillcupboard", "Add more resources to your Tool Cupboard to prevent your base from decaying");

		// Token: 0x06002CA8 RID: 11432 RVA: 0x00022DA2 File Offset: 0x00020FA2
		public override Translate.Phrase GetPhrase()
		{
			return TipFillToolCupboard.Phrase;
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06002CA9 RID: 11433 RVA: 0x00022DA9 File Offset: 0x00020FA9
		public int CupboardOpenedTimes
		{
			get
			{
				return Analytics.CupboardOpened;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06002CAA RID: 11434 RVA: 0x000E18EC File Offset: 0x000DFAEC
		public override bool ShouldShow
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				if (this.CupboardOpenedTimes > 10)
				{
					return false;
				}
				BuildingPrivlidge buildingPrivilege = LocalPlayer.Entity.GetBuildingPrivilege();
				return !(buildingPrivilege == null) && (!buildingPrivilege.AnyAuthed() || buildingPrivilege.IsAuthed(LocalPlayer.Entity)) && buildingPrivilege.GetProtectedMinutes(false) <= 60f;
			}
		}
	}
}
