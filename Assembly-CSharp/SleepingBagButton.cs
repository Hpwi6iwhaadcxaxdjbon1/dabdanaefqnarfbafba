using System;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006B9 RID: 1721
public class SleepingBagButton : MonoBehaviour
{
	// Token: 0x04002278 RID: 8824
	public GameObject timerInfo;

	// Token: 0x04002279 RID: 8825
	public Text BagName;

	// Token: 0x0400227A RID: 8826
	public Text LockTime;

	// Token: 0x0400227B RID: 8827
	internal Button button;

	// Token: 0x0400227C RID: 8828
	internal RespawnInformation.SpawnOptions spawnOptions;

	// Token: 0x0400227D RID: 8829
	internal float releaseTime;

	// Token: 0x1700027A RID: 634
	// (get) Token: 0x0600264A RID: 9802 RVA: 0x0001DD95 File Offset: 0x0001BF95
	public float timerSeconds
	{
		get
		{
			return Mathf.Clamp(this.releaseTime - Time.realtimeSinceStartup, 0f, 216000f);
		}
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x0600264B RID: 9803 RVA: 0x0001DDB2 File Offset: 0x0001BFB2
	public string friendlyName
	{
		get
		{
			if (this.spawnOptions == null || string.IsNullOrEmpty(this.spawnOptions.name))
			{
				return "Null Sleeping Bag";
			}
			return this.spawnOptions.name;
		}
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x000C9E50 File Offset: 0x000C8050
	public void Setup(RespawnInformation.SpawnOptions options)
	{
		this.button = base.GetComponent<Button>();
		this.spawnOptions = options;
		if (options.unlockSeconds > 0f)
		{
			this.button.interactable = false;
			this.timerInfo.SetActive(true);
			this.releaseTime = Time.realtimeSinceStartup + options.unlockSeconds;
		}
		else
		{
			this.button.interactable = true;
			this.timerInfo.SetActive(false);
			this.releaseTime = 0f;
		}
		this.BagName.text = this.friendlyName;
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000C9EE0 File Offset: 0x000C80E0
	public void Update()
	{
		if (this.releaseTime == 0f)
		{
			return;
		}
		if (this.releaseTime < Time.realtimeSinceStartup)
		{
			this.releaseTime = 0f;
			this.timerInfo.SetActive(false);
			this.button.interactable = true;
			return;
		}
		this.LockTime.text = this.timerSeconds.ToString("N0");
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x0001DDDF File Offset: 0x0001BFDF
	public void DoSpawn()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "respawn_sleepingbag", new object[]
		{
			this.spawnOptions.id
		});
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x0001DE0A File Offset: 0x0001C00A
	public void DeleteBag()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "respawn_sleepingbag_remove", new object[]
		{
			this.spawnOptions.id
		});
	}
}
