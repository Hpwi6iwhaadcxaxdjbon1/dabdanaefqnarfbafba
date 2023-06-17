using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006DD RID: 1757
public class UIFogOverlay : MonoBehaviour
{
	// Token: 0x040022F3 RID: 8947
	public static UIFogOverlay Instance;

	// Token: 0x040022F4 RID: 8948
	public CanvasGroup group;

	// Token: 0x040022F5 RID: 8949
	public Color baseColor;

	// Token: 0x040022F6 RID: 8950
	public Image overlayImage;

	// Token: 0x060026EB RID: 9963 RVA: 0x0001E665 File Offset: 0x0001C865
	public static void SetAlpha(float alpha)
	{
		UIFogOverlay.Instance.group.alpha = alpha;
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x0001E677 File Offset: 0x0001C877
	public void OnEnable()
	{
		UIFogOverlay.Instance = this;
		this.baseColor = this.overlayImage.color;
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000CB700 File Offset: 0x000C9900
	public void Update()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		Vector3 position = LocalPlayer.Entity.eyes.position;
		float num = 0f;
		foreach (SmokeGrenade smokeGrenade in SmokeGrenade.activeGrenades)
		{
			if (!(smokeGrenade == null) && smokeGrenade.IsOn())
			{
				float value = Vector3.Distance(position, smokeGrenade.transform.position);
				float value2 = position.y - smokeGrenade.transform.position.y;
				float a = Mathf.InverseLerp(-1f, 0f, value2);
				float b = 1f - Mathf.InverseLerp(5f, 8f, value);
				num = Mathf.Max(Mathf.Min(a, b), num);
			}
		}
		float target = num;
		this.group.alpha = Mathf.MoveTowards(this.group.alpha, target, Time.deltaTime * 3f);
		if (this.group.alpha > 0f)
		{
			float value3 = TOD_Sky.Instance.LerpValue * TOD_Sky.Instance.Day.LightIntensity;
			float b2 = Mathf.InverseLerp(1f, 7f, value3);
			this.overlayImage.color = this.baseColor * b2;
		}
	}
}
