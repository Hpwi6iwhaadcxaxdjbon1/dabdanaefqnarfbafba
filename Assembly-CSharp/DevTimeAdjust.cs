using System;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class DevTimeAdjust : MonoBehaviour
{
	// Token: 0x060011CF RID: 4559 RVA: 0x0000F8AE File Offset: 0x0000DAAE
	private void Start()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = PlayerPrefs.GetFloat("DevTime");
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x00075BB8 File Offset: 0x00073DB8
	private void OnGUI()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		float num = (float)Screen.width * 0.2f;
		Rect rect = new Rect((float)Screen.width - (num + 20f), (float)Screen.height - 30f, num, 20f);
		float num2 = TOD_Sky.Instance.Cycle.Hour;
		num2 = GUI.HorizontalSlider(rect, num2, 0f, 24f);
		rect.y -= 20f;
		GUI.Label(rect, "Time Of Day");
		if (num2 != TOD_Sky.Instance.Cycle.Hour)
		{
			TOD_Sky.Instance.Cycle.Hour = num2;
			PlayerPrefs.SetFloat("DevTime", num2);
		}
	}
}
