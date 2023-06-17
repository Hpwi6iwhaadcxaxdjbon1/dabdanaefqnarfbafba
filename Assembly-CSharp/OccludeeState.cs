using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020007AD RID: 1965
public class OccludeeState : OcclusionCulling.SmartListValue
{
	// Token: 0x04002644 RID: 9796
	public int slot;

	// Token: 0x04002645 RID: 9797
	public bool isStatic;

	// Token: 0x04002646 RID: 9798
	public int layer;

	// Token: 0x04002647 RID: 9799
	public OcclusionCulling.OnVisibilityChanged onVisibilityChanged;

	// Token: 0x04002648 RID: 9800
	public OcclusionCulling.Cell cell;

	// Token: 0x04002649 RID: 9801
	public OcclusionCulling.SimpleList<OccludeeState.State> states;

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06002AA0 RID: 10912 RVA: 0x000212FA File Offset: 0x0001F4FA
	public bool isVisible
	{
		get
		{
			return this.states[this.slot].isVisible > 0;
		}
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000D9370 File Offset: 0x000D7570
	public OccludeeState Initialize(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, int slot, Vector4 sphereBounds, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged)
	{
		states[slot] = new OccludeeState.State
		{
			sphereBounds = sphereBounds,
			minTimeVisible = minTimeVisible,
			waitTime = (isVisible ? (Time.time + minTimeVisible) : 0f),
			waitFrame = (uint)(Time.frameCount + 1),
			isVisible = (isVisible ? 1 : 0),
			active = 1,
			callback = ((onVisibilityChanged != null) ? 1 : 0)
		};
		this.slot = slot;
		this.isStatic = isStatic;
		this.layer = layer;
		this.onVisibilityChanged = onVisibilityChanged;
		this.cell = null;
		this.states = states;
		return this;
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x00021315 File Offset: 0x0001F515
	public void Invalidate()
	{
		this.states[this.slot] = OccludeeState.State.Unused;
		this.slot = -1;
		this.onVisibilityChanged = null;
		this.cell = null;
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000D941C File Offset: 0x000D761C
	public void MakeVisible()
	{
		this.states.array[this.slot].waitTime = Time.time + this.states[this.slot].minTimeVisible;
		this.states.array[this.slot].isVisible = 1;
		if (this.onVisibilityChanged != null)
		{
			this.onVisibilityChanged(true);
		}
	}

	// Token: 0x020007AE RID: 1966
	[StructLayout(2, Pack = 1, Size = 32)]
	public struct State
	{
		// Token: 0x0400264A RID: 9802
		[FieldOffset(0)]
		public Vector4 sphereBounds;

		// Token: 0x0400264B RID: 9803
		[FieldOffset(16)]
		public float minTimeVisible;

		// Token: 0x0400264C RID: 9804
		[FieldOffset(20)]
		public float waitTime;

		// Token: 0x0400264D RID: 9805
		[FieldOffset(24)]
		public uint waitFrame;

		// Token: 0x0400264E RID: 9806
		[FieldOffset(28)]
		public byte isVisible;

		// Token: 0x0400264F RID: 9807
		[FieldOffset(29)]
		public byte active;

		// Token: 0x04002650 RID: 9808
		[FieldOffset(30)]
		public byte callback;

		// Token: 0x04002651 RID: 9809
		[FieldOffset(31)]
		public byte pad1;

		// Token: 0x04002652 RID: 9810
		public static OccludeeState.State Unused = new OccludeeState.State
		{
			active = 0
		};
	}
}
