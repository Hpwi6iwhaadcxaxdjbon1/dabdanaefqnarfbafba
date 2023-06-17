using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000675 RID: 1653
public class UIIntegerEntry : MonoBehaviour
{
	// Token: 0x040020D7 RID: 8407
	public InputField textEntry;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060024DF RID: 9439 RVA: 0x000C2E98 File Offset: 0x000C1098
	// (remove) Token: 0x060024E0 RID: 9440 RVA: 0x000C2ED0 File Offset: 0x000C10D0
	public event Action textChanged;

	// Token: 0x060024E1 RID: 9441 RVA: 0x0001CFAE File Offset: 0x0001B1AE
	public void OnAmountTextChanged()
	{
		this.textChanged.Invoke();
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x0001CFBB File Offset: 0x0001B1BB
	public void SetAmount(int amount)
	{
		if (amount == this.GetIntAmount())
		{
			return;
		}
		this.textEntry.text = amount.ToString();
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x000C2F08 File Offset: 0x000C1108
	public int GetIntAmount()
	{
		int result = 0;
		int.TryParse(this.textEntry.text, ref result);
		return result;
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x0001CFD9 File Offset: 0x0001B1D9
	public void PlusMinus(int delta)
	{
		this.SetAmount(this.GetIntAmount() + delta);
	}
}
