using System;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000705 RID: 1797
public class ConvarWater : MonoBehaviour
{
	// Token: 0x04002361 RID: 9057
	[FormerlySerializedAs("waterEx")]
	public WaterSystem water;

	// Token: 0x04002362 RID: 9058
	internal ConsoleSystem.Command water_quality;

	// Token: 0x0600277C RID: 10108 RVA: 0x0001ECA9 File Offset: 0x0001CEA9
	private void OnEnable()
	{
		this.water_quality = ConsoleSystem.Index.Client.Find("water.quality");
		if (this.water_quality != null)
		{
			this.water_quality.OnValueChanged += new Action<ConsoleSystem.Command>(this.OnConvarChanged);
			this.OnConvarChanged(this.water_quality);
		}
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x0001ECE6 File Offset: 0x0001CEE6
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.water_quality != null)
		{
			this.water_quality.OnValueChanged -= new Action<ConsoleSystem.Command>(this.OnConvarChanged);
		}
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x0001ED0F File Offset: 0x0001CF0F
	private void OnConvarChanged(ConsoleSystem.Command cmd)
	{
		if (this.water != null)
		{
			this.water.enabled = false;
			this.water.enabled = true;
		}
	}
}
