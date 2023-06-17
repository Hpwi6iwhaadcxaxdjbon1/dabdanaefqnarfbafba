using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200059A RID: 1434
public class FoliageDisplacementManager : SingletonComponent<FoliageDisplacementManager>, IClientComponent
{
	// Token: 0x04001CA6 RID: 7334
	private SpecialPurposeCamera handle = new SpecialPurposeCamera("Foliage Displacement");

	// Token: 0x060020C1 RID: 8385 RVA: 0x00019FA1 File Offset: 0x000181A1
	public static void Add(FoliageDisplacement instance)
	{
		if (SingletonComponent<FoliageDisplacementManager>.Instance)
		{
			SingletonComponent<FoliageDisplacementManager>.Instance.handle.Add(instance.transform, instance.mesh, instance.material, instance.moving, instance.billboard);
		}
	}

	// Token: 0x060020C2 RID: 8386 RVA: 0x00019FDC File Offset: 0x000181DC
	public static void Remove(FoliageDisplacement instance)
	{
		if (SingletonComponent<FoliageDisplacementManager>.Instance)
		{
			SingletonComponent<FoliageDisplacementManager>.Instance.handle.Remove(instance.transform, instance.mesh, instance.material, instance.moving, instance.billboard);
		}
	}

	// Token: 0x060020C3 RID: 8387 RVA: 0x0001A017 File Offset: 0x00018217
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.DestroyHandle();
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x000B1A08 File Offset: 0x000AFC08
	private void CreateHandle()
	{
		this.handle.Create(512, 512, 0, RenderTextureFormat.ARGB32);
		this.handle.orthographicSize = 32f;
		this.handle.nearClipPlane = 4f;
		this.handle.farClipPlane = 64f;
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x0001A027 File Offset: 0x00018227
	private void DestroyHandle()
	{
		this.handle.Destroy();
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x000B1A5C File Offset: 0x000AFC5C
	protected void Update()
	{
		if (!MainCamera.mainCamera)
		{
			return;
		}
		if (!this.handle.created)
		{
			this.CreateHandle();
		}
		Vector3 vector = MainCamera.position + Vector3.up * 32f;
		float orthographicSize = this.handle.orthographicSize;
		Vector2 v = new Vector2(vector.x - orthographicSize, vector.z - orthographicSize);
		Vector2 v2 = new Vector2(vector.x + orthographicSize, vector.z + orthographicSize);
		if (!Grass.displace)
		{
			Shader.SetGlobalVector("_FoliageDisplaceMin", v);
			Shader.SetGlobalVector("_FoliageDisplaceMax", v2);
			Shader.SetGlobalTexture("_FoliageDisplaceTex", Texture2D.blackTexture);
			return;
		}
		this.handle.position = vector;
		this.handle.Refresh();
		Shader.SetGlobalVector("_FoliageDisplaceMin", v);
		Shader.SetGlobalVector("_FoliageDisplaceMax", v2);
		Shader.SetGlobalTexture("_FoliageDisplaceTex", this.handle.texture);
	}
}
