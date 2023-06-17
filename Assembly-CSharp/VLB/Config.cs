using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x020007CA RID: 1994
	[HelpURL("http://saladgamer.com/vlb-doc/config/")]
	public class Config : ScriptableObject
	{
		// Token: 0x04002734 RID: 10036
		public int geometryLayerID = 1;

		// Token: 0x04002735 RID: 10037
		public string geometryTag = "Untagged";

		// Token: 0x04002736 RID: 10038
		public int geometryRenderQueue = 3000;

		// Token: 0x04002737 RID: 10039
		public bool forceSinglePass;

		// Token: 0x04002738 RID: 10040
		[SerializeField]
		[HighlightNull]
		private Shader beamShader1Pass;

		// Token: 0x04002739 RID: 10041
		[FormerlySerializedAs("BeamShader")]
		[FormerlySerializedAs("beamShader")]
		[HighlightNull]
		[SerializeField]
		private Shader beamShader2Pass;

		// Token: 0x0400273A RID: 10042
		public int sharedMeshSides = 24;

		// Token: 0x0400273B RID: 10043
		public int sharedMeshSegments = 5;

		// Token: 0x0400273C RID: 10044
		[Range(0.01f, 2f)]
		public float globalNoiseScale = 0.5f;

		// Token: 0x0400273D RID: 10045
		public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

		// Token: 0x0400273E RID: 10046
		[HighlightNull]
		public TextAsset noise3DData;

		// Token: 0x0400273F RID: 10047
		public int noise3DSize = 64;

		// Token: 0x04002740 RID: 10048
		[HighlightNull]
		public ParticleSystem dustParticlesPrefab;

		// Token: 0x04002741 RID: 10049
		private static Config m_Instance;

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06002B99 RID: 11161 RVA: 0x00021D0F File Offset: 0x0001FF0F
		public Shader beamShader
		{
			get
			{
				if (!this.forceSinglePass)
				{
					return this.beamShader2Pass;
				}
				return this.beamShader1Pass;
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06002B9A RID: 11162 RVA: 0x00021D26 File Offset: 0x0001FF26
		public Vector4 globalNoiseParam
		{
			get
			{
				return new Vector4(this.globalNoiseVelocity.x, this.globalNoiseVelocity.y, this.globalNoiseVelocity.z, this.globalNoiseScale);
			}
		}

		// Token: 0x06002B9B RID: 11163 RVA: 0x000DF64C File Offset: 0x000DD84C
		public void Reset()
		{
			this.geometryLayerID = 1;
			this.geometryTag = "Untagged";
			this.geometryRenderQueue = 3000;
			this.beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
			this.beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
			this.sharedMeshSides = 24;
			this.sharedMeshSegments = 5;
			this.globalNoiseScale = 0.5f;
			this.globalNoiseVelocity = Consts.NoiseVelocityDefault;
			this.noise3DData = (Resources.Load("Noise3D_64x64x64") as TextAsset);
			this.noise3DSize = 64;
			this.dustParticlesPrefab = (Resources.Load("DustParticles", typeof(ParticleSystem)) as ParticleSystem);
		}

		// Token: 0x06002B9C RID: 11164 RVA: 0x000DF6F8 File Offset: 0x000DD8F8
		public ParticleSystem NewVolumetricDustParticles()
		{
			if (!this.dustParticlesPrefab)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Failed to instantiate VolumetricDustParticles prefab.");
				}
				return null;
			}
			ParticleSystem particleSystem = Object.Instantiate<ParticleSystem>(this.dustParticlesPrefab);
			particleSystem.useAutoRandomSeed = false;
			particleSystem.name = "Dust Particles";
			particleSystem.gameObject.hideFlags = Consts.ProceduralObjectsHideFlags;
			particleSystem.gameObject.SetActive(true);
			return particleSystem;
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06002B9D RID: 11165 RVA: 0x000DF760 File Offset: 0x000DD960
		public static Config Instance
		{
			get
			{
				if (Config.m_Instance == null)
				{
					Config[] array = Resources.LoadAll<Config>("Config");
					Debug.Assert(array.Length != 0, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", typeof(Config)));
					Config.m_Instance = array[0];
				}
				return Config.m_Instance;
			}
		}
	}
}
