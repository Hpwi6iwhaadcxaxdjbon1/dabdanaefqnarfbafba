using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006EE RID: 1774
public class LocalizeText : MonoBehaviour, IClientComponent, ILanguageChanged
{
	// Token: 0x0400230B RID: 8971
	public string token;

	// Token: 0x0400230C RID: 8972
	[TextArea]
	public string english;

	// Token: 0x0400230D RID: 8973
	public string append;

	// Token: 0x0400230E RID: 8974
	public LocalizeText.SpecialMode specialMode;

	// Token: 0x0400230F RID: 8975
	private object[] Tokens = new object[]
	{
		"(arg1)",
		"(arg2)",
		"(arg3)",
		"(arg4)"
	};

	// Token: 0x06002729 RID: 10025 RVA: 0x000CBF84 File Offset: 0x000CA184
	private string GetText(bool englishVersion)
	{
		string text = englishVersion ? this.english : Translate.Get(this.token, this.english);
		text = string.Format(text, this.Tokens);
		if (this.specialMode == LocalizeText.SpecialMode.AllUppercase)
		{
			return text.ToUpper();
		}
		if (this.specialMode == LocalizeText.SpecialMode.AllLowercase)
		{
			return text.ToLower();
		}
		return text + this.append;
	}

	// Token: 0x0600272A RID: 10026 RVA: 0x0001E87F File Offset: 0x0001CA7F
	private void OnEnable()
	{
		this.OnLanguageChanged();
		GlobalMessages.onLanguageChanged.Add(this);
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x0001D1BA File Offset: 0x0001B3BA
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onLanguageChanged.Remove(this);
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x000CBFE8 File Offset: 0x000CA1E8
	public void OnLanguageChanged()
	{
		Text component = base.GetComponent<Text>();
		if (component)
		{
			component.text = this.GetText(false);
		}
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000CC014 File Offset: 0x000CA214
	private void OnValidate()
	{
		Text component = base.GetComponent<Text>();
		if (component)
		{
			component.text = this.GetText(true);
		}
	}

	// Token: 0x0600272E RID: 10030 RVA: 0x0001E892 File Offset: 0x0001CA92
	public void SetToken(int i, object token)
	{
		this.Tokens[i] = token;
		this.OnLanguageChanged();
	}

	// Token: 0x020006EF RID: 1775
	public enum SpecialMode
	{
		// Token: 0x04002311 RID: 8977
		None,
		// Token: 0x04002312 RID: 8978
		AllUppercase,
		// Token: 0x04002313 RID: 8979
		AllLowercase
	}
}
