using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x020008B5 RID: 2229
	public static class Client
	{
		// Token: 0x04002AEF RID: 10991
		public const float UseDistance = 2f;

		// Token: 0x04002AF0 RID: 10992
		private static Scene _entityScene;

		// Token: 0x04002AF1 RID: 10993
		private static Scene _effectScene;

		// Token: 0x04002AF2 RID: 10994
		private static Scene _decorScene;

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x0600301D RID: 12317 RVA: 0x00025073 File Offset: 0x00023273
		public static Scene EntityScene
		{
			get
			{
				if (!Client._entityScene.IsValid())
				{
					Client._entityScene = SceneManager.CreateScene("Client Entities");
				}
				return Client._entityScene;
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x0600301E RID: 12318 RVA: 0x00025095 File Offset: 0x00023295
		public static Scene EffectScene
		{
			get
			{
				if (!Client._effectScene.IsValid())
				{
					Client._effectScene = SceneManager.CreateScene("Effects");
				}
				return Client._effectScene;
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x0600301F RID: 12319 RVA: 0x000250B7 File Offset: 0x000232B7
		public static Scene DecorScene
		{
			get
			{
				if (!Client._decorScene.IsValid())
				{
					Client._decorScene = SceneManager.CreateScene("Decor");
				}
				return Client._decorScene;
			}
		}
	}
}
