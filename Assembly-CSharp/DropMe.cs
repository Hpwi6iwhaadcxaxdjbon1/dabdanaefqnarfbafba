using System;
using System.Linq;
using ConVar;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020006EC RID: 1772
public class DropMe : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	// Token: 0x0400230A RID: 8970
	public string[] droppableTypes;

	// Token: 0x06002720 RID: 10016 RVA: 0x000CBDB4 File Offset: 0x000C9FB4
	public bool Accepts(DragMe drag)
	{
		bool result = Enumerable.Contains<string>(this.droppableTypes, drag.dragType);
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log(string.Concat(new object[]
			{
				"[dragdrop] DropMe.Accepts ",
				base.gameObject,
				" = ",
				result.ToString()
			}));
		}
		return result;
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x00002ECE File Offset: 0x000010CE
	public void StopHighlight()
	{
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x00002ECE File Offset: 0x000010CE
	public void StartHighlight()
	{
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x000CBE10 File Offset: 0x000CA010
	public virtual void OnDrop(PointerEventData eventData)
	{
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log(string.Concat(new object[]
			{
				"[dragdrop] DropMe.OnDrop ",
				base.gameObject,
				" => ",
				DragMe.dragging
			}));
		}
		if (DragMe.dragging == null)
		{
			return;
		}
		if (!this.Accepts(DragMe.dragging))
		{
			return;
		}
		if (DragMe.data == null)
		{
			return;
		}
		object data = DragMe.data;
		if (data == null)
		{
			Debug.LogWarning("DropMe.OnDrop - no value");
			return;
		}
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] DropMe.OnDrop " + base.gameObject + " => OnDroppedValue");
		}
		base.SendMessage("OnDroppedValue", data, SendMessageOptions.DontRequireReceiver);
	}
}
