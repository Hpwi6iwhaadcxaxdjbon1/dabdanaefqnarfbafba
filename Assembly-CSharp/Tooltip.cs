using System;
using System.Text.RegularExpressions;
using Facepunch;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006CA RID: 1738
public class Tooltip : BaseMonoBehaviour, IClientComponent, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x040022AE RID: 8878
	public static GameObject Current;

	// Token: 0x040022AF RID: 8879
	[TextArea]
	public string Text;

	// Token: 0x040022B0 RID: 8880
	public GameObject TooltipObject;

	// Token: 0x040022B1 RID: 8881
	public string token = "";

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06002690 RID: 9872 RVA: 0x0001E09E File Offset: 0x0001C29E
	public string english
	{
		get
		{
			return this.Text;
		}
	}

	// Token: 0x06002691 RID: 9873 RVA: 0x0001E0A6 File Offset: 0x0001C2A6
	public void OnPointerEnter(PointerEventData eventData)
	{
		base.CancelInvoke(new Action(this.OpenTooltip));
		base.Invoke(new Action(this.OpenTooltip), 0.15f);
	}

	// Token: 0x06002692 RID: 9874 RVA: 0x0001E0D1 File Offset: 0x0001C2D1
	public void OpenTooltip()
	{
		Tooltip.KillTooltip();
		this.CreateTextTooltip();
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x0001E0DE File Offset: 0x0001C2DE
	public void OnPointerExit(PointerEventData eventData)
	{
		base.CancelInvoke(new Action(this.OpenTooltip));
		Tooltip.KillTooltip();
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x0001E0F7 File Offset: 0x0001C2F7
	private static void KillTooltip()
	{
		if (Tooltip.Current != null)
		{
			Tooltip.Current.transform.SetParent(null);
			Object.Destroy(Tooltip.Current);
		}
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000CA9E4 File Offset: 0x000C8BE4
	private void CreateTextTooltip()
	{
		Canvas componentInChildren = base.transform.root.GetComponentInChildren<Canvas>();
		if (componentInChildren == null)
		{
			return;
		}
		Tooltip.Current = Object.Instantiate<GameObject>(this.TooltipObject);
		Tooltip.Current.transform.SetParent(componentInChildren.transform, false);
		Text componentInChildren2 = Tooltip.Current.GetComponentInChildren<Text>();
		if (componentInChildren2)
		{
			string text = this.Text;
			if (!string.IsNullOrEmpty(this.token))
			{
				text = Translate.Get(this.token, this.Text);
			}
			componentInChildren2.text = Tooltip.ParseTokens(text);
		}
		Tooltip.Current.GetComponentInChildren<TooltipContainer>();
		Tooltip.Current.SendMessage("SetSourceRect", base.transform as RectTransform);
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x000CAAA4 File Offset: 0x000C8CA4
	private static string ParseTokens(string text)
	{
		if (!text.Contains("{"))
		{
			return text;
		}
		if (!text.Contains("}"))
		{
			return text;
		}
		foreach (object obj in Regex.Matches(text, "\\{(.*?)\\}"))
		{
			Match match = (Match)obj;
			string value = match.Value;
			string text2 = value.Trim(new char[]
			{
				'{',
				'}',
				'%'
			});
			if (value.get_Chars(1) == '%')
			{
				string[] buttonsWithBind = Input.GetButtonsWithBind(text2);
				if (buttonsWithBind.Length == 0)
				{
					text = text.Replace(match.Value, "(Unbound Button)");
				}
				else
				{
					text = text.Replace(match.Value, string.Format("\"{0}\"", buttonsWithBind[0]));
				}
			}
			else
			{
				text = text.Replace(match.Value, "UNKNOWN TOKEN");
			}
		}
		return text;
	}
}
