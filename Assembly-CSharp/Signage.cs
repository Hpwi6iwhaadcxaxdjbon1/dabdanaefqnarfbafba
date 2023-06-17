using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class Signage : BaseCombatEntity, ILOD
{
	// Token: 0x040002BF RID: 703
	private Option __menuOption_Menu_ChangeText;

	// Token: 0x040002C0 RID: 704
	private Option __menuOption_Menu_LockSign;

	// Token: 0x040002C1 RID: 705
	private Option __menuOption_Menu_UnLockSign;

	// Token: 0x040002C2 RID: 706
	private float textureRequestDistance = 100f;

	// Token: 0x040002C3 RID: 707
	private bool textureRequestSent;

	// Token: 0x040002C4 RID: 708
	private LODCell cell;

	// Token: 0x040002C5 RID: 709
	public GameObjectRef changeTextDialog;

	// Token: 0x040002C6 RID: 710
	public MeshPaintableSource paintableSource;

	// Token: 0x040002C7 RID: 711
	[NonSerialized]
	public uint textureID;

	// Token: 0x040002C8 RID: 712
	internal uint loadedTexture;

	// Token: 0x06000581 RID: 1409 RVA: 0x0003FAB8 File Offset: 0x0003DCB8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Signage.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_ChangeText", 0.1f))
			{
				if (this.CanUpdateSign(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ChangeText.show = true;
					this.__menuOption_Menu_ChangeText.showDisabled = false;
					this.__menuOption_Menu_ChangeText.longUseOnly = false;
					this.__menuOption_Menu_ChangeText.order = -10;
					this.__menuOption_Menu_ChangeText.icon = "sign";
					this.__menuOption_Menu_ChangeText.desc = "sign_paint_desc";
					this.__menuOption_Menu_ChangeText.title = "sign_paint";
					if (this.__menuOption_Menu_ChangeText.function == null)
					{
						this.__menuOption_Menu_ChangeText.function = new Action<BasePlayer>(this.Menu_ChangeText);
					}
					list.Add(this.__menuOption_Menu_ChangeText);
				}
			}
			using (TimeWarning.New("Menu_LockSign", 0.1f))
			{
				if (this.CanLockSign(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_LockSign.show = true;
					this.__menuOption_Menu_LockSign.showDisabled = false;
					this.__menuOption_Menu_LockSign.longUseOnly = false;
					this.__menuOption_Menu_LockSign.order = 0;
					this.__menuOption_Menu_LockSign.icon = "lock";
					this.__menuOption_Menu_LockSign.desc = "sign_lock_desc";
					this.__menuOption_Menu_LockSign.title = "sign_lock";
					if (this.__menuOption_Menu_LockSign.function == null)
					{
						this.__menuOption_Menu_LockSign.function = new Action<BasePlayer>(this.Menu_LockSign);
					}
					list.Add(this.__menuOption_Menu_LockSign);
				}
			}
			using (TimeWarning.New("Menu_UnLockSign", 0.1f))
			{
				if (this.CanUnlockSign(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_UnLockSign.show = true;
					this.__menuOption_Menu_UnLockSign.showDisabled = false;
					this.__menuOption_Menu_UnLockSign.longUseOnly = false;
					this.__menuOption_Menu_UnLockSign.order = 0;
					this.__menuOption_Menu_UnLockSign.icon = "unlock";
					this.__menuOption_Menu_UnLockSign.desc = "sign_unlock_desc";
					this.__menuOption_Menu_UnLockSign.title = "sign_unlock";
					if (this.__menuOption_Menu_UnLockSign.function == null)
					{
						this.__menuOption_Menu_UnLockSign.function = new Action<BasePlayer>(this.Menu_UnLockSign);
					}
					list.Add(this.__menuOption_Menu_UnLockSign);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000582 RID: 1410 RVA: 0x00006E1D File Offset: 0x0000501D
	public override bool HasMenuOptions
	{
		get
		{
			return this.CanUpdateSign(LocalPlayer.Entity) || this.CanLockSign(LocalPlayer.Entity) || this.CanUnlockSign(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0003FD80 File Offset: 0x0003DF80
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Signage.OnRpcMessage", 0.1f))
		{
			if (rpc == 393230256U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: RecieveTexture ");
				}
				using (TimeWarning.New("RecieveTexture", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RecieveTexture(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in RecieveTexture", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00006E52 File Offset: 0x00005052
	public virtual bool CanUpdateSign(BasePlayer player)
	{
		return player.IsAdmin || player.IsDeveloper || (player.CanBuild() && (!base.IsLocked() || player.userID == base.OwnerID));
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00006E88 File Offset: 0x00005088
	public bool CanUnlockSign(BasePlayer player)
	{
		return base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00006E9B File Offset: 0x0000509B
	public bool CanLockSign(BasePlayer player)
	{
		return !base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0003FE9C File Offset: 0x0003E09C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sign != null && info.msg.sign.imageid != this.textureID)
		{
			this.textureID = info.msg.sign.imageid;
			if (base.isClient && this.textureID > 0U)
			{
				this.RequestTextureUpdate();
			}
		}
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00006EAE File Offset: 0x000050AE
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		LODGrid.Add(this, base.transform, ref this.cell);
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0003FF04 File Offset: 0x0003E104
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		if (UIDialog.isOpen)
		{
			UIDialog uidialog = ListComponent<UIDialog>.InstanceList[0];
			if (uidialog != null)
			{
				uidialog.CloseDialog();
			}
		}
		LODGrid.Remove(this, base.transform, ref this.cell);
		this.FreeTexture();
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00006EC9 File Offset: 0x000050C9
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0003FF54 File Offset: 0x0003E154
	public void ChangeLOD()
	{
		if (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(this.textureRequestDistance))
		{
			if (!this.textureRequestSent)
			{
				this.textureRequestSent = true;
				base.Invoke(new Action(this.RequestTextureUpdate), 0.1f);
				return;
			}
		}
		else if (this.textureRequestSent)
		{
			this.textureRequestSent = false;
			this.FreeTexture();
		}
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00006EDD File Offset: 0x000050DD
	[BaseEntity.Menu.Icon("lock")]
	[BaseEntity.Menu.Description("sign_lock_desc", "Prevent any further edits to this sign. No-one else will be able to edit the sign anymore.")]
	[BaseEntity.Menu("sign_lock", "Lock Edits")]
	[BaseEntity.Menu.ShowIf("CanLockSign")]
	public void Menu_LockSign(BasePlayer player)
	{
		base.ServerRPC("LockSign");
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00006EEA File Offset: 0x000050EA
	[BaseEntity.Menu.Description("sign_unlock_desc", "Allow anyone to edit this sign")]
	[BaseEntity.Menu("sign_unlock", "Unlock Edits")]
	[BaseEntity.Menu.ShowIf("CanUnlockSign")]
	[BaseEntity.Menu.Icon("unlock")]
	public void Menu_UnLockSign(BasePlayer player)
	{
		base.ServerRPC("UnLockSign");
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0003FFB8 File Offset: 0x0003E1B8
	public static void RebuildAll()
	{
		foreach (BaseNetworkable baseNetworkable in BaseNetworkable.clientEntities)
		{
			Signage signage = baseNetworkable as Signage;
			if (!(signage == null))
			{
				signage.RequestTextureUpdate();
			}
		}
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00006EF7 File Offset: 0x000050F7
	public override string Categorize()
	{
		return "sign";
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x00006EFE File Offset: 0x000050FE
	private void FreeTexture()
	{
		if (this.paintableSource)
		{
			this.paintableSource.Free();
		}
		this.loadedTexture = 0U;
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x00040014 File Offset: 0x0003E214
	private void RequestTextureUpdate()
	{
		if (this.textureID == 0U)
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		byte[] array = FileStorage.client.Get(this.textureID, FileStorage.Type.png, this.net.ID);
		if (array == null)
		{
			base.RequestFileFromServer(this.textureID, FileStorage.Type.png, "RecieveTexture");
			return;
		}
		this.LoadTexture(this.textureID, array);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00040074 File Offset: 0x0003E274
	private void LoadTexture(uint id, byte[] data)
	{
		if (Global.censorsigns)
		{
			this.loadedTexture = 0U;
			this.paintableSource.Clear();
			return;
		}
		if (id == this.loadedTexture)
		{
			return;
		}
		this.loadedTexture = id;
		if (this.paintableSource)
		{
			this.paintableSource.Load(data);
		}
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x000400C8 File Offset: 0x0003E2C8
	[BaseEntity.Menu.Icon("sign")]
	[BaseEntity.Menu.ShowIf("CanUpdateSign")]
	[BaseEntity.Menu.Description("sign_paint_desc", "Paint a new design on this sign")]
	[BaseEntity.Menu("sign_paint", "Paint Sign", Order = -10)]
	public void Menu_ChangeText(BasePlayer player)
	{
		if (base.GetComponentInChildren<MeshPaintableSource>() == null)
		{
			return;
		}
		ChangeSignText component = GameManager.client.CreatePrefab(this.changeTextDialog.resourcePath, true).GetComponent<ChangeSignText>();
		component.Setup(this.paintableSource);
		component.onUpdateTexture = delegate(Texture2D x)
		{
			this.OnTextureWasEdited(x);
		};
		component.OpenDialog();
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00040124 File Offset: 0x0003E324
	public void OnTextureWasEdited(Texture2D texture)
	{
		if (this.paintableSource == null)
		{
			return;
		}
		this.paintableSource.UpdateFrom(texture);
		byte[] array = ImageConversion.EncodeToPNG(this.paintableSource.texture);
		FileStorage.client.RemoveEntityNum(this.net.ID, 0U);
		FileStorage.client.Store(array, FileStorage.Type.png, this.net.ID, 0U);
		base.ServerRPC<uint, byte[]>("UpdateSign", (uint)array.Length, array);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0004019C File Offset: 0x0003E39C
	[BaseEntity.RPC_Client]
	public void RecieveTexture(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		byte[] data = msg.read.BytesWithSize();
		uint entityID = (msg.read.unread > 0) ? msg.read.UInt32() : this.net.ID;
		if (!ImageProcessing.IsValidPNG(data, 1024, 1024))
		{
			return;
		}
		if (FileStorage.client.Store(data, FileStorage.Type.png, entityID, 0U) != num)
		{
			Debug.LogWarning("Client/Server FileStorage CRC differs");
		}
		this.LoadTexture(num, data);
		this.textureID = num;
	}
}
