using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000634 RID: 1588
public class MonumentMarker : MonoBehaviour
{
	// Token: 0x04001F94 RID: 8084
	public Text text;

	// Token: 0x06002383 RID: 9091 RVA: 0x000BC798 File Offset: 0x000BA998
	public void Setup(MonumentInfo info)
	{
		string translated = info.displayPhrase.translated;
		this.text.text = (string.IsNullOrEmpty(translated) ? "Monument" : translated);
	}
}
