using System;
using System.Collections.Generic;

namespace Rust.AI
{
	// Token: 0x02000906 RID: 2310
	public class AStarNodeList : List<AStarNode>
	{
		// Token: 0x04002C20 RID: 11296
		private readonly AStarNodeList.AStarNodeComparer comparer = new AStarNodeList.AStarNodeComparer();

		// Token: 0x06003127 RID: 12583 RVA: 0x000ED014 File Offset: 0x000EB214
		public bool Contains(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003128 RID: 12584 RVA: 0x000ED050 File Offset: 0x000EB250
		public AStarNode GetAStarNodeOf(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return astarNode;
				}
			}
			return null;
		}

		// Token: 0x06003129 RID: 12585 RVA: 0x00025931 File Offset: 0x00023B31
		public void AStarNodeSort()
		{
			base.Sort(this.comparer);
		}

		// Token: 0x02000907 RID: 2311
		private class AStarNodeComparer : IComparer<AStarNode>
		{
			// Token: 0x0600312B RID: 12587 RVA: 0x00025952 File Offset: 0x00023B52
			int IComparer<AStarNode>.Compare(AStarNode lhs, AStarNode rhs)
			{
				if (lhs < rhs)
				{
					return -1;
				}
				if (lhs > rhs)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
