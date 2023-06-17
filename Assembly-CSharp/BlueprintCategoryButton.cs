using System;
using Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000643 RID: 1603
public class BlueprintCategoryButton : MonoBehaviour, IInventoryChanged, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x04001FE0 RID: 8160
	public Text amountLabel;

	// Token: 0x04001FE1 RID: 8161
	public ItemCategory Category;

	// Token: 0x04001FE2 RID: 8162
	public GameObject BackgroundHighlight;

	// Token: 0x04001FE3 RID: 8163
	public SoundDefinition clickSound;

	// Token: 0x04001FE4 RID: 8164
	public SoundDefinition hoverSound;

	// Token: 0x04001FE5 RID: 8165
	private int CurrentAmount = -1;

	// Token: 0x04001FE6 RID: 8166
	private bool Selected;

	// Token: 0x060023BC RID: 9148 RVA: 0x0001C377 File Offset: 0x0001A577
	private void OnEnable()
	{
		GlobalMessages.onInventoryChanged.Add(this);
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x0001C384 File Offset: 0x0001A584
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x0001C41A File Offset: 0x0001A61A
	public void OnInventoryChanged()
	{
		this.UpdateValue();
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000BD984 File Offset: 0x000BBB84
	private void UpdateValue()
	{
		if (this.Category == ItemCategory.Common)
		{
			this.amountLabel.gameObject.SetActive(false);
			return;
		}
		int num = UIBlueprints.CountForCategory(this.Category, true);
		if (this.CurrentAmount == num)
		{
			return;
		}
		this.amountLabel.text = string.Format("{0:N0}", num);
		this.CurrentAmount = num;
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000BD9E8 File Offset: 0x000BBBE8
	public void ValueChanged(bool b)
	{
		this.Selected = b;
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.BackgroundHighlight);
		if (b)
		{
			LeanTween.scale(base.gameObject, Vector3.one * 1.1f, 0.1f).setEase(27);
			LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
			LeanTween.alphaCanvas(this.BackgroundHighlight.GetComponent<CanvasGroup>(), 1f, 0.04f);
			LeanTween.scale(this.BackgroundHighlight, Vector3.one, 0.1f).setEase(28);
			SoundManager.PlayOneshot(this.clickSound, null, true, default(Vector3));
			return;
		}
		LeanTween.scale(this.BackgroundHighlight, new Vector3(0f, 1f, 1f), 1f);
		LeanTween.scale(base.gameObject, Vector3.one, 1f).setDelay(0.4f);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0.4f, 0.6f).setDelay(0.1f);
		LeanTween.alphaCanvas(this.BackgroundHighlight.GetComponent<CanvasGroup>(), 0f, 0.2f);
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000BDB28 File Offset: 0x000BBD28
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.Selected)
		{
			return;
		}
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0.9f, 0.1f);
		LeanTween.scale(base.gameObject, Vector3.one * 1.1f, 0.5f).setEase(TweenMode.Punch);
		SoundManager.PlayOneshot(this.hoverSound, null, true, default(Vector3));
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x0001C422 File Offset: 0x0001A622
	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.Selected)
		{
			return;
		}
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0.4f, 0.1f);
	}
}
