using System;
using UnityEngine;

// Token: 0x02000429 RID: 1065
public class InputState
{
	// Token: 0x04001643 RID: 5699
	public InputMessage current = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x04001644 RID: 5700
	public InputMessage previous = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x04001645 RID: 5701
	private int SwallowedButtons;

	// Token: 0x060019A1 RID: 6561 RVA: 0x000152E3 File Offset: 0x000134E3
	public bool IsDown(BUTTON btn)
	{
		return this.current != null && (this.SwallowedButtons & (int)btn) != (int)btn && (this.current.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x0001530C File Offset: 0x0001350C
	public bool WasDown(BUTTON btn)
	{
		return this.previous != null && (this.previous.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x00015328 File Offset: 0x00013528
	public bool WasJustPressed(BUTTON btn)
	{
		return this.IsDown(btn) && !this.WasDown(btn);
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x0001533F File Offset: 0x0001353F
	public bool WasJustReleased(BUTTON btn)
	{
		return !this.IsDown(btn) && this.WasDown(btn);
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x00015353 File Offset: 0x00013553
	public void SwallowButton(BUTTON btn)
	{
		if (this.current == null)
		{
			return;
		}
		this.SwallowedButtons |= (int)btn;
	}

	// Token: 0x060019A6 RID: 6566 RVA: 0x0001536C File Offset: 0x0001356C
	private Quaternion AimAngle()
	{
		if (this.current == null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(this.current.aimAngles);
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x00090F84 File Offset: 0x0008F184
	public void Flip(InputMessage newcurrent)
	{
		this.SwallowedButtons = 0;
		this.previous.aimAngles = this.current.aimAngles;
		this.previous.buttons = this.current.buttons;
		this.previous.mouseDelta = this.current.mouseDelta;
		this.current.aimAngles = newcurrent.aimAngles;
		this.current.buttons = newcurrent.buttons;
		this.current.mouseDelta = newcurrent.mouseDelta;
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x0001538C File Offset: 0x0001358C
	public void Clear()
	{
		this.current.buttons = 0;
		this.previous.buttons = 0;
		this.SwallowedButtons = 0;
	}
}
