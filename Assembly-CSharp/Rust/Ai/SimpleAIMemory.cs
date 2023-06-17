using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000908 RID: 2312
	public class SimpleAIMemory
	{
		// Token: 0x04002C21 RID: 11297
		public List<BaseEntity> Visible = new List<BaseEntity>();

		// Token: 0x04002C22 RID: 11298
		public List<SimpleAIMemory.SeenInfo> All = new List<SimpleAIMemory.SeenInfo>();

		// Token: 0x0600312D RID: 12589 RVA: 0x000ED08C File Offset: 0x000EB28C
		public void Update(BaseEntity ent)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					SimpleAIMemory.SeenInfo seenInfo = this.All[i];
					seenInfo.Position = ent.transform.position;
					seenInfo.Timestamp = Mathf.Max(Time.realtimeSinceStartup, seenInfo.Timestamp);
					this.All[i] = seenInfo;
					return;
				}
			}
			this.All.Add(new SimpleAIMemory.SeenInfo
			{
				Entity = ent,
				Position = ent.transform.position,
				Timestamp = Time.realtimeSinceStartup
			});
			this.Visible.Add(ent);
		}

		// Token: 0x0600312E RID: 12590 RVA: 0x000ED154 File Offset: 0x000EB354
		public void AddDanger(Vector3 position, float amount)
		{
			this.All.Add(new SimpleAIMemory.SeenInfo
			{
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = amount
			});
		}

		// Token: 0x0600312F RID: 12591 RVA: 0x000ED194 File Offset: 0x000EB394
		internal void Forget(float secondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (Time.realtimeSinceStartup - this.All[i].Timestamp > secondsOld)
				{
					if (this.All[i].Entity != null)
					{
						this.Visible.Remove(this.All[i].Entity);
					}
					this.All.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x02000909 RID: 2313
		public struct SeenInfo
		{
			// Token: 0x04002C23 RID: 11299
			public BaseEntity Entity;

			// Token: 0x04002C24 RID: 11300
			public Vector3 Position;

			// Token: 0x04002C25 RID: 11301
			public float Timestamp;

			// Token: 0x04002C26 RID: 11302
			public float Danger;
		}
	}
}
