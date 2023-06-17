using System;
using UnityEngine;

// Token: 0x02000733 RID: 1843
public class ProcessedTexture
{
	// Token: 0x040023D8 RID: 9176
	protected RenderTexture result;

	// Token: 0x040023D9 RID: 9177
	protected Material material;

	// Token: 0x06002832 RID: 10290 RVA: 0x0001F57C File Offset: 0x0001D77C
	public void Dispose()
	{
		this.DestroyRenderTexture(ref this.result);
		this.DestroyMaterial(ref this.material);
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x0001F596 File Offset: 0x0001D796
	protected RenderTexture CreateRenderTexture(string name, int width, int height, bool linear)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.anisoLevel = 0;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x0001F5CD File Offset: 0x0001D7CD
	protected void DestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt == null)
		{
			return;
		}
		Object.Destroy(rt);
		rt = null;
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x000CF3BC File Offset: 0x000CD5BC
	protected RenderTexture CreateTemporary()
	{
		return RenderTexture.GetTemporary(this.result.width, this.result.height, this.result.depth, this.result.format, this.result.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
	}

	// Token: 0x06002836 RID: 10294 RVA: 0x0001F5E4 File Offset: 0x0001D7E4
	protected void ReleaseTemporary(RenderTexture rt)
	{
		RenderTexture.ReleaseTemporary(rt);
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x0001F5EC File Offset: 0x0001D7EC
	protected Material CreateMaterial(string shader)
	{
		return this.CreateMaterial(Shader.Find(shader));
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x0001F5FA File Offset: 0x0001D7FA
	protected Material CreateMaterial(Shader shader)
	{
		return new Material(shader)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x0001F5CD File Offset: 0x0001D7CD
	protected void DestroyMaterial(ref Material mat)
	{
		if (mat == null)
		{
			return;
		}
		Object.Destroy(mat);
		mat = null;
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x0001F60A File Offset: 0x0001D80A
	public static implicit operator Texture(ProcessedTexture t)
	{
		return t.result;
	}
}
