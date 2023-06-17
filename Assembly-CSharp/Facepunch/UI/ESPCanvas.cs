using System;
using UnityEngine;

namespace Facepunch.UI
{
	// Token: 0x020008A0 RID: 2208
	public class ESPCanvas : MonoBehaviour
	{
		// Token: 0x04002A78 RID: 10872
		[Tooltip("Max amount of elements to show at once")]
		public int MaxElements;

		// Token: 0x04002A79 RID: 10873
		[Tooltip("Amount of times per second we should update the visible panels")]
		public float RefreshRate = 5f;

		// Token: 0x04002A7A RID: 10874
		[Tooltip("This object will be duplicated in place")]
		public ESPPlayerInfo Source;

		// Token: 0x04002A7B RID: 10875
		[Tooltip("Entities this far away won't be overlayed")]
		public float MaxDistance = 64f;

		// Token: 0x04002A7C RID: 10876
		protected ESPPlayerInfo[] Elements;

		// Token: 0x04002A7D RID: 10877
		protected RealTimeSince timeSinceRefreshed;

		// Token: 0x06002FAF RID: 12207 RVA: 0x000EAF3C File Offset: 0x000E913C
		public void Awake()
		{
			this.Elements = ComponentExtensions.Duplicate<ESPPlayerInfo>(this.Source, this.MaxElements, true);
			for (int i = 0; i < this.Elements.Length; i++)
			{
				this.Elements[i].Clear();
			}
		}

		// Token: 0x06002FB0 RID: 12208 RVA: 0x000EAF84 File Offset: 0x000E9184
		public void Update()
		{
			if (this.timeSinceRefreshed < 1f / this.RefreshRate)
			{
				return;
			}
			this.timeSinceRefreshed = 0f;
			for (int i = 0; i < this.Elements.Length; i++)
			{
				BasePlayer entity = this.Elements[i].Entity;
				if (!this.ShouldShow(entity))
				{
					this.Elements[i].Clear();
				}
			}
			BasePlayer[] buffer = BasePlayer.VisiblePlayerList.Buffer;
			int count = BasePlayer.VisiblePlayerList.Count;
			for (int j = 0; j < count; j++)
			{
				BasePlayer basePlayer = buffer[j];
				if (this.ShouldShow(basePlayer) && !this.StartWatching(basePlayer))
				{
					break;
				}
			}
		}

		// Token: 0x06002FB1 RID: 12209 RVA: 0x000EB034 File Offset: 0x000E9234
		private bool StartWatching(BasePlayer entity)
		{
			ESPPlayerInfo[] elements = this.Elements;
			for (int i = 0; i < elements.Length; i++)
			{
				if (elements[i].Entity == entity)
				{
					return true;
				}
			}
			for (int j = 0; j < this.Elements.Length; j++)
			{
				if (this.Elements[j].Entity == null)
				{
					this.Elements[j].Init(entity);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002FB2 RID: 12210 RVA: 0x000EB0A4 File Offset: 0x000E92A4
		public bool ShouldShow(BasePlayer ent)
		{
			return !(LocalPlayer.Entity == null) && !(ent == null) && !(ent == LocalPlayer.Entity) && !ent.IsDead() && !ent.IsNpc && ((RelationshipManager.TeamsEnabled() && ent.currentTeam == LocalPlayer.Entity.currentTeam) || ent.Distance(MainCamera.position) <= 10f);
		}
	}
}
