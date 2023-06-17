using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007A8 RID: 1960
internal class ExplosionsSpriteSheetAnimation : MonoBehaviour
{
	// Token: 0x04002620 RID: 9760
	public int TilesX = 4;

	// Token: 0x04002621 RID: 9761
	public int TilesY = 4;

	// Token: 0x04002622 RID: 9762
	public float AnimationFPS = 30f;

	// Token: 0x04002623 RID: 9763
	public bool IsInterpolateFrames;

	// Token: 0x04002624 RID: 9764
	public int StartFrameOffset;

	// Token: 0x04002625 RID: 9765
	public bool IsLoop = true;

	// Token: 0x04002626 RID: 9766
	public float StartDelay;

	// Token: 0x04002627 RID: 9767
	public AnimationCurve FrameOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04002628 RID: 9768
	private bool isInizialised;

	// Token: 0x04002629 RID: 9769
	private int index;

	// Token: 0x0400262A RID: 9770
	private int count;

	// Token: 0x0400262B RID: 9771
	private int allCount;

	// Token: 0x0400262C RID: 9772
	private float animationLifeTime;

	// Token: 0x0400262D RID: 9773
	private bool isVisible;

	// Token: 0x0400262E RID: 9774
	private bool isCorutineStarted;

	// Token: 0x0400262F RID: 9775
	private Renderer currentRenderer;

	// Token: 0x04002630 RID: 9776
	private Material instanceMaterial;

	// Token: 0x04002631 RID: 9777
	private float currentInterpolatedTime;

	// Token: 0x04002632 RID: 9778
	private float animationStartTime;

	// Token: 0x04002633 RID: 9779
	private bool animationStoped;

	// Token: 0x06002A88 RID: 10888 RVA: 0x0002113B File Offset: 0x0001F33B
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x000D8EE0 File Offset: 0x000D70E0
	private void InitDefaultVariables()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		if (this.currentRenderer == null)
		{
			throw new Exception("UvTextureAnimator can't get renderer");
		}
		if (!this.currentRenderer.enabled)
		{
			this.currentRenderer.enabled = true;
		}
		this.allCount = 0;
		this.animationStoped = false;
		this.animationLifeTime = (float)(this.TilesX * this.TilesY) / this.AnimationFPS;
		this.count = this.TilesY * this.TilesX;
		this.index = this.TilesX - 1;
		Vector3 zero = Vector3.zero;
		this.StartFrameOffset -= this.StartFrameOffset / this.count * this.count;
		Vector2 value = new Vector2(1f / (float)this.TilesX, 1f / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", value);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x00021163 File Offset: 0x0001F363
	private void Play()
	{
		if (this.isCorutineStarted)
		{
			return;
		}
		if (this.StartDelay > 0.0001f)
		{
			base.Invoke("PlayDelay", this.StartDelay);
		}
		else
		{
			base.StartCoroutine(this.UpdateCorutine());
		}
		this.isCorutineStarted = true;
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x000211A2 File Offset: 0x0001F3A2
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x000211B1 File Offset: 0x0001F3B1
	private void OnEnable()
	{
		if (!this.isInizialised)
		{
			return;
		}
		this.InitDefaultVariables();
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06002A8D RID: 10893 RVA: 0x000211CF File Offset: 0x0001F3CF
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x06002A8E RID: 10894 RVA: 0x000211F0 File Offset: 0x0001F3F0
	private IEnumerator UpdateCorutine()
	{
		this.animationStartTime = Time.time;
		while (this.isVisible && (this.IsLoop || !this.animationStoped))
		{
			this.UpdateFrame();
			if (!this.IsLoop && this.animationStoped)
			{
				break;
			}
			float value = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num = this.FrameOverTime.Evaluate(Mathf.Clamp01(value));
			yield return new WaitForSeconds(1f / (this.AnimationFPS * num));
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x000D9004 File Offset: 0x000D7204
	private void UpdateFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		if (this.count == this.allCount)
		{
			this.animationStartTime = Time.time;
			this.allCount = 0;
			this.animationStoped = true;
		}
		Vector2 value = new Vector2((float)this.index / (float)this.TilesX - (float)(this.index / this.TilesX), 1f - (float)(this.index / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", value);
		}
		if (this.IsInterpolateFrames)
		{
			this.currentInterpolatedTime = 0f;
		}
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000D90DC File Offset: 0x000D72DC
	private void Update()
	{
		if (!this.IsInterpolateFrames)
		{
			return;
		}
		this.currentInterpolatedTime += Time.deltaTime;
		int num = this.index + 1;
		if (this.allCount == 0)
		{
			num = this.index;
		}
		Vector4 value = new Vector4(1f / (float)this.TilesX, 1f / (float)this.TilesY, (float)num / (float)this.TilesX - (float)(num / this.TilesX), 1f - (float)(num / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetVector("_MainTex_NextFrame", value);
			float value2 = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num2 = this.FrameOverTime.Evaluate(Mathf.Clamp01(value2));
			this.instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(this.currentInterpolatedTime * this.AnimationFPS * num2));
		}
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000211FF File Offset: 0x0001F3FF
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}
}
