using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020006B2 RID: 1714
[ExecuteInEditMode]
public class CameraEx : MonoBehaviour
{
	// Token: 0x04002245 RID: 8773
	public bool overrideAmbientLight;

	// Token: 0x04002246 RID: 8774
	public AmbientMode ambientMode;

	// Token: 0x04002247 RID: 8775
	public Color ambientGroundColor;

	// Token: 0x04002248 RID: 8776
	public Color ambientEquatorColor;

	// Token: 0x04002249 RID: 8777
	public Color ambientLight;

	// Token: 0x0400224A RID: 8778
	public float ambientIntensity;

	// Token: 0x0400224B RID: 8779
	internal Color old_ambientLight;

	// Token: 0x0400224C RID: 8780
	internal Color old_ambientGroundColor;

	// Token: 0x0400224D RID: 8781
	internal Color old_ambientEquatorColor;

	// Token: 0x0400224E RID: 8782
	internal float old_ambientIntensity;

	// Token: 0x0400224F RID: 8783
	internal AmbientMode old_ambientMode;

	// Token: 0x04002250 RID: 8784
	public float aspect;

	// Token: 0x06002626 RID: 9766 RVA: 0x000C9408 File Offset: 0x000C7608
	private void OnPreRender()
	{
		if (this.overrideAmbientLight)
		{
			this.old_ambientLight = RenderSettings.ambientLight;
			this.old_ambientIntensity = RenderSettings.ambientIntensity;
			this.old_ambientMode = RenderSettings.ambientMode;
			this.old_ambientGroundColor = RenderSettings.ambientGroundColor;
			this.old_ambientEquatorColor = RenderSettings.ambientEquatorColor;
			this.old_ambientGroundColor = RenderSettings.ambientGroundColor;
			RenderSettings.ambientMode = this.ambientMode;
			RenderSettings.ambientLight = this.ambientLight;
			RenderSettings.ambientIntensity = this.ambientIntensity;
			RenderSettings.ambientGroundColor = this.ambientLight;
			RenderSettings.ambientEquatorColor = this.ambientEquatorColor;
			RenderSettings.ambientGroundColor = this.ambientGroundColor;
		}
		if (this.aspect > 0f)
		{
			base.GetComponent<Camera>().aspect = this.aspect;
		}
	}

	// Token: 0x06002627 RID: 9767 RVA: 0x000C94C4 File Offset: 0x000C76C4
	private void OnPostRender()
	{
		if (this.overrideAmbientLight)
		{
			RenderSettings.ambientMode = this.ambientMode;
			RenderSettings.ambientLight = this.old_ambientLight;
			RenderSettings.ambientIntensity = this.old_ambientIntensity;
			RenderSettings.ambientMode = this.old_ambientMode;
			RenderSettings.ambientGroundColor = this.old_ambientGroundColor;
			RenderSettings.ambientEquatorColor = this.old_ambientEquatorColor;
			RenderSettings.ambientGroundColor = this.old_ambientGroundColor;
		}
	}
}
