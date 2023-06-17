using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006B6 RID: 1718
public class ProgressBar : UIBehaviour
{
	// Token: 0x04002263 RID: 8803
	public static ProgressBar Instance;

	// Token: 0x04002264 RID: 8804
	private Action<BasePlayer> action;

	// Token: 0x04002265 RID: 8805
	private float timeFinished;

	// Token: 0x04002266 RID: 8806
	private float timeCounter;

	// Token: 0x04002267 RID: 8807
	public GameObject scaleTarget;

	// Token: 0x04002268 RID: 8808
	public Image progressField;

	// Token: 0x04002269 RID: 8809
	public Image iconField;

	// Token: 0x0400226A RID: 8810
	public Text leftField;

	// Token: 0x0400226B RID: 8811
	public Text rightField;

	// Token: 0x0400226C RID: 8812
	public SoundDefinition clipOpen;

	// Token: 0x0400226D RID: 8813
	public SoundDefinition clipCancel;

	// Token: 0x0400226E RID: 8814
	public bool IsOpen;

	// Token: 0x0400226F RID: 8815
	private bool isClosing;

	// Token: 0x04002270 RID: 8816
	private CanvasGroup canvasGroup;

	// Token: 0x06002638 RID: 9784 RVA: 0x000C9960 File Offset: 0x000C7B60
	protected override void Start()
	{
		base.Start();
		ProgressBar.Instance = this;
		this.canvasGroup = base.GetComponentInChildren<CanvasGroup>();
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.IsOpen = false;
		this.isClosing = true;
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x000C99BC File Offset: 0x000C7BBC
	public void Open(string name, Sprite sprite, Action<BasePlayer> action, float time)
	{
		this.timeCounter = 0f;
		this.timeFinished = time;
		this.action = action;
		this.IsOpen = true;
		this.isClosing = false;
		this.iconField.sprite = sprite;
		this.leftField.text = name;
		this.rightField.text = string.Empty;
		this.UpdateProgressBar();
		this.PlayOpenSound();
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(21);
		this.scaleTarget.transform.transform.localScale = Vector3.one * 1f;
		LeanTween.scale(this.scaleTarget, Vector3.one, 0.1f).setEase(24);
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x000C9AAC File Offset: 0x000C7CAC
	public void UpdateProgressBar()
	{
		Vector3 localScale = this.progressField.transform.localScale;
		localScale.x = Mathf.Clamp01(this.timeCounter / this.timeFinished);
		this.progressField.transform.localScale = localScale;
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x000C9AF4 File Offset: 0x000C7CF4
	public void PlayOpenSound()
	{
		SoundManager.PlayOneshot(this.clipOpen, null, true, default(Vector3));
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x000C9B18 File Offset: 0x000C7D18
	public void PlayCancelSound()
	{
		SoundManager.PlayOneshot(this.clipCancel, null, true, default(Vector3));
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x000C9B3C File Offset: 0x000C7D3C
	public void Close(bool success = false)
	{
		if (this.isClosing)
		{
			return;
		}
		this.isClosing = true;
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(21);
		LeanTween.scale(this.scaleTarget, Vector3.one * (success ? 1.5f : 0.5f), 0.2f).setEase(21);
		this.IsOpen = false;
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000C9BC4 File Offset: 0x000C7DC4
	private void Update()
	{
		if (this.IsOpen)
		{
			this.timeCounter += Time.deltaTime;
			this.UpdateProgressBar();
			if (this.timeCounter >= this.timeFinished)
			{
				this.action.Invoke(LocalPlayer.Entity);
				this.Close(true);
				return;
			}
			LookatHealth.Enabled = false;
			LookatTooltip.Enabled = false;
		}
	}
}
