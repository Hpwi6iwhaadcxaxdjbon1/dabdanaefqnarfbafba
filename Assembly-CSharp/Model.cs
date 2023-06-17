using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class Model : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x04000D20 RID: 3360
	public SphereCollider collision;

	// Token: 0x04000D21 RID: 3361
	public Transform rootBone;

	// Token: 0x04000D22 RID: 3362
	public Transform headBone;

	// Token: 0x04000D23 RID: 3363
	public Transform eyeBone;

	// Token: 0x04000D24 RID: 3364
	public Animator animator;

	// Token: 0x04000D25 RID: 3365
	[HideInInspector]
	public Transform[] boneTransforms;

	// Token: 0x04000D26 RID: 3366
	[HideInInspector]
	public string[] boneNames;

	// Token: 0x04000D27 RID: 3367
	internal int skin;

	// Token: 0x04000D28 RID: 3368
	private LODGroup _lodGroup;

	// Token: 0x06001049 RID: 4169 RVA: 0x0000E672 File Offset: 0x0000C872
	protected void OnEnable()
	{
		this.skin = -1;
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x0000E67B File Offset: 0x0000C87B
	public int GetSkin()
	{
		return this.skin;
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x0006E19C File Offset: 0x0006C39C
	private Transform FindBoneInternal(string name)
	{
		for (int i = 0; i < this.boneNames.Length; i++)
		{
			if (!(this.boneNames[i] != name))
			{
				return this.boneTransforms[i];
			}
		}
		return null;
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x0006E1D8 File Offset: 0x0006C3D8
	public Transform FindBone(string name)
	{
		Transform result = this.rootBone;
		if (string.IsNullOrEmpty(name))
		{
			return result;
		}
		for (int i = 0; i < this.boneNames.Length; i++)
		{
			if (this.boneNames[i].Equals(name, 3))
			{
				Transform transform = this.boneTransforms[i];
				if (!(transform == null))
				{
					result = transform;
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x0006E234 File Offset: 0x0006C434
	public Transform FindClosestBone(Vector3 worldPos)
	{
		Transform result = this.rootBone;
		float num = float.MaxValue;
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			Transform transform = this.boneTransforms[i];
			if (!(transform == null))
			{
				float num2 = Vector3.Distance(transform.position, worldPos);
				if (num2 < num)
				{
					result = transform;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x0600104E RID: 4174 RVA: 0x0000E683 File Offset: 0x0000C883
	public LODGroup lodGroup
	{
		get
		{
			if (this._lodGroup == null)
			{
				this._lodGroup = base.GetComponent<LODGroup>();
			}
			return this._lodGroup;
		}
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x0000E6A5 File Offset: 0x0000C8A5
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.lodGroup != null)
		{
			this.lodGroup.localReferencePoint = Vector3.zero;
			this.lodGroup.RecalculateBounds();
		}
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x0006E28C File Offset: 0x0006C48C
	public void SyncBones(Model other)
	{
		Transform[] array = other.boneTransforms;
		Transform[] array2 = this.boneTransforms;
		string[] array3 = other.boneNames;
		string[] array4 = this.boneNames;
		int num = Mathf.Min(array.Length, array2.Length);
		base.transform.position = other.transform.position;
		base.transform.rotation = other.transform.rotation;
		for (int i = 0; i < num; i++)
		{
			Transform transform;
			Transform transform2;
			if (array2.Length == array.Length)
			{
				transform = array[i];
				transform2 = array2[i];
			}
			else if (num == array.Length)
			{
				transform = array[i];
				transform2 = this.FindBoneInternal(array3[i]);
			}
			else
			{
				transform2 = array2[i];
				transform = other.FindBoneInternal(array4[i]);
			}
			if (!(transform == null) && !(transform2 == null))
			{
				transform2.position = transform.position;
				transform2.rotation = transform.rotation;
			}
		}
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x0000E6D8 File Offset: 0x0000C8D8
	public void Trigger(string triggerName)
	{
		if (this.animator == null)
		{
			return;
		}
		if (this.animator.isActiveAndEnabled)
		{
			this.animator.SetTrigger(triggerName);
		}
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x0006E378 File Offset: 0x0006C578
	public static Transform GetTransform(Transform bone, Vector3 position, BaseEntity entity)
	{
		Transform result;
		try
		{
			if (bone.gameObject.GetComponentInParent<Model>() != null)
			{
				result = bone;
			}
			else if (entity != null)
			{
				if (entity.model && entity.model.rootBone)
				{
					result = entity.model.FindClosestBone(position);
				}
				else
				{
					result = entity.transform;
				}
			}
			else
			{
				result = null;
			}
		}
		finally
		{
		}
		return result;
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x0006E3F4 File Offset: 0x0006C5F4
	public void ApplyVisibility(bool vis, bool animatorVis, bool shadowVis)
	{
		if (this.lodGroup != null)
		{
			float num = (float)(vis ? 0 : 100000);
			if (num != this.lodGroup.localReferencePoint.x)
			{
				this.lodGroup.localReferencePoint = new Vector3(num, num, num);
			}
		}
		if (this.animator != null)
		{
			this.animator.enabled = animatorVis;
		}
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x0006E45C File Offset: 0x0006C65C
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this == null)
		{
			return;
		}
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (this.rootBone == null)
		{
			this.rootBone = base.transform;
		}
		this.boneTransforms = this.rootBone.GetComponentsInChildren<Transform>(true);
		this.boneNames = new string[this.boneTransforms.Length];
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			this.boneNames[i] = this.boneTransforms[i].name;
		}
		if (clientside && Effects.motionblur && this.lodGroup != null)
		{
			ObjectMotionVectorFix.DisableObjectMotionVectors(this.lodGroup);
		}
	}
}
