using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Facepunch.Extend;
using Facepunch.Math;
using JSON;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200068C RID: 1676
public class NewsSource : MonoBehaviour
{
	// Token: 0x04002162 RID: 8546
	public NewsSource.Story[] story;

	// Token: 0x04002163 RID: 8547
	public Text title;

	// Token: 0x04002164 RID: 8548
	public Text date;

	// Token: 0x04002165 RID: 8549
	public Text text;

	// Token: 0x04002166 RID: 8550
	public Text authorName;

	// Token: 0x04002167 RID: 8551
	public RawImage image;

	// Token: 0x04002168 RID: 8552
	public VerticalLayoutGroup layoutGroup;

	// Token: 0x04002169 RID: 8553
	public Button button;

	// Token: 0x06002565 RID: 9573 RVA: 0x0001D4B3 File Offset: 0x0001B6B3
	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateNews());
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000C53C4 File Offset: 0x000C35C4
	public void SetStory(int i)
	{
		if (this.story == null)
		{
			return;
		}
		if (this.story.Length <= i)
		{
			return;
		}
		base.StopAllCoroutines();
		this.title.text = this.story[i].name;
		this.date.text = NumberExtensions.FormatSecondsLong((long)(Epoch.Current - this.story[i].date));
		string text = Regex.Replace(this.story[i].text, "\\[img\\].*\\[\\/img\\]", string.Empty, 1);
		text = text.Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"");
		text = text.Replace("[list]", "<color=#F7EBE1aa>");
		text = text.Replace("[/list]", "</color>");
		text = text.Replace("[*]", "\t\t» ");
		text = Regex.Replace(text, "\\[(.*?)\\]", string.Empty, 1);
		text = text.Trim();
		Match match = Regex.Match(this.story[i].text, "url=(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])");
		Match match2 = Regex.Match(this.story[i].text, "(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])(.png|.jpg)");
		if (match != null)
		{
			string url = match.Value.Replace("url=", "");
			if (url == null || url.Trim().Length <= 0)
			{
				url = this.story[i].url;
			}
			this.button.gameObject.SetActive(true);
			this.button.onClick.RemoveAllListeners();
			this.button.onClick.AddListener(delegate()
			{
				Debug.Log("Opening URL: " + url);
				Application.OpenURL(url);
			});
		}
		else
		{
			this.button.gameObject.SetActive(false);
		}
		this.text.text = text;
		this.authorName.text = string.Format("posted by {0}", this.story[i].author);
		if (this.image != null)
		{
			if (this.story[i].texture)
			{
				this.SetHeadlineTexture(this.story[i].texture);
				return;
			}
			if (match2 != null)
			{
				base.StartCoroutine(this.LoadHeaderImage(match2.Value, i));
			}
		}
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x000C5638 File Offset: 0x000C3838
	private void SetHeadlineTexture(Texture tex)
	{
		float num = (float)tex.height / (float)tex.width;
		this.image.texture = tex;
		this.image.rectTransform.sizeDelta = new Vector2(0f, this.image.rectTransform.rect.width * num);
		this.image.enabled = true;
		RectOffset padding = this.layoutGroup.padding;
		padding.top = (int)(this.image.rectTransform.rect.width * num) / 2;
		this.layoutGroup.padding = padding;
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x0001D4C2 File Offset: 0x0001B6C2
	private IEnumerator LoadHeaderImage(string url, int i)
	{
		this.image.enabled = false;
		WWW www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("Couldn't load header image: " + www.error);
			www.Dispose();
			yield break;
		}
		Texture2D textureNonReadable = www.textureNonReadable;
		textureNonReadable.name = url;
		this.story[i].texture = textureNonReadable;
		this.SetHeadlineTexture(this.story[i].texture);
		www.Dispose();
		yield break;
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x0001D4DF File Offset: 0x0001B6DF
	private IEnumerator UpdateNews()
	{
		WWW www = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
		yield return www;
		Object @object = Object.Parse(www.text);
		www.Dispose();
		if (@object == null)
		{
			yield break;
		}
		Array array = @object.GetObject("appnews").GetArray("newsitems");
		List<NewsSource.Story> list = new List<NewsSource.Story>();
		foreach (Value value in array)
		{
			string @string = value.Obj.GetString("contents", "Missing URL");
			list.Add(new NewsSource.Story
			{
				name = value.Obj.GetString("title", "Missing Title"),
				url = value.Obj.GetString("url", "Missing URL"),
				date = value.Obj.GetInt("date", 0),
				text = @string,
				author = value.Obj.GetString("author", "Missing Author")
			});
		}
		this.story = list.ToArray();
		this.SetStory(0);
		yield break;
	}

	// Token: 0x0200068D RID: 1677
	public struct Story
	{
		// Token: 0x0400216A RID: 8554
		public string name;

		// Token: 0x0400216B RID: 8555
		public string url;

		// Token: 0x0400216C RID: 8556
		public int date;

		// Token: 0x0400216D RID: 8557
		public string text;

		// Token: 0x0400216E RID: 8558
		public string author;

		// Token: 0x0400216F RID: 8559
		public Texture texture;
	}
}
