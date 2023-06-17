using System;
using UnityEngine;

// Token: 0x02000717 RID: 1815
[ExecuteInEditMode]
public class OneActiveSibling : MonoBehaviour
{
	// Token: 0x060027C5 RID: 10181 RVA: 0x000CDAC4 File Offset: 0x000CBCC4
	[ComponentHelp("This component will disable all of its siblings when it becomes enabled. This can be useful in situations where you only ever want one of the children active - but don't want to manage turning each one off.")]
	private void OnEnable()
	{
		foreach (Transform transform in base.transform.GetSiblings(false))
		{
			transform.gameObject.SetActive(false);
		}
	}
}
