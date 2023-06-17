using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Network;
using UnityEngine;

// Token: 0x02000245 RID: 581
[ConsoleSystem.Factory("global")]
public class DiagnosticsConSys : ConsoleSystem
{
	// Token: 0x06001158 RID: 4440 RVA: 0x00073958 File Offset: 0x00071B58
	private static void DumpAnimators(string targetFolder)
	{
		Animator[] array = Object.FindObjectsOfType<Animator>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All animators");
		stringBuilder.AppendLine();
		foreach (Animator animator in array)
		{
			stringBuilder.AppendFormat("{1}\t{0}", animator.transform.GetRecursiveName(""), animator.enabled);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All animators - grouped by object name");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Animator> grouping in Enumerable.OrderByDescending<IGrouping<string, Animator>, int>(Enumerable.GroupBy<Animator, string>(array, (Animator x) => x.transform.GetRecursiveName("")), (IGrouping<string, Animator> x) => Enumerable.Count<Animator>(x)))
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", Enumerable.First<Animator>(grouping).transform.GetRecursiveName(""), Enumerable.Count<Animator>(grouping));
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("All animators - grouped by enabled/disabled");
		stringBuilder3.AppendLine();
		foreach (IGrouping<string, Animator> grouping2 in Enumerable.OrderByDescending<IGrouping<string, Animator>, int>(Enumerable.GroupBy<Animator, string>(array, (Animator x) => x.transform.GetRecursiveName(x.enabled ? "" : " (DISABLED)")), (IGrouping<string, Animator> x) => Enumerable.Count<Animator>(x)))
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", Enumerable.First<Animator>(grouping2).transform.GetRecursiveName(Enumerable.First<Animator>(grouping2).enabled ? "" : " (DISABLED)"), Enumerable.Count<Animator>(grouping2));
			stringBuilder3.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.Enabled.txt", stringBuilder3.ToString());
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x00073BC8 File Offset: 0x00071DC8
	[ServerVar]
	[ClientVar]
	public static void dump(ConsoleSystem.Arg args)
	{
		if (Directory.Exists("diagnostics"))
		{
			Directory.CreateDirectory("diagnostics");
		}
		int num = 1;
		while (Directory.Exists("diagnostics/" + num))
		{
			num++;
		}
		Directory.CreateDirectory("diagnostics/" + num);
		string targetFolder = "diagnostics/" + num + "/";
		DiagnosticsConSys.DumpLODGroups(targetFolder);
		DiagnosticsConSys.DumpSystemInformation(targetFolder);
		DiagnosticsConSys.DumpGameObjects(targetFolder);
		DiagnosticsConSys.DumpObjects(targetFolder);
		DiagnosticsConSys.DumpEntities(targetFolder);
		DiagnosticsConSys.DumpNetwork(targetFolder);
		DiagnosticsConSys.DumpPhysics(targetFolder);
		DiagnosticsConSys.DumpAnimators(targetFolder);
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0000F477 File Offset: 0x0000D677
	private static void DumpSystemInformation(string targetFolder)
	{
		DiagnosticsConSys.WriteTextToFile(targetFolder + "System.Info.txt", SystemInfoGeneralText.currentInfo);
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0000F48E File Offset: 0x0000D68E
	private static void WriteTextToFile(string file, string text)
	{
		File.WriteAllText(file, text);
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x00073C68 File Offset: 0x00071E68
	private static void DumpEntities(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All entities");
		stringBuilder.AppendLine();
		foreach (BaseNetworkable baseNetworkable in BaseNetworkable.clientEntities)
		{
			stringBuilder.AppendFormat("{1}\t{0}", baseNetworkable.PrefabName, (baseNetworkable.net != null) ? baseNetworkable.net.ID : 0U);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.CL.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All entities");
		stringBuilder2.AppendLine();
		foreach (IGrouping<uint, BaseNetworkable> grouping in Enumerable.OrderByDescending<IGrouping<uint, BaseNetworkable>, int>(Enumerable.GroupBy<BaseNetworkable, uint>(BaseNetworkable.clientEntities, (BaseNetworkable x) => x.prefabID), (IGrouping<uint, BaseNetworkable> x) => Enumerable.Count<BaseNetworkable>(x)))
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", Enumerable.First<BaseNetworkable>(grouping).PrefabName, Enumerable.Count<BaseNetworkable>(grouping));
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.CL.Counts.txt", stringBuilder2.ToString());
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x0000F497 File Offset: 0x0000D697
	private static void DumpLODGroups(string targetFolder)
	{
		DiagnosticsConSys.DumpLODGroupTotals(targetFolder);
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x00073DEC File Offset: 0x00071FEC
	private static void DumpLODGroupTotals(string targetFolder)
	{
		IEnumerable<LODGroup> enumerable = Object.FindObjectsOfType<LODGroup>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("LODGroups");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, LODGroup> grouping in Enumerable.OrderByDescending<IGrouping<string, LODGroup>, int>(Enumerable.GroupBy<LODGroup, string>(enumerable, (LODGroup x) => x.transform.GetRecursiveName("")), (IGrouping<string, LODGroup> x) => Enumerable.Count<LODGroup>(x)))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", grouping.Key, Enumerable.Count<LODGroup>(grouping));
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "LODGroups.Objects.txt", stringBuilder.ToString());
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x00073ECC File Offset: 0x000720CC
	private static void DumpNetwork(string targetFolder)
	{
		if (Net.cl.IsConnected())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Client Network Statistics");
			stringBuilder.AppendLine();
			stringBuilder.Append(Net.cl.GetDebug(null).Replace("\n", "\r\n"));
			stringBuilder.AppendLine();
			DiagnosticsConSys.WriteTextToFile(targetFolder + "Network.Client.txt", stringBuilder.ToString());
		}
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x00073F3C File Offset: 0x0007213C
	private static void DumpObjects(string targetFolder)
	{
		Object[] array = Object.FindObjectsOfType<Object>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active UnityEngine.Object, ordered by count");
		stringBuilder.AppendLine();
		foreach (IGrouping<Type, Object> grouping in Enumerable.OrderByDescending<IGrouping<Type, Object>, int>(Enumerable.GroupBy<Object, Type>(array, (Object x) => x.GetType()), (IGrouping<Type, Object> x) => Enumerable.Count<Object>(x)))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", Enumerable.First<Object>(grouping).GetType().Name, Enumerable.Count<Object>(grouping));
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Object.Count.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All active UnityEngine.ScriptableObject, ordered by count");
		stringBuilder2.AppendLine();
		foreach (IGrouping<Type, Object> grouping2 in Enumerable.OrderByDescending<IGrouping<Type, Object>, int>(Enumerable.GroupBy<Object, Type>(Enumerable.Where<Object>(array, (Object x) => x is ScriptableObject), (Object x) => x.GetType()), (IGrouping<Type, Object> x) => Enumerable.Count<Object>(x)))
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", Enumerable.First<Object>(grouping2).GetType().Name, Enumerable.Count<Object>(grouping2));
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.ScriptableObject.Count.txt", stringBuilder2.ToString());
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x0000F49F File Offset: 0x0000D69F
	private static void DumpPhysics(string targetFolder)
	{
		DiagnosticsConSys.DumpTotals(targetFolder);
		DiagnosticsConSys.DumpColliders(targetFolder);
		DiagnosticsConSys.DumpRigidBodies(targetFolder);
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0007412C File Offset: 0x0007232C
	private static void DumpTotals(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Information");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total Colliders:\t{0:N0}", Enumerable.Count<Collider>(Object.FindObjectsOfType<Collider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Active Colliders:\t{0:N0}", Enumerable.Count<Collider>(Enumerable.Where<Collider>(Object.FindObjectsOfType<Collider>(), (Collider x) => x.enabled)));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total RigidBodys:\t{0:N0}", Enumerable.Count<Rigidbody>(Object.FindObjectsOfType<Rigidbody>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Mesh Colliders:\t{0:N0}", Enumerable.Count<MeshCollider>(Object.FindObjectsOfType<MeshCollider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Box Colliders:\t{0:N0}", Enumerable.Count<BoxCollider>(Object.FindObjectsOfType<BoxCollider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Sphere Colliders:\t{0:N0}", Enumerable.Count<SphereCollider>(Object.FindObjectsOfType<SphereCollider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Capsule Colliders:\t{0:N0}", Enumerable.Count<CapsuleCollider>(Object.FindObjectsOfType<CapsuleCollider>()));
		stringBuilder.AppendLine();
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.txt", stringBuilder.ToString());
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x00074284 File Offset: 0x00072484
	private static void DumpColliders(string targetFolder)
	{
		IEnumerable<Collider> enumerable = Object.FindObjectsOfType<Collider>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Colliders");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Collider> grouping in Enumerable.OrderByDescending<IGrouping<string, Collider>, int>(Enumerable.GroupBy<Collider, string>(enumerable, (Collider x) => x.transform.GetRecursiveName("")), (IGrouping<string, Collider> x) => Enumerable.Count<Collider>(x)))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			string text = "{1:N0}\t{0} ({2:N0} triggers) ({3:N0} enabled)";
			object[] array = new object[4];
			array[0] = grouping.Key;
			array[1] = Enumerable.Count<Collider>(grouping);
			array[2] = Enumerable.Count<Collider>(grouping, (Collider x) => x.isTrigger);
			array[3] = Enumerable.Count<Collider>(grouping, (Collider x) => x.enabled);
			stringBuilder2.AppendFormat(text, array);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.Colliders.Objects.txt", stringBuilder.ToString());
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x000743D0 File Offset: 0x000725D0
	private static void DumpRigidBodies(string targetFolder)
	{
		IEnumerable<Rigidbody> enumerable = Object.FindObjectsOfType<Rigidbody>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("RigidBody");
		stringBuilder.AppendLine();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("RigidBody");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Rigidbody> grouping in Enumerable.OrderByDescending<IGrouping<string, Rigidbody>, int>(Enumerable.GroupBy<Rigidbody, string>(enumerable, (Rigidbody x) => x.transform.GetRecursiveName("")), (IGrouping<string, Rigidbody> x) => Enumerable.Count<Rigidbody>(x)))
		{
			StringBuilder stringBuilder3 = stringBuilder;
			string text = "{1:N0}\t{0} ({2:N0} awake) ({3:N0} kinematic) ({4:N0} non-discrete)";
			object[] array = new object[5];
			array[0] = grouping.Key;
			array[1] = Enumerable.Count<Rigidbody>(grouping);
			array[2] = Enumerable.Count<Rigidbody>(grouping, (Rigidbody x) => !x.IsSleeping());
			array[3] = Enumerable.Count<Rigidbody>(grouping, (Rigidbody x) => x.isKinematic);
			array[4] = Enumerable.Count<Rigidbody>(grouping, (Rigidbody x) => x.collisionDetectionMode > 0);
			stringBuilder3.AppendFormat(text, array);
			stringBuilder.AppendLine();
			foreach (Rigidbody rigidbody in grouping)
			{
				stringBuilder2.AppendFormat("{0} -{1}{2}{3}", new object[]
				{
					grouping.Key,
					rigidbody.isKinematic ? " KIN" : "",
					rigidbody.IsSleeping() ? " SLEEP" : "",
					rigidbody.useGravity ? " GRAVITY" : ""
				});
				stringBuilder2.AppendLine();
				stringBuilder2.AppendFormat("Mass: {0}\tVelocity: {1}\tsleepThreshold: {2}", rigidbody.mass, rigidbody.velocity, rigidbody.sleepThreshold);
				stringBuilder2.AppendLine();
				stringBuilder2.AppendLine();
			}
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.Objects.txt", stringBuilder.ToString());
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.All.txt", stringBuilder2.ToString());
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x00074674 File Offset: 0x00072874
	private static void DumpGameObjects(string targetFolder)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects");
		stringBuilder.AppendLine();
		foreach (Transform tx in rootObjects)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, tx, 0, false);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects including components");
		stringBuilder.AppendLine();
		foreach (Transform tx2 in rootObjects)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, tx2, 0, true);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.Components.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects excluding children");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Transform> grouping in Enumerable.OrderByDescending<IGrouping<string, Transform>, int>(Enumerable.GroupBy<Transform, string>(rootObjects, (Transform x) => x.name), (IGrouping<string, Transform> x) => Enumerable.Count<Transform>(x)))
		{
			Transform transform = Enumerable.First<Transform>(grouping);
			stringBuilder.AppendFormat("{1:N0}\t{0}", transform.name, Enumerable.Count<Transform>(grouping));
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects including children");
		stringBuilder.AppendLine();
		foreach (KeyValuePair<Transform, int> keyValuePair in Enumerable.OrderByDescending<KeyValuePair<Transform, int>, int>(Enumerable.Select<IGrouping<string, Transform>, KeyValuePair<Transform, int>>(Enumerable.GroupBy<Transform, string>(rootObjects, (Transform x) => x.name), (IGrouping<string, Transform> x) => new KeyValuePair<Transform, int>(Enumerable.First<Transform>(x), Enumerable.Sum<Transform>(x, (Transform y) => y.GetAllChildren().Count))), (KeyValuePair<Transform, int> x) => x.Value))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", keyValuePair.Key.name, keyValuePair.Value);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.Children.txt", stringBuilder.ToString());
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x00074910 File Offset: 0x00072B10
	private static void DumpGameObjectRecursive(StringBuilder str, Transform tx, int indent, bool includeComponents = false)
	{
		if (tx == null)
		{
			return;
		}
		for (int i = 0; i < indent; i++)
		{
			str.Append(" ");
		}
		str.AppendFormat("{0} {1:N0}", tx.name, tx.GetComponents<Component>().Length - 1);
		str.AppendLine();
		if (includeComponents)
		{
			foreach (Component component in tx.GetComponents<Component>())
			{
				if (!(component is Transform))
				{
					for (int k = 0; k < indent + 1; k++)
					{
						str.Append(" ");
					}
					str.AppendFormat("[c] {0}", (component == null) ? "NULL" : component.GetType().ToString());
					str.AppendLine();
				}
			}
		}
		for (int l = 0; l < tx.childCount; l++)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(str, tx.GetChild(l), indent + 2, includeComponents);
		}
	}
}
