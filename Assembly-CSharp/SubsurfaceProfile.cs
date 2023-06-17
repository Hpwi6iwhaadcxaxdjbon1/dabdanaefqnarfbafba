using System;
using Rust;
using UnityEngine;

// Token: 0x0200058E RID: 1422
public class SubsurfaceProfile : ScriptableObject
{
	// Token: 0x04001C76 RID: 7286
	private static SubsurfaceProfileTexture profileTexture = new SubsurfaceProfileTexture();

	// Token: 0x04001C77 RID: 7287
	public SubsurfaceProfileData Data = SubsurfaceProfileData.Default;

	// Token: 0x04001C78 RID: 7288
	private int id = -1;

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x0600207B RID: 8315 RVA: 0x00019B24 File Offset: 0x00017D24
	public static Texture2D Texture
	{
		get
		{
			if (SubsurfaceProfile.profileTexture == null)
			{
				return null;
			}
			return SubsurfaceProfile.profileTexture.Texture;
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x0600207C RID: 8316 RVA: 0x00019B39 File Offset: 0x00017D39
	public int Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x00019B41 File Offset: 0x00017D41
	private void OnEnable()
	{
		this.id = SubsurfaceProfile.profileTexture.AddProfile(this.Data, this);
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x00019B5A File Offset: 0x00017D5A
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		SubsurfaceProfile.profileTexture.RemoveProfile(this.id);
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x00019B74 File Offset: 0x00017D74
	public void Update()
	{
		SubsurfaceProfile.profileTexture.UpdateProfile(this.id, this.Data);
	}
}
