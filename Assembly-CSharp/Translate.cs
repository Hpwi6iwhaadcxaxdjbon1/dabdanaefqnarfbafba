using System;
using System.Collections.Generic;
using Facepunch;
using JSON;
using UnityEngine;

// Token: 0x02000231 RID: 561
public static class Translate
{
	// Token: 0x04000DD1 RID: 3537
	private static Dictionary<string, string> translations;

	// Token: 0x04000DD2 RID: 3538
	private static string language = "en";

	// Token: 0x060010E7 RID: 4327 RVA: 0x0000EC90 File Offset: 0x0000CE90
	public static string TranslateMouseButton(string mouseButton)
	{
		if (mouseButton == "mouse0")
		{
			return "Left Mouse";
		}
		if (mouseButton == "mouse1")
		{
			return "Right Mouse";
		}
		if (mouseButton == "mouse2")
		{
			return "Center Mouse";
		}
		return mouseButton;
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x0000ECCC File Offset: 0x0000CECC
	public static void Init()
	{
		Translate.LoadLanguage(Translate.language);
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x0000ECD8 File Offset: 0x0000CED8
	public static void LoadLanguage(string lang)
	{
		Translate.translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		Translate.AddLanguageFile("Assets/Localization/" + lang + "/engine.json");
		Translate.AddLanguageFile("Assets/Localization/" + lang + "/phrases.json");
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x00071E7C File Offset: 0x0007007C
	private static void AddLanguageFile(string fileName)
	{
		TextAsset textAsset = FileSystem.Load<TextAsset>(fileName, true);
		if (textAsset == null)
		{
			return;
		}
		Object @object = Object.Parse(textAsset.text);
		if (@object == null)
		{
			Debug.LogError("Error loading translation file: " + fileName);
		}
		foreach (KeyValuePair<string, Value> keyValuePair in @object)
		{
			if (!Translate.translations.ContainsKey(keyValuePair.Key))
			{
				string text = keyValuePair.Value.Str.Replace("\\n", "\n").Trim();
				Translate.translations.Add(keyValuePair.Key, text);
			}
		}
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x00071F34 File Offset: 0x00070134
	public static string Get(string key, string def = null)
	{
		if (def == null)
		{
			def = "#" + key;
		}
		if (string.IsNullOrEmpty(key))
		{
			return def;
		}
		if (Translate.translations == null)
		{
			Translate.Init();
		}
		string result;
		if (Translate.translations.TryGetValue(key, ref result))
		{
			return result;
		}
		return def;
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x0000ED13 File Offset: 0x0000CF13
	public static string GetLanguage()
	{
		return Translate.language;
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0000ED1A File Offset: 0x0000CF1A
	public static void SetLanguage(string str)
	{
		if (Translate.GetLanguage() == str)
		{
			return;
		}
		Translate.language = str;
		Translate.LoadLanguage(Translate.language);
		GlobalMessages.OnLanguageChanged();
	}

	// Token: 0x02000232 RID: 562
	[Serializable]
	public class Phrase
	{
		// Token: 0x04000DD3 RID: 3539
		public string token;

		// Token: 0x04000DD4 RID: 3540
		[TextArea]
		public string english;

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x0000ED4B File Offset: 0x0000CF4B
		public virtual string translated
		{
			get
			{
				if (string.IsNullOrEmpty(this.token))
				{
					return this.english;
				}
				return Translate.Get(this.token, this.english);
			}
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x0000ED72 File Offset: 0x0000CF72
		public bool IsValid()
		{
			return !string.IsNullOrEmpty(this.token);
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x0000ED82 File Offset: 0x0000CF82
		public Phrase(string t = "", string eng = "")
		{
			this.token = t;
			this.english = eng;
		}
	}

	// Token: 0x02000233 RID: 563
	[Serializable]
	public class TokenisedPhrase : Translate.Phrase
	{
		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060010F2 RID: 4338 RVA: 0x00071F7C File Offset: 0x0007017C
		public override string translated
		{
			get
			{
				return base.translated.Replace("[inventory.toggle]", string.Format("[{0}]", Input.GetButtonWithBind("inventory.toggle").ToUpper())).Replace("[inventory.togglecrafting]", string.Format("[{0}]", Input.GetButtonWithBind("inventory.togglecrafting").ToUpper())).Replace("[+map]", string.Format("[{0}]", Input.GetButtonWithBind("+map").ToUpper())).Replace("[inventory.examineheld]", string.Format("[{0}]", Input.GetButtonWithBind("inventory.examineheld").ToUpper())).Replace("[slot2]", string.Format("[{0}]", Input.GetButtonWithBind("+slot2").ToUpper())).Replace("[attack]", string.Format("[{0}]", Translate.TranslateMouseButton(Input.GetButtonWithBind("+attack")).ToUpper())).Replace("[attack2]", string.Format("[{0}]", Translate.TranslateMouseButton(Input.GetButtonWithBind("+attack2")).ToUpper())).Replace("[+use]", string.Format("[{0}]", Translate.TranslateMouseButton(Input.GetButtonWithBind("+use")).ToUpper())).Replace("[+altlook]", string.Format("[{0}]", Translate.TranslateMouseButton(Input.GetButtonWithBind("+altlook")).ToUpper())).Replace("[+reload]", string.Format("[{0}]", Translate.TranslateMouseButton(Input.GetButtonWithBind("+reload")).ToUpper())).Replace("[+voice]", string.Format("[{0}]", Translate.TranslateMouseButton(Input.GetButtonWithBind("+voice")).ToUpper()));
			}
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x0000ED98 File Offset: 0x0000CF98
		public TokenisedPhrase(string t = "", string eng = "") : base(t, eng)
		{
		}
	}
}
