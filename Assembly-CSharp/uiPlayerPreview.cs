using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020006B4 RID: 1716
public class uiPlayerPreview : SingletonComponent<uiPlayerPreview>
{
	// Token: 0x04002256 RID: 8790
	public Camera previewCamera;

	// Token: 0x04002257 RID: 8791
	public PlayerModel playermodel;

	// Token: 0x04002258 RID: 8792
	public ReflectionProbe reflectionProbe;

	// Token: 0x04002259 RID: 8793
	public SegmentMaskPositioning segmentMask;

	// Token: 0x0400225A RID: 8794
	internal PlayerModel needsUpdateFrom;

	// Token: 0x0400225B RID: 8795
	private PostProcessLayer postProcessLayer;

	// Token: 0x0400225C RID: 8796
	private bool wasOpen;

	// Token: 0x0600262B RID: 9771 RVA: 0x0001DBE3 File Offset: 0x0001BDE3
	public static void Create()
	{
		if (SingletonComponent<uiPlayerPreview>.Instance)
		{
			return;
		}
		GameManager.client.CreatePrefab("assets/prefabs/player/player_preview.prefab", true).Identity();
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x0001DC07 File Offset: 0x0001BE07
	private void Start()
	{
		this.postProcessLayer = this.previewCamera.GetComponent<PostProcessLayer>();
		this.playermodel.censorshipCube.layer = this.playermodel.gameObject.layer;
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x0001DC3A File Offset: 0x0001BE3A
	public void UpdateFrom(PlayerModel mdl)
	{
		if (this.playermodel == null)
		{
			return;
		}
		this.needsUpdateFrom = mdl;
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x000C97CC File Offset: 0x000C79CC
	private void Update()
	{
		if (!UIInventory.isOpen)
		{
			this.previewCamera.enabled = false;
			this.wasOpen = false;
			return;
		}
		if (UIInventory.isOpen && !this.wasOpen)
		{
			this.postProcessLayer.ResetHistory();
			this.wasOpen = true;
		}
		if (this.needsUpdateFrom)
		{
			this.playermodel.UpdateSkeleton(this.needsUpdateFrom.skeletonScale.seed);
			this.playermodel.UpdateFrom(this.needsUpdateFrom);
			this.needsUpdateFrom = null;
			if (LocalPlayer.Entity)
			{
				LocalPlayer.Entity.UpdateClothingItems(this.playermodel.multiMesh);
			}
			this.CleanupLayers();
		}
		this.previewCamera.enabled = true;
		this.segmentMask.Refresh();
		this.UpdatePlayerLookAt();
		if (LocalPlayer.Entity)
		{
			this.playermodel.voiceVolume = LocalPlayer.Entity.voiceSpeaker.currentVolume;
		}
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x00002ECE File Offset: 0x000010CE
	private void UpdatePlayerLookAt()
	{
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000C98C0 File Offset: 0x000C7AC0
	private void CleanupLayers()
	{
		Light[] componentsInChildren = this.playermodel.GetComponentsInChildren<Light>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].cullingMask = 524288;
		}
	}
}
