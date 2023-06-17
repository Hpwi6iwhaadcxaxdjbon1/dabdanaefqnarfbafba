using System;
using System.IO;
using Facepunch.Extend;
using Facepunch.Network.Raknet;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000848 RID: 2120
	public class Client
	{
		// Token: 0x04002944 RID: 10564
		public static float tickrate = 20f;

		// Token: 0x04002945 RID: 10565
		[ClientVar]
		public static int maxpeerspersecond = 100;

		// Token: 0x04002946 RID: 10566
		[ClientVar]
		public static bool prediction = true;

		// Token: 0x04002947 RID: 10567
		[ClientVar(Help = "Max amount of unacknowledged messages before we assume we're congested")]
		public static int maxunack = 4;

		// Token: 0x04002948 RID: 10568
		[ClientVar(Saved = true, Help = "When enabled voice-chat will be in push-to-talk mode instead of always on")]
		public static bool pushtotalk = true;

		// Token: 0x04002949 RID: 10569
		[ClientVar]
		public static bool debugdragdrop = false;

		// Token: 0x0400294A RID: 10570
		[ClientVar(AllowRunFromServer = true)]
		public static float camlerp = 1f;

		// Token: 0x0400294B RID: 10571
		[ClientVar(AllowRunFromServer = true)]
		public static float camspeed = 1f;

		// Token: 0x0400294C RID: 10572
		[ClientVar(Saved = true, AllowRunFromServer = true)]
		public static float camdist = 2f;

		// Token: 0x0400294D RID: 10573
		[ClientVar(Saved = true, AllowRunFromServer = true)]
		public static string cambone = "";

		// Token: 0x0400294E RID: 10574
		[ClientVar(Saved = true, AllowRunFromServer = true)]
		public static float camfov = 70f;

		// Token: 0x0400294F RID: 10575
		[ClientVar(Saved = true, AllowRunFromServer = true)]
		public static Vector3 camoffset = new Vector3(0f, 1f, 0f);

		// Token: 0x04002950 RID: 10576
		[ClientVar(Saved = true, AllowRunFromServer = true)]
		public static bool camoffset_relative = false;

		// Token: 0x04002951 RID: 10577
		[ClientVar(Saved = true, Help = "The radius of the sphere trace used to determine what you're looking at")]
		public static float lookatradius = 0.2f;

		// Token: 0x04002952 RID: 10578
		[ClientVar(ClientInfo = true, Saved = true)]
		public static int RockSkin;

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06002E36 RID: 11830 RVA: 0x00023BA5 File Offset: 0x00021DA5
		// (set) Token: 0x06002E37 RID: 11831 RVA: 0x00023BAC File Offset: 0x00021DAC
		[ClientVar]
		public static float maxreceivetime
		{
			get
			{
				return Client.MaxReceiveTime;
			}
			set
			{
				Client.MaxReceiveTime = Mathf.Clamp(value, 1f, 1000f);
			}
		}

		// Token: 0x06002E38 RID: 11832 RVA: 0x000E73B4 File Offset: 0x000E55B4
		[ClientVar]
		public static string connect(string address = "127.0.0.1:28015")
		{
			if (Net.cl.IsConnected())
			{
				return "You're already connecting or connected to a server";
			}
			ConnectionScreen.Show();
			string[] array = address.Split(new char[]
			{
				':'
			});
			if (array.Length != 2)
			{
				ConnectionScreen.FailedWith("invalid_address");
				return "Invalid address!";
			}
			int num = StringExtensions.ToInt(array[1], 0);
			if (num == 0 || array[0].Length < 4)
			{
				ConnectionScreen.FailedWith("invalid_address");
				return "Invalid address!";
			}
			if (SingletonComponent<Client>.Instance == null)
			{
				ConnectionScreen.FailedWith("invalid_address");
				return "Client is missing!";
			}
			try
			{
				SingletonComponent<Client>.Instance.Connect(array[0], num);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				ConnectionScreen.FailedWithException(ex);
			}
			ConnectionScreen.SetStatus("connecting");
			return "Connecting..";
		}

		// Token: 0x06002E39 RID: 11833 RVA: 0x00023BC3 File Offset: 0x00021DC3
		[ClientVar]
		public static string fps()
		{
			return Performance.report.frameRate.ToString() + " FPS";
		}

		// Token: 0x06002E3A RID: 11834 RVA: 0x000E7484 File Offset: 0x000E5684
		[ClientVar]
		public static string disconnect()
		{
			if (Client.IsPlayingDemo)
			{
				SingletonComponent<Client>.Instance.StopPlayingDemo(false);
				return null;
			}
			if (Client.IsRecordingDemo)
			{
				SingletonComponent<Client>.Instance.StopRecordingDemo();
				return null;
			}
			if (!Net.cl.IsConnected())
			{
				return "You're not connected";
			}
			if (SingletonComponent<Client>.Instance != null)
			{
				SingletonComponent<Client>.Instance.CancelAuthTicket();
			}
			Net.cl.Disconnect("disconnect", true);
			return "Disconnected";
		}

		// Token: 0x06002E3B RID: 11835 RVA: 0x000E74F8 File Offset: 0x000E56F8
		[ClientVar]
		public static void report()
		{
			string text = "";
			text += "Entity List\n";
			foreach (BaseNetworkable baseNetworkable in BaseNetworkable.clientEntities)
			{
				BaseEntity baseEntity = (BaseEntity)baseNetworkable;
				if (baseEntity.IsValid())
				{
					text = string.Concat(new string[]
					{
						text,
						baseEntity.net.ID.ToString().PadRight(10),
						" ",
						baseEntity.PrefabName.PadRight(50),
						"\n"
					});
				}
				else
				{
					text = string.Concat(new string[]
					{
						text,
						"(NOT VALID)".PadRight(10),
						" ",
						baseEntity.PrefabName.PadRight(50),
						"\n"
					});
				}
			}
			File.WriteAllText(("cl_report." + DateTime.Now.ToString() + ".txt").Replace('\\', '-').Replace('/', '-').Replace(' ', '_').Replace(':', '.'), text);
		}

		// Token: 0x06002E3C RID: 11836 RVA: 0x000E7634 File Offset: 0x000E5834
		[ClientVar(Help = "Print the current player position.")]
		public static string printpos()
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!(entity == null))
			{
				return entity.transform.position.ToString();
			}
			return "invalid player";
		}

		// Token: 0x06002E3D RID: 11837 RVA: 0x000E7670 File Offset: 0x000E5870
		[ClientVar(Help = "Print the current player rotation.")]
		public static string printrot()
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!(entity == null))
			{
				return entity.transform.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x06002E3E RID: 11838 RVA: 0x000E76B4 File Offset: 0x000E58B4
		[ClientVar(Help = "Print the current player eyes.")]
		public static string printeyes()
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!(entity == null))
			{
				return entity.eyes.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x06002E3F RID: 11839 RVA: 0x000E76F8 File Offset: 0x000E58F8
		[ClientVar(Help = "Print the current player input.")]
		public static string printinput()
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!(entity == null))
			{
				return entity.input.ClientLookVars().ToString();
			}
			return "invalid player";
		}

		// Token: 0x06002E40 RID: 11840 RVA: 0x000E7734 File Offset: 0x000E5934
		[ClientVar(Help = "Print the current player head.")]
		public static string printhead()
		{
			BasePlayer entity = LocalPlayer.Entity;
			if (!(entity == null))
			{
				return entity.input.HeadLookVars().ToString();
			}
			return "invalid player";
		}

		// Token: 0x06002E41 RID: 11841 RVA: 0x000E7770 File Offset: 0x000E5970
		public static string GetClientFolder(string folder)
		{
			if (Directory.Exists(folder))
			{
				return folder;
			}
			Directory.CreateDirectory(folder);
			return folder;
		}

		// Token: 0x06002E42 RID: 11842 RVA: 0x00023BDE File Offset: 0x00021DDE
		[ClientVar(AllowRunFromServer = true)]
		public static void sv(ConsoleSystem.Arg arg)
		{
			ConsoleNetwork.ClientRunOnServer(arg.FullString);
		}

		// Token: 0x06002E43 RID: 11843 RVA: 0x00023BEC File Offset: 0x00021DEC
		[ClientVar]
		public static void consoletoggle()
		{
			SingletonComponent<DeveloperTools>.Instance.ToggleConsole();
		}

		// Token: 0x06002E44 RID: 11844 RVA: 0x00023BF8 File Offset: 0x00021DF8
		[ClientVar]
		public static int ping()
		{
			if (!Net.cl.IsConnected())
			{
				return 0;
			}
			return Net.cl.GetLastPing();
		}

		// Token: 0x06002E45 RID: 11845 RVA: 0x00023C12 File Offset: 0x00021E12
		[ClientVar]
		public static void benchmark()
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "timedemo", new object[]
			{
				"benchmark"
			});
		}
	}
}
