using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004EB RID: 1259
public class TerrainPath : TerrainExtension
{
	// Token: 0x040019A4 RID: 6564
	internal List<PathList> Roads = new List<PathList>();

	// Token: 0x040019A5 RID: 6565
	internal List<PathList> Rivers = new List<PathList>();

	// Token: 0x040019A6 RID: 6566
	internal List<PathList> Powerlines = new List<PathList>();

	// Token: 0x040019A7 RID: 6567
	internal List<MonumentInfo> Monuments = new List<MonumentInfo>();

	// Token: 0x040019A8 RID: 6568
	internal List<RiverInfo> RiverObjs = new List<RiverInfo>();

	// Token: 0x040019A9 RID: 6569
	internal List<LakeInfo> LakeObjs = new List<LakeInfo>();

	// Token: 0x040019AA RID: 6570
	internal List<Vector3> OceanPatrolClose = new List<Vector3>();

	// Token: 0x040019AB RID: 6571
	internal List<Vector3> OceanPatrolFar = new List<Vector3>();

	// Token: 0x040019AC RID: 6572
	private Dictionary<string, List<PowerlineNode>> wires = new Dictionary<string, List<PowerlineNode>>();

	// Token: 0x06001D55 RID: 7509 RVA: 0x00017888 File Offset: 0x00015A88
	public void Clear()
	{
		this.Roads.Clear();
		this.Rivers.Clear();
		this.Powerlines.Clear();
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x0009FFFC File Offset: 0x0009E1FC
	public void AddWire(PowerlineNode node)
	{
		string name = node.transform.root.name;
		if (!this.wires.ContainsKey(name))
		{
			this.wires.Add(name, new List<PowerlineNode>());
		}
		this.wires[name].Add(node);
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x000A004C File Offset: 0x0009E24C
	public void CreateWires()
	{
		List<GameObject> list = new List<GameObject>();
		int num = 0;
		Material material = null;
		foreach (KeyValuePair<string, List<PowerlineNode>> keyValuePair in this.wires)
		{
			foreach (PowerlineNode powerlineNode in keyValuePair.Value)
			{
				MegaWireConnectionHelper component = powerlineNode.GetComponent<MegaWireConnectionHelper>();
				if (component)
				{
					if (list.Count == 0)
					{
						material = powerlineNode.WireMaterial;
						num = component.connections.Count;
					}
					else
					{
						GameObject gameObject = list[list.Count - 1];
						if (powerlineNode.WireMaterial != material || component.connections.Count != num || (gameObject.transform.position - powerlineNode.transform.position).sqrMagnitude > powerlineNode.MaxDistance * powerlineNode.MaxDistance)
						{
							this.CreateWire(keyValuePair.Key, list, material);
							list.Clear();
						}
					}
					list.Add(powerlineNode.gameObject);
				}
			}
			this.CreateWire(keyValuePair.Key, list, material);
			list.Clear();
		}
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x000A01D8 File Offset: 0x0009E3D8
	private void CreateWire(string name, List<GameObject> objects, Material material)
	{
		if (objects.Count >= 3 && material != null)
		{
			MegaWire megaWire = MegaWire.Create(null, objects, material, "Powerline Wires", null, 1f, 0.1f);
			if (megaWire)
			{
				megaWire.enabled = false;
				megaWire.RunPhysics(megaWire.warmPhysicsTime);
				megaWire.gameObject.SetHierarchyGroup(name, true, false);
			}
		}
	}
}
