using System;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class ChangeSignText : UIDialog
{
	// Token: 0x04001F2E RID: 7982
	public Action<Texture2D> onUpdateTexture;

	// Token: 0x04001F2F RID: 7983
	public GameObject objectContainer;

	// Token: 0x0600230B RID: 8971 RVA: 0x000BAA2C File Offset: 0x000B8C2C
	public void Setup(MeshPaintableSource source)
	{
		source.Init();
		this.objectContainer.transform.DestroyAllChildren(true);
		GameObject gameObject = Object.Instantiate<GameObject>(source.sourceObject);
		gameObject.transform.SetParent(this.objectContainer.transform, false);
		gameObject.transform.localPosition = source.localPosition;
		gameObject.transform.localEulerAngles = source.localRotation;
		gameObject.layer = this.objectContainer.layer;
		if (gameObject.GetComponent<Collider>())
		{
			Object.DestroyImmediate(gameObject.GetComponent<Collider>());
		}
		if (gameObject.GetComponent<MeshPaintableSource>())
		{
			Object.DestroyImmediate(gameObject.GetComponent<MeshPaintableSource>());
		}
		MeshPaintable meshPaintable = gameObject.AddComponent<MeshPaintable>();
		meshPaintable.replacementTextureName = source.replacementTextureName;
		meshPaintable.textureWidth = source.texture.width;
		meshPaintable.textureHeight = source.texture.height;
		meshPaintable.Init();
		meshPaintable.targetTexture.SetPixels32(source.texture.GetPixels32());
		meshPaintable.targetTexture.Apply();
		gameObject.AddComponent<MeshCollider>().sharedMesh = source.collisionMesh;
		base.GetComponentInChildren<Camera>().fieldOfView = source.cameraFOV;
		Vector3 localPosition = this.objectContainer.transform.localPosition;
		localPosition.z = source.cameraDistance;
		this.objectContainer.transform.localPosition = localPosition;
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000BAB84 File Offset: 0x000B8D84
	public void UpdateSign()
	{
		MeshPaintable componentInChildren = base.GetComponentInChildren<MeshPaintable>();
		if (componentInChildren && componentInChildren.targetTexture && componentInChildren.hasChanges)
		{
			if (this.onUpdateTexture != null)
			{
				this.onUpdateTexture.Invoke(componentInChildren.targetTexture);
			}
			componentInChildren.hasChanges = false;
		}
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void OpenDialog()
	{
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}
}
