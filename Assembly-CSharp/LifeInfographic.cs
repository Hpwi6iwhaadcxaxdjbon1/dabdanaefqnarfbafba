using System;
using Facepunch.Extend;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200067B RID: 1659
public class LifeInfographic : MonoBehaviour
{
	// Token: 0x040020F8 RID: 8440
	[NonSerialized]
	public PlayerLifeStory life;

	// Token: 0x040020F9 RID: 8441
	public GameObject container;

	// Token: 0x06002505 RID: 9477 RVA: 0x000C3694 File Offset: 0x000C1894
	public void Refresh()
	{
		for (int i = 0; i < this.container.transform.childCount; i++)
		{
			this.container.transform.GetChild(i).gameObject.SetActive(false);
		}
		if (this.life == null)
		{
			return;
		}
		this.Refresh_Age();
		this.Refresh_Death();
		base.BroadcastMessage("InfographicUpdated", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x0001D0BA File Offset: 0x0001B2BA
	private void Refresh_Age()
	{
		if (this.life.secondsSleeping > 120f)
		{
			this.Show("age.sleep", 0UL);
			return;
		}
		this.Show("age", 0UL);
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x000C36FC File Offset: 0x000C18FC
	private void Refresh_Death()
	{
		if (this.life.deathInfo == null || this.life.deathInfo.attackerSteamID <= 0UL)
		{
			this.Show("death.unknown", 0UL);
			return;
		}
		if (string.IsNullOrEmpty(this.life.deathInfo.inflictorName))
		{
			this.Show("death.player", 0UL);
			return;
		}
		this.Show("death.player.weapon", 0UL);
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000C376C File Offset: 0x000C196C
	private void Show(string name, ulong steamid = 0UL)
	{
		Transform transform = this.container.transform.Find(name);
		transform.gameObject.SetActive(true);
		if (steamid > 0UL)
		{
			Transform transform2 = TransformEx.FindChildRecursive(transform, "avatar");
			if (transform2)
			{
				RawImage component = transform2.GetComponent<RawImage>();
				if (component)
				{
					component.texture = SingletonComponent<SteamClient>.Instance.GetAvatarTexture(steamid);
				}
			}
		}
	}
}
