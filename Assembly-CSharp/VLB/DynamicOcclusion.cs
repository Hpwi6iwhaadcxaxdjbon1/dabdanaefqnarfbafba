using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020007CC RID: 1996
	[RequireComponent(typeof(VolumetricLightBeam))]
	[DisallowMultipleComponent]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
	[ExecuteInEditMode]
	public class DynamicOcclusion : MonoBehaviour
	{
		// Token: 0x0400277B RID: 10107
		public LayerMask layerMask = -1;

		// Token: 0x0400277C RID: 10108
		public float minOccluderArea;

		// Token: 0x0400277D RID: 10109
		public int waitFrameCount = 3;

		// Token: 0x0400277E RID: 10110
		public float minSurfaceRatio = 0.5f;

		// Token: 0x0400277F RID: 10111
		public float maxSurfaceDot = 0.25f;

		// Token: 0x04002780 RID: 10112
		public PlaneAlignment planeAlignment;

		// Token: 0x04002781 RID: 10113
		public float planeOffset = 0.1f;

		// Token: 0x04002782 RID: 10114
		private VolumetricLightBeam m_Master;

		// Token: 0x04002783 RID: 10115
		private int m_FrameCountToWait;

		// Token: 0x04002784 RID: 10116
		private float m_RangeMultiplier = 1f;

		// Token: 0x04002785 RID: 10117
		private uint m_PrevNonSubHitDirectionId;

		// Token: 0x06002BA2 RID: 11170 RVA: 0x00021D62 File Offset: 0x0001FF62
		private void OnValidate()
		{
			this.minOccluderArea = Mathf.Max(this.minOccluderArea, 0f);
			this.waitFrameCount = Mathf.Clamp(this.waitFrameCount, 1, 60);
		}

		// Token: 0x06002BA3 RID: 11171 RVA: 0x00021D8E File Offset: 0x0001FF8E
		private void OnEnable()
		{
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
		}

		// Token: 0x06002BA4 RID: 11172 RVA: 0x00021DAC File Offset: 0x0001FFAC
		private void OnDisable()
		{
			this.SetHitNull();
		}

		// Token: 0x06002BA5 RID: 11173 RVA: 0x000DF888 File Offset: 0x000DDA88
		private void Start()
		{
			if (Application.isPlaying)
			{
				TriggerZone component = base.GetComponent<TriggerZone>();
				if (component)
				{
					this.m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
				}
			}
		}

		// Token: 0x06002BA6 RID: 11174 RVA: 0x00021DB4 File Offset: 0x0001FFB4
		private void LateUpdate()
		{
			if (this.m_FrameCountToWait <= 0)
			{
				this.ProcessRaycasts();
				this.m_FrameCountToWait = this.waitFrameCount;
			}
			this.m_FrameCountToWait--;
		}

		// Token: 0x06002BA7 RID: 11175 RVA: 0x000DF8C4 File Offset: 0x000DDAC4
		private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
		{
			float num = angleDiff * 0.5f;
			return Quaternion.Euler(Random.Range(-num, num), Random.Range(-num, num), Random.Range(-num, num)) * direction;
		}

		// Token: 0x06002BA8 RID: 11176 RVA: 0x000DF8FC File Offset: 0x000DDAFC
		private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
		{
			RaycastHit[] array = Physics.RaycastAll(rayPos, rayDir, this.m_Master.fadeEnd * this.m_RangeMultiplier, this.layerMask.value);
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].collider.isTrigger && array[i].collider.bounds.GetMaxArea2D() >= this.minOccluderArea && array[i].distance < num2)
				{
					num2 = array[i].distance;
					num = i;
				}
			}
			if (num != -1)
			{
				return array[num];
			}
			return default(RaycastHit);
		}

		// Token: 0x06002BA9 RID: 11177 RVA: 0x000DF9AC File Offset: 0x000DDBAC
		private Vector3 GetDirection(uint dirInt)
		{
			dirInt %= (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length;
			switch (dirInt)
			{
			case 0U:
				return base.transform.up;
			case 1U:
				return base.transform.right;
			case 2U:
				return -base.transform.up;
			case 3U:
				return -base.transform.right;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x06002BAA RID: 11178 RVA: 0x00021DDF File Offset: 0x0001FFDF
		private bool IsHitValid(RaycastHit hit)
		{
			return hit.collider && Vector3.Dot(hit.normal, -base.transform.forward) >= this.maxSurfaceDot;
		}

		// Token: 0x06002BAB RID: 11179 RVA: 0x000DFA28 File Offset: 0x000DDC28
		private void ProcessRaycasts()
		{
			RaycastHit hit = this.GetBestHit(base.transform.position, base.transform.forward);
			if (this.IsHitValid(hit))
			{
				if (this.minSurfaceRatio > 0.5f)
				{
					for (uint num = 0U; num < (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length; num += 1U)
					{
						Vector3 direction = this.GetDirection(num + this.m_PrevNonSubHitDirectionId);
						Vector3 vector = base.transform.position + direction * this.m_Master.coneRadiusStart * (this.minSurfaceRatio * 2f - 1f);
						Vector3 a = base.transform.position + base.transform.forward * this.m_Master.fadeEnd + direction * this.m_Master.coneRadiusEnd * (this.minSurfaceRatio * 2f - 1f);
						RaycastHit bestHit = this.GetBestHit(vector, a - vector);
						if (!this.IsHitValid(bestHit))
						{
							this.m_PrevNonSubHitDirectionId = num;
							this.SetHitNull();
							return;
						}
						if (bestHit.distance > hit.distance)
						{
							hit = bestHit;
						}
					}
				}
				this.SetHit(hit);
				return;
			}
			this.SetHitNull();
		}

		// Token: 0x06002BAC RID: 11180 RVA: 0x000DFB84 File Offset: 0x000DDD84
		private void SetHit(RaycastHit hit)
		{
			PlaneAlignment planeAlignment = this.planeAlignment;
			if (planeAlignment != PlaneAlignment.Surface && planeAlignment == PlaneAlignment.Beam)
			{
				this.SetClippingPlane(new Plane(-base.transform.forward, hit.point));
				return;
			}
			this.SetClippingPlane(new Plane(hit.normal, hit.point));
		}

		// Token: 0x06002BAD RID: 11181 RVA: 0x00021E18 File Offset: 0x00020018
		private void SetHitNull()
		{
			this.SetClippingPlaneOff();
		}

		// Token: 0x06002BAE RID: 11182 RVA: 0x00021E20 File Offset: 0x00020020
		private void SetClippingPlane(Plane planeWS)
		{
			planeWS = planeWS.TranslateCustom(planeWS.normal * this.planeOffset);
			this.m_Master.SetClippingPlane(planeWS);
		}

		// Token: 0x06002BAF RID: 11183 RVA: 0x00021E48 File Offset: 0x00020048
		private void SetClippingPlaneOff()
		{
			this.m_Master.SetClippingPlaneOff();
		}

		// Token: 0x020007CD RID: 1997
		private enum Direction
		{
			// Token: 0x04002787 RID: 10119
			Up,
			// Token: 0x04002788 RID: 10120
			Right,
			// Token: 0x04002789 RID: 10121
			Down,
			// Token: 0x0400278A RID: 10122
			Left
		}
	}
}
