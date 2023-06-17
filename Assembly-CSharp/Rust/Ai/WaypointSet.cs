using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008CE RID: 2254
	public class WaypointSet : MonoBehaviour, IServerComponent
	{
		// Token: 0x04002B57 RID: 11095
		[SerializeField]
		private List<WaypointSet.Waypoint> _points = new List<WaypointSet.Waypoint>();

		// Token: 0x04002B58 RID: 11096
		[SerializeField]
		private WaypointSet.NavModes navMode;

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06003062 RID: 12386 RVA: 0x0002540B File Offset: 0x0002360B
		// (set) Token: 0x06003063 RID: 12387 RVA: 0x00025413 File Offset: 0x00023613
		public List<WaypointSet.Waypoint> Points
		{
			get
			{
				return this._points;
			}
			set
			{
				this._points = value;
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06003064 RID: 12388 RVA: 0x0002541C File Offset: 0x0002361C
		public WaypointSet.NavModes NavMode
		{
			get
			{
				return this.navMode;
			}
		}

		// Token: 0x06003065 RID: 12389 RVA: 0x000EC5E0 File Offset: 0x000EA7E0
		private void OnDrawGizmos()
		{
			for (int i = 0; i < this.Points.Count; i++)
			{
				Transform transform = this.Points[i].Transform;
				if (transform != null)
				{
					if (this.Points[i].IsOccupied)
					{
						Gizmos.color = Color.red;
					}
					else
					{
						Gizmos.color = Color.cyan;
					}
					Gizmos.DrawSphere(transform.position, 0.25f);
					Gizmos.color = Color.cyan;
					if (i + 1 < this.Points.Count)
					{
						Gizmos.DrawLine(transform.position, this.Points[i + 1].Transform.position);
					}
					else if (this.NavMode == WaypointSet.NavModes.Loop)
					{
						Gizmos.DrawLine(transform.position, this.Points[0].Transform.position);
					}
					Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
					foreach (Transform transform2 in this.Points[i].LookatPoints)
					{
						Gizmos.DrawSphere(transform2.position, 0.1f);
						Gizmos.DrawLine(transform.position, transform2.position);
					}
				}
			}
		}

		// Token: 0x020008CF RID: 2255
		public enum NavModes
		{
			// Token: 0x04002B5A RID: 11098
			Loop,
			// Token: 0x04002B5B RID: 11099
			PingPong
		}

		// Token: 0x020008D0 RID: 2256
		[Serializable]
		public struct Waypoint
		{
			// Token: 0x04002B5C RID: 11100
			public Transform Transform;

			// Token: 0x04002B5D RID: 11101
			public float WaitTime;

			// Token: 0x04002B5E RID: 11102
			public Transform[] LookatPoints;

			// Token: 0x04002B5F RID: 11103
			[NonSerialized]
			public bool IsOccupied;
		}
	}
}
