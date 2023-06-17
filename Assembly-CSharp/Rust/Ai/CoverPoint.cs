using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008C0 RID: 2240
	public class CoverPoint
	{
		// Token: 0x04002B06 RID: 11014
		public CoverPoint.CoverType NormalCoverType;

		// Token: 0x04002B07 RID: 11015
		public bool IsDynamic;

		// Token: 0x04002B08 RID: 11016
		public Transform SourceTransform;

		// Token: 0x04002B09 RID: 11017
		private Vector3 _staticPosition;

		// Token: 0x04002B0A RID: 11018
		private Vector3 _staticNormal;

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06003031 RID: 12337 RVA: 0x00025148 File Offset: 0x00023348
		// (set) Token: 0x06003032 RID: 12338 RVA: 0x00025150 File Offset: 0x00023350
		public CoverPointVolume Volume { get; private set; }

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06003033 RID: 12339 RVA: 0x00025159 File Offset: 0x00023359
		// (set) Token: 0x06003034 RID: 12340 RVA: 0x00025183 File Offset: 0x00023383
		public Vector3 Position
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.position;
				}
				return this._staticPosition;
			}
			set
			{
				this._staticPosition = value;
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06003035 RID: 12341 RVA: 0x0002518C File Offset: 0x0002338C
		// (set) Token: 0x06003036 RID: 12342 RVA: 0x000251B6 File Offset: 0x000233B6
		public Vector3 Normal
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.forward;
				}
				return this._staticNormal;
			}
			set
			{
				this._staticNormal = value;
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06003037 RID: 12343 RVA: 0x000251BF File Offset: 0x000233BF
		// (set) Token: 0x06003038 RID: 12344 RVA: 0x000251C7 File Offset: 0x000233C7
		public BaseEntity ReservedFor { get; set; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06003039 RID: 12345 RVA: 0x000251D0 File Offset: 0x000233D0
		public bool IsReserved
		{
			get
			{
				return this.ReservedFor != null;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x0600303A RID: 12346 RVA: 0x000251DE File Offset: 0x000233DE
		// (set) Token: 0x0600303B RID: 12347 RVA: 0x000251E6 File Offset: 0x000233E6
		public bool IsCompromised { get; set; }

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x0600303C RID: 12348 RVA: 0x000251EF File Offset: 0x000233EF
		// (set) Token: 0x0600303D RID: 12349 RVA: 0x000251F7 File Offset: 0x000233F7
		public float Score { get; set; }

		// Token: 0x0600303E RID: 12350 RVA: 0x00025200 File Offset: 0x00023400
		public bool IsValidFor(BaseEntity entity)
		{
			return !this.IsCompromised && (this.ReservedFor == null || this.ReservedFor == entity);
		}

		// Token: 0x0600303F RID: 12351 RVA: 0x00025228 File Offset: 0x00023428
		public CoverPoint(CoverPointVolume volume, float score)
		{
			this.Volume = volume;
			this.Score = score;
		}

		// Token: 0x06003040 RID: 12352 RVA: 0x0002523E File Offset: 0x0002343E
		public void CoverIsCompromised(float cooldown)
		{
			if (this.IsCompromised)
			{
				return;
			}
			if (this.Volume != null)
			{
				this.Volume.StartCoroutine(this.StartCooldown(cooldown));
			}
		}

		// Token: 0x06003041 RID: 12353 RVA: 0x0002526A File Offset: 0x0002346A
		private IEnumerator StartCooldown(float cooldown)
		{
			this.IsCompromised = true;
			yield return CoroutineEx.waitForSeconds(cooldown);
			this.IsCompromised = false;
			yield break;
		}

		// Token: 0x06003042 RID: 12354 RVA: 0x000EC1B8 File Offset: 0x000EA3B8
		public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
		{
			Vector3 normalized = (this.Position - point).normalized;
			return Vector3.Dot(this.Normal, normalized) < arcThreshold;
		}

		// Token: 0x020008C1 RID: 2241
		public enum CoverType
		{
			// Token: 0x04002B0F RID: 11023
			Full,
			// Token: 0x04002B10 RID: 11024
			Partial,
			// Token: 0x04002B11 RID: 11025
			None
		}
	}
}
