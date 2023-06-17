using System;
using Rust;
using UnityEngine;

// Token: 0x02000423 RID: 1059
public class WorldModelOutline : OutlineObject
{
	// Token: 0x04001613 RID: 5651
	private CoverageQueries.Query query;

	// Token: 0x04001614 RID: 5652
	[NonSerialized]
	private const float coverageRadius = 0.06f;

	// Token: 0x04001615 RID: 5653
	private float currentOcclusion;

	// Token: 0x0600198E RID: 6542 RVA: 0x00090714 File Offset: 0x0008E914
	public override Color GetColor()
	{
		float num = Mathf.Clamp01(1f - (MainCamera.Distance(base.transform.position) - OutlineManager.worldModelDistance * 0.8f) / (OutlineManager.worldModelDistance - OutlineManager.worldModelDistance * 0.8f));
		float num2 = 1f;
		if (TOD_Sky.Instance != null)
		{
			num2 = Mathf.Clamp01(0f + 1f * TOD_Sky.Instance.LerpValue);
		}
		float smoothOcclusion = this.GetSmoothOcclusion();
		Color color = base.GetColor();
		if (base.GetCollider() == LocalPlayer.Entity.lookingAtCollider)
		{
			color.a = 0.2f + 0.8f * Mathf.Clamp01(num2 * 2f);
		}
		else
		{
			color.a = 0.8f * Mathf.Clamp01(smoothOcclusion * 3f) * num2;
		}
		color.a *= num;
		return color;
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000151D0 File Offset: 0x000133D0
	public void OnEnable()
	{
		if (CoverageQueries.Supported)
		{
			this.query = new CoverageQueries.Query(base.transform.position, CoverageQueries.RadiusSpace.ScreenNormalized, 0.06f, 100, 50f);
			this.query.Register();
		}
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x00015207 File Offset: 0x00013407
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.query != null)
		{
			this.query.Unregister();
			this.query = null;
		}
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x0001522B File Offset: 0x0001342B
	public override void BecomeVisible()
	{
		this.currentOcclusion = this.SampleOcclusion();
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x00015239 File Offset: 0x00013439
	public override void BecomeInvisible()
	{
		this.currentOcclusion = 1f;
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x000907F8 File Offset: 0x0008E9F8
	public override bool ShouldDisplay()
	{
		bool flag = base.ShouldDisplay();
		return MainCamera.Distance(base.transform.position) <= OutlineManager.worldModelDistance && flag;
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x00015246 File Offset: 0x00013446
	public virtual float GetSmoothOcclusion()
	{
		this.currentOcclusion = Mathf.Lerp(this.currentOcclusion, this.SampleOcclusion(), Time.deltaTime * 2f);
		return this.currentOcclusion;
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x00090828 File Offset: 0x0008EA28
	public virtual float SampleOcclusion()
	{
		if (this.query != null && this.query.IsRegistered)
		{
			Collider collider = base.GetCollider();
			if (collider)
			{
				Vector3 a = collider.ClosestPointOnBounds(base.transform.position + Vector3.up * 10f);
				this.query.Update(a + Vector3.up * 0.06f, 0.06f);
				return Mathf.Clamp01(this.query.result.smoothCoverage);
			}
		}
		return 0f;
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x000908C0 File Offset: 0x0008EAC0
	public override float SampleVisibility()
	{
		float num = base.SampleVisibility();
		if (num == 0f)
		{
			this.currentOcclusion = 1f;
		}
		else if (this.query == null || !this.query.IsRegistered)
		{
			this.currentOcclusion = Mathf.Clamp01(1f - num);
		}
		else
		{
			this.currentOcclusion = this.SampleOcclusion();
		}
		return num;
	}
}
