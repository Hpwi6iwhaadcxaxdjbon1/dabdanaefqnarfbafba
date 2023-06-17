using System;
using Rust;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class Gib : MonoBehaviour
{
	// Token: 0x04000CAD RID: 3245
	public static int gibCount;

	// Token: 0x04000CAE RID: 3246
	[NonSerialized]
	public MeshFilter meshFilter;

	// Token: 0x04000CAF RID: 3247
	[NonSerialized]
	public MeshRenderer meshRenderer;

	// Token: 0x06000FDD RID: 4061 RVA: 0x0000E08C File Offset: 0x0000C28C
	private void Start()
	{
		GameManager.Destroy(base.gameObject, 5f + Random.Range(0f, 5f));
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x0000E0AE File Offset: 0x0000C2AE
	private void OnEnable()
	{
		Gib.gibCount++;
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x0000E0BC File Offset: 0x0000C2BC
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		Gib.gibCount--;
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x0006BB68 File Offset: 0x00069D68
	public static Gib Create(Mesh mesh, Material[] materials, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = new GameObject("GIB");
		gameObject.layer = 26;
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		Gib gib = gameObject.AddComponent<Gib>();
		gib.meshFilter = gameObject.AddComponent<MeshFilter>();
		gib.meshFilter.sharedMesh = mesh;
		gib.meshRenderer = gameObject.AddComponent<MeshRenderer>();
		gib.meshRenderer.sharedMaterials = materials;
		return gib;
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x0000E0D2 File Offset: 0x0000C2D2
	public void CreatePhysics(PhysicMaterial physicsMaterial)
	{
		this.CreatePhysics(physicsMaterial, Vector3Ex.Range(-1f, 1f), Vector3Ex.Range(-10f, 10f));
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x0006BBD8 File Offset: 0x00069DD8
	public void CreatePhysics(PhysicMaterial physicsMaterial, Vector3 vel, Vector3 angVel)
	{
		MeshCollider meshCollider = base.gameObject.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = this.meshFilter.sharedMesh;
		meshCollider.convex = true;
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		rigidbody.mass = meshCollider.bounds.size.magnitude;
		rigidbody.interpolation = 1;
		rigidbody.velocity = vel;
		rigidbody.angularVelocity = angVel;
		rigidbody.WakeUp();
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x0006BC54 File Offset: 0x00069E54
	public static string GetEffect(PhysicMaterial physicMaterial)
	{
		string nameLower = physicMaterial.GetNameLower();
		if (nameLower == "wood")
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		if (nameLower == "concrete")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (nameLower == "metal")
		{
			return "assets/bundled/prefabs/fx/building/metal_sheet_gib.prefab";
		}
		if (nameLower == "rock")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (!(nameLower == "flesh"))
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
	}
}
