using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C6 RID: 1734
public class TextEntryCookie : MonoBehaviour
{
	// Token: 0x1700027C RID: 636
	// (get) Token: 0x0600267E RID: 9854 RVA: 0x0001DFDA File Offset: 0x0001C1DA
	public InputField control
	{
		get
		{
			return base.GetComponent<InputField>();
		}
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000CA7D8 File Offset: 0x000C89D8
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("TextEntryCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			this.control.text = @string;
		}
		this.control.onValueChanged.Invoke(this.control.text);
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x0001DFE2 File Offset: 0x0001C1E2
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		PlayerPrefs.SetString("TextEntryCookie_" + base.name, this.control.text);
	}
}
