using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007BE RID: 1982
[AddComponentMenu("Image Effects/Sonic Ether/SE Screen-Space Shadows")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class SEScreenSpaceShadows : MonoBehaviour
{
	// Token: 0x040026DE RID: 9950
	private CommandBuffer blendShadowsCommandBuffer;

	// Token: 0x040026DF RID: 9951
	private CommandBuffer renderShadowsCommandBuffer;

	// Token: 0x040026E0 RID: 9952
	private Camera attachedCamera;

	// Token: 0x040026E1 RID: 9953
	public Light sun;

	// Token: 0x040026E2 RID: 9954
	[Range(0f, 1f)]
	public float blendStrength = 1f;

	// Token: 0x040026E3 RID: 9955
	[Range(0f, 1f)]
	public float accumulation = 0.9f;

	// Token: 0x040026E4 RID: 9956
	[Range(0.1f, 5f)]
	public float lengthFade = 0.7f;

	// Token: 0x040026E5 RID: 9957
	[Range(0.01f, 5f)]
	public float range = 0.7f;

	// Token: 0x040026E6 RID: 9958
	[Range(0f, 1f)]
	public float zThickness = 0.1f;

	// Token: 0x040026E7 RID: 9959
	[Range(2f, 92f)]
	public int samples = 32;

	// Token: 0x040026E8 RID: 9960
	[Range(0.5f, 4f)]
	public float nearSampleQuality = 1.5f;

	// Token: 0x040026E9 RID: 9961
	[Range(0f, 1f)]
	public float traceBias = 0.03f;

	// Token: 0x040026EA RID: 9962
	public bool stochasticSampling = true;

	// Token: 0x040026EB RID: 9963
	public bool leverageTemporalAA;

	// Token: 0x040026EC RID: 9964
	public bool bilateralBlur = true;

	// Token: 0x040026ED RID: 9965
	[Range(1f, 2f)]
	public int blurPasses = 1;

	// Token: 0x040026EE RID: 9966
	[Range(0.01f, 0.5f)]
	public float blurDepthTolerance = 0.1f;

	// Token: 0x040026EF RID: 9967
	private Material material;

	// Token: 0x040026F0 RID: 9968
	private object initChecker;

	// Token: 0x040026F1 RID: 9969
	private bool sunInitialized;

	// Token: 0x040026F2 RID: 9970
	private int temporalJitterCounter;

	// Token: 0x040026F3 RID: 9971
	private bool previousBilateralBlurSetting;

	// Token: 0x040026F4 RID: 9972
	private int previousBlurPassesSetting = 1;

	// Token: 0x040026F5 RID: 9973
	private Texture2D noBlendTex;

	// Token: 0x06002B35 RID: 11061 RVA: 0x000DCC48 File Offset: 0x000DAE48
	private void AddCommandBufferClean(Light light, CommandBuffer commandBuffer, LightEvent lightEvent)
	{
		bool flag = false;
		CommandBuffer[] commandBuffers = light.GetCommandBuffers(lightEvent);
		for (int i = 0; i < commandBuffers.Length; i++)
		{
			if (commandBuffers[i].name == commandBuffer.name)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			light.AddCommandBuffer(lightEvent, commandBuffer);
		}
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x000DCC90 File Offset: 0x000DAE90
	private void AddCommandBufferClean(Camera camera, CommandBuffer commandBuffer, CameraEvent cameraEvent)
	{
		bool flag = false;
		CommandBuffer[] commandBuffers = camera.GetCommandBuffers(cameraEvent);
		for (int i = 0; i < commandBuffers.Length; i++)
		{
			if (commandBuffers[i].name == commandBuffer.name)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			camera.AddCommandBuffer(cameraEvent, commandBuffer);
		}
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x000DCCD8 File Offset: 0x000DAED8
	private void RemoveCommandBuffer(Light light, CommandBuffer commandBuffer, LightEvent lightEvent)
	{
		CommandBuffer[] commandBuffers = light.GetCommandBuffers(lightEvent);
		List<CommandBuffer> list = new List<CommandBuffer>();
		foreach (CommandBuffer commandBuffer2 in commandBuffers)
		{
			if (commandBuffer2.name != commandBuffer.name)
			{
				list.Add(commandBuffer2);
			}
		}
		light.RemoveCommandBuffers(lightEvent);
		foreach (CommandBuffer buffer in list)
		{
			light.AddCommandBuffer(lightEvent, buffer);
		}
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x000DCD6C File Offset: 0x000DAF6C
	private void RemoveCommandBuffer(Camera camera, CommandBuffer commandBuffer, CameraEvent cameraEvent)
	{
		CommandBuffer[] commandBuffers = camera.GetCommandBuffers(cameraEvent);
		List<CommandBuffer> list = new List<CommandBuffer>();
		foreach (CommandBuffer commandBuffer2 in commandBuffers)
		{
			if (commandBuffer2.name != commandBuffer.name)
			{
				list.Add(commandBuffer2);
			}
		}
		camera.RemoveCommandBuffers(cameraEvent);
		foreach (CommandBuffer buffer in list)
		{
			camera.AddCommandBuffer(cameraEvent, buffer);
		}
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x0002190A File Offset: 0x0001FB0A
	private void RemoveCommandBuffers()
	{
		this.RemoveCommandBuffer(this.attachedCamera, this.renderShadowsCommandBuffer, CameraEvent.BeforeLighting);
		if (this.sun != null)
		{
			this.RemoveCommandBuffer(this.sun, this.blendShadowsCommandBuffer, LightEvent.AfterScreenspaceMask);
		}
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x00021940 File Offset: 0x0001FB40
	public bool GetCompatibleRenderPath()
	{
		if (this.attachedCamera)
		{
			return this.attachedCamera.actualRenderingPath == RenderingPath.DeferredShading;
		}
		return base.GetComponent<Camera>().actualRenderingPath == RenderingPath.DeferredShading;
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x000DCE00 File Offset: 0x000DB000
	private void Init()
	{
		if (this.initChecker != null)
		{
			return;
		}
		if (!this.attachedCamera)
		{
			this.attachedCamera = base.GetComponent<Camera>();
			this.attachedCamera.depthTextureMode |= DepthTextureMode.Depth;
		}
		if (!this.GetCompatibleRenderPath())
		{
			this.initChecker = null;
			return;
		}
		this.material = new Material(Shader.Find("Hidden/SEScreenSpaceShadows"));
		this.sunInitialized = false;
		this.blendShadowsCommandBuffer = new CommandBuffer
		{
			name = "SE Screen Space Shadows: Blend"
		};
		this.blendShadowsCommandBuffer.Blit(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CurrentActive, this.material, 0);
		this.renderShadowsCommandBuffer = new CommandBuffer
		{
			name = "SE Screen Space Shadows: Render"
		};
		int nameID = Shader.PropertyToID("SEScreenShadowBuffer0");
		int nameID2 = Shader.PropertyToID("SEScreenShadowBuffer1");
		int nameID3 = Shader.PropertyToID("DepthSource");
		RenderTextureFormat format = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8) ? RenderTextureFormat.R8 : RenderTextureFormat.RFloat;
		this.renderShadowsCommandBuffer.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Bilinear, format);
		if (this.bilateralBlur)
		{
			this.renderShadowsCommandBuffer.GetTemporaryRT(nameID2, -1, -1, 0, FilterMode.Bilinear, format);
		}
		this.renderShadowsCommandBuffer.GetTemporaryRT(nameID3, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RFloat);
		this.renderShadowsCommandBuffer.Blit(nameID, nameID3, this.material, 2);
		this.renderShadowsCommandBuffer.Blit(nameID3, nameID, this.material, 1);
		if (this.bilateralBlur)
		{
			for (int i = 0; i < this.blurPasses; i++)
			{
				this.renderShadowsCommandBuffer.SetGlobalVector("SESSSBlurKernel", new Vector2(0f, 1f));
				this.renderShadowsCommandBuffer.Blit(nameID, nameID2, this.material, 3);
				this.renderShadowsCommandBuffer.SetGlobalVector("SESSSBlurKernel", new Vector2(1f, 0f));
				this.renderShadowsCommandBuffer.Blit(nameID2, nameID, this.material, 3);
			}
		}
		this.renderShadowsCommandBuffer.SetGlobalTexture("SEScreenSpaceShadowsTexture", nameID);
		this.AddCommandBufferClean(this.attachedCamera, this.renderShadowsCommandBuffer, CameraEvent.BeforeLighting);
		this.noBlendTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		this.noBlendTex.SetPixel(0, 0, Color.white);
		this.noBlendTex.Apply();
		this.initChecker = new object();
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x0002196C File Offset: 0x0001FB6C
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x00021974 File Offset: 0x0001FB74
	private void OnDisable()
	{
		this.RemoveCommandBuffers();
		this.initChecker = null;
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000DD06C File Offset: 0x000DB26C
	private void OnPreRender()
	{
		this.Init();
		if (this.initChecker == null)
		{
			return;
		}
		if (this.sun != null && !this.sunInitialized)
		{
			this.AddCommandBufferClean(this.sun, this.blendShadowsCommandBuffer, LightEvent.AfterScreenspaceMask);
			this.sunInitialized = true;
		}
		if (this.leverageTemporalAA)
		{
			this.temporalJitterCounter = (this.temporalJitterCounter + 1) % 8;
		}
		if (this.previousBilateralBlurSetting != this.bilateralBlur || this.previousBlurPassesSetting != this.blurPasses)
		{
			this.RemoveCommandBuffers();
			this.initChecker = null;
			this.Init();
		}
		this.previousBilateralBlurSetting = this.bilateralBlur;
		this.previousBlurPassesSetting = this.blurPasses;
		if (!this.sunInitialized)
		{
			return;
		}
		this.material.SetMatrix("ProjectionMatrix", this.attachedCamera.projectionMatrix);
		this.material.SetMatrix("ProjectionMatrixInverse", this.attachedCamera.projectionMatrix.inverse);
		this.material.SetVector("SunlightVector", base.transform.InverseTransformDirection(this.sun.transform.forward));
		this.material.SetVector("ScreenRes", new Vector4((float)this.attachedCamera.pixelWidth, (float)this.attachedCamera.pixelHeight, 1f / (float)this.attachedCamera.pixelWidth, 1f / (float)this.attachedCamera.pixelHeight));
		this.material.SetFloat("BlendStrength", this.blendStrength);
		this.material.SetFloat("Accumulation", this.accumulation);
		this.material.SetFloat("Range", this.range);
		this.material.SetFloat("ZThickness", this.zThickness);
		this.material.SetInt("Samples", this.samples);
		this.material.SetFloat("NearQualityCutoff", 1f / this.nearSampleQuality);
		this.material.SetFloat("TraceBias", this.traceBias);
		this.material.SetFloat("StochasticSampling", this.stochasticSampling ? 1f : 0f);
		this.material.SetInt("TJitter", this.temporalJitterCounter);
		this.material.SetFloat("LengthFade", this.lengthFade);
		this.material.SetFloat("BlurDepthTolerance", this.blurDepthTolerance);
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x00021983 File Offset: 0x0001FB83
	private void OnPostRender()
	{
		Shader.SetGlobalTexture("SEScreenSpaceShadowsTexture", this.noBlendTex);
	}
}
