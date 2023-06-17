using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class SkinnedMeshCollider : MonoBehaviour, IClientComponent
{
	// Token: 0x04000DF5 RID: 3573
	public GameManifest.MeshColliderInfo colliderInfo;

	// Token: 0x04000DF6 RID: 3574
	private float rebuildTime = float.MinValue;

	// Token: 0x04000DF7 RID: 3575
	private const float rebuildDeltaTime = 0.033333335f;

	// Token: 0x04000DF8 RID: 3576
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x04000DF9 RID: 3577
	private List<Triangle> triangles = new List<Triangle>();

	// Token: 0x04000DFA RID: 3578
	private List<Transform> bones = new List<Transform>();

	// Token: 0x0600113B RID: 4411 RVA: 0x00072A10 File Offset: 0x00070C10
	public void TraceAll(SkinnedMeshCollision collision, HitTest test, List<TraceInfo> hits)
	{
		SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		this.UpdateMesh(component);
		Vector3 origin = base.transform.InverseTransformPoint(test.AttackRay.origin);
		Vector3 direction = base.transform.InverseTransformDirection(test.AttackRay.direction);
		Ray ray = new Ray(origin, direction);
		if (!this.Intersects(ray, test.Forgiveness, this.bounds))
		{
			return;
		}
		if (ConVar.Vis.hitboxes)
		{
			UnityEngine.DDraw.Bounds(base.transform.localToWorldMatrix, this.bounds, Color.white, 0.1f);
		}
		TraceInfo traceInfo = new TraceInfo
		{
			distance = float.MaxValue
		};
		float num = (test.BestHit && test.type != HitTest.Type.MeleeAttack) ? float.PositiveInfinity : test.MaxDistance;
		for (int i = 0; i < this.triangles.Count; i++)
		{
			RaycastHit raycastHit;
			if (this.triangles[i].Trace(ray, test.Forgiveness, ref raycastHit, num))
			{
				TraceInfo traceInfo2 = new TraceInfo
				{
					valid = true,
					distance = raycastHit.distance,
					bone = this.bones[i],
					partID = this.colliderInfo.hash,
					point = base.transform.TransformPoint(raycastHit.point),
					normal = base.transform.TransformDirection(raycastHit.normal),
					entity = collision.Owner
				};
				traceInfo2.collider = (traceInfo2.entity ? collision.Owner.GetComponent<Collider>() : null);
				if (this.colliderInfo.physicMaterial != null)
				{
					traceInfo2.material = this.colliderInfo.physicMaterial;
				}
				if (test.MultiHit)
				{
					hits.Add(traceInfo2);
				}
				else if (traceInfo.distance > traceInfo2.distance)
				{
					traceInfo = traceInfo2;
				}
			}
		}
		if (!test.MultiHit && traceInfo.distance < 3.4028235E+38f)
		{
			hits.Add(traceInfo);
		}
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0000F327 File Offset: 0x0000D527
	private bool Intersects(Ray ray, float radius, Bounds bounds)
	{
		bounds.Expand(radius);
		return bounds.IntersectRay(ray);
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x00072C34 File Offset: 0x00070E34
	private void UpdateMesh(SkinnedMeshRenderer renderer)
	{
		if (UnityEngine.Time.time < this.rebuildTime + 0.033333335f)
		{
			return;
		}
		this.rebuildTime = UnityEngine.Time.time;
		UnityEngine.Mesh mesh = AssetPool.Get<UnityEngine.Mesh>();
		List<int> list = Pool.GetList<int>();
		List<Vector3> list2 = Pool.GetList<Vector3>();
		List<BoneWeight> list3 = Pool.GetList<BoneWeight>();
		renderer.BakeMesh(mesh);
		this.bounds = mesh.bounds;
		renderer.sharedMesh.GetBoneWeights(list3);
		Transform[] array = renderer.bones;
		Transform rootBone = renderer.rootBone;
		mesh.GetTriangles(list, 0);
		mesh.GetVertices(list2);
		this.triangles.Clear();
		this.bones.Clear();
		for (int i = 0; i < list.Count; i += 3)
		{
			int num = list[i];
			int num2 = list[i + 1];
			int num3 = list[i + 2];
			Vector3 vector = list2[num];
			Vector3 vector2 = list2[num2];
			Vector3 vector3 = list2[num3];
			this.triangles.Add(new Triangle(vector, vector2, vector3));
			BoneWeight boneWeight = list3[num];
			Transform transform = rootBone;
			float num4 = 0f;
			if (boneWeight.weight0 > num4)
			{
				transform = array[boneWeight.boneIndex0];
				num4 = boneWeight.weight0;
			}
			if (boneWeight.weight1 > num4)
			{
				transform = array[boneWeight.boneIndex1];
				num4 = boneWeight.weight1;
			}
			if (boneWeight.weight2 > num4)
			{
				transform = array[boneWeight.boneIndex2];
				num4 = boneWeight.weight2;
			}
			if (boneWeight.weight3 > num4)
			{
				transform = array[boneWeight.boneIndex3];
				num4 = boneWeight.weight3;
			}
			this.bones.Add(transform);
		}
		AssetPool.Free(ref mesh);
		Pool.FreeList<int>(ref list);
		Pool.FreeList<Vector3>(ref list2);
		Pool.FreeList<BoneWeight>(ref list3);
	}
}
