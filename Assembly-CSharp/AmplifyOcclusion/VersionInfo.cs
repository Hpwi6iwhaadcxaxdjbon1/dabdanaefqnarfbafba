using System;
using UnityEngine;

namespace AmplifyOcclusion
{
	// Token: 0x020007E7 RID: 2023
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x0400281C RID: 10268
		public const byte Major = 2;

		// Token: 0x0400281D RID: 10269
		public const byte Minor = 0;

		// Token: 0x0400281E RID: 10270
		public const byte Release = 0;

		// Token: 0x0400281F RID: 10271
		private static string StageSuffix = "_dev002";

		// Token: 0x04002820 RID: 10272
		[SerializeField]
		private int m_major;

		// Token: 0x04002821 RID: 10273
		[SerializeField]
		private int m_minor;

		// Token: 0x04002822 RID: 10274
		[SerializeField]
		private int m_release;

		// Token: 0x06002C2D RID: 11309 RVA: 0x000226A4 File Offset: 0x000208A4
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 2, 0, 0) + VersionInfo.StageSuffix;
		}

		// Token: 0x06002C2E RID: 11310 RVA: 0x000226CC File Offset: 0x000208CC
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix;
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06002C2F RID: 11311 RVA: 0x00022703 File Offset: 0x00020903
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x06002C30 RID: 11312 RVA: 0x0002271F File Offset: 0x0002091F
		private VersionInfo()
		{
			this.m_major = 2;
			this.m_minor = 0;
			this.m_release = 0;
		}

		// Token: 0x06002C31 RID: 11313 RVA: 0x0002273C File Offset: 0x0002093C
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x06002C32 RID: 11314 RVA: 0x00022759 File Offset: 0x00020959
		public static VersionInfo Current()
		{
			return new VersionInfo(2, 0, 0);
		}

		// Token: 0x06002C33 RID: 11315 RVA: 0x00022763 File Offset: 0x00020963
		public static bool Matches(VersionInfo version)
		{
			return 2 == version.m_major && version.m_minor == 0 && version.m_release == 0;
		}
	}
}
