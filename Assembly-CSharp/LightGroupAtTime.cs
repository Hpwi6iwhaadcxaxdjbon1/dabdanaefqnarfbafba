using System;
using System.Collections.Generic;
using UnityEngine;
using VLB;

// Token: 0x02000208 RID: 520
public class LightGroupAtTime : FacepunchBehaviour
{
	// Token: 0x04000CC6 RID: 3270
	public AnimationCurve IntensityScaleOverTime = new AnimationCurve
	{
		keys = new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(8f, 0f),
			new Keyframe(12f, 0f),
			new Keyframe(19f, 1f),
			new Keyframe(24f, 1f)
		}
	};

	// Token: 0x04000CC7 RID: 3271
	public Transform SearchRoot;

	// Token: 0x04000CC8 RID: 3272
	private List<KeyValuePair<Light, float>> lights = new List<KeyValuePair<Light, float>>();

	// Token: 0x04000CC9 RID: 3273
	private List<KeyValuePair<Renderer, Color>> renderers = new List<KeyValuePair<Renderer, Color>>();

	// Token: 0x04000CCA RID: 3274
	private List<VolumetricLightBeam> beams = new List<VolumetricLightBeam>();

	// Token: 0x04000CCB RID: 3275
	private MaterialPropertyBlock block;

	// Token: 0x04000CCC RID: 3276
	private int lightIndex;

	// Token: 0x04000CCD RID: 3277
	private int rendererIndex;

	// Token: 0x04000CCE RID: 3278
	private int beamIndex;

	// Token: 0x04000CCF RID: 3279
	private readonly int EmissionPropertyID = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04000CD0 RID: 3280
	private readonly int LightsPerFrame = 10;

	// Token: 0x04000CD1 RID: 3281
	private readonly int RenderersPerFrame = 10;

	// Token: 0x04000CD2 RID: 3282
	private readonly int BeamsPerFrame = 10;

	// Token: 0x06000FF7 RID: 4087 RVA: 0x0006C1F0 File Offset: 0x0006A3F0
	private void OnEnable()
	{
		Transform transform = (this.SearchRoot != null) ? this.SearchRoot : base.transform;
		Light[] componentsInChildren = transform.GetComponentsInChildren<Light>();
		Renderer[] componentsInChildren2 = transform.GetComponentsInChildren<Renderer>();
		transform.GetComponentsInChildren<VolumetricLightBeam>(this.beams);
		foreach (Light light in componentsInChildren)
		{
			this.lights.Add(new KeyValuePair<Light, float>(light, light.intensity));
		}
		foreach (Renderer renderer in componentsInChildren2)
		{
			foreach (Material material in renderer.sharedMaterials)
			{
				if (material != null && material.IsKeywordEnabled("_EMISSION") && material.HasProperty(this.EmissionPropertyID))
				{
					this.renderers.Add(new KeyValuePair<Renderer, Color>(renderer, material.GetColor(this.EmissionPropertyID)));
					break;
				}
			}
		}
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x0000E1AA File Offset: 0x0000C3AA
	private void OnDisable()
	{
		this.lights.Clear();
		this.renderers.Clear();
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x0006C2E4 File Offset: 0x0006A4E4
	private void Update()
	{
		float num = Mathf.Max(0f, this.IntensityScaleOverTime.Evaluate(TOD_Sky.Instance.Cycle.Hour));
		this.block = ((this.block != null) ? this.block : new MaterialPropertyBlock());
		this.block.Clear();
		int num2 = Mathf.Min(this.lights.Count, this.LightsPerFrame);
		for (int i = 0; i < num2; i++)
		{
			List<KeyValuePair<Light, float>> list = this.lights;
			int num3 = this.lightIndex;
			this.lightIndex = num3 + 1;
			KeyValuePair<Light, float> keyValuePair = list[num3];
			keyValuePair.Key.intensity = keyValuePair.Value * num;
			this.lightIndex = ((this.lightIndex < this.lights.Count) ? this.lightIndex : 0);
		}
		int num4 = Mathf.Min(this.renderers.Count, this.RenderersPerFrame);
		for (int j = 0; j < num4; j++)
		{
			List<KeyValuePair<Renderer, Color>> list2 = this.renderers;
			int num3 = this.rendererIndex;
			this.rendererIndex = num3 + 1;
			KeyValuePair<Renderer, Color> keyValuePair2 = list2[num3];
			this.block.SetColor(this.EmissionPropertyID, keyValuePair2.Value * num);
			keyValuePair2.Key.SetPropertyBlock(this.block);
			this.rendererIndex = ((this.rendererIndex < this.renderers.Count) ? this.rendererIndex : 0);
		}
		int num5 = Mathf.Min(this.beams.Count, this.BeamsPerFrame);
		for (int k = 0; k < num5; k++)
		{
			List<VolumetricLightBeam> list3 = this.beams;
			int num3 = this.beamIndex;
			this.beamIndex = num3 + 1;
			VolumetricLightBeam volumetricLightBeam = list3[num3];
			volumetricLightBeam.color = new Color(volumetricLightBeam.color.r, volumetricLightBeam.color.g, volumetricLightBeam.color.b, num);
			this.beamIndex = ((this.beamIndex < this.beams.Count) ? this.beamIndex : 0);
		}
	}
}
