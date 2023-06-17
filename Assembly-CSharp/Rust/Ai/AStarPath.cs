using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000904 RID: 2308
	public static class AStarPath
	{
		// Token: 0x0600311F RID: 12575 RVA: 0x00025893 File Offset: 0x00023A93
		private static float Heuristic(BasePathNode from, BasePathNode to)
		{
			return Vector3.Distance(from.transform.position, to.transform.position);
		}

		// Token: 0x06003120 RID: 12576 RVA: 0x000ECE70 File Offset: 0x000EB070
		public static bool FindPath(BasePathNode start, BasePathNode goal, out Stack<BasePathNode> path, out float pathCost)
		{
			path = null;
			pathCost = -1f;
			bool result = false;
			if (start == goal)
			{
				return false;
			}
			AStarNodeList astarNodeList = new AStarNodeList();
			HashSet<BasePathNode> hashSet = new HashSet<BasePathNode>();
			AStarNode astarNode = new AStarNode(0f, AStarPath.Heuristic(start, goal), null, start);
			astarNodeList.Add(astarNode);
			while (astarNodeList.Count > 0)
			{
				AStarNode astarNode2 = astarNodeList[0];
				astarNodeList.RemoveAt(0);
				hashSet.Add(astarNode2.Node);
				if (astarNode2.Satisfies(goal))
				{
					path = new Stack<BasePathNode>();
					pathCost = 0f;
					while (astarNode2.Parent != null)
					{
						pathCost += astarNode2.F;
						path.Push(astarNode2.Node);
						astarNode2 = astarNode2.Parent;
					}
					if (astarNode2 != null)
					{
						path.Push(astarNode2.Node);
					}
					result = true;
					break;
				}
				foreach (BasePathNode basePathNode in astarNode2.Node.linked)
				{
					if (!hashSet.Contains(basePathNode))
					{
						float num = astarNode2.G + AStarPath.Heuristic(astarNode2.Node, basePathNode);
						AStarNode astarNode3 = astarNodeList.GetAStarNodeOf(basePathNode);
						if (astarNode3 == null)
						{
							astarNode3 = new AStarNode(num, AStarPath.Heuristic(basePathNode, goal), astarNode2, basePathNode);
							astarNodeList.Add(astarNode3);
							astarNodeList.AStarNodeSort();
						}
						else if (num < astarNode3.G)
						{
							astarNode3.Update(num, astarNode3.H, astarNode2, basePathNode);
							astarNodeList.AStarNodeSort();
						}
					}
				}
			}
			return result;
		}
	}
}
