using System;
using UnityEngine;

// Token: 0x0200079F RID: 1951
public class ExplosionsFPS : MonoBehaviour
{
	// Token: 0x040025EE RID: 9710
	private readonly GUIStyle guiStyleHeader = new GUIStyle();

	// Token: 0x040025EF RID: 9711
	private float timeleft;

	// Token: 0x040025F0 RID: 9712
	private float fps;

	// Token: 0x040025F1 RID: 9713
	private int frames;

	// Token: 0x06002A66 RID: 10854 RVA: 0x00020EEC File Offset: 0x0001F0EC
	private void Awake()
	{
		this.guiStyleHeader.fontSize = 14;
		this.guiStyleHeader.normal.textColor = new Color(1f, 1f, 1f);
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x00020F1F File Offset: 0x0001F11F
	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 30f, 30f), "FPS: " + (int)this.fps, this.guiStyleHeader);
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000D88DC File Offset: 0x000D6ADC
	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.fps = (float)this.frames;
			this.timeleft = 1f;
			this.frames = 0;
		}
	}
}
