using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020007D8 RID: 2008
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-triggerzone/")]
	public class TriggerZone : MonoBehaviour
	{
		// Token: 0x040027AB RID: 10155
		public bool setIsTrigger = true;

		// Token: 0x040027AC RID: 10156
		public float rangeMultiplier = 1f;

		// Token: 0x040027AD RID: 10157
		private const int kMeshColliderNumSides = 8;

		// Token: 0x040027AE RID: 10158
		private Mesh m_Mesh;

		// Token: 0x06002BC3 RID: 11203 RVA: 0x000E0370 File Offset: 0x000DE570
		private void Update()
		{
			VolumetricLightBeam component = base.GetComponent<VolumetricLightBeam>();
			if (component)
			{
				MeshCollider orAddComponent = base.gameObject.GetOrAddComponent<MeshCollider>();
				Debug.Assert(orAddComponent);
				float lengthZ = component.fadeEnd * this.rangeMultiplier;
				float radiusEnd = Mathf.LerpUnclamped(component.coneRadiusStart, component.coneRadiusEnd, this.rangeMultiplier);
				this.m_Mesh = MeshGenerator.GenerateConeZ_Radius(lengthZ, component.coneRadiusStart, radiusEnd, 8, 0, false);
				this.m_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				orAddComponent.sharedMesh = this.m_Mesh;
				if (this.setIsTrigger)
				{
					orAddComponent.convex = true;
					orAddComponent.isTrigger = true;
				}
				Object.Destroy(this);
			}
		}
	}
}
