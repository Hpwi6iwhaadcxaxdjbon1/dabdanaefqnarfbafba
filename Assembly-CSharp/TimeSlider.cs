using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C7 RID: 1735
public class TimeSlider : MonoBehaviour
{
	// Token: 0x040022AA RID: 8874
	private Slider slider;

	// Token: 0x06002682 RID: 9858 RVA: 0x0001E00C File Offset: 0x0001C20C
	private void Start()
	{
		this.slider = base.GetComponent<Slider>();
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x0001E01A File Offset: 0x0001C21A
	private void Update()
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		this.slider.value = TOD_Sky.Instance.Cycle.Hour;
	}

	// Token: 0x06002684 RID: 9860 RVA: 0x0001E044 File Offset: 0x0001C244
	public void OnValue(float f)
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = f;
		TOD_Sky.Instance.UpdateAmbient();
		TOD_Sky.Instance.UpdateReflection();
		TOD_Sky.Instance.UpdateFog();
	}
}
