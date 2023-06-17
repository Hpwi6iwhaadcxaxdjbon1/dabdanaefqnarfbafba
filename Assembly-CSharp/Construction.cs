using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200019D RID: 413
public class Construction : PrefabAttribute
{
	// Token: 0x04000B3C RID: 2876
	public BaseEntity.Menu.Option info;

	// Token: 0x04000B3D RID: 2877
	public bool canBypassBuildingPermission;

	// Token: 0x04000B3E RID: 2878
	public bool canRotate;

	// Token: 0x04000B3F RID: 2879
	public bool checkVolumeOnRotate;

	// Token: 0x04000B40 RID: 2880
	public bool checkVolumeOnUpgrade;

	// Token: 0x04000B41 RID: 2881
	public bool canPlaceAtMaxDistance;

	// Token: 0x04000B42 RID: 2882
	public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);

	// Token: 0x04000B43 RID: 2883
	[Range(0f, 10f)]
	public float healthMultiplier = 1f;

	// Token: 0x04000B44 RID: 2884
	[Range(0f, 10f)]
	public float costMultiplier = 1f;

	// Token: 0x04000B45 RID: 2885
	[Range(1f, 50f)]
	public float maxplaceDistance = 4f;

	// Token: 0x04000B46 RID: 2886
	public Mesh guideMesh;

	// Token: 0x04000B47 RID: 2887
	[NonSerialized]
	public Socket_Base[] allSockets;

	// Token: 0x04000B48 RID: 2888
	[NonSerialized]
	public BuildingProximity[] allProximities;

	// Token: 0x04000B49 RID: 2889
	[NonSerialized]
	public ConstructionGrade defaultGrade;

	// Token: 0x04000B4A RID: 2890
	[NonSerialized]
	public SocketHandle socketHandle;

	// Token: 0x04000B4B RID: 2891
	[NonSerialized]
	public Bounds bounds;

	// Token: 0x04000B4C RID: 2892
	[NonSerialized]
	public bool isBuildingPrivilege;

	// Token: 0x04000B4D RID: 2893
	[NonSerialized]
	public ConstructionGrade[] grades;

	// Token: 0x04000B4E RID: 2894
	[NonSerialized]
	public Deployable deployable;

	// Token: 0x04000B4F RID: 2895
	[NonSerialized]
	public ConstructionPlaceholder placeholder;

	// Token: 0x04000B50 RID: 2896
	public static string lastPlacementError;

	// Token: 0x06000E2E RID: 3630 RVA: 0x00063C90 File Offset: 0x00061E90
	public bool HasMaleSockets(Construction.Target target)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x00063CD4 File Offset: 0x00061ED4
	public void FindMaleSockets(Construction.Target target, List<Socket_Base> sockets)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				sockets.Add(socket_Base);
			}
		}
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x00063D1C File Offset: 0x00061F1C
	public GameObject CreateGuideSkin()
	{
		if (this.deployable)
		{
			if (this.deployable.guideMesh == null)
			{
				return new GameObject("Empty Guide");
			}
			GameObject gameObject = new GameObject("Deployable Guide");
			gameObject.AddComponent<MeshFilter>().sharedMesh = this.deployable.guideMesh;
			gameObject.transform.localScale = this.deployable.guideMeshScale;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			Material[] array = new Material[this.deployable.guideMesh.subMeshCount];
			for (int i = 0; i < this.deployable.guideMesh.subMeshCount; i++)
			{
				array[i] = FileSystem.Load<Material>("Assets/Content/materials/guide_bad.mat", true);
			}
			meshRenderer.sharedMaterials = array;
			return gameObject;
		}
		else
		{
			if (this.guideMesh)
			{
				GameObject gameObject2 = new GameObject("Construction Guide");
				gameObject2.AddComponent<MeshFilter>().sharedMesh = this.guideMesh;
				MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
				meshRenderer2.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer2.receiveShadows = false;
				Material[] array2 = new Material[this.guideMesh.subMeshCount];
				for (int j = 0; j < this.guideMesh.subMeshCount; j++)
				{
					array2[j] = FileSystem.Load<Material>("Assets/Content/materials/guide_bad.mat", true);
				}
				meshRenderer2.sharedMaterials = array2;
				return gameObject2;
			}
			foreach (ConstructionGrade constructionGrade in Enumerable.OrderBy<ConstructionGrade, BuildingGrade.Enum>(Enumerable.Where<ConstructionGrade>(this.grades, (ConstructionGrade x) => x != null), (ConstructionGrade x) => x.gradeBase.type))
			{
				if (constructionGrade.skinObject.isValid)
				{
					GameObject gameObject3 = GameManager.client.FindPrefab(constructionGrade.skinObject.resourcePath);
					if (!(gameObject3 == null))
					{
						GameObject gameObject4 = Facepunch.Instantiate.GameObject(gameObject3, Vector3.zero, Quaternion.identity);
						if (!(gameObject4 == null))
						{
							gameObject4.name = "Construction Guide";
							MonoBehaviour[] componentsInChildren = gameObject4.GetComponentsInChildren<MonoBehaviour>(true);
							for (int k = 0; k < componentsInChildren.Length; k++)
							{
								GameManager.Destroy(componentsInChildren[k], 0f);
							}
							Collider[] componentsInChildren2 = gameObject4.GetComponentsInChildren<Collider>(true);
							for (int k = 0; k < componentsInChildren2.Length; k++)
							{
								GameManager.Destroy(componentsInChildren2[k], 0f);
							}
							gameObject4.SetActive(true);
							return gameObject4;
						}
					}
				}
			}
			return null;
		}
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x00063FD0 File Offset: 0x000621D0
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isBuildingPrivilege = rootObj.GetComponent<BuildingPrivlidge>();
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.deployable = base.GetComponent<Deployable>();
		this.placeholder = base.GetComponentInChildren<ConstructionPlaceholder>();
		this.allSockets = base.GetComponentsInChildren<Socket_Base>(true);
		this.allProximities = base.GetComponentsInChildren<BuildingProximity>(true);
		this.socketHandle = Enumerable.FirstOrDefault<SocketHandle>(base.GetComponentsInChildren<SocketHandle>(true));
		ConstructionGrade[] components = rootObj.GetComponents<ConstructionGrade>();
		this.grades = new ConstructionGrade[5];
		foreach (ConstructionGrade constructionGrade in components)
		{
			constructionGrade.construction = this;
			this.grades[(int)constructionGrade.gradeBase.type] = constructionGrade;
		}
		for (int j = 0; j < this.grades.Length; j++)
		{
			if (!(this.grades[j] == null))
			{
				this.defaultGrade = this.grades[j];
				return;
			}
		}
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0000D139 File Offset: 0x0000B339
	protected override Type GetIndexedType()
	{
		return typeof(Construction);
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x000640C4 File Offset: 0x000622C4
	public bool UpdatePlacement(Transform transform, Construction common, ref Construction.Target target)
	{
		if (!target.valid)
		{
			return false;
		}
		if (!common.canBypassBuildingPermission && !target.player.CanBuild())
		{
			Construction.lastPlacementError = "Player doesn't have permission";
			return false;
		}
		List<Socket_Base> list = Pool.GetList<Socket_Base>();
		common.FindMaleSockets(target, list);
		foreach (Socket_Base socket_Base in list)
		{
			Construction.Placement placement = null;
			if (!(target.entity != null) || !(target.socket != null) || !target.entity.IsOccupied(target.socket))
			{
				if (placement == null)
				{
					placement = socket_Base.DoPlacement(target);
				}
				if (placement != null)
				{
					if (!socket_Base.CheckSocketMods(placement))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
					}
					else if (!this.TestPlacingThroughRock(ref placement, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through rock";
					}
					else if (!Construction.TestPlacingThroughWall(ref placement, transform, common, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through wall";
					}
					else if (Vector3.Distance(placement.position, target.player.eyes.position) > common.maxplaceDistance + 1f)
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Too far away";
					}
					else
					{
						DeployVolume[] volumes = PrefabAttribute.client.FindAll<DeployVolume>(this.prefabID);
						if (DeployVolume.Check(placement.position, placement.rotation, volumes, -1))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Not enough space";
						}
						else if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
						}
						else if (common.isBuildingPrivilege && !target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Cannot stack building privileges";
						}
						else
						{
							bool flag = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
							if (common.canBypassBuildingPermission || !flag)
							{
								target.inBuildingPrivilege = flag;
								transform.position = placement.position;
								transform.rotation = placement.rotation;
								Pool.FreeList<Socket_Base>(ref list);
								return true;
							}
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Building privilege";
						}
					}
				}
			}
		}
		Pool.FreeList<Socket_Base>(ref list);
		return false;
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x000643E4 File Offset: 0x000625E4
	private bool TestPlacingThroughRock(ref Construction.Placement placement, Construction.Target target)
	{
		OBB obb;
		obb..ctor(placement.position, Vector3.one, placement.rotation, this.bounds);
		Vector3 center = target.player.GetCenter(true);
		Vector3 origin = target.ray.origin;
		if (Physics.Linecast(center, origin, 65536, 1))
		{
			return false;
		}
		RaycastHit raycastHit;
		Vector3 vector = obb.Trace(target.ray, ref raycastHit, float.PositiveInfinity) ? raycastHit.point : obb.ClosestPoint(origin);
		return !Physics.Linecast(origin, vector, 65536, 1);
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x00064474 File Offset: 0x00062674
	private static bool TestPlacingThroughWall(ref Construction.Placement placement, Transform transform, Construction common, Construction.Target target)
	{
		Vector3 vector = placement.position - target.ray.origin;
		RaycastHit hit;
		if (!Physics.Raycast(target.ray.origin, vector.normalized, ref hit, vector.magnitude, 2097152))
		{
			return true;
		}
		StabilityEntity stabilityEntity = hit.GetEntity() as StabilityEntity;
		if (stabilityEntity != null && target.entity == stabilityEntity)
		{
			return true;
		}
		float num = vector.magnitude - hit.distance;
		if (num < 0.2f)
		{
			return true;
		}
		Construction.lastPlacementError = "object in placement path";
		if (Input.GetKey(KeyCode.KeypadDivide))
		{
			DDraw.Line(hit.point, target.ray.origin, Color.red, 0.1f, true, true);
			DDraw.Text(hit.collider.gameObject.name, hit.point, Color.red, 0.1f);
			DDraw.Text(num.ToString("0.00"), hit.point + Vector3.up * 0.5f, Color.red, 0.1f);
		}
		transform.position = hit.point;
		transform.rotation = placement.rotation;
		return false;
	}

	// Token: 0x0200019E RID: 414
	public class Grade
	{
		// Token: 0x04000B51 RID: 2897
		public BuildingGrade grade;

		// Token: 0x04000B52 RID: 2898
		public float maxHealth;

		// Token: 0x04000B53 RID: 2899
		public List<ItemAmount> costToBuild;

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000E37 RID: 3639 RVA: 0x0000D145 File Offset: 0x0000B345
		public PhysicMaterial physicMaterial
		{
			get
			{
				return this.grade.physicMaterial;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000E38 RID: 3640 RVA: 0x0000D152 File Offset: 0x0000B352
		public ProtectionProperties damageProtecton
		{
			get
			{
				return this.grade.damageProtecton;
			}
		}
	}

	// Token: 0x0200019F RID: 415
	public struct Target
	{
		// Token: 0x04000B54 RID: 2900
		public bool valid;

		// Token: 0x04000B55 RID: 2901
		public Ray ray;

		// Token: 0x04000B56 RID: 2902
		public BaseEntity entity;

		// Token: 0x04000B57 RID: 2903
		public Socket_Base socket;

		// Token: 0x04000B58 RID: 2904
		public bool onTerrain;

		// Token: 0x04000B59 RID: 2905
		public Vector3 position;

		// Token: 0x04000B5A RID: 2906
		public Vector3 normal;

		// Token: 0x04000B5B RID: 2907
		public Vector3 rotation;

		// Token: 0x04000B5C RID: 2908
		public BasePlayer player;

		// Token: 0x04000B5D RID: 2909
		public bool inBuildingPrivilege;

		// Token: 0x06000E3A RID: 3642 RVA: 0x00064608 File Offset: 0x00062808
		public Quaternion GetWorldRotation(bool female)
		{
			Quaternion rhs = this.socket.rotation;
			if (this.socket.male && this.socket.female && female)
			{
				rhs = this.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
			}
			return this.entity.transform.rotation * rhs;
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x0006467C File Offset: 0x0006287C
		public Vector3 GetWorldPosition()
		{
			return this.entity.transform.localToWorldMatrix.MultiplyPoint3x4(this.socket.position);
		}
	}

	// Token: 0x020001A0 RID: 416
	public class Placement
	{
		// Token: 0x04000B5E RID: 2910
		public Vector3 position;

		// Token: 0x04000B5F RID: 2911
		public Quaternion rotation;
	}
}
