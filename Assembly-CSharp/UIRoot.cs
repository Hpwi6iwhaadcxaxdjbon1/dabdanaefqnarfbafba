using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E0 RID: 1760
public abstract class UIRoot : MonoBehaviour
{
	// Token: 0x040022F9 RID: 8953
	private GraphicRaycaster[] graphicRaycasters;

	// Token: 0x040022FA RID: 8954
	public Canvas overlayCanvas;

	// Token: 0x060026F5 RID: 9973 RVA: 0x000CB9D4 File Offset: 0x000C9BD4
	private void ToggleRaycasters(bool state)
	{
		for (int i = 0; i < this.graphicRaycasters.Length; i++)
		{
			GraphicRaycaster graphicRaycaster = this.graphicRaycasters[i];
			if (graphicRaycaster.enabled != state)
			{
				graphicRaycaster.enabled = state;
			}
		}
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x0001E6C4 File Offset: 0x0001C8C4
	protected virtual void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x0001E6D1 File Offset: 0x0001C8D1
	protected virtual void Start()
	{
		this.graphicRaycasters = base.GetComponentsInChildren<GraphicRaycaster>(true);
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x0001E6E0 File Offset: 0x0001C8E0
	protected void Update()
	{
		this.Refresh();
	}

	// Token: 0x060026F9 RID: 9977
	protected abstract void Refresh();
}
