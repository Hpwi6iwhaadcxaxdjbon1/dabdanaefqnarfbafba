using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar
{
	// Token: 0x02000863 RID: 2147
	[ConsoleSystem.Factory("global")]
	public class Global : ConsoleSystem
	{
		// Token: 0x04002984 RID: 10628
		private static int _developer;

		// Token: 0x04002985 RID: 10629
		[ServerVar]
		[ClientVar]
		public static int maxthreads = 8;

		// Token: 0x04002986 RID: 10630
		private static int _censornudity = 0;

		// Token: 0x04002987 RID: 10631
		private static bool _censorsigns = false;

		// Token: 0x04002988 RID: 10632
		[ClientVar(Saved = true)]
		[ServerVar(Saved = true)]
		public static int perf = 0;

		// Token: 0x04002989 RID: 10633
		[ClientVar(ClientInfo = true, Saved = true, Help = "If you're an admin this will enable god mode")]
		public static bool god = false;

		// Token: 0x0400298A RID: 10634
		[ClientVar(ClientInfo = true, Saved = true, Help = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.")]
		public static bool specnet = false;

		// Token: 0x0400298B RID: 10635
		[ClientVar(ClientInfo = true, Saved = true)]
		public static bool streamermode = false;

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06002EAD RID: 11949 RVA: 0x00023F0A File Offset: 0x0002210A
		// (set) Token: 0x06002EAE RID: 11950 RVA: 0x00023F11 File Offset: 0x00022111
		[ClientVar]
		[ServerVar]
		public static bool timewarning
		{
			get
			{
				return TimeWarning.Enabled;
			}
			set
			{
				TimeWarning.Enabled = value;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06002EB0 RID: 11952 RVA: 0x00023F21 File Offset: 0x00022121
		// (set) Token: 0x06002EAF RID: 11951 RVA: 0x00023F19 File Offset: 0x00022119
		[ClientVar]
		[ServerVar]
		public static int developer
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return 0;
				}
				if (!LocalPlayer.Entity.IsAdmin)
				{
					return 0;
				}
				return Global._developer;
			}
			set
			{
				Global._developer = value;
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06002EB1 RID: 11953 RVA: 0x00023F45 File Offset: 0x00022145
		// (set) Token: 0x06002EB2 RID: 11954 RVA: 0x00023F5F File Offset: 0x0002215F
		[ClientVar(Saved = true)]
		public static int censornudity
		{
			get
			{
				if (SteamClient.localCountry == "CN")
				{
					return 2;
				}
				return Global._censornudity;
			}
			set
			{
				if (SteamClient.localCountry == "CN")
				{
					value = 2;
				}
				if (Global._censornudity == value)
				{
					return;
				}
				Global._censornudity = value;
				PlayerModel.RebuildAll();
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06002EB3 RID: 11955 RVA: 0x00023F89 File Offset: 0x00022189
		// (set) Token: 0x06002EB4 RID: 11956 RVA: 0x00023F90 File Offset: 0x00022190
		[ClientVar(Saved = true)]
		public static bool censorsigns
		{
			get
			{
				return Global._censorsigns;
			}
			set
			{
				if (Global._censorsigns == value)
				{
					return;
				}
				Global._censorsigns = value;
				Signage.RebuildAll();
			}
		}

		// Token: 0x06002EB5 RID: 11957 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void restart(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002EB6 RID: 11958 RVA: 0x00023FA6 File Offset: 0x000221A6
		[ServerVar]
		[ClientVar]
		public static void quit(ConsoleSystem.Arg args)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
			Debug.Log("Quitting");
			Application.Quit();
		}

		// Token: 0x06002EB7 RID: 11959 RVA: 0x000E88B8 File Offset: 0x000E6AB8
		[ClientVar]
		public static void writecfg()
		{
			string text = ConsoleSystem.SaveToConfigString(false);
			if (!Directory.Exists("cfg"))
			{
				Directory.CreateDirectory("cfg");
			}
			try
			{
				File.WriteAllText("cfg/client.cfg", text);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Could not write client.cfg (" + ex.Message + ")");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string[]> keyValuePair in Input.GetAllBinds())
			{
				stringBuilder.AppendLine("bind " + keyValuePair.Key + " " + StringExtensions.QuoteSafe(string.Join(";", keyValuePair.Value)));
			}
			try
			{
				File.WriteAllText("cfg/keys.cfg", stringBuilder.ToString());
			}
			catch (Exception ex2)
			{
				Debug.LogWarning("Could not write keys.cfg (" + ex2.Message + ")");
			}
		}

		// Token: 0x06002EB8 RID: 11960 RVA: 0x000E89D0 File Offset: 0x000E6BD0
		[ClientVar]
		public static void readcfg(ConsoleSystem.Arg arg)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "exec client.cfg", Array.Empty<object>());
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "exec keys_default.cfg", Array.Empty<object>());
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "exec keys.cfg", Array.Empty<object>());
			arg.ReplyWith("Config Loaded");
		}

		// Token: 0x06002EB9 RID: 11961 RVA: 0x000E8A28 File Offset: 0x000E6C28
		[ClientVar]
		public static void exec(ConsoleSystem.Arg arg)
		{
			string text = "cfg/" + arg.FullString;
			if (!File.Exists(text))
			{
				return;
			}
			try
			{
				ConsoleSystem.RunFile(ConsoleSystem.Option.Client.Quiet(), File.ReadAllText(text));
				ConsoleSystem.HasChanges = false;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Could not load " + text);
				Debug.LogWarning(ex.ToString());
			}
		}

		// Token: 0x06002EBA RID: 11962 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002EBB RID: 11963 RVA: 0x000E8A9C File Offset: 0x000E6C9C
		[ClientVar]
		[ServerVar]
		public static void objects(ConsoleSystem.Arg args)
		{
			Object[] array = Object.FindObjectsOfType<Object>();
			string text = "";
			Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
			Dictionary<Type, long> dictionary2 = new Dictionary<Type, long>();
			foreach (Object @object in array)
			{
				int runtimeMemorySize = Profiler.GetRuntimeMemorySize(@object);
				if (dictionary.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, int> dictionary3 = dictionary;
					Type type = @object.GetType();
					int num = dictionary3[type];
					dictionary3[type] = num + 1;
				}
				else
				{
					dictionary.Add(@object.GetType(), 1);
				}
				if (dictionary2.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, long> dictionary4 = dictionary2;
					Type type = @object.GetType();
					dictionary4[type] += (long)runtimeMemorySize;
				}
				else
				{
					dictionary2.Add(@object.GetType(), (long)runtimeMemorySize);
				}
			}
			foreach (KeyValuePair<Type, long> keyValuePair in Enumerable.OrderByDescending<KeyValuePair<Type, long>, long>(dictionary2, delegate(KeyValuePair<Type, long> x)
			{
				KeyValuePair<Type, long> keyValuePair2 = x;
				return keyValuePair2.Value;
			}))
			{
				text = string.Concat(new object[]
				{
					text,
					dictionary[keyValuePair.Key].ToString().PadLeft(10),
					" ",
					NumberExtensions.FormatBytes<long>(keyValuePair.Value, false).PadLeft(15),
					"\t",
					keyValuePair.Key,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x06002EBC RID: 11964 RVA: 0x000E8C38 File Offset: 0x000E6E38
		[ServerVar]
		[ClientVar]
		public static void textures(ConsoleSystem.Arg args)
		{
			Texture[] array = Object.FindObjectsOfType<Texture>();
			string text = "";
			foreach (Texture texture in array)
			{
				string text2 = NumberExtensions.FormatBytes<int>(Profiler.GetRuntimeMemorySize(texture), false);
				text = string.Concat(new string[]
				{
					text,
					texture.ToString().PadRight(30),
					texture.name.PadRight(30),
					text2,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x06002EBD RID: 11965 RVA: 0x000E8CB8 File Offset: 0x000E6EB8
		[ClientVar]
		[ServerVar]
		public static void colliders(ConsoleSystem.Arg args)
		{
			int num = Enumerable.Count<Collider>(Enumerable.Where<Collider>(Object.FindObjectsOfType<Collider>(), (Collider x) => x.enabled));
			int num2 = Enumerable.Count<Collider>(Enumerable.Where<Collider>(Object.FindObjectsOfType<Collider>(), (Collider x) => !x.enabled));
			string text = string.Concat(new object[]
			{
				num,
				" colliders enabled, ",
				num2,
				" disabled"
			});
			args.ReplyWith(text);
		}

		// Token: 0x06002EBE RID: 11966 RVA: 0x00023FCC File Offset: 0x000221CC
		[ClientVar]
		[ServerVar]
		public static void error(ConsoleSystem.Arg args)
		{
			((GameObject)null).transform.position = Vector3.zero;
		}

		// Token: 0x06002EBF RID: 11967 RVA: 0x000E8D58 File Offset: 0x000E6F58
		[ClientVar]
		[ServerVar]
		public static void queue(ConsoleSystem.Arg args)
		{
			string text = "";
			args.ReplyWith(text);
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x00023FDE File Offset: 0x000221DE
		[ClientVar]
		public static void status_cl(ConsoleSystem.Arg args)
		{
			args.ReplyWith(LocalPlayer.Entity.GetDebugStatus());
		}

		// Token: 0x06002EC1 RID: 11969 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void teleport(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002EC2 RID: 11970 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void teleport2me(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002EC3 RID: 11971 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void teleportany(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002EC4 RID: 11972 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void teleportpos(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002EC5 RID: 11973 RVA: 0x00023FF0 File Offset: 0x000221F0
		[ServerVar]
		[ClientVar]
		public static void free(ConsoleSystem.Arg args)
		{
			Pool.clear_prefabs(args);
			Pool.clear_assets(args);
			Pool.clear_memory(args);
			GC.collect();
			GC.unload();
		}

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06002EC6 RID: 11974 RVA: 0x0002400E File Offset: 0x0002220E
		// (set) Token: 0x06002EC7 RID: 11975 RVA: 0x00024015 File Offset: 0x00022215
		[ClientVar(ClientInfo = true, Saved = true)]
		public static string language
		{
			get
			{
				return Translate.GetLanguage();
			}
			set
			{
				Translate.SetLanguage(value);
			}
		}

		// Token: 0x06002EC8 RID: 11976 RVA: 0x000E8D74 File Offset: 0x000E6F74
		[ServerVar(ServerUser = true)]
		[ClientVar]
		public static void version(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(string.Format("Protocol: {0}\nBuild Date: {1}\nUnity Version: {2}\nChangeset: {3}\nBranch: {4}", new object[]
			{
				Protocol.printable,
				BuildInfo.Current.BuildDate,
				Application.unityVersion,
				BuildInfo.Current.Scm.ChangeId,
				BuildInfo.Current.Scm.Branch
			}));
		}

		// Token: 0x06002EC9 RID: 11977 RVA: 0x0002401D File Offset: 0x0002221D
		[ClientVar]
		[ServerVar]
		public static void sysinfo(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfoGeneralText.currentInfo);
		}

		// Token: 0x06002ECA RID: 11978 RVA: 0x0002402A File Offset: 0x0002222A
		[ClientVar]
		[ServerVar]
		public static void sysuid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfo.deviceUniqueIdentifier);
		}

		// Token: 0x06002ECB RID: 11979 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void breakitem(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002ECC RID: 11980 RVA: 0x000E8DE0 File Offset: 0x000E6FE0
		[ClientVar]
		[ServerVar]
		public static void subscriptions(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("group");
			global::Client instance = SingletonComponent<global::Client>.Instance;
			if (instance)
			{
				foreach (uint num in instance.subscriptions)
				{
					textTable.AddRow(new string[]
					{
						"cl",
						num.ToString()
					});
				}
			}
			arg.ReplyWith(textTable.ToString());
		}
	}
}
