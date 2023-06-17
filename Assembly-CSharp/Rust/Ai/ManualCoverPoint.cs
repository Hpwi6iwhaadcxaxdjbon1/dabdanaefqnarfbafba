using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008C6 RID: 2246
	public class ManualCoverPoint : FacepunchBehaviour
	{
		// Token: 0x04002B24 RID: 11044
		public bool IsDynamic;

		// Token: 0x04002B25 RID: 11045
		public float Score = 2f;

		// Token: 0x04002B26 RID: 11046
		public CoverPointVolume Volume;

		// Token: 0x04002B27 RID: 11047
		public Vector3 Normal;

		// Token: 0x04002B28 RID: 11048
		public CoverPoint.CoverType NormalCoverType;

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06003051 RID: 12369 RVA: 0x000079E3 File Offset: 0x00005BE3
		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06003052 RID: 12370 RVA: 0x000252C5 File Offset: 0x000234C5
		public float DirectionMagnitude
		{
			get
			{
				if (this.Volume != null)
				{
					return this.Volume.CoverPointRayLength;
				}
				return 1f;
			}
		}

		// Token: 0x06003053 RID: 12371 RVA: 0x000252E6 File Offset: 0x000234E6
		private void Awake()
		{
			if (base.transform.parent != null)
			{
				this.Volume = base.transform.parent.GetComponent<CoverPointVolume>();
			}
		}

		// Token: 0x06003054 RID: 12372 RVA: 0x000EC468 File Offset: 0x000EA668
		public CoverPoint ToCoverPoint(CoverPointVolume volume)
		{
			this.Volume = volume;
			if (this.IsDynamic)
			{
				CoverPoint coverPoint = new CoverPoint(this.Volume, this.Score);
				coverPoint.IsDynamic = true;
				coverPoint.SourceTransform = base.transform;
				coverPoint.NormalCoverType = this.NormalCoverType;
				Transform transform = base.transform;
				coverPoint.Position = ((transform != null) ? transform.position : Vector3.zero);
				return coverPoint;
			}
			Vector3 normalized = (base.transform.rotation * this.Normal).normalized;
			return new CoverPoint(this.Volume, this.Score)
			{
				IsDynamic = false,
				Position = base.transform.position,
				Normal = normalized,
				NormalCoverType = this.NormalCoverType
			};
		}
	}
}
