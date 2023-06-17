using System;
using ConVar;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E2 RID: 1762
public class UIRootScaled : UIRoot
{
	// Token: 0x040022FB RID: 8955
	private static UIRootScaled Instance;

	// Token: 0x040022FC RID: 8956
	public CanvasScaler scaler;

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x060026FD RID: 9981 RVA: 0x0001E6F0 File Offset: 0x0001C8F0
	public static Canvas DragOverlayCanvas
	{
		get
		{
			return UIRootScaled.Instance.overlayCanvas;
		}
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x0001E6FC File Offset: 0x0001C8FC
	protected override void Awake()
	{
		UIRootScaled.Instance = this;
		base.Awake();
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000CBA10 File Offset: 0x000C9C10
	protected override void Refresh()
	{
		Vector2 vector = new Vector2(1280f / ConVar.Graphics.uiscale, 720f / ConVar.Graphics.uiscale);
		if (this.scaler.referenceResolution != vector)
		{
			this.scaler.referenceResolution = vector;
		}
	}
}
