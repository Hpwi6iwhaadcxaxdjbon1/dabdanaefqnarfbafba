using System;
using ConVar;
using UnityEngine;

// Token: 0x020007AA RID: 1962
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class NGSS_Directional : MonoBehaviour
{
	// Token: 0x04002637 RID: 9783
	[Range(0f, 0.02f)]
	[Tooltip("Overall softness for both PCF and PCSS shadows.\nRecommended value: 0.01.")]
	public float PCSS_GLOBAL_SOFTNESS = 0.01f;

	// Token: 0x04002638 RID: 9784
	[Tooltip("PCSS softness when shadows is close to caster.\nRecommended value: 0.05.")]
	[Range(0f, 1f)]
	public float PCSS_FILTER_DIR_MIN = 0.05f;

	// Token: 0x04002639 RID: 9785
	[Range(0f, 0.5f)]
	[Tooltip("PCSS softness when shadows is far from caster.\nRecommended value: 0.25.\nIf too high can lead to visible artifacts when early bailout is enabled.")]
	public float PCSS_FILTER_DIR_MAX = 0.25f;

	// Token: 0x0400263A RID: 9786
	[Range(0f, 10f)]
	[Tooltip("Amount of banding or noise. Example: 0.0 gives 100 % Banding and 10.0 gives 100 % Noise.")]
	public float BANDING_NOISE_AMOUNT = 1f;

	// Token: 0x0400263B RID: 9787
	[Tooltip("Recommended values: Mobile = 16, Consoles = 25, Desktop Low = 32, Desktop High = 64")]
	public NGSS_Directional.SAMPLER_COUNT SAMPLERS_COUNT;

	// Token: 0x06002A99 RID: 10905 RVA: 0x00021238 File Offset: 0x0001F438
	private void Update()
	{
		this.SetGlobalSettings(ConVar.Graphics.shadowquality == 2);
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x000D92F4 File Offset: 0x000D74F4
	private void SetGlobalSettings(bool enabled)
	{
		if (enabled)
		{
			Shader.SetGlobalFloat("NGSS_PCSS_GLOBAL_SOFTNESS", this.PCSS_GLOBAL_SOFTNESS);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (this.PCSS_FILTER_DIR_MIN > this.PCSS_FILTER_DIR_MAX) ? this.PCSS_FILTER_DIR_MAX : this.PCSS_FILTER_DIR_MIN);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (this.PCSS_FILTER_DIR_MAX < this.PCSS_FILTER_DIR_MIN) ? this.PCSS_FILTER_DIR_MIN : this.PCSS_FILTER_DIR_MAX);
			Shader.SetGlobalFloat("NGSS_POISSON_SAMPLING_NOISE_DIR", this.BANDING_NOISE_AMOUNT);
		}
	}

	// Token: 0x020007AB RID: 1963
	public enum SAMPLER_COUNT
	{
		// Token: 0x0400263D RID: 9789
		SAMPLERS_16,
		// Token: 0x0400263E RID: 9790
		SAMPLERS_25,
		// Token: 0x0400263F RID: 9791
		SAMPLERS_32,
		// Token: 0x04002640 RID: 9792
		SAMPLERS_64
	}
}
