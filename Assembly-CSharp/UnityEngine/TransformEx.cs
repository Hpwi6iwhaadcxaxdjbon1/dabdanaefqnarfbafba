using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000839 RID: 2105
	public static class TransformEx
	{
		// Token: 0x06002D92 RID: 11666 RVA: 0x000E4FE8 File Offset: 0x000E31E8
		public static string GetRecursiveName(this Transform transform, string strEndName = "")
		{
			string text = transform.name;
			if (!string.IsNullOrEmpty(strEndName))
			{
				text = text + "/" + strEndName;
			}
			if (transform.parent != null)
			{
				text = transform.parent.GetRecursiveName(text);
			}
			return text;
		}

		// Token: 0x06002D93 RID: 11667 RVA: 0x000E5030 File Offset: 0x000E3230
		public static void RemoveComponent<T>(this Transform transform) where T : Component
		{
			T component = transform.GetComponent<T>();
			if (component == null)
			{
				return;
			}
			GameManager.Destroy(component, 0f);
		}

		// Token: 0x06002D94 RID: 11668 RVA: 0x000E5064 File Offset: 0x000E3264
		public static void DestroyAllChildren(this Transform transform, bool immediate = false)
		{
			List<GameObject> list = Pool.GetList<GameObject>();
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (!transform2.CompareTag("persist"))
				{
					list.Add(transform2.gameObject);
				}
			}
			if (immediate)
			{
				using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						GameObject instance = enumerator2.Current;
						GameManager.DestroyImmediate(instance, false);
					}
					goto IL_B6;
				}
			}
			foreach (GameObject instance2 in list)
			{
				GameManager.Destroy(instance2, 0f);
			}
			IL_B6:
			Pool.FreeList<GameObject>(ref list);
		}

		// Token: 0x06002D95 RID: 11669 RVA: 0x000E5158 File Offset: 0x000E3358
		public static void RetireAllChildren(this Transform transform, GameManager gameManager)
		{
			List<GameObject> list = Pool.GetList<GameObject>();
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (!transform2.CompareTag("persist"))
				{
					list.Add(transform2.gameObject);
				}
			}
			foreach (GameObject instance in list)
			{
				gameManager.Retire(instance);
			}
			Pool.FreeList<GameObject>(ref list);
		}

		// Token: 0x06002D96 RID: 11670 RVA: 0x000236FA File Offset: 0x000218FA
		public static List<Transform> GetChildren(this Transform transform)
		{
			return Enumerable.ToList<Transform>(Enumerable.Cast<Transform>(transform));
		}

		// Token: 0x06002D97 RID: 11671 RVA: 0x000E520C File Offset: 0x000E340C
		public static void OrderChildren(this Transform tx, Func<Transform, object> selector)
		{
			foreach (Transform transform in Enumerable.OrderBy<Transform, object>(Enumerable.Cast<Transform>(tx), selector))
			{
				transform.SetAsLastSibling();
			}
		}

		// Token: 0x06002D98 RID: 11672 RVA: 0x000E525C File Offset: 0x000E345C
		public static List<Transform> GetAllChildren(this Transform transform)
		{
			List<Transform> list = new List<Transform>();
			if (transform != null)
			{
				transform.AddAllChildren(list);
			}
			return list;
		}

		// Token: 0x06002D99 RID: 11673 RVA: 0x000E5280 File Offset: 0x000E3480
		public static void AddAllChildren(this Transform transform, List<Transform> list)
		{
			list.Add(transform);
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (!(child == null))
				{
					child.AddAllChildren(list);
				}
			}
		}

		// Token: 0x06002D9A RID: 11674 RVA: 0x000E52C0 File Offset: 0x000E34C0
		public static Transform[] GetChildrenWithTag(this Transform transform, string strTag)
		{
			return Enumerable.ToArray<Transform>(Enumerable.Where<Transform>(transform.GetAllChildren(), (Transform x) => x.CompareTag(strTag)));
		}

		// Token: 0x06002D9B RID: 11675 RVA: 0x00023707 File Offset: 0x00021907
		public static void Identity(this GameObject go)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
		}

		// Token: 0x06002D9C RID: 11676 RVA: 0x00023739 File Offset: 0x00021939
		public static GameObject CreateChild(this GameObject go)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = go.transform;
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06002D9D RID: 11677 RVA: 0x00023757 File Offset: 0x00021957
		public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
		{
			GameObject gameObject = Instantiate.GameObject(prefab, null);
			gameObject.transform.SetParent(go.transform, false);
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06002D9E RID: 11678 RVA: 0x000E52F8 File Offset: 0x000E34F8
		public static void SetLayerRecursive(this GameObject go, int Layer)
		{
			if (go.layer != Layer)
			{
				go.layer = Layer;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				go.transform.GetChild(i).gameObject.SetLayerRecursive(Layer);
			}
		}

		// Token: 0x06002D9F RID: 11679 RVA: 0x000E5344 File Offset: 0x000E3544
		public static bool DropToGround(this Transform transform, bool alignToNormal = false, float fRange = 100f)
		{
			Vector3 position;
			Vector3 upwards;
			if (transform.GetGroundInfo(out position, out upwards, fRange))
			{
				transform.position = position;
				if (alignToNormal)
				{
					transform.rotation = Quaternion.LookRotation(transform.forward, upwards);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06002DA0 RID: 11680 RVA: 0x00023778 File Offset: 0x00021978
		public static bool GetGroundInfo(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfo(transform.position, out pos, out normal, range, transform);
		}

		// Token: 0x06002DA1 RID: 11681 RVA: 0x00023789 File Offset: 0x00021989
		public static bool GetGroundInfoTerrainOnly(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfoTerrainOnly(transform.position, out pos, out normal, range);
		}

		// Token: 0x06002DA2 RID: 11682 RVA: 0x000E5380 File Offset: 0x000E3580
		public static Bounds WorkoutRenderBounds(this Transform tx)
		{
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			foreach (Renderer renderer in tx.GetComponentsInChildren<Renderer>())
			{
				if (!(renderer is ParticleSystemRenderer))
				{
					if (bounds.center == Vector3.zero)
					{
						bounds = renderer.bounds;
					}
					else
					{
						bounds.Encapsulate(renderer.bounds);
					}
				}
			}
			return bounds;
		}

		// Token: 0x06002DA3 RID: 11683 RVA: 0x000E53EC File Offset: 0x000E35EC
		public static List<T> GetSiblings<T>(this Transform transform, bool includeSelf = false)
		{
			List<T> list = new List<T>();
			if (transform.parent == null)
			{
				return list;
			}
			for (int i = 0; i < transform.parent.childCount; i++)
			{
				Transform child = transform.parent.GetChild(i);
				if (includeSelf || !(child == transform))
				{
					T component = child.GetComponent<T>();
					if (component != null)
					{
						list.Add(component);
					}
				}
			}
			return list;
		}

		// Token: 0x06002DA4 RID: 11684 RVA: 0x000E5458 File Offset: 0x000E3658
		public static void DestroyChildren(this Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				GameManager.Destroy(transform.GetChild(i).gameObject, 0f);
			}
		}

		// Token: 0x06002DA5 RID: 11685 RVA: 0x000E548C File Offset: 0x000E368C
		public static void SetChildrenActive(this Transform transform, bool b)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(b);
			}
		}

		// Token: 0x06002DA6 RID: 11686 RVA: 0x000E54BC File Offset: 0x000E36BC
		public static Transform ActiveChild(this Transform transform, string name, bool bDisableOthers)
		{
			Transform result = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name.Equals(name, 3))
				{
					result = child;
					child.gameObject.SetActive(true);
				}
				else if (bDisableOthers)
				{
					child.gameObject.SetActive(false);
				}
			}
			return result;
		}

		// Token: 0x06002DA7 RID: 11687 RVA: 0x000E5514 File Offset: 0x000E3714
		public static T GetComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
		{
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			T result = (list.Count > 0) ? list[0] : default(T);
			Pool.FreeList<T>(ref list);
			return result;
		}

		// Token: 0x06002DA8 RID: 11688 RVA: 0x00023799 File Offset: 0x00021999
		public static void SetHierarchyGroup(this Transform transform, string strRoot, bool groupActive = true, bool persistant = false)
		{
			transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		// Token: 0x06002DA9 RID: 11689 RVA: 0x000E5554 File Offset: 0x000E3754
		public static Bounds GetBounds(this Transform transform, bool includeRenderers = true, bool includeColliders = true, bool includeInactive = true)
		{
			Bounds result = new Bounds(Vector3.zero, Vector3.zero);
			if (includeRenderers)
			{
				foreach (MeshFilter meshFilter in transform.GetComponentsInChildren<MeshFilter>(includeInactive))
				{
					if (meshFilter.sharedMesh)
					{
						Matrix4x4 matrix = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
						Bounds bounds = meshFilter.sharedMesh.bounds;
						result.Encapsulate(bounds.Transform(matrix));
					}
				}
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive))
				{
					if (skinnedMeshRenderer.sharedMesh)
					{
						Matrix4x4 matrix2 = transform.worldToLocalMatrix * skinnedMeshRenderer.transform.localToWorldMatrix;
						Bounds bounds2 = skinnedMeshRenderer.sharedMesh.bounds;
						result.Encapsulate(bounds2.Transform(matrix2));
					}
				}
			}
			if (includeColliders)
			{
				foreach (MeshCollider meshCollider in transform.GetComponentsInChildren<MeshCollider>(includeInactive))
				{
					if (meshCollider.sharedMesh && !meshCollider.isTrigger)
					{
						Matrix4x4 matrix3 = transform.worldToLocalMatrix * meshCollider.transform.localToWorldMatrix;
						Bounds bounds3 = meshCollider.sharedMesh.bounds;
						result.Encapsulate(bounds3.Transform(matrix3));
					}
				}
			}
			return result;
		}
	}
}
