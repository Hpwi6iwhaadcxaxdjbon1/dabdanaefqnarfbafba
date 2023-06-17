using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006AC RID: 1708
[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
	// Token: 0x04002210 RID: 8720
	public static PieMenu Instance;

	// Token: 0x04002211 RID: 8721
	public Image middleBox;

	// Token: 0x04002212 RID: 8722
	public PieShape pieBackgroundBlur;

	// Token: 0x04002213 RID: 8723
	public PieShape pieBackground;

	// Token: 0x04002214 RID: 8724
	public PieShape pieSelection;

	// Token: 0x04002215 RID: 8725
	public GameObject pieOptionPrefab;

	// Token: 0x04002216 RID: 8726
	public GameObject optionsCanvas;

	// Token: 0x04002217 RID: 8727
	public PieMenu.MenuOption[] options;

	// Token: 0x04002218 RID: 8728
	public GameObject scaleTarget;

	// Token: 0x04002219 RID: 8729
	public float sliceGaps = 10f;

	// Token: 0x0400221A RID: 8730
	[Range(0f, 1f)]
	public float outerSize = 1f;

	// Token: 0x0400221B RID: 8731
	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	// Token: 0x0400221C RID: 8732
	[Range(0f, 1f)]
	public float iconSize = 0.8f;

	// Token: 0x0400221D RID: 8733
	[Range(0f, 360f)]
	public float startRadius;

	// Token: 0x0400221E RID: 8734
	[Range(0f, 360f)]
	public float radiusSize = 360f;

	// Token: 0x0400221F RID: 8735
	public Image middleImage;

	// Token: 0x04002220 RID: 8736
	public Text middleTitle;

	// Token: 0x04002221 RID: 8737
	public Text middleDesc;

	// Token: 0x04002222 RID: 8738
	public Text middleRequired;

	// Token: 0x04002223 RID: 8739
	public Color colorIconActive;

	// Token: 0x04002224 RID: 8740
	public Color colorIconHovered;

	// Token: 0x04002225 RID: 8741
	public Color colorIconDisabled;

	// Token: 0x04002226 RID: 8742
	public Color colorBackgroundDisabled;

	// Token: 0x04002227 RID: 8743
	public SoundDefinition clipOpen;

	// Token: 0x04002228 RID: 8744
	public SoundDefinition clipCancel;

	// Token: 0x04002229 RID: 8745
	public SoundDefinition clipChanged;

	// Token: 0x0400222A RID: 8746
	public SoundDefinition clipSelected;

	// Token: 0x0400222B RID: 8747
	public PieMenu.MenuOption defaultOption;

	// Token: 0x0400222C RID: 8748
	private bool isClosing;

	// Token: 0x0400222D RID: 8749
	private CanvasGroup canvasGroup;

	// Token: 0x0400222E RID: 8750
	public bool IsOpen;

	// Token: 0x0400222F RID: 8751
	internal PieMenu.MenuOption selectedOption;

	// Token: 0x04002230 RID: 8752
	private static AnimationCurve easePunch = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.112586f, 0.9976035f),
		new Keyframe(0.3120486f, 0.01720615f),
		new Keyframe(0.4316337f, 0.17030682f),
		new Keyframe(0.5524869f, 0.03141804f),
		new Keyframe(0.6549395f, 0.002909959f),
		new Keyframe(0.770987f, 0.009817753f),
		new Keyframe(0.8838775f, 0.001939224f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x0600260D RID: 9741 RVA: 0x000C862C File Offset: 0x000C682C
	protected override void Start()
	{
		base.Start();
		PieMenu.Instance = this;
		this.canvasGroup = base.GetComponentInChildren<CanvasGroup>();
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.IsOpen = false;
		this.isClosing = true;
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x0001DAFF File Offset: 0x0001BCFF
	public void Clear()
	{
		this.options = new PieMenu.MenuOption[0];
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000C8688 File Offset: 0x000C6888
	public void AddOption(PieMenu.MenuOption option)
	{
		List<PieMenu.MenuOption> list = Enumerable.ToList<PieMenu.MenuOption>(this.options);
		list.Add(option);
		this.options = list.ToArray();
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x000C86B4 File Offset: 0x000C68B4
	public void FinishAndOpen()
	{
		this.IsOpen = true;
		this.isClosing = false;
		this.SetDefaultOption();
		this.Rebuild();
		this.UpdateInteraction(false);
		this.PlayOpenSound();
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(21);
		this.scaleTarget.transform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(this.scaleTarget, Vector3.one, 0.1f).setEase(24);
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x0001DB0D File Offset: 0x0001BD0D
	protected override void OnEnable()
	{
		base.OnEnable();
		this.Rebuild();
	}

	// Token: 0x06002612 RID: 9746 RVA: 0x000C8768 File Offset: 0x000C6968
	public void SetDefaultOption()
	{
		this.defaultOption = null;
		for (int i = 0; i < this.options.Length; i++)
		{
			if (!this.options[i].disabled)
			{
				if (this.defaultOption == null)
				{
					this.defaultOption = this.options[i];
				}
				if (this.options[i].selected)
				{
					this.defaultOption = this.options[i];
					return;
				}
			}
		}
	}

	// Token: 0x06002613 RID: 9747 RVA: 0x000C87D4 File Offset: 0x000C69D4
	public void PlayOpenSound()
	{
		SoundManager.PlayOneshot(this.clipOpen, null, true, default(Vector3));
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x000C87F8 File Offset: 0x000C69F8
	public void PlayCancelSound()
	{
		SoundManager.PlayOneshot(this.clipCancel, null, true, default(Vector3));
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000C881C File Offset: 0x000C6A1C
	public void Close(bool success = false)
	{
		if (this.isClosing)
		{
			return;
		}
		this.isClosing = true;
		NeedsCursor component = base.GetComponent<NeedsCursor>();
		if (component != null)
		{
			component.enabled = false;
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(21);
		LeanTween.scale(this.scaleTarget, Vector3.one * (success ? 1.5f : 0.5f), 0.2f).setEase(21);
		this.IsOpen = false;
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000C88BC File Offset: 0x000C6ABC
	private void Update()
	{
		if (!Application.isPlaying)
		{
			this.Rebuild();
		}
		if (this.pieBackground.innerSize != this.innerSize || this.pieBackground.outerSize != this.outerSize || this.pieBackground.startRadius != this.startRadius || this.pieBackground.endRadius != this.startRadius + this.radiusSize)
		{
			this.pieBackground.startRadius = this.startRadius;
			this.pieBackground.endRadius = this.startRadius + this.radiusSize;
			this.pieBackground.innerSize = this.innerSize;
			this.pieBackground.outerSize = this.outerSize;
			this.pieBackground.SetVerticesDirty();
		}
		this.UpdateInteraction(true);
		if (this.IsOpen)
		{
			CursorManager.HoldOpen(false);
			IngameMenuBackground.Enabled = true;
		}
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000C899C File Offset: 0x000C6B9C
	public void Rebuild()
	{
		this.options = Enumerable.ToArray<PieMenu.MenuOption>(Enumerable.OrderBy<PieMenu.MenuOption, int>(this.options, (PieMenu.MenuOption x) => x.order));
		while (this.optionsCanvas.transform.childCount > 0)
		{
			GameManager.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject, true);
		}
		float num = this.radiusSize / (float)this.options.Length;
		for (int i = 0; i < this.options.Length; i++)
		{
			GameObject gameObject = Instantiate.GameObject(this.pieOptionPrefab, null);
			gameObject.transform.SetParent(this.optionsCanvas.transform, false);
			this.options[i].option = gameObject.GetComponent<PieOption>();
			this.options[i].option.UpdateOption(this.startRadius + (float)i * num - num * 0.25f, num, this.sliceGaps, this.options[i].name, this.outerSize, this.innerSize, this.iconSize, this.options[i].sprite);
		}
		this.selectedOption = null;
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x000C8AD0 File Offset: 0x000C6CD0
	public void UpdateInteraction(bool allowLerp = true)
	{
		if (this.isClosing)
		{
			return;
		}
		Vector3 vector = Input.mousePosition - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		float num = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		float num2 = this.radiusSize / (float)this.options.Length;
		for (int i = 0; i < this.options.Length; i++)
		{
			float target = this.startRadius + (float)i * num2 + num2 * 0.5f - num2 * 0.25f;
			if ((vector.magnitude < 32f && this.options[i] == this.defaultOption) || (vector.magnitude >= 32f && Mathf.Abs(Mathf.DeltaAngle(num, target)) < num2 * 0.5f))
			{
				if (allowLerp)
				{
					this.pieSelection.startRadius = Mathf.MoveTowardsAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius) * 30f + 10f));
					this.pieSelection.endRadius = Mathf.MoveTowardsAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius) * 30f + 10f));
				}
				else
				{
					this.pieSelection.startRadius = this.options[i].option.background.startRadius;
					this.pieSelection.endRadius = this.options[i].option.background.endRadius;
				}
				this.pieSelection.SetVerticesDirty();
				this.middleImage.sprite = this.options[i].sprite;
				this.middleTitle.text = this.options[i].name;
				this.middleDesc.text = this.options[i].desc;
				this.middleRequired.text = "";
				string text = this.options[i].requirements;
				if (text != null)
				{
					text = text.Replace("[e]", "<color=#CD412B>");
					text = text.Replace("[/e]", "</color>");
					this.middleRequired.text = text;
				}
				this.options[i].option.imageIcon.color = this.colorIconHovered;
				if (this.selectedOption != this.options[i])
				{
					if (this.selectedOption != null && !this.options[i].disabled)
					{
						SoundManager.PlayOneshot(this.clipChanged, null, true, default(Vector3));
						this.scaleTarget.transform.localScale = Vector3.one;
						LeanTween.scale(this.scaleTarget, Vector3.one * 1.03f, 0.2f).setEase(PieMenu.easePunch);
					}
					this.selectedOption = this.options[i];
				}
			}
			else
			{
				this.options[i].option.imageIcon.color = this.colorIconActive;
			}
			if (this.options[i].disabled)
			{
				this.options[i].option.imageIcon.color = this.colorIconDisabled;
				this.options[i].option.background.color = this.colorBackgroundDisabled;
			}
		}
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000C8EB4 File Offset: 0x000C70B4
	public bool DoSelect()
	{
		if (this.selectedOption == null)
		{
			return false;
		}
		if (this.selectedOption.disabled)
		{
			return false;
		}
		SoundManager.PlayOneshot(this.clipSelected, null, true, default(Vector3));
		if (this.selectedOption.action != null)
		{
			try
			{
				this.selectedOption.action.Invoke(LocalPlayer.Entity);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		return true;
	}

	// Token: 0x020006AD RID: 1709
	[Serializable]
	public class MenuOption
	{
		// Token: 0x04002231 RID: 8753
		public string name;

		// Token: 0x04002232 RID: 8754
		public string desc;

		// Token: 0x04002233 RID: 8755
		public string requirements;

		// Token: 0x04002234 RID: 8756
		public Sprite sprite;

		// Token: 0x04002235 RID: 8757
		public bool disabled;

		// Token: 0x04002236 RID: 8758
		public int order;

		// Token: 0x04002237 RID: 8759
		[NonSerialized]
		public Action<BasePlayer> action;

		// Token: 0x04002238 RID: 8760
		[NonSerialized]
		public PieOption option;

		// Token: 0x04002239 RID: 8761
		[NonSerialized]
		public bool selected;
	}
}
