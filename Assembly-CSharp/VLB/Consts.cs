using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x020007CB RID: 1995
	public static class Consts
	{
		// Token: 0x04002742 RID: 10050
		private const string HelpUrlBase = "http://saladgamer.com/vlb-doc/";

		// Token: 0x04002743 RID: 10051
		public const string HelpUrlBeam = "http://saladgamer.com/vlb-doc/comp-lightbeam/";

		// Token: 0x04002744 RID: 10052
		public const string HelpUrlDustParticles = "http://saladgamer.com/vlb-doc/comp-dustparticles/";

		// Token: 0x04002745 RID: 10053
		public const string HelpUrlDynamicOcclusion = "http://saladgamer.com/vlb-doc/comp-dynocclusion/";

		// Token: 0x04002746 RID: 10054
		public const string HelpUrlTriggerZone = "http://saladgamer.com/vlb-doc/comp-triggerzone/";

		// Token: 0x04002747 RID: 10055
		public const string HelpUrlConfig = "http://saladgamer.com/vlb-doc/config/";

		// Token: 0x04002748 RID: 10056
		public static readonly bool ProceduralObjectsVisibleInEditor = true;

		// Token: 0x04002749 RID: 10057
		public static readonly Color FlatColor = Color.white;

		// Token: 0x0400274A RID: 10058
		public const ColorMode ColorModeDefault = ColorMode.Flat;

		// Token: 0x0400274B RID: 10059
		public const float Alpha = 1f;

		// Token: 0x0400274C RID: 10060
		public const float SpotAngleDefault = 35f;

		// Token: 0x0400274D RID: 10061
		public const float SpotAngleMin = 0.1f;

		// Token: 0x0400274E RID: 10062
		public const float SpotAngleMax = 179.9f;

		// Token: 0x0400274F RID: 10063
		public const float ConeRadiusStart = 0.1f;

		// Token: 0x04002750 RID: 10064
		public const MeshType GeomMeshType = MeshType.Shared;

		// Token: 0x04002751 RID: 10065
		public const int GeomSidesDefault = 18;

		// Token: 0x04002752 RID: 10066
		public const int GeomSidesMin = 3;

		// Token: 0x04002753 RID: 10067
		public const int GeomSidesMax = 256;

		// Token: 0x04002754 RID: 10068
		public const int GeomSegmentsDefault = 5;

		// Token: 0x04002755 RID: 10069
		public const int GeomSegmentsMin = 0;

		// Token: 0x04002756 RID: 10070
		public const int GeomSegmentsMax = 64;

		// Token: 0x04002757 RID: 10071
		public const bool GeomCap = false;

		// Token: 0x04002758 RID: 10072
		public const AttenuationEquation AttenuationEquationDefault = AttenuationEquation.Quadratic;

		// Token: 0x04002759 RID: 10073
		public const float AttenuationCustomBlending = 0.5f;

		// Token: 0x0400275A RID: 10074
		public const float FadeStart = 0f;

		// Token: 0x0400275B RID: 10075
		public const float FadeEnd = 3f;

		// Token: 0x0400275C RID: 10076
		public const float FadeMinThreshold = 0.01f;

		// Token: 0x0400275D RID: 10077
		public const float DepthBlendDistance = 2f;

		// Token: 0x0400275E RID: 10078
		public const float CameraClippingDistance = 0.5f;

		// Token: 0x0400275F RID: 10079
		public const float FresnelPowMaxValue = 10f;

		// Token: 0x04002760 RID: 10080
		public const float FresnelPow = 8f;

		// Token: 0x04002761 RID: 10081
		public const float GlareFrontal = 0.5f;

		// Token: 0x04002762 RID: 10082
		public const float GlareBehind = 0.5f;

		// Token: 0x04002763 RID: 10083
		public const float NoiseIntensityMin = 0f;

		// Token: 0x04002764 RID: 10084
		public const float NoiseIntensityMax = 1f;

		// Token: 0x04002765 RID: 10085
		public const float NoiseIntensityDefault = 0.5f;

		// Token: 0x04002766 RID: 10086
		public const float NoiseScaleMin = 0.01f;

		// Token: 0x04002767 RID: 10087
		public const float NoiseScaleMax = 2f;

		// Token: 0x04002768 RID: 10088
		public const float NoiseScaleDefault = 0.5f;

		// Token: 0x04002769 RID: 10089
		public static readonly Vector3 NoiseVelocityDefault = new Vector3(0.07f, 0.18f, 0.05f);

		// Token: 0x0400276A RID: 10090
		public const BlendingMode BlendingModeDefault = BlendingMode.Additive;

		// Token: 0x0400276B RID: 10091
		public static readonly BlendMode[] BlendingMode_SrcFactor;

		// Token: 0x0400276C RID: 10092
		public static readonly BlendMode[] BlendingMode_DstFactor;

		// Token: 0x0400276D RID: 10093
		public static readonly bool[] BlendingMode_AlphaAsBlack;

		// Token: 0x0400276E RID: 10094
		public const float DynOcclusionMinSurfaceRatioDefault = 0.5f;

		// Token: 0x0400276F RID: 10095
		public const float DynOcclusionMinSurfaceRatioMin = 50f;

		// Token: 0x04002770 RID: 10096
		public const float DynOcclusionMinSurfaceRatioMax = 100f;

		// Token: 0x04002771 RID: 10097
		public const float DynOcclusionMaxSurfaceDotDefault = 0.25f;

		// Token: 0x04002772 RID: 10098
		public const float DynOcclusionMaxSurfaceAngleMin = 45f;

		// Token: 0x04002773 RID: 10099
		public const float DynOcclusionMaxSurfaceAngleMax = 90f;

		// Token: 0x04002774 RID: 10100
		public const int ConfigGeometryLayerIDDefault = 1;

		// Token: 0x04002775 RID: 10101
		public const string ConfigGeometryTagDefault = "Untagged";

		// Token: 0x04002776 RID: 10102
		public const RenderQueue ConfigGeometryRenderQueueDefault = RenderQueue.Transparent;

		// Token: 0x04002777 RID: 10103
		public const bool ConfigGeometryForceSinglePassDefault = false;

		// Token: 0x04002778 RID: 10104
		public const int ConfigNoise3DSizeDefault = 64;

		// Token: 0x04002779 RID: 10105
		public const int ConfigSharedMeshSides = 24;

		// Token: 0x0400277A RID: 10106
		public const int ConfigSharedMeshSegments = 5;

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06002BA0 RID: 11168 RVA: 0x00021D54 File Offset: 0x0001FF54
		public static HideFlags ProceduralObjectsHideFlags
		{
			get
			{
				if (!Consts.ProceduralObjectsVisibleInEditor)
				{
					return HideFlags.HideAndDontSave;
				}
				return HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset;
			}
		}

		// Token: 0x06002BA1 RID: 11169 RVA: 0x000DF810 File Offset: 0x000DDA10
		// Note: this type is marked as 'beforefieldinit'.
		static Consts()
		{
			BlendMode[] array = new BlendMode[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.42A4001D1CFDC98C761C0CFE5497A75F739D92F8).FieldHandle);
			Consts.BlendingMode_SrcFactor = array;
			BlendMode[] array2 = new BlendMode[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.D950356082C70AD4018410AD313BA99D655D4D4A).FieldHandle);
			Consts.BlendingMode_DstFactor = array2;
			bool[] array3 = new bool[3];
			array3[0] = true;
			array3[1] = true;
			Consts.BlendingMode_AlphaAsBlack = array3;
		}
	}
}
