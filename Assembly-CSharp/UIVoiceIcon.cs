using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E8 RID: 1768
public class UIVoiceIcon : MonoBehaviour
{
	// Token: 0x04002303 RID: 8963
	public Text nameText;

	// Token: 0x04002304 RID: 8964
	public RawImage avatar;

	// Token: 0x06002712 RID: 10002 RVA: 0x0001E7F0 File Offset: 0x0001C9F0
	public void Setup(ulong steamid, string name)
	{
		this.nameText.text = name;
		if (SingletonComponent<SteamClient>.Instance != null)
		{
			this.avatar.texture = SingletonComponent<SteamClient>.Instance.GetAvatarTexture(steamid);
		}
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000CBAC8 File Offset: 0x000C9CC8
	public void UpdateVolume(float volume)
	{
		Color color = this.avatar.color;
		color.a = Mathf.Lerp(0.1f, 0.7f, volume);
		this.avatar.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(50f, 40f, volume), Mathf.Lerp(50f, 60f, volume));
		this.avatar.color = color;
	}
}
