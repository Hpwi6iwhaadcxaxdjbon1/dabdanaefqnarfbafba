using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020007DC RID: 2012
	[RequireComponent(typeof(VolumetricLightBeam))]
	[DisallowMultipleComponent]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dustparticles/")]
	[ExecuteInEditMode]
	public class VolumetricDustParticles : MonoBehaviour
	{
		// Token: 0x040027B6 RID: 10166
		[Range(0f, 1f)]
		public float alpha = 0.5f;

		// Token: 0x040027B7 RID: 10167
		[Range(0.0001f, 0.1f)]
		public float size = 0.01f;

		// Token: 0x040027B8 RID: 10168
		public VolumetricDustParticles.Direction direction = VolumetricDustParticles.Direction.Random;

		// Token: 0x040027B9 RID: 10169
		public float speed = 0.03f;

		// Token: 0x040027BA RID: 10170
		public float density = 5f;

		// Token: 0x040027BB RID: 10171
		[Range(0f, 1f)]
		public float spawnMaxDistance = 0.7f;

		// Token: 0x040027BC RID: 10172
		public bool cullingEnabled = true;

		// Token: 0x040027BD RID: 10173
		public float cullingMaxDistance = 10f;

		// Token: 0x040027BF RID: 10175
		public static bool isFeatureSupported = true;

		// Token: 0x040027C0 RID: 10176
		private ParticleSystem m_Particles;

		// Token: 0x040027C1 RID: 10177
		private ParticleSystemRenderer m_Renderer;

		// Token: 0x040027C2 RID: 10178
		private static bool ms_NoMainCameraLogged = false;

		// Token: 0x040027C3 RID: 10179
		private static Camera ms_MainCamera = null;

		// Token: 0x040027C4 RID: 10180
		private VolumetricLightBeam m_Master;

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06002BDD RID: 11229 RVA: 0x000220E4 File Offset: 0x000202E4
		// (set) Token: 0x06002BDE RID: 11230 RVA: 0x000220EC File Offset: 0x000202EC
		public bool isCulled { get; private set; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06002BDF RID: 11231 RVA: 0x000220F5 File Offset: 0x000202F5
		public bool particlesAreInstantiated
		{
			get
			{
				return this.m_Particles;
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06002BE0 RID: 11232 RVA: 0x00022102 File Offset: 0x00020302
		public int particlesCurrentCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.particleCount;
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06002BE1 RID: 11233 RVA: 0x000E0664 File Offset: 0x000DE864
		public int particlesMaxCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.main.maxParticles;
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06002BE2 RID: 11234 RVA: 0x000E0694 File Offset: 0x000DE894
		public Camera mainCamera
		{
			get
			{
				if (!VolumetricDustParticles.ms_MainCamera)
				{
					VolumetricDustParticles.ms_MainCamera = Camera.main;
					if (!VolumetricDustParticles.ms_MainCamera && !VolumetricDustParticles.ms_NoMainCameraLogged)
					{
						Debug.LogErrorFormat(base.gameObject, "In order to use 'VolumetricDustParticles' culling, you must have a MainCamera defined in your scene.", Array.Empty<object>());
						VolumetricDustParticles.ms_NoMainCameraLogged = true;
					}
				}
				return VolumetricDustParticles.ms_MainCamera;
			}
		}

		// Token: 0x06002BE3 RID: 11235 RVA: 0x0002211E File Offset: 0x0002031E
		private void Start()
		{
			this.isCulled = false;
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
			this.InstantiateParticleSystem();
			this.SetActiveAndPlay();
		}

		// Token: 0x06002BE4 RID: 11236 RVA: 0x000E06EC File Offset: 0x000DE8EC
		private void InstantiateParticleSystem()
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
			this.m_Particles = Config.Instance.NewVolumetricDustParticles();
			if (this.m_Particles)
			{
				this.m_Particles.transform.SetParent(base.transform, false);
				this.m_Renderer = this.m_Particles.GetComponent<ParticleSystemRenderer>();
			}
		}

		// Token: 0x06002BE5 RID: 11237 RVA: 0x0002214F File Offset: 0x0002034F
		private void OnEnable()
		{
			this.SetActiveAndPlay();
		}

		// Token: 0x06002BE6 RID: 11238 RVA: 0x00022157 File Offset: 0x00020357
		private void SetActiveAndPlay()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(true);
				this.SetParticleProperties();
				this.m_Particles.Play(true);
			}
		}

		// Token: 0x06002BE7 RID: 11239 RVA: 0x00022189 File Offset: 0x00020389
		private void OnDisable()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(false);
			}
		}

		// Token: 0x06002BE8 RID: 11240 RVA: 0x000221A9 File Offset: 0x000203A9
		private void OnDestroy()
		{
			if (this.m_Particles)
			{
				Object.DestroyImmediate(this.m_Particles.gameObject);
			}
			this.m_Particles = null;
		}

		// Token: 0x06002BE9 RID: 11241 RVA: 0x000221CF File Offset: 0x000203CF
		private void Update()
		{
			if (Application.isPlaying)
			{
				this.UpdateCulling();
			}
			this.SetParticleProperties();
		}

		// Token: 0x06002BEA RID: 11242 RVA: 0x000E0764 File Offset: 0x000DE964
		private void SetParticleProperties()
		{
			if (this.m_Particles && this.m_Particles.gameObject.activeSelf)
			{
				float t = Mathf.Clamp01(1f - this.m_Master.fresnelPow / 10f);
				float num = this.m_Master.fadeEnd * this.spawnMaxDistance;
				float num2 = num * this.density;
				int maxParticles = (int)(num2 * 4f);
				ParticleSystem.MainModule main = this.m_Particles.main;
				ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
				startLifetime.mode = 3;
				startLifetime.constantMin = 4f;
				startLifetime.constantMax = 6f;
				main.startLifetime = startLifetime;
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				startSize.mode = 3;
				startSize.constantMin = this.size * 0.9f;
				startSize.constantMax = this.size * 1.1f;
				main.startSize = startSize;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				if (this.m_Master.colorMode == ColorMode.Flat)
				{
					startColor.mode = 0;
					Color color = this.m_Master.color;
					color.a *= this.alpha;
					startColor.color = color;
				}
				else
				{
					startColor.mode = 1;
					Gradient colorGradient = this.m_Master.colorGradient;
					GradientColorKey[] colorKeys = colorGradient.colorKeys;
					GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
					for (int i = 0; i < alphaKeys.Length; i++)
					{
						GradientAlphaKey[] array = alphaKeys;
						int num3 = i;
						array[num3].alpha = array[num3].alpha * this.alpha;
					}
					Gradient gradient = new Gradient();
					gradient.SetKeys(colorKeys, alphaKeys);
					startColor.gradient = gradient;
				}
				main.startColor = startColor;
				ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
				startSpeed.constant = this.speed;
				main.startSpeed = startSpeed;
				main.maxParticles = maxParticles;
				ParticleSystem.ShapeModule shape = this.m_Particles.shape;
				shape.shapeType = 8;
				shape.radius = this.m_Master.coneRadiusStart * Mathf.Lerp(0.3f, 1f, t);
				shape.angle = this.m_Master.coneAngle * 0.5f * Mathf.Lerp(0.7f, 1f, t);
				shape.length = num;
				shape.arc = 360f;
				shape.randomDirectionAmount = ((this.direction == VolumetricDustParticles.Direction.Random) ? 1f : 0f);
				ParticleSystem.EmissionModule emission = this.m_Particles.emission;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				rateOverTime.constant = num2;
				emission.rateOverTime = rateOverTime;
				if (this.m_Renderer)
				{
					this.m_Renderer.sortingLayerID = this.m_Master.sortingLayerID;
					this.m_Renderer.sortingOrder = this.m_Master.sortingOrder;
				}
			}
		}

		// Token: 0x06002BEB RID: 11243 RVA: 0x000E0A28 File Offset: 0x000DEC28
		private void UpdateCulling()
		{
			if (this.m_Particles)
			{
				bool flag = true;
				if (this.cullingEnabled && this.m_Master.hasGeometry)
				{
					if (this.mainCamera)
					{
						float num = this.cullingMaxDistance * this.cullingMaxDistance;
						flag = (this.m_Master.bounds.SqrDistance(this.mainCamera.transform.position) <= num);
					}
					else
					{
						this.cullingEnabled = false;
					}
				}
				if (this.m_Particles.gameObject.activeSelf != flag)
				{
					this.m_Particles.gameObject.SetActive(flag);
					this.isCulled = !flag;
				}
				if (flag && !this.m_Particles.isPlaying)
				{
					this.m_Particles.Play();
				}
			}
		}

		// Token: 0x020007DD RID: 2013
		public enum Direction
		{
			// Token: 0x040027C6 RID: 10182
			Beam,
			// Token: 0x040027C7 RID: 10183
			Random
		}
	}
}
