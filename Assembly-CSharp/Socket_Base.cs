using System;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class Socket_Base : PrefabAttribute
{
	// Token: 0x04000BA0 RID: 2976
	public bool male = true;

	// Token: 0x04000BA1 RID: 2977
	public bool maleDummy;

	// Token: 0x04000BA2 RID: 2978
	public bool female;

	// Token: 0x04000BA3 RID: 2979
	public bool femaleDummy;

	// Token: 0x04000BA4 RID: 2980
	public bool monogamous;

	// Token: 0x04000BA5 RID: 2981
	[NonSerialized]
	public Vector3 position;

	// Token: 0x04000BA6 RID: 2982
	[NonSerialized]
	public Quaternion rotation;

	// Token: 0x04000BA7 RID: 2983
	public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);

	// Token: 0x04000BA8 RID: 2984
	public Vector3 selectCenter = new Vector3(0f, 0f, 1f);

	// Token: 0x04000BA9 RID: 2985
	[HideInInspector]
	public string socketName;

	// Token: 0x04000BAA RID: 2986
	[NonSerialized]
	public SocketMod[] socketMods;

	// Token: 0x06000E7F RID: 3711 RVA: 0x0000D0DB File Offset: 0x0000B2DB
	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + rotation * this.worldPosition;
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0000D339 File Offset: 0x0000B539
	public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
	{
		return new OBB(position + rotation * this.worldPosition, Vector3.one, rotation * this.worldRotation, new Bounds(this.selectCenter, this.selectSize));
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0000D374 File Offset: 0x0000B574
	protected override Type GetIndexedType()
	{
		return typeof(Socket_Base);
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x000654F4 File Offset: 0x000636F4
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		this.socketMods = base.GetComponentsInChildren<SocketMod>(true);
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].baseSocket = this;
		}
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0000D380 File Offset: 0x0000B580
	public virtual bool TestTarget(Construction.Target target)
	{
		return target.socket != null;
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0006555C File Offset: 0x0006375C
	public virtual bool IsCompatible(Socket_Base socket)
	{
		return !(socket == null) && (socket.male || this.male) && (socket.female || this.female) && socket.GetType() == base.GetType();
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0000D38E File Offset: 0x0000B58E
	public virtual bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		return this.IsCompatible(socket);
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x000655AC File Offset: 0x000637AC
	public virtual Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion = Quaternion.LookRotation(target.normal, Vector3.up) * Quaternion.Euler(target.rotation);
		Vector3 a = target.position;
		a -= quaternion * this.position;
		return new Construction.Placement
		{
			rotation = quaternion,
			position = a
		};
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x00065608 File Offset: 0x00063808
	public virtual bool CheckSocketMods(Construction.Placement placement)
	{
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModifyPlacement(placement);
		}
		foreach (SocketMod socketMod in this.socketMods)
		{
			if (!socketMod.DoCheck(placement))
			{
				if (socketMod.FailedPhrase.IsValid())
				{
					Construction.lastPlacementError = "Failed Check: (" + socketMod.FailedPhrase.translated + ")";
				}
				return false;
			}
		}
		return true;
	}
}
