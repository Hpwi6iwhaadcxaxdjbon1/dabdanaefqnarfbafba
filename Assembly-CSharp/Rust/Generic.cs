using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x020008B4 RID: 2228
	public static class Generic
	{
		// Token: 0x04002AEE RID: 10990
		private static Scene _batchingScene;

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x0600301C RID: 12316 RVA: 0x00025051 File Offset: 0x00023251
		public static Scene BatchingScene
		{
			get
			{
				if (!Generic._batchingScene.IsValid())
				{
					Generic._batchingScene = SceneManager.CreateScene("Batching");
				}
				return Generic._batchingScene;
			}
		}
	}
}
