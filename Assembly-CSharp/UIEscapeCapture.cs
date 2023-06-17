using System;
using System.Collections.Generic;
using UnityEngine.Events;

// Token: 0x020006DC RID: 1756
public class UIEscapeCapture : ListComponent<UIEscapeCapture>
{
	// Token: 0x040022F2 RID: 8946
	public UnityEvent onEscape;

	// Token: 0x060026E9 RID: 9961 RVA: 0x000CB6A8 File Offset: 0x000C98A8
	public static bool EscapePressed()
	{
		using (IEnumerator<UIEscapeCapture> enumerator = ListComponent<UIEscapeCapture>.InstanceList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.onEscape.Invoke();
				return true;
			}
		}
		return false;
	}
}
