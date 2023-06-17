using System;
using Rust;
using UnityEngine;

// Token: 0x020007C5 RID: 1989
[AddComponentMenu("Rendering/Visualize Texture Density")]
[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class VisualizeTexelDensity : MonoBehaviour
{
	// Token: 0x0400271B RID: 10011
	public Shader shader;

	// Token: 0x0400271C RID: 10012
	public string shaderTag = "RenderType";

	// Token: 0x0400271D RID: 10013
	[Range(1f, 1024f)]
	public int texelsPerMeter = 256;

	// Token: 0x0400271E RID: 10014
	[Range(0f, 1f)]
	public float overlayOpacity = 0.5f;

	// Token: 0x0400271F RID: 10015
	public bool showHUD = true;

	// Token: 0x04002720 RID: 10016
	private Camera mainCamera;

	// Token: 0x04002721 RID: 10017
	private bool initialized;

	// Token: 0x04002722 RID: 10018
	private int screenWidth;

	// Token: 0x04002723 RID: 10019
	private int screenHeight;

	// Token: 0x04002724 RID: 10020
	private Camera texelDensityCamera;

	// Token: 0x04002725 RID: 10021
	private RenderTexture texelDensityRT;

	// Token: 0x04002726 RID: 10022
	private Texture texelDensityGradTex;

	// Token: 0x04002727 RID: 10023
	private Material texelDensityOverlayMat;

	// Token: 0x04002728 RID: 10024
	private static VisualizeTexelDensity instance;

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06002B69 RID: 11113 RVA: 0x00021ADE File Offset: 0x0001FCDE
	public static VisualizeTexelDensity Instance
	{
		get
		{
			return VisualizeTexelDensity.instance;
		}
	}

	// Token: 0x06002B6A RID: 11114 RVA: 0x00021AE5 File Offset: 0x0001FCE5
	private void Awake()
	{
		VisualizeTexelDensity.instance = this;
		this.mainCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x00021AF9 File Offset: 0x0001FCF9
	private void OnEnable()
	{
		this.mainCamera = base.GetComponent<Camera>();
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
		this.LoadResources();
		this.initialized = true;
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x00021B2A File Offset: 0x0001FD2A
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.SafeDestroyViewTexelDensity();
		this.SafeDestroyViewTexelDensityRT();
		this.initialized = false;
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x000DE688 File Offset: 0x000DC888
	private void LoadResources()
	{
		if (this.texelDensityGradTex == null)
		{
			this.texelDensityGradTex = (Resources.Load("TexelDensityGrad") as Texture);
		}
		if (this.texelDensityOverlayMat == null)
		{
			this.texelDensityOverlayMat = new Material(Shader.Find("Hidden/TexelDensityOverlay"))
			{
				hideFlags = HideFlags.DontSave
			};
		}
	}

	// Token: 0x06002B6E RID: 11118 RVA: 0x000DE6E4 File Offset: 0x000DC8E4
	private void SafeDestroyViewTexelDensity()
	{
		if (this.texelDensityCamera != null)
		{
			Object.DestroyImmediate(this.texelDensityCamera.gameObject);
			this.texelDensityCamera = null;
		}
		if (this.texelDensityGradTex != null)
		{
			Resources.UnloadAsset(this.texelDensityGradTex);
			this.texelDensityGradTex = null;
		}
		if (this.texelDensityOverlayMat != null)
		{
			Object.DestroyImmediate(this.texelDensityOverlayMat);
			this.texelDensityOverlayMat = null;
		}
	}

	// Token: 0x06002B6F RID: 11119 RVA: 0x00021B47 File Offset: 0x0001FD47
	private void SafeDestroyViewTexelDensityRT()
	{
		if (this.texelDensityRT != null)
		{
			Graphics.SetRenderTarget(null);
			Object.DestroyImmediate(this.texelDensityRT);
			this.texelDensityRT = null;
		}
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000DE758 File Offset: 0x000DC958
	private void UpdateViewTexelDensity(bool screenResized)
	{
		if (this.texelDensityCamera == null)
		{
			GameObject gameObject = new GameObject("Texel Density Camera", new Type[]
			{
				typeof(Camera)
			})
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			gameObject.transform.parent = this.mainCamera.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			this.texelDensityCamera = gameObject.GetComponent<Camera>();
			this.texelDensityCamera.CopyFrom(this.mainCamera);
			this.texelDensityCamera.renderingPath = RenderingPath.Forward;
			this.texelDensityCamera.allowMSAA = false;
			this.texelDensityCamera.allowHDR = false;
			this.texelDensityCamera.clearFlags = CameraClearFlags.Skybox;
			this.texelDensityCamera.depthTextureMode = DepthTextureMode.None;
			this.texelDensityCamera.SetReplacementShader(this.shader, this.shaderTag);
			this.texelDensityCamera.enabled = false;
		}
		if (this.texelDensityRT == null || screenResized || !this.texelDensityRT.IsCreated())
		{
			this.texelDensityCamera.targetTexture = null;
			this.SafeDestroyViewTexelDensityRT();
			this.texelDensityRT = new RenderTexture(this.screenWidth, this.screenHeight, 24, RenderTextureFormat.ARGB32)
			{
				hideFlags = HideFlags.DontSave
			};
			this.texelDensityRT.name = "TexelDensityRT";
			this.texelDensityRT.filterMode = FilterMode.Point;
			this.texelDensityRT.wrapMode = TextureWrapMode.Clamp;
			this.texelDensityRT.Create();
		}
		if (this.texelDensityCamera.targetTexture != this.texelDensityRT)
		{
			this.texelDensityCamera.targetTexture = this.texelDensityRT;
		}
		Shader.SetGlobalFloat("global_TexelsPerMeter", (float)this.texelsPerMeter);
		Shader.SetGlobalTexture("global_TexelDensityGrad", this.texelDensityGradTex);
		this.texelDensityCamera.fieldOfView = this.mainCamera.fieldOfView;
		this.texelDensityCamera.nearClipPlane = this.mainCamera.nearClipPlane;
		this.texelDensityCamera.farClipPlane = this.mainCamera.farClipPlane;
		this.texelDensityCamera.cullingMask = this.mainCamera.cullingMask;
	}

	// Token: 0x06002B71 RID: 11121 RVA: 0x00021B6F File Offset: 0x0001FD6F
	private bool CheckScreenResized(int width, int height)
	{
		if (this.screenWidth != width || this.screenHeight != height)
		{
			this.screenWidth = width;
			this.screenHeight = height;
			return true;
		}
		return false;
	}

	// Token: 0x06002B72 RID: 11122 RVA: 0x000DE978 File Offset: 0x000DCB78
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.initialized)
		{
			this.UpdateViewTexelDensity(this.CheckScreenResized(source.width, source.height));
			this.texelDensityCamera.Render();
			this.texelDensityOverlayMat.SetTexture("_TexelDensityMap", this.texelDensityRT);
			this.texelDensityOverlayMat.SetFloat("_Opacity", this.overlayOpacity);
			Graphics.Blit(source, destination, this.texelDensityOverlayMat, 0);
			return;
		}
		Graphics.Blit(source, destination);
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x000DE9F4 File Offset: 0x000DCBF4
	private void DrawGUIText(float x, float y, Vector2 size, string text, GUIStyle fontStyle)
	{
		fontStyle.normal.textColor = Color.black;
		GUI.Label(new Rect(x - 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y - 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x - 1f, y - 1f, size.x, size.y), text, fontStyle);
		fontStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(x, y, size.x, size.y), text, fontStyle);
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x000DEAE0 File Offset: 0x000DCCE0
	private void OnGUI()
	{
		if (this.initialized && this.showHUD)
		{
			string text = "Texels Per Meter";
			string text2 = "0";
			string text3 = this.texelsPerMeter.ToString();
			string text4 = (this.texelsPerMeter << 1).ToString() + "+";
			float num = (float)this.texelDensityGradTex.width;
			float num2 = (float)(this.texelDensityGradTex.height * 2);
			float num3 = (float)((Screen.width - this.texelDensityGradTex.width) / 2);
			float num4 = 32f;
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)Screen.width, (float)Screen.height, 0f);
			Graphics.DrawTexture(new Rect(num3 - 2f, num4 - 2f, num + 4f, num2 + 4f), Texture2D.whiteTexture);
			Graphics.DrawTexture(new Rect(num3, num4, num, num2), this.texelDensityGradTex);
			GL.PopMatrix();
			GUIStyle guistyle = new GUIStyle();
			guistyle.fontSize = 13;
			Vector2 vector = guistyle.CalcSize(new GUIContent(text));
			Vector2 size = guistyle.CalcSize(new GUIContent(text2));
			Vector2 vector2 = guistyle.CalcSize(new GUIContent(text3));
			Vector2 vector3 = guistyle.CalcSize(new GUIContent(text4));
			this.DrawGUIText(((float)Screen.width - vector.x) / 2f, num4 - vector.y - 5f, vector, text, guistyle);
			this.DrawGUIText(num3, num4 + num2 + 6f, size, text2, guistyle);
			this.DrawGUIText(((float)Screen.width - vector2.x) / 2f, num4 + num2 + 6f, vector2, text3, guistyle);
			this.DrawGUIText(num3 + num - vector3.x, num4 + num2 + 6f, vector3, text4, guistyle);
		}
	}
}
