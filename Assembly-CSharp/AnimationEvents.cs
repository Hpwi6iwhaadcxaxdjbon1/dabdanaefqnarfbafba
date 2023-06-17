using System;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class AnimationEvents : BaseMonoBehaviour
{
	// Token: 0x04000E91 RID: 3729
	public Transform rootObject;

	// Token: 0x04000E92 RID: 3730
	public HeldEntity targetEntity;

	// Token: 0x04000E93 RID: 3731
	[Tooltip("Path to the effect folder for these animations. Relative to this object.")]
	public string effectFolder;

	// Token: 0x04000E94 RID: 3732
	public string localFolder;

	// Token: 0x04000E95 RID: 3733
	public bool IsBusy;

	// Token: 0x060011E6 RID: 4582 RVA: 0x0000F907 File Offset: 0x0000DB07
	protected void OnEnable()
	{
		if (this.rootObject == null)
		{
			this.rootObject = base.transform;
		}
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0000F923 File Offset: 0x0000DB23
	public void SetBusy(int i)
	{
		this.IsBusy = (i != 0);
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x00076208 File Offset: 0x00074408
	public void Hide(string childName)
	{
		Transform transform = base.transform.Find(childName);
		if (!transform)
		{
			return;
		}
		transform.gameObject.GetComponent<Renderer>().enabled = false;
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x0007623C File Offset: 0x0007443C
	public void Show(string childName)
	{
		Transform transform = base.transform.Find(childName);
		if (!transform)
		{
			return;
		}
		transform.gameObject.GetComponent<Renderer>().enabled = true;
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x00076270 File Offset: 0x00074470
	public void HideGameObject(string childName)
	{
		Transform transform = base.transform.Find(childName);
		if (!transform)
		{
			return;
		}
		transform.gameObject.SetActive(false);
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x000762A0 File Offset: 0x000744A0
	public void ShowGameObject(string childName)
	{
		Transform transform = base.transform.Find(childName);
		if (!transform)
		{
			return;
		}
		transform.gameObject.SetActive(true);
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x000762D0 File Offset: 0x000744D0
	public void DoEffect(string strEvent)
	{
		if (!StringEx.IsLower(strEvent))
		{
			strEvent = strEvent.ToLower();
		}
		if (string.IsNullOrEmpty(this.effectFolder))
		{
			Effect.client.Run("assets/bundled/prefabs/fx/" + strEvent + ".prefab", base.gameObject);
			return;
		}
		Effect.client.Run(StringFormatCache.Get("{0}/{1}/{2}.prefab", this.localFolder, this.effectFolder, strEvent), base.gameObject);
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x0000F92F File Offset: 0x0000DB2F
	public void Broadcast(string strEvent)
	{
		this.rootObject.BroadcastMessage(strEvent, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0000F93E File Offset: 0x0000DB3E
	public void Message(string strEvent)
	{
		this.rootObject.SendMessage(strEvent, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0000F94D File Offset: 0x0000DB4D
	public void Event(string name)
	{
		if (SingletonComponent<ViewmodelEditor>.Instance != null)
		{
			SingletonComponent<ViewmodelEditor>.Instance.DoAnimationEvent(name);
		}
		if (this.targetEntity)
		{
			this.targetEntity.OnViewmodelEvent(name);
		}
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0000F980 File Offset: 0x0000DB80
	public void Strike()
	{
		base.Invoke(new Action(this.Event_Strike), 0f);
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x0000F999 File Offset: 0x0000DB99
	public void Event_Strike()
	{
		this.Event("Strike");
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x0000F9A6 File Offset: 0x0000DBA6
	public void Throw()
	{
		base.Invoke(new Action(this.Event_Throw), 0f);
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x0000F9BF File Offset: 0x0000DBBF
	public void Event_Throw()
	{
		this.Event("Throw");
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x00076338 File Offset: 0x00074538
	public void PlaySound(SoundDefinition def)
	{
		SoundManager.PlayOneshot(def, base.gameObject, false, default(Vector3));
	}
}
