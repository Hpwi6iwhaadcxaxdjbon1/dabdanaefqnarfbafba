using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200067F RID: 1663
public class LookAtIOEnt : MonoBehaviour
{
	// Token: 0x0400210D RID: 8461
	public Text objectTitle;

	// Token: 0x0400210E RID: 8462
	public RectTransform slotToolTip;

	// Token: 0x0400210F RID: 8463
	public Text slotTitle;

	// Token: 0x04002110 RID: 8464
	public Text slotConnection;

	// Token: 0x04002111 RID: 8465
	public Text slotPower;

	// Token: 0x04002112 RID: 8466
	public Text powerText;

	// Token: 0x04002113 RID: 8467
	public Text passthroughText;

	// Token: 0x04002114 RID: 8468
	public Text chargeLeftText;

	// Token: 0x04002115 RID: 8469
	public IOEntityUISlotEntry[] inputEntries;

	// Token: 0x04002116 RID: 8470
	public IOEntityUISlotEntry[] outputEntries;

	// Token: 0x04002117 RID: 8471
	public Color NoPowerColor;

	// Token: 0x04002118 RID: 8472
	public CanvasGroup group;

	// Token: 0x04002119 RID: 8473
	public GameObjectRef handlePrefab;

	// Token: 0x0400211A RID: 8474
	public GameObjectRef handleOccupiedPrefab;

	// Token: 0x0400211B RID: 8475
	public GameObjectRef selectedHandlePrefab;

	// Token: 0x0400211C RID: 8476
	public GameObjectRef pluggedHandlePrefab;

	// Token: 0x0400211D RID: 8477
	public RectTransform clearNotification;

	// Token: 0x0400211E RID: 8478
	public CanvasGroup wireInfoGroup;

	// Token: 0x0400211F RID: 8479
	public Text wireLengthText;

	// Token: 0x04002120 RID: 8480
	public Text wireClipsText;

	// Token: 0x04002121 RID: 8481
	public Text errorReasonTextTooFar;

	// Token: 0x04002122 RID: 8482
	public Text errorReasonTextNoSurface;

	// Token: 0x04002123 RID: 8483
	public Text errorShortCircuit;

	// Token: 0x04002124 RID: 8484
	private IOEntity lastLooking;

	// Token: 0x04002125 RID: 8485
	private float nextUpdatetime;

	// Token: 0x04002126 RID: 8486
	public GameObject activeHandle;

	// Token: 0x04002127 RID: 8487
	public GameObject pluggedHandle;

	// Token: 0x04002128 RID: 8488
	public List<GameObject> inputHandles = new List<GameObject>();

	// Token: 0x04002129 RID: 8489
	public List<GameObject> outputHandles = new List<GameObject>();

	// Token: 0x0400212A RID: 8490
	private static int selectedIndex = -1;

	// Token: 0x0400212B RID: 8491
	private static bool selectedWasInput = false;

	// Token: 0x0400212C RID: 8492
	private bool wiretoolWasPendingInput;

	// Token: 0x0400212D RID: 8493
	private bool wiretoolWasPendingOutput;

	// Token: 0x0400212E RID: 8494
	private bool wireToolWasPendingRoot;

	// Token: 0x06002514 RID: 9492 RVA: 0x000C3AB4 File Offset: 0x000C1CB4
	private void Update()
	{
		bool flag = LocalPlayer.Entity != null && this.UpdateLookingAtIOEnt() && WireTool.CanPlayerUseWires(LocalPlayer.Entity);
		if (!flag)
		{
			this.lastLooking = null;
		}
		this.UpdateClearing();
		this.UpdateWireInfo();
		this.group.alpha = Mathf.MoveTowards(this.group.alpha, flag ? 1f : 0f, Time.deltaTime * 5f);
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x0001D122 File Offset: 0x0001B322
	public void ClearHandles()
	{
		this.SetupHandles(null, true, true);
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x000C3B30 File Offset: 0x000C1D30
	public void SetupHandles(IOEntity ent, bool showInputs = true, bool showOutputs = true)
	{
		LookAtIOEnt.selectedIndex = -1;
		Object.Destroy(this.activeHandle);
		Object.Destroy(this.pluggedHandle);
		for (int i = this.inputHandles.Count - 1; i >= 0; i--)
		{
			Object.Destroy(this.inputHandles[i]);
		}
		this.inputHandles.Clear();
		for (int j = this.outputHandles.Count - 1; j >= 0; j--)
		{
			Object.Destroy(this.outputHandles[j]);
		}
		this.outputHandles.Clear();
		if (LocalPlayer.Entity.IsHoldingEntity<WireTool>())
		{
			WireTool component = LocalPlayer.Entity.GetHeldEntity().GetComponent<WireTool>();
			if (component.HasPendingPlug())
			{
				IOEntity ent2 = component.pending.ent;
				IOEntity.IOSlot ioslot = (component.pending.input ? component.pending.ent.inputs : component.pending.ent.outputs)[component.pending.index];
				this.pluggedHandle = GameManager.client.CreatePrefab(this.pluggedHandlePrefab.resourcePath, ent2.transform.TransformPoint(ioslot.handlePosition), Quaternion.LookRotation(ent2.transform.forward, ent2.transform.up), true);
			}
		}
		if (ent != null)
		{
			this.activeHandle = GameManager.client.CreatePrefab(this.selectedHandlePrefab.resourcePath, ent.transform.position, Quaternion.LookRotation(ent.transform.forward, ent.transform.up), true);
			if (showInputs)
			{
				foreach (IOEntity.IOSlot ioslot2 in ent.inputs)
				{
					string strPrefab = (ioslot2.connectedTo.Get(false) == null) ? this.handlePrefab.resourcePath : this.handleOccupiedPrefab.resourcePath;
					this.inputHandles.Add(GameManager.client.CreatePrefab(strPrefab, ent.transform.TransformPoint(ioslot2.handlePosition), Quaternion.LookRotation(ent.transform.forward, ent.transform.up), true));
				}
			}
			if (showOutputs)
			{
				foreach (IOEntity.IOSlot ioslot3 in ent.outputs)
				{
					string strPrefab2 = (ioslot3.connectedTo.Get(false) == null) ? this.handlePrefab.resourcePath : this.handleOccupiedPrefab.resourcePath;
					this.outputHandles.Add(GameManager.client.CreatePrefab(strPrefab2, ent.transform.TransformPoint(ioslot3.handlePosition), Quaternion.LookRotation(ent.transform.forward, ent.transform.up), true));
				}
			}
		}
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x0001D12D File Offset: 0x0001B32D
	public static int GetSelectedIndex()
	{
		return LookAtIOEnt.selectedIndex;
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x0001D134 File Offset: 0x0001B334
	public static bool SelectedIsInput()
	{
		return LookAtIOEnt.selectedWasInput;
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000C3E10 File Offset: 0x000C2010
	public void UpdateClearing()
	{
		HeldEntity heldEntity = (LocalPlayer.Entity != null) ? LocalPlayer.Entity.GetHeldEntity() : null;
		WireTool wireTool = (heldEntity != null) ? heldEntity.GetComponent<WireTool>() : null;
		if (wireTool != null)
		{
			float clearProgress = wireTool.GetClearProgress();
			PowerBar.Instance.SetProgress(clearProgress);
			PowerBar.Instance.SetVisible(clearProgress > 0f);
			this.clearNotification.gameObject.SetActive(clearProgress > 0f);
			return;
		}
		this.clearNotification.gameObject.SetActive(false);
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000C3EA8 File Offset: 0x000C20A8
	public void UpdateWireInfo()
	{
		float alpha = 0f;
		HeldEntity heldEntity = (LocalPlayer.Entity != null) ? LocalPlayer.Entity.GetHeldEntity() : null;
		if (heldEntity != null)
		{
			WireTool component = heldEntity.GetComponent<WireTool>();
			if (component && component.HasPendingWire())
			{
				bool flag = component.ValidSurface();
				float pendingLengthRemaining = component.GetPendingLengthRemaining();
				alpha = 1f;
				this.wireLengthText.text = pendingLengthRemaining.ToString("N2");
				this.wireClipsText.text = component.GetWireClicksRemaining().ToString("N0");
				this.errorReasonTextNoSurface.gameObject.SetActive(!flag);
				this.errorReasonTextTooFar.gameObject.SetActive(flag && pendingLengthRemaining <= 0f);
			}
		}
		this.wireInfoGroup.alpha = alpha;
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000C3F8C File Offset: 0x000C218C
	public void UpdateLookingHandle(bool includeInputs, bool includeOutputs)
	{
		Vector3 position = this.activeHandle.transform.position;
		Vector3 lhs = Vector3.zero;
		float num = 0.025f;
		float num2 = num;
		int num3 = -1;
		bool flag = false;
		if (includeInputs)
		{
			for (int i = 0; i < this.inputHandles.Count; i++)
			{
				GameObject gameObject = this.inputHandles[i];
				float num4 = Vector2.Distance(MainCamera.mainCamera.WorldToViewportPoint(gameObject.transform.position), Vector2.one * 0.5f);
				if (num4 < num2)
				{
					num2 = num4;
					lhs = gameObject.transform.position;
					num3 = i;
					flag = true;
				}
			}
		}
		if (includeOutputs)
		{
			for (int j = 0; j < this.outputHandles.Count; j++)
			{
				GameObject gameObject2 = this.outputHandles[j];
				float num5 = Vector2.Distance(MainCamera.mainCamera.WorldToViewportPoint(gameObject2.transform.position), Vector2.one * 0.5f);
				if (num5 < num2)
				{
					num2 = num5;
					lhs = gameObject2.transform.position;
					num3 = j;
					flag = false;
				}
			}
		}
		if (lhs != Vector3.zero && num2 < num)
		{
			LookAtIOEnt.selectedIndex = num3;
			LookAtIOEnt.selectedWasInput = flag;
			return;
		}
		LookAtIOEnt.selectedIndex = -1;
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000C40D8 File Offset: 0x000C22D8
	public Vector3 GetSelectedHandleWorldPosition()
	{
		if (LookAtIOEnt.selectedIndex == -1)
		{
			return Vector3.zero;
		}
		IOEntity.IOSlot ioslot = (LookAtIOEnt.selectedWasInput ? this.lastLooking.inputs : this.lastLooking.outputs)[LookAtIOEnt.selectedIndex];
		return this.lastLooking.transform.TransformPoint(ioslot.handlePosition);
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000C4130 File Offset: 0x000C2330
	public void UpdateLookingUI()
	{
		Vector3 position = this.activeHandle.transform.position;
		this.activeHandle.gameObject.SetActive(LookAtIOEnt.selectedIndex != -1);
		if (LookAtIOEnt.selectedIndex != -1)
		{
			Vector3 selectedHandleWorldPosition = this.GetSelectedHandleWorldPosition();
			this.activeHandle.transform.position = selectedHandleWorldPosition;
			float y = (this.lastLooking != null && this.lastLooking.HasMenuOptions) ? 100f : 32f;
			this.slotToolTip.localPosition = new Vector2(this.slotToolTip.localPosition.x, y);
			if (this.lastLooking != null)
			{
				IOEntity.IOSlot ioslot = (LookAtIOEnt.selectedWasInput ? this.lastLooking.inputs : this.lastLooking.outputs)[LookAtIOEnt.selectedIndex];
				if (ioslot != null)
				{
					IOEntity ioentity = ioslot.connectedTo.Get(false);
					this.slotTitle.text = ioslot.niceName;
					this.slotConnection.text = ((ioentity == null) ? "Empty" : ioentity.GetDisplayName());
					this.slotPower.text = (LookAtIOEnt.selectedWasInput ? this.lastLooking.client_powerin.ToString() : this.lastLooking.client_powerout.ToString());
					bool flag = (ioslot.rootConnectionsOnly && !this.wireToolWasPendingRoot) || (this.wireToolWasPendingRoot && !this.lastLooking.IsRootEntity());
					this.activeHandle.GetComponent<MaterialSwap>().SetOverrideEnabled((ioentity != null && (this.wiretoolWasPendingInput || this.wiretoolWasPendingOutput)) || flag);
					this.errorShortCircuit.gameObject.SetActive(this.lastLooking.HasFlag(BaseEntity.Flags.Reserved7));
				}
			}
		}
		this.slotToolTip.gameObject.SetActive(LookAtIOEnt.selectedIndex != -1);
		if (Vector3.Distance(this.activeHandle.transform.position, position) < 0.001f)
		{
			this.activeHandle.transform.localScale = Vector3.Lerp(this.activeHandle.transform.localScale, Vector3.one, Time.deltaTime * 12f);
		}
		else
		{
			this.activeHandle.transform.localScale = Vector3.zero;
		}
		this.slotToolTip.localScale = this.activeHandle.transform.localScale;
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x000C43AC File Offset: 0x000C25AC
	public bool UpdateLookingAtIOEnt()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (!entity)
		{
			return false;
		}
		HeldEntity heldEntity = entity.GetHeldEntity();
		if (heldEntity == null)
		{
			this.ClearHandles();
			return false;
		}
		WireTool component = heldEntity.GetComponent<WireTool>();
		if (component == null)
		{
			this.ClearHandles();
			return false;
		}
		IOEntity ioentity = (entity.lookingAtEntity != null && entity.lookingAtEntity is IOEntity) ? entity.lookingAtEntity.GetComponent<IOEntity>() : null;
		if (ioentity == null)
		{
			this.ClearHandles();
			return false;
		}
		bool flag = component.PendingPlugIsInput() != this.wiretoolWasPendingInput || component.PendingPlugIsOutput() != this.wiretoolWasPendingOutput;
		this.wiretoolWasPendingInput = component.PendingPlugIsInput();
		this.wiretoolWasPendingOutput = component.PendingPlugIsOutput();
		this.wireToolWasPendingRoot = component.PendingPlugRoot();
		if (ioentity != this.lastLooking || flag)
		{
			ioentity.RequestAdditionalData();
			this.powerText.text = "-";
			this.passthroughText.text = "-";
			this.SetupHandles(ioentity, !component.PendingPlugIsInput(), !component.PendingPlugIsOutput());
		}
		else if (Time.realtimeSinceStartup > this.nextUpdatetime)
		{
			this.nextUpdatetime = Time.realtimeSinceStartup + 1f;
			ioentity.RequestAdditionalData();
		}
		this.lastLooking = ioentity;
		this.UpdateLookingHandle(!component.PendingPlugIsInput(), !component.PendingPlugIsOutput());
		this.UpdateLookingUI();
		this.objectTitle.text = ioentity.GetDisplayName();
		this.powerText.gameObject.transform.parent.gameObject.SetActive(!ioentity.IsRootEntity());
		this.powerText.text = ioentity.client_powerin.ToString();
		this.powerText.color = (ioentity.IsPowered() ? Color.white : this.NoPowerColor);
		this.passthroughText.text = ioentity.client_powerout.ToString();
		this.powerText.gameObject.transform.parent.gameObject.SetActive(false);
		this.passthroughText.gameObject.transform.parent.gameObject.SetActive(false);
		ElectricBattery component2 = ioentity.GetComponent<ElectricBattery>();
		this.chargeLeftText.gameObject.transform.parent.gameObject.SetActive(component2 != null);
		if (component2 != null)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)component2.capacitySeconds);
			string text;
			if (timeSpan.Hours > 0)
			{
				text = string.Format("{0:N0}h {1:N0}m {2:N0}s ", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			else if (timeSpan.Minutes > 0)
			{
				text = string.Format("{0:N0}m {1:N0}s ", timeSpan.Minutes, timeSpan.Seconds);
			}
			else
			{
				text = string.Format("{0:N0}s ", timeSpan.Seconds);
			}
			this.chargeLeftText.text = text;
		}
		return true;
	}
}
