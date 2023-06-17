using System;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020005A2 RID: 1442
public class LightCloneShadow : MonoBehaviour
{
	// Token: 0x04001CD1 RID: 7377
	public bool cloneShadowMap;

	// Token: 0x04001CD2 RID: 7378
	public string shaderPropNameMap = "_MainLightShadowMap";

	// Token: 0x04001CD3 RID: 7379
	[Range(0f, 2f)]
	public int cloneShadowMapDownscale = 1;

	// Token: 0x04001CD4 RID: 7380
	public RenderTexture map;

	// Token: 0x04001CD5 RID: 7381
	public bool cloneShadowMask = true;

	// Token: 0x04001CD6 RID: 7382
	public string shaderPropNameMask = "_MainLightShadowMask";

	// Token: 0x04001CD7 RID: 7383
	[Range(0f, 2f)]
	public int cloneShadowMaskDownscale = 1;

	// Token: 0x04001CD8 RID: 7384
	public RenderTexture mask;

	// Token: 0x04001CD9 RID: 7385
	private Light light;

	// Token: 0x04001CDA RID: 7386
	private CommandBuffer mapCB;

	// Token: 0x04001CDB RID: 7387
	private CommandBuffer maskCB;

	// Token: 0x04001CDC RID: 7388
	private int screenWidth;

	// Token: 0x04001CDD RID: 7389
	private int screenHeight;

	// Token: 0x06002113 RID: 8467 RVA: 0x0001A4A8 File Offset: 0x000186A8
	private void OnEnable()
	{
		this.light = base.GetComponent<Light>();
		if (this.light.type != LightType.Directional)
		{
			Debug.LogWarning("[LightCloneShadow] Requires a directional light. Disabling.");
			base.enabled = false;
		}
	}

	// Token: 0x06002114 RID: 8468 RVA: 0x0001A4D5 File Offset: 0x000186D5
	private void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.DestroyShadowClone();
	}

	// Token: 0x06002115 RID: 8469 RVA: 0x000B247C File Offset: 0x000B067C
	private void Update()
	{
		if (this.cloneShadowMap && this.map != null)
		{
			Shader.SetGlobalTexture(this.shaderPropNameMask, this.map);
		}
		else
		{
			Shader.SetGlobalTexture(this.shaderPropNameMask, Texture2D.blackTexture);
		}
		if (this.cloneShadowMask && this.mask != null)
		{
			Shader.SetGlobalTexture(this.shaderPropNameMask, this.mask);
		}
		else
		{
			Shader.SetGlobalTexture(this.shaderPropNameMask, Texture2D.whiteTexture);
		}
		this.UpdateShadowMask();
	}

	// Token: 0x06002116 RID: 8470 RVA: 0x000B2504 File Offset: 0x000B0704
	private void AllocateShadowClone()
	{
		if (this.cloneShadowMap)
		{
			string name = this.light.name + "-CloneShadowMap";
			int num = Mathf.Min(4096, Mathf.NextPowerOfTwo((int)((float)Mathf.Max(Screen.width, Screen.height) * 1.9f)));
			int height = (QualitySettings.shadowCascades == 2) ? (num / 2) : num;
			this.map = new RenderTexture(num, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
			this.map.name = name;
			this.map.hideFlags = HideFlags.DontSave;
			this.map.filterMode = FilterMode.Point;
			this.map.wrapMode = TextureWrapMode.Clamp;
			this.map.Create();
			Shader.SetGlobalTexture(this.shaderPropNameMap, this.map);
			this.mapCB = new CommandBuffer();
			this.mapCB.name = name;
			this.mapCB.SetShadowSamplingMode(BuiltinRenderTextureType.CurrentActive, ShadowSamplingMode.RawDepth);
			this.mapCB.Blit(BuiltinRenderTextureType.CurrentActive, this.map);
			CommandBuffer[] commandBuffers = this.light.GetCommandBuffers(LightEvent.AfterShadowMap);
			for (int i = 0; i < commandBuffers.Length; i++)
			{
				if (commandBuffers[i].name == base.name)
				{
					this.light.RemoveCommandBuffer(LightEvent.AfterShadowMap, commandBuffers[i]);
				}
			}
			this.light.AddCommandBuffer(LightEvent.AfterShadowMap, this.mapCB);
		}
		if (this.cloneShadowMask)
		{
			string name2 = this.light.name + "-CloneShadowMask";
			int width = Screen.width >> this.cloneShadowMaskDownscale;
			int height2 = Screen.height >> this.cloneShadowMaskDownscale;
			this.mask = new RenderTexture(width, height2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			this.mask.name = name2;
			this.mask.hideFlags = HideFlags.DontSave;
			this.mask.filterMode = FilterMode.Point;
			this.mask.wrapMode = TextureWrapMode.Clamp;
			this.mask.Create();
			Shader.SetGlobalTexture(this.shaderPropNameMask, this.mask);
			this.maskCB = new CommandBuffer();
			this.maskCB.name = name2;
			this.maskCB.Blit(BuiltinRenderTextureType.CurrentActive, this.mask);
			CommandBuffer[] commandBuffers2 = this.light.GetCommandBuffers(LightEvent.AfterScreenspaceMask);
			for (int j = 0; j < commandBuffers2.Length; j++)
			{
				if (commandBuffers2[j].name == base.name)
				{
					this.light.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, commandBuffers2[j]);
				}
			}
			this.light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this.maskCB);
		}
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x000B2798 File Offset: 0x000B0998
	private void DestroyShadowClone()
	{
		if (this.map != null)
		{
			RenderTexture.active = null;
			this.map.Release();
			Object.DestroyImmediate(this.map);
			this.map = null;
		}
		if (this.mapCB != null)
		{
			this.light.RemoveCommandBuffer(LightEvent.AfterShadowMap, this.mapCB);
			this.mapCB.Dispose();
			this.mapCB = null;
		}
		if (this.mask != null)
		{
			RenderTexture.active = null;
			this.mask.Release();
			Object.DestroyImmediate(this.mask);
			this.mask = null;
		}
		if (this.maskCB != null)
		{
			this.light.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, this.maskCB);
			this.maskCB.Dispose();
			this.maskCB = null;
		}
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000B2860 File Offset: 0x000B0A60
	private void UpdateShadowMask()
	{
		if ((this.cloneShadowMap && this.map == null) || (this.cloneShadowMask && (this.mask == null || this.screenWidth != Screen.width || this.screenHeight != Screen.height)))
		{
			this.DestroyShadowClone();
			this.AllocateShadowClone();
			this.screenWidth = Screen.width;
			this.screenHeight = Screen.height;
		}
	}
}
