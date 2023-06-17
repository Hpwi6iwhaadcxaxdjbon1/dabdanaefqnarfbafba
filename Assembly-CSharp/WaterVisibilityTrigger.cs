using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public class WaterVisibilityTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x04001BE7 RID: 7143
	private long enteredTick;

	// Token: 0x04001BE8 RID: 7144
	private static long ticks = 1L;

	// Token: 0x04001BE9 RID: 7145
	private static SortedList<long, WaterVisibilityTrigger> tracker = new SortedList<long, WaterVisibilityTrigger>();

	// Token: 0x06001FD5 RID: 8149 RVA: 0x000192E7 File Offset: 0x000174E7
	public static void Reset()
	{
		WaterVisibilityTrigger.ticks = 1L;
		WaterVisibilityTrigger.tracker.Clear();
	}

	// Token: 0x06001FD6 RID: 8150 RVA: 0x000192FA File Offset: 0x000174FA
	protected void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
	}

	// Token: 0x06001FD7 RID: 8151 RVA: 0x0000508F File Offset: 0x0000328F
	private int GetVisibilityMask()
	{
		return 0;
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x00019315 File Offset: 0x00017515
	private void ToggleVisibility()
	{
		if (WaterSystem.Instance != null)
		{
			WaterSystem.Instance.ToggleVisibility(this.GetVisibilityMask());
		}
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x00019334 File Offset: 0x00017534
	private void ResetVisibility()
	{
		if (WaterSystem.Instance != null)
		{
			WaterSystem.Instance.ToggleVisibility(255);
		}
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x00019352 File Offset: 0x00017552
	private void ToggleCollision(Collider other)
	{
		if (WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, true);
		}
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x00019378 File Offset: 0x00017578
	private void ResetCollision(Collider other)
	{
		if (WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, false);
		}
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x000ADCD4 File Offset: 0x000ABED4
	protected void OnTriggerEnter(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && !WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			long num = WaterVisibilityTrigger.ticks;
			WaterVisibilityTrigger.ticks = num + 1L;
			this.enteredTick = num;
			WaterVisibilityTrigger.tracker.Add(this.enteredTick, this);
			this.ToggleVisibility();
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ToggleCollision(other);
		}
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x000ADD54 File Offset: 0x000ABF54
	protected void OnTriggerExit(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
			if (WaterVisibilityTrigger.tracker.Count > 0)
			{
				WaterVisibilityTrigger.tracker.Values[WaterVisibilityTrigger.tracker.Count - 1].ToggleVisibility();
			}
			else
			{
				this.ResetVisibility();
			}
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ResetCollision(other);
		}
	}
}
