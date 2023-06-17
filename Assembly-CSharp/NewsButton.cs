using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200068A RID: 1674
public class NewsButton : MonoBehaviour
{
	// Token: 0x04002155 RID: 8533
	public int storyNumber;

	// Token: 0x04002156 RID: 8534
	public NewsSource.Story story;

	// Token: 0x04002157 RID: 8535
	public CanvasGroup canvasGroup;

	// Token: 0x04002158 RID: 8536
	public Text text;

	// Token: 0x04002159 RID: 8537
	public Text author;

	// Token: 0x0400215A RID: 8538
	public RawImage image;

	// Token: 0x0400215B RID: 8539
	private float randomness;

	// Token: 0x0400215C RID: 8540
	private Vector2 textureSize = Vector2.one;

	// Token: 0x06002559 RID: 9561 RVA: 0x0001D430 File Offset: 0x0001B630
	private void Awake()
	{
		this.canvasGroup.alpha = 0f;
		this.randomness = Random.Range(0f, 100f);
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x000C50C8 File Offset: 0x000C32C8
	private void UpdateStory(NewsSource.Story[] stories)
	{
		this.story = stories[this.storyNumber];
		this.text.text = this.story.name;
		this.author.text = this.story.author;
		this.canvasGroup.alpha = 1f;
		Match match = Regex.Match(this.story.text, "http:\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])(.png|.jpg)");
		if (match != null)
		{
			base.StartCoroutine(this.LoadHeaderImage(match.Value));
		}
		this.image.SetNativeSize();
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x0001D457 File Offset: 0x0001B657
	public void OpenURL()
	{
		Client.Steam.Overlay.OpenUrl(this.story.url);
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x0001D473 File Offset: 0x0001B673
	private IEnumerator LoadHeaderImage(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("Couldn't load header image: " + www.error);
			www.Dispose();
			yield break;
		}
		this.image.texture = www.texture;
		this.image.texture.name = url;
		this.image.SetNativeSize();
		this.textureSize = new Vector2((float)this.image.texture.width, (float)this.image.texture.height);
		www.Dispose();
		yield break;
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x000C515C File Offset: 0x000C335C
	private void Update()
	{
		if (this.image.texture == null)
		{
			return;
		}
		float num = this.randomness + Time.realtimeSinceStartup * 0.03f;
		RectTransform rectTransform = base.transform as RectTransform;
		float num2 = rectTransform.rect.width / this.textureSize.x;
		this.image.SetNativeSize();
		this.image.rectTransform.sizeDelta = new Vector2(rectTransform.rect.width * 1.5f, this.textureSize.y * num2 * (1.3f + Mathf.Sin(num * 0.1f) * 0.2f));
		Vector2 vector = new Vector2(this.image.rectTransform.rect.width - rectTransform.rect.width, this.image.rectTransform.rect.height - rectTransform.rect.height);
		float x = vector.x * Mathf.Sin(num * 1f) * 0.48f;
		float y = vector.y * Mathf.Cos(num * 1.2f) * 0.48f;
		this.image.rectTransform.localPosition = new Vector2(x, y);
	}
}
