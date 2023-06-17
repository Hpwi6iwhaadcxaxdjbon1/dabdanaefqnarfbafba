using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000879 RID: 2169
	[ConsoleSystem.Factory("pool")]
	public class Pool : ConsoleSystem
	{
		// Token: 0x040029C9 RID: 10697
		[ServerVar]
		[ClientVar]
		public static int mode = 2;

		// Token: 0x040029CA RID: 10698
		[ClientVar]
		[ServerVar]
		public static bool enabled = true;

		// Token: 0x040029CB RID: 10699
		[ServerVar]
		[ClientVar]
		public static bool debug = false;

		// Token: 0x06002F38 RID: 12088 RVA: 0x000E9534 File Offset: 0x000E7734
		[ClientVar]
		public static void print_rigcache(ConsoleSystem.Arg arg)
		{
			if (SkinnedMeshRendererCache.dictionary.Count == 0)
			{
				arg.ReplyWith("Rig cache is empty.");
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("mesh");
			textTable.AddColumn("bone");
			foreach (KeyValuePair<Mesh, SkinnedMeshRendererCache.RigInfo> keyValuePair in SkinnedMeshRendererCache.dictionary)
			{
				textTable.AddRow(new string[]
				{
					keyValuePair.Key.name,
					keyValuePair.Value.root
				});
			}
			arg.ReplyWith(textTable.ToString());
		}

		// Token: 0x06002F39 RID: 12089 RVA: 0x000E95EC File Offset: 0x000E77EC
		[ServerVar]
		[ClientVar]
		public static void print_memory(ConsoleSystem.Arg arg)
		{
			if (Pool.directory.Count == 0)
			{
				arg.ReplyWith("Memory pool is empty.");
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("pooled");
			textTable.AddColumn("active");
			textTable.AddColumn("hits");
			textTable.AddColumn("misses");
			textTable.AddColumn("spills");
			foreach (KeyValuePair<Type, Pool.ICollection> keyValuePair in Enumerable.OrderByDescending<KeyValuePair<Type, Pool.ICollection>, long>(Pool.directory, (KeyValuePair<Type, Pool.ICollection> x) => x.Value.ItemsCreated))
			{
				string text = keyValuePair.Key.ToString().Replace("System.Collections.Generic.", "");
				Pool.ICollection value = keyValuePair.Value;
				textTable.AddRow(new string[]
				{
					text,
					NumberExtensions.FormatNumberShort(value.ItemsInStack),
					NumberExtensions.FormatNumberShort(value.ItemsInUse),
					NumberExtensions.FormatNumberShort(value.ItemsTaken),
					NumberExtensions.FormatNumberShort(value.ItemsCreated),
					NumberExtensions.FormatNumberShort(value.ItemsSpilled)
				});
			}
			arg.ReplyWith(textTable.ToString());
		}

		// Token: 0x06002F3A RID: 12090 RVA: 0x000E9748 File Offset: 0x000E7948
		[ClientVar]
		[ServerVar]
		public static void print_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.client.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("count");
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = StringPool.Get(keyValuePair.Key);
				string text3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || StringEx.Contains(text2, @string, 1))
				{
					textTable.AddRow(new string[]
					{
						text,
						Path.GetFileNameWithoutExtension(text2),
						text3
					});
				}
			}
			arg.ReplyWith(textTable.ToString());
		}

		// Token: 0x06002F3B RID: 12091 RVA: 0x000E9864 File Offset: 0x000E7A64
		[ClientVar]
		[ServerVar]
		public static void print_assets(ConsoleSystem.Arg arg)
		{
			if (AssetPool.storage.Count == 0)
			{
				arg.ReplyWith("Asset pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("allocated");
			textTable.AddColumn("available");
			foreach (KeyValuePair<Type, AssetPool.Pool> keyValuePair in AssetPool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = keyValuePair.Value.allocated.ToString();
				string text3 = keyValuePair.Value.available.ToString();
				if (string.IsNullOrEmpty(@string) || StringEx.Contains(text, @string, 1))
				{
					textTable.AddRow(new string[]
					{
						text,
						text2,
						text3
					});
				}
			}
			arg.ReplyWith(textTable.ToString());
		}

		// Token: 0x06002F3C RID: 12092 RVA: 0x00024538 File Offset: 0x00022738
		[ServerVar]
		[ClientVar]
		public static void clear_memory(ConsoleSystem.Arg arg)
		{
			Pool.Clear();
		}

		// Token: 0x06002F3D RID: 12093 RVA: 0x0002453F File Offset: 0x0002273F
		[ClientVar]
		[ServerVar]
		public static void clear_prefabs(ConsoleSystem.Arg arg)
		{
			GameManager.client.pool.Clear();
		}

		// Token: 0x06002F3E RID: 12094 RVA: 0x00024550 File Offset: 0x00022750
		[ClientVar]
		[ServerVar]
		public static void clear_assets(ConsoleSystem.Arg arg)
		{
			AssetPool.Clear();
		}

		// Token: 0x06002F3F RID: 12095 RVA: 0x000E996C File Offset: 0x000E7B6C
		[ServerVar]
		[ClientVar]
		public static void export_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.client.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = StringPool.Get(keyValuePair.Key);
				string text3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || StringEx.Contains(text2, @string, 1))
				{
					stringBuilder.AppendLine(string.Format("{0},{1},{2}", text, Path.GetFileNameWithoutExtension(text2), text3));
				}
			}
			File.WriteAllText("prefabs.csv", stringBuilder.ToString());
		}
	}
}
