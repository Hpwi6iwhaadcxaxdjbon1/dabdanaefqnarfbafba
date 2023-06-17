using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x020008C4 RID: 2244
	public class CoverPointVolume : MonoBehaviour, IServerComponent
	{
		// Token: 0x04002B16 RID: 11030
		public float DefaultCoverPointScore = 1f;

		// Token: 0x04002B17 RID: 11031
		public float CoverPointRayLength = 1f;

		// Token: 0x04002B18 RID: 11032
		public LayerMask CoverLayerMask;

		// Token: 0x04002B19 RID: 11033
		public Transform BlockerGroup;

		// Token: 0x04002B1A RID: 11034
		public Transform ManualCoverPointGroup;

		// Token: 0x04002B1B RID: 11035
		[ServerVar(Help = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
		public static float cover_point_sample_step_size = 6f;

		// Token: 0x04002B1C RID: 11036
		[ServerVar(Help = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
		public static float cover_point_sample_step_height = 2f;

		// Token: 0x04002B1D RID: 11037
		public readonly List<CoverPoint> CoverPoints = new List<CoverPoint>();

		// Token: 0x04002B1E RID: 11038
		private readonly List<CoverPointBlockerVolume> _coverPointBlockers = new List<CoverPointBlockerVolume>();

		// Token: 0x04002B1F RID: 11039
		private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		// Token: 0x0600304A RID: 12362 RVA: 0x00025297 File Offset: 0x00023497
		[ContextMenu("Clear Cover Points")]
		private void ClearCoverPoints()
		{
			this.CoverPoints.Clear();
			this._coverPointBlockers.Clear();
		}

		// Token: 0x0600304B RID: 12363 RVA: 0x000EC248 File Offset: 0x000EA448
		public Bounds GetBounds()
		{
			if (Mathf.Approximately(this.bounds.center.sqrMagnitude, 0f))
			{
				this.bounds = new Bounds(base.transform.position, base.transform.localScale);
			}
			return this.bounds;
		}

		// Token: 0x0600304C RID: 12364 RVA: 0x000EC29C File Offset: 0x000EA49C
		private CoverPoint CalculateCoverPoint(NavMeshHit info)
		{
			RaycastHit raycastHit;
			CoverPointVolume.CoverType coverType = this.ProvidesCoverInDir(new Ray(info.position, -info.normal), this.CoverPointRayLength, out raycastHit);
			if (coverType == CoverPointVolume.CoverType.None)
			{
				return null;
			}
			CoverPoint coverPoint = new CoverPoint(this, this.DefaultCoverPointScore)
			{
				Position = info.position,
				Normal = -info.normal
			};
			if (coverType == CoverPointVolume.CoverType.Full)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
			}
			else if (coverType == CoverPointVolume.CoverType.Partial)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
			}
			return coverPoint;
		}

		// Token: 0x0600304D RID: 12365 RVA: 0x000EC31C File Offset: 0x000EA51C
		internal CoverPointVolume.CoverType ProvidesCoverInDir(Ray ray, float maxDistance, out RaycastHit rayHit)
		{
			rayHit = default(RaycastHit);
			if (Vector3Ex.IsNaNOrInfinity(ray.origin))
			{
				return CoverPointVolume.CoverType.None;
			}
			if (Vector3Ex.IsNaNOrInfinity(ray.direction))
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction == Vector3.zero)
			{
				return CoverPointVolume.CoverType.None;
			}
			ray.origin += PlayerEyes.EyeOffset;
			if (Physics.Raycast(ray.origin, ray.direction, ref rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Full;
			}
			ray.origin += PlayerEyes.DuckOffset;
			if (Physics.Raycast(ray.origin, ray.direction, ref rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Partial;
			}
			return CoverPointVolume.CoverType.None;
		}

		// Token: 0x0600304E RID: 12366 RVA: 0x000EC3E0 File Offset: 0x000EA5E0
		public bool Contains(Vector3 point)
		{
			Bounds bounds = new Bounds(base.transform.position, base.transform.localScale);
			return bounds.Contains(point);
		}

		// Token: 0x020008C5 RID: 2245
		internal enum CoverType
		{
			// Token: 0x04002B21 RID: 11041
			None,
			// Token: 0x04002B22 RID: 11042
			Partial,
			// Token: 0x04002B23 RID: 11043
			Full
		}
	}
}
