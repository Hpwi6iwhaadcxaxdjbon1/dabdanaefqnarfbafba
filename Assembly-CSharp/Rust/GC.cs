using System;
using ConVar;
using UnityEngine;
using UnityEngine.Scripting;

namespace Rust
{
	// Token: 0x020008AB RID: 2219
	public class GC : MonoBehaviour, IClientComponent
	{
		// Token: 0x04002AC4 RID: 10948
		private static float gcTime;

		// Token: 0x04002AC5 RID: 10949
		private static GarbageCollector.Mode gcMode;

		// Token: 0x04002AC6 RID: 10950
		private int heapSize;

		// Token: 0x04002AC7 RID: 10951
		private int heapBaseline;

		// Token: 0x04002AC8 RID: 10952
		private int collectionCount;

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06002FFF RID: 12287 RVA: 0x00024F02 File Offset: 0x00023102
		public static bool Enabled
		{
			get
			{
				return GC.gcMode == GarbageCollector.Mode.Enabled;
			}
		}

		// Token: 0x06003000 RID: 12288 RVA: 0x00024F0C File Offset: 0x0002310C
		public static void Collect()
		{
			GC.SetMode(GarbageCollector.Mode.Enabled);
			GC.Collect();
			GC.SetMode(GC.gcMode);
		}

		// Token: 0x06003001 RID: 12289 RVA: 0x00024F23 File Offset: 0x00023123
		public static void Pause(float time)
		{
			GC.gcTime = Mathf.Max(GC.gcTime, UnityEngine.Time.realtimeSinceStartup + time);
		}

		// Token: 0x06003002 RID: 12290 RVA: 0x00024F3B File Offset: 0x0002313B
		public static void Reset(float time)
		{
			GC.gcTime = Mathf.Min(GC.gcTime, UnityEngine.Time.realtimeSinceStartup + time);
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x00024F53 File Offset: 0x00023153
		private static void SetMode(GarbageCollector.Mode mode)
		{
			if (GarbageCollector.GCMode != mode)
			{
				GarbageCollector.GCMode = mode;
			}
		}

		// Token: 0x06003004 RID: 12292 RVA: 0x00024F63 File Offset: 0x00023163
		private static int GetTotalMemory()
		{
			return (int)(GC.GetTotalMemory(false) / 1048576L);
		}

		// Token: 0x06003005 RID: 12293 RVA: 0x00024F73 File Offset: 0x00023173
		private static int CollectionCount()
		{
			return GC.CollectionCount(0);
		}

		// Token: 0x06003006 RID: 12294 RVA: 0x000EBDB4 File Offset: 0x000E9FB4
		private void UpdateState()
		{
			this.heapSize = (this.heapBaseline = GC.GetTotalMemory());
			this.collectionCount = GC.CollectionCount();
		}

		// Token: 0x06003007 RID: 12295 RVA: 0x00024F7B File Offset: 0x0002317B
		protected void OnEnable()
		{
			this.UpdateState();
		}

		// Token: 0x06003008 RID: 12296 RVA: 0x00024F83 File Offset: 0x00023183
		protected void OnDisable()
		{
			GC.SetMode(GarbageCollector.Mode.Enabled);
		}

		// Token: 0x06003009 RID: 12297 RVA: 0x000EBDE0 File Offset: 0x000E9FE0
		protected void LateUpdate()
		{
			int totalMemory = GC.GetTotalMemory();
			int num = GC.CollectionCount();
			if (num != this.collectionCount)
			{
				this.heapBaseline = totalMemory;
			}
			this.heapSize = totalMemory;
			this.collectionCount = num;
			if (UnityEngine.Time.realtimeSinceStartup > GC.gcTime)
			{
				GC.gcMode = GarbageCollector.Mode.Enabled;
			}
			else
			{
				GC.gcMode = GarbageCollector.Mode.Disabled;
			}
			GC.SetMode(GC.gcMode);
			if (GC.Enabled)
			{
				if (this.heapSize - this.heapBaseline > GC.buffer / 2)
				{
					GC.Collect();
					this.UpdateState();
					return;
				}
			}
			else if (this.heapSize - this.heapBaseline > GC.buffer)
			{
				GC.Collect();
				this.UpdateState();
			}
		}
	}
}
