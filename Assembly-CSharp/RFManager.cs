using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200035B RID: 859
public class RFManager
{
	// Token: 0x0400131F RID: 4895
	public static Dictionary<int, List<IRFObject>> _listeners = new Dictionary<int, List<IRFObject>>();

	// Token: 0x04001320 RID: 4896
	public static Dictionary<int, List<IRFObject>> _broadcasters = new Dictionary<int, List<IRFObject>>();

	// Token: 0x04001321 RID: 4897
	public static int minFreq = 1;

	// Token: 0x04001322 RID: 4898
	public static int maxFreq = 9999;

	// Token: 0x06001628 RID: 5672 RVA: 0x00012B97 File Offset: 0x00010D97
	public static int ClampFrequency(int freq)
	{
		return Mathf.Clamp(freq, RFManager.minFreq, RFManager.maxFreq);
	}
}
