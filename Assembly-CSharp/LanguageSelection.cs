using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000683 RID: 1667
public class LanguageSelection : MonoBehaviour, ILanguageChanged
{
	// Token: 0x04002141 RID: 8513
	public GameObject languagePopup;

	// Token: 0x04002142 RID: 8514
	public GameObject buttonContainer;

	// Token: 0x04002143 RID: 8515
	public Image flagImage;

	// Token: 0x0600252D RID: 9517 RVA: 0x0001D1AD File Offset: 0x0001B3AD
	private void OnEnable()
	{
		GlobalMessages.onLanguageChanged.Add(this);
	}

	// Token: 0x0600252E RID: 9518 RVA: 0x0001D1BA File Offset: 0x0001B3BA
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onLanguageChanged.Remove(this);
	}

	// Token: 0x0600252F RID: 9519 RVA: 0x0001D1D0 File Offset: 0x0001B3D0
	private void Start()
	{
		this.languagePopup.SetActive(false);
		this.BuildAll();
		this.flagImage.sprite = FileSystem.Load<Sprite>("Assets/Icons/flags/" + Translate.GetLanguage() + ".png", true);
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x0001D209 File Offset: 0x0001B409
	public void OnLanguageChanged()
	{
		this.flagImage.sprite = FileSystem.Load<Sprite>("Assets/Icons/flags/" + Translate.GetLanguage() + ".png", true);
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x0001D230 File Offset: 0x0001B430
	private void ChangeLanguage(string language)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "language", new object[]
		{
			language
		});
	}

	// Token: 0x06002532 RID: 9522 RVA: 0x000C4C40 File Offset: 0x000C2E40
	[ContextMenu("Build All")]
	private void BuildAll()
	{
		this.buttonContainer.transform.DestroyAllChildren(true);
		Sprite[] array = FileSystem.LoadAll<Sprite>("Assets/Icons/flags", "");
		for (int i = 0; i < array.Length; i++)
		{
			Sprite sprite = array[i];
			GameObject gameObject = new GameObject(sprite.name);
			gameObject.transform.SetParent(this.buttonContainer.transform);
			Image image = gameObject.AddComponent<Image>();
			image.sprite = sprite;
			Button button = gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			string name = sprite.name;
			button.onClick.AddListener(delegate()
			{
				this.ChangeLanguage(name);
				this.languagePopup.SetActive(false);
			});
		}
	}
}
