using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000421 RID: 1057
public class OutlineManager : MonoBehaviour, IClientComponent
{
	// Token: 0x04001609 RID: 5641
	public static Material blurMat;

	// Token: 0x0400160A RID: 5642
	public List<OutlineObject> objectsToRender;

	// Token: 0x0400160B RID: 5643
	public float blurAmount = 2f;

	// Token: 0x0400160C RID: 5644
	public Material glowSolidMaterial;

	// Token: 0x0400160D RID: 5645
	public Material blendGlowMaterial;

	// Token: 0x0400160E RID: 5646
	public static float worldModelDistance = 4f;

	// Token: 0x0400160F RID: 5647
	public float nextUpdateTime;

	// Token: 0x0600197A RID: 6522 RVA: 0x000150DE File Offset: 0x000132DE
	private void BlurCreate()
	{
		if (OutlineManager.blurMat == null)
		{
			OutlineManager.blurMat = new Material(Shader.Find("Hidden/Rust/SeparableBlur"))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x00015109 File Offset: 0x00013309
	private void BlurDestroy()
	{
		Object.DestroyImmediate(OutlineManager.blurMat);
		OutlineManager.blurMat = null;
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x000902D8 File Offset: 0x0008E4D8
	public static void BlurRT(RenderTexture myRT, float radius, int passNum)
	{
		Vector2 vector = new Vector2(radius / (float)myRT.width, radius / (float)myRT.height);
		RenderTexture temporary = RenderTexture.GetTemporary(myRT.width, myRT.height, 0, myRT.format);
		OutlineManager.blurMat.SetVector("offsets", new Vector4(vector.x, 0f, 0f, 0f));
		UnityEngine.Graphics.Blit(myRT, temporary, OutlineManager.blurMat, passNum);
		OutlineManager.blurMat.SetVector("offsets", new Vector4(0f, vector.y, 0f, 0f));
		UnityEngine.Graphics.Blit(temporary, myRT, OutlineManager.blurMat, passNum);
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x0001511B File Offset: 0x0001331B
	public void OnEnable()
	{
		this.BlurCreate();
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x00015123 File Offset: 0x00013323
	public void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.BlurDestroy();
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x00015133 File Offset: 0x00013333
	public void Update()
	{
		this.UpdateOutlines();
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x0009038C File Offset: 0x0008E58C
	public void UpdateOutlines()
	{
		if (!Effects.showoutlines)
		{
			return;
		}
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextUpdateTime)
		{
			return;
		}
		this.nextUpdateTime = UnityEngine.Time.realtimeSinceStartup + 0.5f;
		this.ClearOutlines();
		List<DroppedItem> list = Pool.GetList<DroppedItem>();
		global::Vis.Entities<DroppedItem>(LocalPlayer.Entity.transform.position, OutlineManager.worldModelDistance, list, -1, 2);
		foreach (DroppedItem droppedItem in list)
		{
			if (!droppedItem.isServer)
			{
				OutlineObject componentInChildren = droppedItem.GetComponentInChildren<OutlineObject>();
				if (!(componentInChildren == null) && !this.objectsToRender.Contains(componentInChildren) && componentInChildren.SampleVisibility() >= 1f)
				{
					this.objectsToRender.Add(componentInChildren);
					componentInChildren.BecomeVisible();
				}
			}
		}
		Pool.FreeList<DroppedItem>(ref list);
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x0001513B File Offset: 0x0001333B
	public void ClearOutlines()
	{
		this.objectsToRender.Clear();
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x0009047C File Offset: 0x0008E67C
	public void CleanupOutlines()
	{
		for (int i = this.objectsToRender.Count - 1; i >= 0; i--)
		{
			OutlineObject outlineObject = this.objectsToRender[i];
			if (outlineObject == null || !outlineObject.ShouldDisplay())
			{
				outlineObject.BecomeInvisible();
				this.objectsToRender.Remove(outlineObject);
			}
		}
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x000904D4 File Offset: 0x0008E6D4
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!Effects.showoutlines)
		{
			UnityEngine.Graphics.Blit(source, destination);
			return;
		}
		this.CleanupOutlines();
		if (this.objectsToRender.Count == 0)
		{
			UnityEngine.Graphics.Blit(source, destination);
			return;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(MainCamera.mainCamera.pixelWidth, MainCamera.mainCamera.pixelHeight, 16, RenderTextureFormat.ARGB32);
		RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
		RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
		UnityEngine.Graphics.SetRenderTarget(temporary);
		GL.Clear(true, true, Color.clear);
		Matrix4x4 worldToCameraMatrix = MainCamera.mainCamera.worldToCameraMatrix;
		Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(MainCamera.mainCamera.projectionMatrix, true);
		this.glowSolidMaterial.SetMatrix("_CameraViewProj", gpuprojectionMatrix * worldToCameraMatrix);
		if (this.glowSolidMaterial.SetPass(0))
		{
			foreach (OutlineObject outlineObject in this.objectsToRender)
			{
				for (int i = 0; i < outlineObject.meshes.Length; i++)
				{
					this.glowSolidMaterial.SetColor("_Color", outlineObject.GetColor());
					this.glowSolidMaterial.SetPass(0);
					if (outlineObject.meshTransforms[i] != null && outlineObject.meshes[i] != null)
					{
						UnityEngine.Graphics.DrawMeshNow(outlineObject.meshes[i], outlineObject.meshTransforms[i].localToWorldMatrix);
					}
				}
			}
		}
		OutlineManager.BlurRT(temporary, this.blurAmount, 4);
		this.blendGlowMaterial.SetTexture("_GlowTexture", temporary);
		UnityEngine.Graphics.Blit(source, destination, this.blendGlowMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
	}
}
