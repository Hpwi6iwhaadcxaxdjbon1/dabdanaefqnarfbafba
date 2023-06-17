using System;

namespace Rust.AI
{
	// Token: 0x02000905 RID: 2309
	public class AStarNode
	{
		// Token: 0x04002C1C RID: 11292
		public AStarNode Parent;

		// Token: 0x04002C1D RID: 11293
		public float G;

		// Token: 0x04002C1E RID: 11294
		public float H;

		// Token: 0x04002C1F RID: 11295
		public BasePathNode Node;

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06003121 RID: 12577 RVA: 0x000258B0 File Offset: 0x00023AB0
		public float F
		{
			get
			{
				return this.G + this.H;
			}
		}

		// Token: 0x06003122 RID: 12578 RVA: 0x000258BF File Offset: 0x00023ABF
		public AStarNode(float g, float h, AStarNode parent, BasePathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x06003123 RID: 12579 RVA: 0x000258E4 File Offset: 0x00023AE4
		public void Update(float g, float h, AStarNode parent, BasePathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x06003124 RID: 12580 RVA: 0x00025903 File Offset: 0x00023B03
		public bool Satisfies(BasePathNode node)
		{
			return this.Node == node;
		}

		// Token: 0x06003125 RID: 12581 RVA: 0x00025911 File Offset: 0x00023B11
		public static bool operator <(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F < rhs.F;
		}

		// Token: 0x06003126 RID: 12582 RVA: 0x00025921 File Offset: 0x00023B21
		public static bool operator >(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F > rhs.F;
		}
	}
}
