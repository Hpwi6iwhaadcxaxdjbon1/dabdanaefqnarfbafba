using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class MeshPaintableSource : MonoBehaviour, IClientComponent
{
	// Token: 0x04000D0C RID: 3340
	public int texWidth = 256;

	// Token: 0x04000D0D RID: 3341
	public int texHeight = 128;

	// Token: 0x04000D0E RID: 3342
	public string replacementTextureName = "_DecalTexture";

	// Token: 0x04000D0F RID: 3343
	public float cameraFOV = 60f;

	// Token: 0x04000D10 RID: 3344
	public float cameraDistance = 2f;

	// Token: 0x04000D11 RID: 3345
	[NonSerialized]
	public Texture2D texture;

	// Token: 0x04000D12 RID: 3346
	public GameObject sourceObject;

	// Token: 0x04000D13 RID: 3347
	public Mesh collisionMesh;

	// Token: 0x04000D14 RID: 3348
	public Vector3 localPosition;

	// Token: 0x04000D15 RID: 3349
	public Vector3 localRotation;

	// Token: 0x04000D16 RID: 3350
	private static MaterialPropertyBlock block;

	// Token: 0x06001038 RID: 4152 RVA: 0x0006DCB4 File Offset: 0x0006BEB4
	public void Init()
	{
		if (this.texture)
		{
			return;
		}
		this.texture = new Texture2D(this.texWidth, this.texHeight, TextureFormat.ARGB32, false);
		this.texture.name = "MeshPaintableSource_" + base.gameObject.name;
		this.texture.Clear(Color.clear);
		if (MeshPaintableSource.block == null)
		{
			MeshPaintableSource.block = new MaterialPropertyBlock();
		}
		else
		{
			MeshPaintableSource.block.Clear();
		}
		MeshPaintableSource.block.SetTexture(this.replacementTextureName, this.texture);
		List<Renderer> list = Pool.GetList<Renderer>();
		base.transform.root.GetComponentsInChildren<Renderer>(true, list);
		foreach (Renderer renderer in list)
		{
			renderer.SetPropertyBlock(MeshPaintableSource.block);
		}
		Pool.FreeList<Renderer>(ref list);
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x0000E5A5 File Offset: 0x0000C7A5
	public void Free()
	{
		if (this.texture)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x0000E5C6 File Offset: 0x0000C7C6
	public void UpdateFrom(Texture2D input)
	{
		this.Init();
		this.texture.SetPixels32(input.GetPixels32());
		this.texture.Apply(true, false);
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x0006DDB4 File Offset: 0x0006BFB4
	public void Load(byte[] data)
	{
		this.Init();
		if (data != null)
		{
			ImageConversion.LoadImage(this.texture, data);
			if (this.texture.width != this.texWidth || this.texture.height != this.texHeight)
			{
				this.texture.Resize(this.texWidth, this.texHeight);
			}
			this.texture.Apply(true, false);
		}
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x0006DE24 File Offset: 0x0006C024
	public void Clear()
	{
		if (this.texture == null)
		{
			return;
		}
		this.texture.Clear(new Color(0f, 0f, 0f, 0f));
		this.texture.Apply(true, false);
	}
}
