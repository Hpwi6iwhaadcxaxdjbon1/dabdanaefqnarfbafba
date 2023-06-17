using System;
using Network;
using UnityEngine;

namespace Rust
{
	// Token: 0x020008AF RID: 2223
	internal static class GameInfo
	{
		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06003018 RID: 12312 RVA: 0x00024FE9 File Offset: 0x000231E9
		internal static bool IsOfficialServer
		{
			get
			{
				return Application.isEditor || Net.cl.IsOfficialServer;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06003019 RID: 12313 RVA: 0x00024FFE File Offset: 0x000231FE
		internal static bool HasAchievements
		{
			get
			{
				return GameInfo.IsOfficialServer;
			}
		}
	}
}
