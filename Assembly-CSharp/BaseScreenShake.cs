using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200028C RID: 652
public abstract class BaseScreenShake : MonoBehaviour
{
	// Token: 0x04000F2A RID: 3882
	public static List<BaseScreenShake> list = new List<BaseScreenShake>();

	// Token: 0x04000F2B RID: 3883
	public float length = 2f;

	// Token: 0x04000F2C RID: 3884
	internal float timeTaken;

	// Token: 0x04000F2D RID: 3885
	private int currentFrame = -1;

	// Token: 0x06001282 RID: 4738 RVA: 0x00078F68 File Offset: 0x00077168
	public static void Apply(Camera cam, BaseViewModel vm)
	{
		CachedTransform<Camera> cachedTransform = new CachedTransform<Camera>(cam);
		CachedTransform<BaseViewModel> cachedTransform2 = new CachedTransform<BaseViewModel>(vm);
		for (int i = 0; i < BaseScreenShake.list.Count; i++)
		{
			BaseScreenShake.list[i].Run(ref cachedTransform, ref cachedTransform2);
		}
		cachedTransform.Apply();
		cachedTransform2.Apply();
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x0000FE7C File Offset: 0x0000E07C
	protected void OnEnable()
	{
		BaseScreenShake.list.Add(this);
		this.timeTaken = 0f;
		this.Setup();
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x0000FE9A File Offset: 0x0000E09A
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		BaseScreenShake.list.Remove(this);
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x00078FBC File Offset: 0x000771BC
	public void Run(ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (this.timeTaken > this.length)
		{
			return;
		}
		if (Time.frameCount != this.currentFrame)
		{
			this.timeTaken += Time.deltaTime;
			this.currentFrame = Time.frameCount;
		}
		float delta = Mathf.InverseLerp(0f, this.length, this.timeTaken);
		this.Run(delta, ref cam, ref vm);
	}

	// Token: 0x06001286 RID: 4742
	public abstract void Setup();

	// Token: 0x06001287 RID: 4743
	public abstract void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm);
}
