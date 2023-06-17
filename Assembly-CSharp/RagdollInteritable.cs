using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000223 RID: 547
public static class RagdollInteritable
{
	// Token: 0x060010B0 RID: 4272 RVA: 0x00070968 File Offset: 0x0006EB68
	public static void Inherit(Model from, Model to)
	{
		List<IRagdollInhert> list = Pool.GetList<IRagdollInhert>();
		from.GetComponentsInChildren<IRagdollInhert>(true, list);
		foreach (IRagdollInhert ragdollInhert in list)
		{
			Transform transform = ragdollInhert.RagdollInhertTransform();
			if (!(transform == null))
			{
				Transform transform2 = null;
				if (transform.parent == from.transform)
				{
					transform2 = to.transform;
				}
				if (transform2 == null)
				{
					transform2 = to.FindBone(transform.parent.name);
				}
				if (transform2 == null)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"RagdollInheret: Couldn't find ",
						transform.parent.name,
						" to move ",
						transform,
						" to from ",
						from,
						" to ",
						to
					}));
					transform2 = to.transform;
				}
				Vector3 localPosition = transform.localPosition;
				Quaternion localRotation = transform.localRotation;
				transform.parent = transform2;
				transform.localPosition = localPosition;
				transform.localRotation = localRotation;
			}
		}
		Pool.FreeList<IRagdollInhert>(ref list);
	}
}
