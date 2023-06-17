using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000752 RID: 1874
public class PathFinder
{
	// Token: 0x04002417 RID: 9239
	private int[,] costmap;

	// Token: 0x04002418 RID: 9240
	private bool[,] visited;

	// Token: 0x04002419 RID: 9241
	private PathFinder.Point[] neighbors;

	// Token: 0x0400241A RID: 9242
	private static PathFinder.Point[] mooreNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1),
		new PathFinder.Point(-1, 1),
		new PathFinder.Point(1, 1),
		new PathFinder.Point(-1, -1),
		new PathFinder.Point(1, -1)
	};

	// Token: 0x0400241B RID: 9243
	private static PathFinder.Point[] neumannNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1)
	};

	// Token: 0x060028CC RID: 10444 RVA: 0x0001FB4D File Offset: 0x0001DD4D
	public PathFinder(int[,] costmap, bool diagonals = true)
	{
		this.costmap = costmap;
		this.neighbors = (diagonals ? PathFinder.mooreNeighbors : PathFinder.neumannNeighbors);
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x0001FB71 File Offset: 0x0001DD71
	public PathFinder.Node FindPath(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		return this.FindPathReversed(end, start, depth);
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x000D110C File Offset: 0x000CF30C
	private PathFinder.Node FindPathReversed(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int cost = this.costmap[start.y, start.x];
		int heuristic = this.Heuristic(start, end);
		intrusiveMinHeap.Add(new PathFinder.Node(start, cost, heuristic, null));
		this.visited[start.y, start.x] = true;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4 && !this.visited[point.y, point.x])
				{
					this.visited[point.y, point.x] = true;
					int num5 = this.costmap[point.y, point.x];
					if (num5 != 2147483647)
					{
						int cost2 = node.cost + num5;
						int heuristic2 = this.Heuristic(point, end);
						intrusiveMinHeap.Add(new PathFinder.Node(point, cost2, heuristic2, node));
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x0001FB7C File Offset: 0x0001DD7C
	public PathFinder.Node FindPathDirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		return this.FindPathReversed(endList, startList, depth);
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x0001FB87 File Offset: 0x0001DD87
	public PathFinder.Node FindPathUndirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count > endList.Count)
		{
			return this.FindPathReversed(endList, startList, depth);
		}
		return this.FindPathReversed(startList, endList, depth);
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x000D12F4 File Offset: 0x000CF4F4
	private PathFinder.Node FindPathReversed(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		using (List<PathFinder.Point>.Enumerator enumerator = startList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PathFinder.Point point = enumerator.Current;
				int cost = this.costmap[point.y, point.x];
				int heuristic = this.Heuristic(point, endList);
				intrusiveMinHeap.Add(new PathFinder.Node(point, cost, heuristic, null));
				this.visited[point.y, point.x] = true;
			}
			goto IL_1F5;
		}
		IL_F0:
		PathFinder.Node node = intrusiveMinHeap.Pop();
		if (node.heuristic == 0)
		{
			return node;
		}
		for (int i = 0; i < this.neighbors.Length; i++)
		{
			PathFinder.Point point2 = node.point + this.neighbors[i];
			if (point2.x >= num && point2.x <= num2 && point2.y >= num3 && point2.y <= num4 && !this.visited[point2.y, point2.x])
			{
				this.visited[point2.y, point2.x] = true;
				int num5 = this.costmap[point2.y, point2.x];
				if (num5 != 2147483647)
				{
					int cost2 = node.cost + num5;
					int heuristic2 = this.Heuristic(point2, endList);
					intrusiveMinHeap.Add(new PathFinder.Node(point2, cost2, heuristic2, node));
				}
			}
		}
		IL_1F5:
		if (intrusiveMinHeap.Empty || depth-- <= 0)
		{
			return null;
		}
		goto IL_F0;
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x000D151C File Offset: 0x000CF71C
	public PathFinder.Node FindClosestWalkable(PathFinder.Point start, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new bool[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int cost = 1;
		int heuristic = this.Heuristic(start);
		intrusiveMinHeap.Add(new PathFinder.Node(start, cost, heuristic, null));
		this.visited[start.y, start.x] = true;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4 && !this.visited[point.y, point.x])
				{
					this.visited[point.y, point.x] = true;
					int cost2 = node.cost + 1;
					int heuristic2 = this.Heuristic(point);
					intrusiveMinHeap.Add(new PathFinder.Node(point, cost2, heuristic2, node));
				}
			}
		}
		return null;
	}

	// Token: 0x060028D3 RID: 10451 RVA: 0x000D16BC File Offset: 0x000CF8BC
	public PathFinder.Node Reverse(PathFinder.Node start)
	{
		PathFinder.Node node = null;
		PathFinder.Node next = null;
		for (PathFinder.Node node2 = start; node2 != null; node2 = node2.next)
		{
			if (node != null)
			{
				node.next = next;
			}
			next = node;
			node = node2;
		}
		if (node != null)
		{
			node.next = next;
		}
		return node;
	}

	// Token: 0x060028D4 RID: 10452 RVA: 0x000D16F4 File Offset: 0x000CF8F4
	public PathFinder.Node FindEnd(PathFinder.Node start)
	{
		for (PathFinder.Node node = start; node != null; node = node.next)
		{
			if (node.next == null)
			{
				return node;
			}
		}
		return start;
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x0001FBAA File Offset: 0x0001DDAA
	public int Heuristic(PathFinder.Point a)
	{
		if (this.costmap[a.y, a.x] != 2147483647)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x000D171C File Offset: 0x000CF91C
	public int Heuristic(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return num * num + num2 * num2;
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x000D174C File Offset: 0x000CF94C
	public int Heuristic(PathFinder.Point a, List<PathFinder.Point> b)
	{
		int num = int.MaxValue;
		for (int i = 0; i < b.Count; i++)
		{
			num = Mathf.Min(num, this.Heuristic(a, b[i]));
		}
		return num;
	}

	// Token: 0x02000753 RID: 1875
	public struct Point : IEquatable<PathFinder.Point>
	{
		// Token: 0x0400241C RID: 9244
		public int x;

		// Token: 0x0400241D RID: 9245
		public int y;

		// Token: 0x060028D9 RID: 10457 RVA: 0x0001FBCD File Offset: 0x0001DDCD
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x060028DA RID: 10458 RVA: 0x0001FBDD File Offset: 0x0001DDDD
		public static PathFinder.Point operator +(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x + b.x, a.y + b.y);
		}

		// Token: 0x060028DB RID: 10459 RVA: 0x0001FBFE File Offset: 0x0001DDFE
		public static PathFinder.Point operator -(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x - b.x, a.y - b.y);
		}

		// Token: 0x060028DC RID: 10460 RVA: 0x0001FC1F File Offset: 0x0001DE1F
		public static PathFinder.Point operator *(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x * i, p.y * i);
		}

		// Token: 0x060028DD RID: 10461 RVA: 0x0001FC36 File Offset: 0x0001DE36
		public static PathFinder.Point operator /(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x / i, p.y / i);
		}

		// Token: 0x060028DE RID: 10462 RVA: 0x0001FC4D File Offset: 0x0001DE4D
		public static bool operator ==(PathFinder.Point a, PathFinder.Point b)
		{
			return a.Equals(b);
		}

		// Token: 0x060028DF RID: 10463 RVA: 0x0001FC57 File Offset: 0x0001DE57
		public static bool operator !=(PathFinder.Point a, PathFinder.Point b)
		{
			return !a.Equals(b);
		}

		// Token: 0x060028E0 RID: 10464 RVA: 0x0001FC64 File Offset: 0x0001DE64
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		// Token: 0x060028E1 RID: 10465 RVA: 0x0001FC7D File Offset: 0x0001DE7D
		public override bool Equals(object other)
		{
			return other is PathFinder.Point && this.Equals((PathFinder.Point)other);
		}

		// Token: 0x060028E2 RID: 10466 RVA: 0x0001FC95 File Offset: 0x0001DE95
		public bool Equals(PathFinder.Point other)
		{
			return this.x == other.x && this.y == other.y;
		}
	}

	// Token: 0x02000754 RID: 1876
	public class Node : IMinHeapNode<PathFinder.Node>, ILinkedListNode<PathFinder.Node>
	{
		// Token: 0x0400241E RID: 9246
		public PathFinder.Point point;

		// Token: 0x0400241F RID: 9247
		public int cost;

		// Token: 0x04002420 RID: 9248
		public int heuristic;

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x060028E3 RID: 10467 RVA: 0x0001FCB5 File Offset: 0x0001DEB5
		// (set) Token: 0x060028E4 RID: 10468 RVA: 0x0001FCBD File Offset: 0x0001DEBD
		public PathFinder.Node next { get; set; }

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x060028E5 RID: 10469 RVA: 0x0001FCC6 File Offset: 0x0001DEC6
		// (set) Token: 0x060028E6 RID: 10470 RVA: 0x0001FCCE File Offset: 0x0001DECE
		public PathFinder.Node child { get; set; }

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x060028E7 RID: 10471 RVA: 0x0001FCD7 File Offset: 0x0001DED7
		public int order
		{
			get
			{
				return this.cost + this.heuristic;
			}
		}

		// Token: 0x060028E8 RID: 10472 RVA: 0x0001FCE6 File Offset: 0x0001DEE6
		public Node(PathFinder.Point point, int cost, int heuristic, PathFinder.Node next = null)
		{
			this.point = point;
			this.cost = cost;
			this.heuristic = heuristic;
			this.next = next;
		}
	}
}
