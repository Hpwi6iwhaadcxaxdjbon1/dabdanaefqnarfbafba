using System;
using UnityEngine;

// Token: 0x020006D7 RID: 1751
public class UI_LocalVoice : SingletonComponent<UI_LocalVoice>
{
	// Token: 0x040022DA RID: 8922
	public CanvasGroup voiceCanvas;

	// Token: 0x040022DB RID: 8923
	public CanvasGroup levelImage;

	// Token: 0x060026D0 RID: 9936 RVA: 0x0001E484 File Offset: 0x0001C684
	protected override void Awake()
	{
		base.Awake();
		this.voiceCanvas.alpha = 0f;
		this.levelImage.alpha = 0.2f;
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x00002ECE File Offset: 0x000010CE
	public void Update()
	{
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x0001E4AC File Offset: 0x0001C6AC
	public void SetRecording(bool b)
	{
		this.voiceCanvas.alpha = (b ? 1f : 0f);
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x0001E4C8 File Offset: 0x0001C6C8
	public void SetLevel(float f)
	{
		this.levelImage.alpha = f;
	}
}
