using System;
using Network;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200024F RID: 591
public class NetworkInfoGeneralText : MonoBehaviour
{
	// Token: 0x04000E55 RID: 3669
	public Text text;

	// Token: 0x060011A0 RID: 4512 RVA: 0x0000F6E8 File Offset: 0x0000D8E8
	private void Update()
	{
		this.UpdateText();
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x00074F40 File Offset: 0x00073140
	private void UpdateText()
	{
		string text = "";
		if (Net.cl != null)
		{
			text += "Client\n";
			text += Net.cl.GetDebug(null);
			text += "\n";
		}
		this.text.text = text;
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0000F6F0 File Offset: 0x0000D8F0
	private static string ChannelStat(int window, int left)
	{
		return string.Format("{0}/{1}", left, window);
	}
}
