using System;
using UnityEngine;

// Token: 0x0200047E RID: 1150
[Serializable]
public sealed class ByteQuadtree
{
	// Token: 0x040017AF RID: 6063
	[SerializeField]
	private int size;

	// Token: 0x040017B0 RID: 6064
	[SerializeField]
	private int levels;

	// Token: 0x040017B1 RID: 6065
	[SerializeField]
	private ByteMap[] values;

	// Token: 0x06001ACD RID: 6861 RVA: 0x00093BC0 File Offset: 0x00091DC0
	public void UpdateValues(byte[] baseValues)
	{
		this.size = Mathf.RoundToInt(Mathf.Sqrt((float)baseValues.Length));
		this.levels = Mathf.RoundToInt(Mathf.Log((float)this.size, 2f)) + 1;
		this.values = new ByteMap[this.levels];
		this.values[0] = new ByteMap(this.size, baseValues, 1);
		for (int i = 1; i < this.levels; i++)
		{
			ByteMap byteMap = this.values[i - 1];
			ByteMap byteMap2 = this.values[i] = this.CreateLevel(i);
			for (int j = 0; j < byteMap2.Size; j++)
			{
				for (int k = 0; k < byteMap2.Size; k++)
				{
					byteMap2[k, j] = byteMap[2 * k, 2 * j] + byteMap[2 * k + 1, 2 * j] + byteMap[2 * k, 2 * j + 1] + byteMap[2 * k + 1, 2 * j + 1];
				}
			}
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06001ACE RID: 6862 RVA: 0x00016347 File Offset: 0x00014547
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06001ACF RID: 6863 RVA: 0x0001634F File Offset: 0x0001454F
	public ByteQuadtree.Element Root
	{
		get
		{
			return new ByteQuadtree.Element(this, 0, 0, this.levels - 1);
		}
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x00093CD4 File Offset: 0x00091ED4
	private ByteMap CreateLevel(int level)
	{
		int num = 1 << this.levels - level - 1;
		int bytes = 1 + (level + 3) / 4;
		return new ByteMap(num, bytes);
	}

	// Token: 0x0200047F RID: 1151
	public struct Element
	{
		// Token: 0x040017B2 RID: 6066
		private ByteQuadtree source;

		// Token: 0x040017B3 RID: 6067
		private int x;

		// Token: 0x040017B4 RID: 6068
		private int y;

		// Token: 0x040017B5 RID: 6069
		private int level;

		// Token: 0x06001AD2 RID: 6866 RVA: 0x00016361 File Offset: 0x00014561
		public Element(ByteQuadtree source, int x, int y, int level)
		{
			this.source = source;
			this.x = x;
			this.y = y;
			this.level = level;
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x00016380 File Offset: 0x00014580
		public bool IsLeaf
		{
			get
			{
				return this.level == 0;
			}
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06001AD4 RID: 6868 RVA: 0x0001638B File Offset: 0x0001458B
		public bool IsRoot
		{
			get
			{
				return this.level == this.source.levels - 1;
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06001AD5 RID: 6869 RVA: 0x000163A2 File Offset: 0x000145A2
		public int ByteMap
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06001AD6 RID: 6870 RVA: 0x000163AA File Offset: 0x000145AA
		public uint Value
		{
			get
			{
				return this.source.values[this.level][this.x, this.y];
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06001AD7 RID: 6871 RVA: 0x000163CF File Offset: 0x000145CF
		public Vector2 Coords
		{
			get
			{
				return new Vector2((float)this.x, (float)this.y);
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06001AD8 RID: 6872 RVA: 0x000163E4 File Offset: 0x000145E4
		public ByteQuadtree.Element Parent
		{
			get
			{
				if (this.IsRoot)
				{
					throw new Exception("Element is the root and therefore has no parent.");
				}
				return new ByteQuadtree.Element(this.source, this.x / 2, this.y / 2, this.level + 1);
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06001AD9 RID: 6873 RVA: 0x0001641C File Offset: 0x0001461C
		public ByteQuadtree.Element Child1
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06001ADA RID: 6874 RVA: 0x00016454 File Offset: 0x00014654
		public ByteQuadtree.Element Child2
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06001ADB RID: 6875 RVA: 0x0001648E File Offset: 0x0001468E
		public ByteQuadtree.Element Child3
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06001ADC RID: 6876 RVA: 0x000164C8 File Offset: 0x000146C8
		public ByteQuadtree.Element Child4
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06001ADD RID: 6877 RVA: 0x00093D00 File Offset: 0x00091F00
		public ByteQuadtree.Element MaxChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				if (value >= value2 && value >= value3 && value >= value4)
				{
					return child;
				}
				if (value2 >= value3 && value2 >= value4)
				{
					return child2;
				}
				if (value3 >= value4)
				{
					return child3;
				}
				return child4;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06001ADE RID: 6878 RVA: 0x00093D78 File Offset: 0x00091F78
		public ByteQuadtree.Element RandChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				float num = value + value2 + value3 + value4;
				float value5 = Random.value;
				if (value / num >= value5)
				{
					return child;
				}
				if ((value + value2) / num >= value5)
				{
					return child2;
				}
				if ((value + value2 + value3) / num >= value5)
				{
					return child3;
				}
				return child4;
			}
		}
	}
}
