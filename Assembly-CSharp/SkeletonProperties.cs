using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
[CreateAssetMenu(menuName = "Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
	// Token: 0x04001561 RID: 5473
	public GameObject boneReference;

	// Token: 0x04001562 RID: 5474
	[BoneProperty]
	public SkeletonProperties.BoneProperty[] bones;

	// Token: 0x04001563 RID: 5475
	[NonSerialized]
	private Dictionary<uint, SkeletonProperties.BoneProperty> quickLookup;

	// Token: 0x060018FE RID: 6398 RVA: 0x0008ECD8 File Offset: 0x0008CED8
	public void OnValidate()
	{
		if (this.boneReference == null)
		{
			Debug.LogWarning("boneReference is null", this);
			return;
		}
		List<SkeletonProperties.BoneProperty> list = Enumerable.ToList<SkeletonProperties.BoneProperty>(this.bones);
		using (List<Transform>.Enumerator enumerator = this.boneReference.transform.GetAllChildren().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = enumerator.Current;
				if (Enumerable.All<SkeletonProperties.BoneProperty>(list, (SkeletonProperties.BoneProperty x) => x.bone != child.gameObject))
				{
					list.Add(new SkeletonProperties.BoneProperty
					{
						bone = child.gameObject,
						name = new Translate.Phrase("", "")
						{
							token = child.name.ToLower(),
							english = child.name.ToLower()
						}
					});
				}
			}
		}
		this.bones = list.ToArray();
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x0008EDE4 File Offset: 0x0008CFE4
	private void BuildDictionary()
	{
		this.quickLookup = new Dictionary<uint, SkeletonProperties.BoneProperty>();
		foreach (SkeletonProperties.BoneProperty boneProperty in this.bones)
		{
			uint num = StringPool.Get(boneProperty.bone.name);
			if (!this.quickLookup.ContainsKey(num))
			{
				this.quickLookup.Add(num, boneProperty);
			}
			else
			{
				string name = boneProperty.bone.name;
				string name2 = this.quickLookup[num].bone.name;
				Debug.LogWarning(string.Concat(new object[]
				{
					"Duplicate bone id ",
					num,
					" for ",
					name,
					" and ",
					name2
				}));
			}
		}
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x0008EEAC File Offset: 0x0008D0AC
	public SkeletonProperties.BoneProperty FindBone(uint id)
	{
		if (this.quickLookup == null)
		{
			this.BuildDictionary();
		}
		SkeletonProperties.BoneProperty result = null;
		if (!this.quickLookup.TryGetValue(id, ref result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x020003EA RID: 1002
	[Serializable]
	public class BoneProperty
	{
		// Token: 0x04001564 RID: 5476
		public GameObject bone;

		// Token: 0x04001565 RID: 5477
		public Translate.Phrase name;

		// Token: 0x04001566 RID: 5478
		public HitArea area;
	}
}
