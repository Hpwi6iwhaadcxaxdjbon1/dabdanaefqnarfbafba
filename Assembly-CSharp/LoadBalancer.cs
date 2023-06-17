using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rust;
using UnityEngine;

// Token: 0x0200070B RID: 1803
public class LoadBalancer : SingletonComponent<LoadBalancer>
{
	// Token: 0x0400237C RID: 9084
	public static bool Paused;

	// Token: 0x0400237D RID: 9085
	private const float MinMilliseconds = 1f;

	// Token: 0x0400237E RID: 9086
	private const float MaxMilliseconds = 100f;

	// Token: 0x0400237F RID: 9087
	private const int MinBacklog = 1000;

	// Token: 0x04002380 RID: 9088
	private const int MaxBacklog = 100000;

	// Token: 0x04002381 RID: 9089
	private Queue<DeferredAction>[] queues = new Queue<DeferredAction>[]
	{
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>()
	};

	// Token: 0x04002382 RID: 9090
	private Stopwatch watch = Stopwatch.StartNew();

	// Token: 0x060027A0 RID: 10144 RVA: 0x000CD514 File Offset: 0x000CB714
	protected void LateUpdate()
	{
		if (Application.isReceiving)
		{
			return;
		}
		if (Application.isLoading)
		{
			return;
		}
		if (LoadBalancer.Paused)
		{
			return;
		}
		int num = LoadBalancer.Count();
		float t = Mathf.InverseLerp(1000f, 100000f, (float)num);
		float num2 = Mathf.SmoothStep(1f, 100f, t);
		this.watch.Reset();
		this.watch.Start();
		for (int i = 0; i < this.queues.Length; i++)
		{
			Queue<DeferredAction> queue = this.queues[i];
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
				if (this.watch.Elapsed.TotalMilliseconds > (double)num2)
				{
					return;
				}
			}
		}
	}

	// Token: 0x060027A1 RID: 10145 RVA: 0x000CD5C8 File Offset: 0x000CB7C8
	public static int Count()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			return 0;
		}
		Queue<DeferredAction>[] array = SingletonComponent<LoadBalancer>.Instance.queues;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			num += array[i].Count;
		}
		return num;
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000CD60C File Offset: 0x000CB80C
	public static void ProcessAll()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		foreach (Queue<DeferredAction> queue in SingletonComponent<LoadBalancer>.Instance.queues)
		{
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
			}
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x0001EF3A File Offset: 0x0001D13A
	public static void Enqueue(DeferredAction action)
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		SingletonComponent<LoadBalancer>.Instance.queues[action.Index].Enqueue(action);
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x0001EF64 File Offset: 0x0001D164
	private static void CreateInstance()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "LoadBalancer";
		gameObject.AddComponent<LoadBalancer>();
		Object.DontDestroyOnLoad(gameObject);
	}
}
