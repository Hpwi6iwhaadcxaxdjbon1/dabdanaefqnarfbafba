using System;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000E8 RID: 232
public class VendingMachineScreen : MonoBehaviour
{
	// Token: 0x0400072F RID: 1839
	public RawImage largeIcon;

	// Token: 0x04000730 RID: 1840
	public RawImage blueprintIcon;

	// Token: 0x04000731 RID: 1841
	public Text mainText;

	// Token: 0x04000732 RID: 1842
	public Text lowerText;

	// Token: 0x04000733 RID: 1843
	public Text centerText;

	// Token: 0x04000734 RID: 1844
	public RawImage smallIcon;

	// Token: 0x04000735 RID: 1845
	public VendingMachine vendingMachine;

	// Token: 0x04000736 RID: 1846
	public Sprite outOfStockSprite;

	// Token: 0x04000737 RID: 1847
	public Renderer fadeoutMesh;

	// Token: 0x04000738 RID: 1848
	public CanvasGroup screenCanvas;

	// Token: 0x04000739 RID: 1849
	public Renderer light1;

	// Token: 0x0400073A RID: 1850
	public Renderer light2;

	// Token: 0x0400073B RID: 1851
	public float nextImageTime;

	// Token: 0x0400073C RID: 1852
	public int currentImageIndex;

	// Token: 0x0400073D RID: 1853
	private float imageCycleTime = 5f;

	// Token: 0x06000AEE RID: 2798 RVA: 0x0005662C File Offset: 0x0005482C
	public void UpdateLOD()
	{
		if (MainCamera.Distance(base.transform.position) < 15f)
		{
			if (this.screenCanvas.alpha < 1f)
			{
				this.screenCanvas.gameObject.SetActive(true);
				this.screenCanvas.alpha = Mathf.MoveTowards(this.screenCanvas.alpha, 1f, Time.deltaTime * 2f);
			}
		}
		else if (this.screenCanvas.gameObject.activeSelf)
		{
			this.screenCanvas.alpha = Mathf.MoveTowards(this.screenCanvas.alpha, 0f, Time.deltaTime * 2f);
			if (this.screenCanvas.alpha <= 0f)
			{
				this.screenCanvas.gameObject.SetActive(false);
			}
		}
		this.fadeoutMesh.gameObject.SetActive(this.screenCanvas.alpha != 1f);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x00056728 File Offset: 0x00054928
	public void UpdateLightAnimation()
	{
		float num = Time.realtimeSinceStartup / 0.25f;
		int num2 = Mathf.CeilToInt(num % 4f);
		if (num % 12f <= 8f)
		{
			num2 = 0;
		}
		bool enabled = false;
		bool enabled2 = false;
		if (num2 <= 1)
		{
			enabled = false;
			enabled2 = false;
		}
		else if (num2 <= 2)
		{
			enabled = true;
			enabled2 = false;
		}
		else if (num2 <= 3)
		{
			enabled = true;
			enabled2 = true;
		}
		else if (num2 <= 4)
		{
			enabled = false;
			enabled2 = true;
		}
		this.light1.enabled = enabled;
		this.light2.enabled = enabled2;
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x000567A0 File Offset: 0x000549A0
	private void Update()
	{
		this.UpdateLOD();
		this.UpdateLightAnimation();
		if (!this.screenCanvas.gameObject.activeSelf)
		{
			return;
		}
		if (this.vendingMachine == null)
		{
			return;
		}
		if (this.vendingMachine.sellOrders == null)
		{
			return;
		}
		if (this.vendingMachine.sellOrders.sellOrders == null)
		{
			return;
		}
		if (this.vendingMachine.IsVending())
		{
			this.largeIcon.gameObject.SetActive(false);
			this.smallIcon.gameObject.SetActive(true);
			this.mainText.text = "Vending...";
			return;
		}
		this.smallIcon.gameObject.SetActive(false);
		this.largeIcon.gameObject.SetActive(true);
		if (Time.realtimeSinceStartup > this.nextImageTime)
		{
			bool flag = true;
			int num = this.currentImageIndex + 1;
			int count = this.vendingMachine.sellOrders.sellOrders.Count;
			if (num >= count)
			{
				num = 0;
			}
			for (int i = 0; i < count; i++)
			{
				int num2 = num + i;
				if (num2 >= count)
				{
					num2 -= count;
				}
				VendingMachine.SellOrder sellOrder = this.vendingMachine.sellOrders.sellOrders[num2];
				if (sellOrder.inStock > 0)
				{
					ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellOrder.itemToSellID);
					ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(sellOrder.currencyID);
					if (itemDefinition)
					{
						this.currentImageIndex = num2;
						this.largeIcon.texture = itemDefinition.iconSprite.texture;
						this.blueprintIcon.enabled = sellOrder.itemToSellIsBP;
						this.lowerText.text = ((sellOrder.itemToSellAmount > 1) ? ("x" + sellOrder.itemToSellAmount.ToString()) : "");
						this.mainText.text = sellOrder.currencyAmountPerItem.ToString() + " " + itemDefinition2.displayName.translated;
						if (sellOrder.currencyIsBP)
						{
							Text text = this.mainText;
							text.text += " BP";
						}
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.largeIcon.texture = this.outOfStockSprite.texture;
				this.blueprintIcon.enabled = false;
				this.lowerText.text = "";
				this.mainText.text = "";
			}
			this.nextImageTime = Time.realtimeSinceStartup + this.imageCycleTime;
		}
		this.lowerText.gameObject.SetActive(this.lowerText.text != "");
		this.mainText.gameObject.SetActive(this.mainText.text != "");
		this.centerText.gameObject.SetActive(this.centerText.text != "");
	}

	// Token: 0x020000E9 RID: 233
	public enum vmScreenState
	{
		// Token: 0x0400073F RID: 1855
		ItemScroll,
		// Token: 0x04000740 RID: 1856
		Vending,
		// Token: 0x04000741 RID: 1857
		Message,
		// Token: 0x04000742 RID: 1858
		ShopName,
		// Token: 0x04000743 RID: 1859
		OutOfStock
	}
}
