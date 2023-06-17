using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000603 RID: 1539
public class AchievementTodo : BaseMonoBehaviour
{
	// Token: 0x04001ED8 RID: 7896
	public Text text;

	// Token: 0x04001ED9 RID: 7897
	public RectTransform checkIcon;

	// Token: 0x04001EDA RID: 7898
	public RectTransform checkBox;

	// Token: 0x04001EDB RID: 7899
	public Color AliveColor;

	// Token: 0x04001EDC RID: 7900
	public Color DeadColor;

	// Token: 0x04001EDD RID: 7901
	public Color HighlightColor;

	// Token: 0x04001EDE RID: 7902
	private AchievementGroup.AchievementItem Item;

	// Token: 0x04001EDF RID: 7903
	internal bool State;

	// Token: 0x0600228A RID: 8842 RVA: 0x0001B711 File Offset: 0x00019911
	private void Awake()
	{
		if (Client.Instance != null)
		{
			Client.Instance.Achievements.OnAchievementStateChanged += new Action<Achievement>(this.OnAchievementStateChanged);
		}
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x000B90C0 File Offset: 0x000B72C0
	private void OnAchievementStateChanged(Achievement ach)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (this.Item == null)
		{
			return;
		}
		if (this.Item.Achievement != ach)
		{
			return;
		}
		this.State = ach.State;
		if (this.State)
		{
			this.Unlock();
		}
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000B9110 File Offset: 0x000B7310
	internal void Unlock()
	{
		this.State = true;
		LeanTween.scale(base.transform as RectTransform, Vector3.one * 1.1f, 0.6f).setEase(TweenMode.Punch).setDelay(0.1f);
		LeanTween.colorText(this.text.rectTransform, this.HighlightColor, 0.2f);
		LeanTween.color(this.checkBox, this.HighlightColor, 0.2f);
		LeanTween.color(this.checkIcon, this.HighlightColor, 0.2f);
		base.Invoke(new Action(this.UpdateState), 1f);
		SingletonComponent<Achievements>.Instance.OnItemComplete();
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x0001B735 File Offset: 0x00019935
	internal void Init(AchievementGroup.AchievementItem item)
	{
		base.gameObject.SetActive(true);
		this.Item = item;
		this.text.text = item.Phrase.translated;
		this.State = item.Unlocked;
		this.UpdateState();
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000B91C8 File Offset: 0x000B73C8
	private void UpdateState()
	{
		Color color = this.AliveColor;
		if (this.State)
		{
			color = this.DeadColor;
		}
		LeanTween.colorText(this.text.rectTransform, color, 0.5f);
		LeanTween.color(this.checkBox, color, 0.5f);
		if (!this.State)
		{
			color.a = 0f;
		}
		LeanTween.color(this.checkIcon, color, 0.5f);
	}
}
