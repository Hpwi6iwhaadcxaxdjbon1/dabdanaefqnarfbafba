using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ConVar;
using Facepunch.Rcon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000894 RID: 2196
	public class RCon
	{
		// Token: 0x04002A46 RID: 10822
		public static string Password = "";

		// Token: 0x04002A47 RID: 10823
		[ServerVar]
		public static int Port = 0;

		// Token: 0x04002A48 RID: 10824
		[ServerVar]
		public static string Ip = "";

		// Token: 0x04002A49 RID: 10825
		[ServerVar(Help = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.")]
		public static bool Web = true;

		// Token: 0x04002A4A RID: 10826
		[ServerVar(Help = "If true, rcon commands etc will be printed in the console")]
		public static bool Print = false;

		// Token: 0x04002A4B RID: 10827
		internal static RCon.RConListener listener = null;

		// Token: 0x04002A4C RID: 10828
		internal static Listener listenerNew = null;

		// Token: 0x04002A4D RID: 10829
		private static Queue<RCon.Command> Commands = new Queue<RCon.Command>();

		// Token: 0x04002A4E RID: 10830
		private static float lastRunTime = 0f;

		// Token: 0x04002A4F RID: 10831
		internal static List<RCon.BannedAddresses> bannedAddresses = new List<RCon.BannedAddresses>();

		// Token: 0x04002A50 RID: 10832
		private static int responseIdentifier;

		// Token: 0x04002A51 RID: 10833
		private static string responseConnection;

		// Token: 0x04002A52 RID: 10834
		private static bool isInput;

		// Token: 0x04002A53 RID: 10835
		internal static int SERVERDATA_AUTH = 3;

		// Token: 0x04002A54 RID: 10836
		internal static int SERVERDATA_EXECCOMMAND = 2;

		// Token: 0x04002A55 RID: 10837
		internal static int SERVERDATA_AUTH_RESPONSE = 2;

		// Token: 0x04002A56 RID: 10838
		internal static int SERVERDATA_RESPONSE_VALUE = 0;

		// Token: 0x04002A57 RID: 10839
		internal static int SERVERDATA_CONSOLE_LOG = 4;

		// Token: 0x04002A58 RID: 10840
		internal static int SERVERDATA_SWITCH_UTF8 = 5;

		// Token: 0x06002F89 RID: 12169 RVA: 0x000EA29C File Offset: 0x000E849C
		public static void Initialize()
		{
			if (RCon.Port == 0)
			{
				RCon.Port = Server.port;
			}
			RCon.Password = CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", ""));
			if (RCon.Password == "password")
			{
				return;
			}
			if (RCon.Password == "")
			{
				return;
			}
			Output.OnMessage += new Action<string, string, UnityEngine.LogType>(RCon.OnMessage);
			if (RCon.Web)
			{
				RCon.listenerNew = new Listener();
				if (!string.IsNullOrEmpty(RCon.Ip))
				{
					RCon.listenerNew.Address = RCon.Ip;
				}
				RCon.listenerNew.Password = RCon.Password;
				RCon.listenerNew.Port = RCon.Port;
				RCon.listenerNew.SslCertificate = CommandLine.GetSwitch("-rcon.ssl", null);
				RCon.listenerNew.SslCertificatePassword = CommandLine.GetSwitch("-rcon.sslpwd", null);
				RCon.listenerNew.OnMessage = delegate(IPEndPoint ip, string id, string msg)
				{
					Queue<RCon.Command> commands = RCon.Commands;
					lock (commands)
					{
						RCon.Command command = JsonConvert.DeserializeObject<RCon.Command>(msg);
						command.Ip = ip;
						command.ConnectionId = id;
						RCon.Commands.Enqueue(command);
					}
				};
				RCon.listenerNew.Start();
				Debug.Log("WebSocket RCon Started on " + RCon.Port);
				return;
			}
			RCon.listener = new RCon.RConListener();
			Debug.Log("RCon Started on " + RCon.Port);
			Debug.Log("Source style TCP Rcon is deprecated. Please switch to Websocket Rcon before it goes away.");
		}

		// Token: 0x06002F8A RID: 12170 RVA: 0x00024823 File Offset: 0x00022A23
		public static void Shutdown()
		{
			if (RCon.listenerNew != null)
			{
				RCon.listenerNew.Shutdown();
				RCon.listenerNew = null;
			}
			if (RCon.listener != null)
			{
				RCon.listener.Shutdown();
				RCon.listener = null;
			}
		}

		// Token: 0x06002F8B RID: 12171 RVA: 0x000EA400 File Offset: 0x000E8600
		public static void Broadcast(RCon.LogType type, object obj)
		{
			if (RCon.listenerNew == null)
			{
				return;
			}
			RCon.Response response = default(RCon.Response);
			response.Identifier = -1;
			response.Message = JsonConvert.SerializeObject(obj, 1);
			response.Type = type;
			if (string.IsNullOrEmpty(RCon.responseConnection))
			{
				RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, 1));
				return;
			}
			RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, 1));
		}

		// Token: 0x06002F8C RID: 12172 RVA: 0x000EA47C File Offset: 0x000E867C
		public static void Update()
		{
			Queue<RCon.Command> commands = RCon.Commands;
			lock (commands)
			{
				while (RCon.Commands.Count > 0)
				{
					RCon.OnCommand(RCon.Commands.Dequeue());
				}
			}
			if (RCon.listener == null)
			{
				return;
			}
			if (RCon.lastRunTime + 0.02f >= UnityEngine.Time.realtimeSinceStartup)
			{
				return;
			}
			RCon.lastRunTime = UnityEngine.Time.realtimeSinceStartup;
			try
			{
				RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.banTime < UnityEngine.Time.realtimeSinceStartup);
				RCon.listener.Cycle();
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Rcon Exception");
				Debug.LogException(exception);
			}
		}

		// Token: 0x06002F8D RID: 12173 RVA: 0x000EA54C File Offset: 0x000E874C
		public static void BanIP(IPAddress addr, float seconds)
		{
			RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.addr == addr);
			RCon.BannedAddresses bannedAddresses = default(RCon.BannedAddresses);
			bannedAddresses.addr = addr;
			bannedAddresses.banTime = UnityEngine.Time.realtimeSinceStartup + seconds;
		}

		// Token: 0x06002F8E RID: 12174 RVA: 0x000EA5A0 File Offset: 0x000E87A0
		public static bool IsBanned(IPAddress addr)
		{
			return Enumerable.Count<RCon.BannedAddresses>(RCon.bannedAddresses, (RCon.BannedAddresses x) => x.addr == addr && x.banTime > UnityEngine.Time.realtimeSinceStartup) > 0;
		}

		// Token: 0x06002F8F RID: 12175 RVA: 0x000EA5D4 File Offset: 0x000E87D4
		private static void OnCommand(RCon.Command cmd)
		{
			try
			{
				RCon.responseIdentifier = cmd.Identifier;
				RCon.responseConnection = cmd.ConnectionId;
				RCon.isInput = true;
				if (RCon.Print)
				{
					Debug.Log(string.Concat(new object[]
					{
						"[rcon] ",
						cmd.Ip,
						": ",
						cmd.Message
					}));
				}
				RCon.isInput = false;
				string text = ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), cmd.Message, Array.Empty<object>());
				if (text != null)
				{
					RCon.OnMessage(text, string.Empty, UnityEngine.LogType.Log);
				}
			}
			finally
			{
				RCon.responseIdentifier = 0;
				RCon.responseConnection = string.Empty;
			}
		}

		// Token: 0x06002F90 RID: 12176 RVA: 0x000EA68C File Offset: 0x000E888C
		private static void OnMessage(string message, string stacktrace, UnityEngine.LogType type)
		{
			if (RCon.isInput)
			{
				return;
			}
			if (RCon.listenerNew != null)
			{
				RCon.Response response = default(RCon.Response);
				response.Identifier = RCon.responseIdentifier;
				response.Message = message;
				response.Stacktrace = stacktrace;
				response.Type = RCon.LogType.Generic;
				if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception)
				{
					response.Type = RCon.LogType.Error;
				}
				if (type == UnityEngine.LogType.Assert || type == UnityEngine.LogType.Warning)
				{
					response.Type = RCon.LogType.Warning;
				}
				if (string.IsNullOrEmpty(RCon.responseConnection))
				{
					RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, 1));
					return;
				}
				RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, 1));
			}
		}

		// Token: 0x02000895 RID: 2197
		public struct Command
		{
			// Token: 0x04002A59 RID: 10841
			public IPEndPoint Ip;

			// Token: 0x04002A5A RID: 10842
			public string ConnectionId;

			// Token: 0x04002A5B RID: 10843
			public string Name;

			// Token: 0x04002A5C RID: 10844
			public string Message;

			// Token: 0x04002A5D RID: 10845
			public int Identifier;
		}

		// Token: 0x02000896 RID: 2198
		public enum LogType
		{
			// Token: 0x04002A5F RID: 10847
			Generic,
			// Token: 0x04002A60 RID: 10848
			Error,
			// Token: 0x04002A61 RID: 10849
			Warning,
			// Token: 0x04002A62 RID: 10850
			Chat
		}

		// Token: 0x02000897 RID: 2199
		public struct Response
		{
			// Token: 0x04002A63 RID: 10851
			public string Message;

			// Token: 0x04002A64 RID: 10852
			public int Identifier;

			// Token: 0x04002A65 RID: 10853
			[JsonConverter(typeof(StringEnumConverter))]
			public RCon.LogType Type;

			// Token: 0x04002A66 RID: 10854
			public string Stacktrace;
		}

		// Token: 0x02000898 RID: 2200
		internal struct BannedAddresses
		{
			// Token: 0x04002A67 RID: 10855
			public IPAddress addr;

			// Token: 0x04002A68 RID: 10856
			public float banTime;
		}

		// Token: 0x02000899 RID: 2201
		internal class RConClient
		{
			// Token: 0x04002A69 RID: 10857
			private Socket socket;

			// Token: 0x04002A6A RID: 10858
			private bool isAuthorised;

			// Token: 0x04002A6B RID: 10859
			private string connectionName;

			// Token: 0x04002A6C RID: 10860
			private int lastMessageID = -1;

			// Token: 0x04002A6D RID: 10861
			private bool runningConsoleCommand;

			// Token: 0x04002A6E RID: 10862
			private bool utf8Mode;

			// Token: 0x06002F93 RID: 12179 RVA: 0x00024853 File Offset: 0x00022A53
			internal RConClient(Socket cl)
			{
				this.socket = cl;
				this.socket.NoDelay = true;
				this.connectionName = this.socket.RemoteEndPoint.ToString();
			}

			// Token: 0x06002F94 RID: 12180 RVA: 0x0002488B File Offset: 0x00022A8B
			internal bool IsValid()
			{
				return this.socket != null;
			}

			// Token: 0x06002F95 RID: 12181 RVA: 0x000EA7B8 File Offset: 0x000E89B8
			internal void Update()
			{
				if (this.socket == null)
				{
					return;
				}
				if (!this.socket.Connected)
				{
					this.Close("Disconnected");
					return;
				}
				int available = this.socket.Available;
				if (available < 14)
				{
					return;
				}
				if (available > 4096)
				{
					this.Close("overflow");
					return;
				}
				byte[] array = new byte[available];
				this.socket.Receive(array);
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(array, false), this.utf8Mode ? Encoding.UTF8 : Encoding.ASCII))
				{
					int num = binaryReader.ReadInt32();
					if (available < num)
					{
						this.Close("invalid packet");
					}
					else
					{
						this.lastMessageID = binaryReader.ReadInt32();
						int type = binaryReader.ReadInt32();
						string msg = this.ReadNullTerminatedString(binaryReader);
						this.ReadNullTerminatedString(binaryReader);
						if (!this.HandleMessage(type, msg))
						{
							this.Close("invalid packet");
						}
						else
						{
							this.lastMessageID = -1;
						}
					}
				}
			}

			// Token: 0x06002F96 RID: 12182 RVA: 0x000EA8BC File Offset: 0x000E8ABC
			internal bool HandleMessage(int type, string msg)
			{
				if (!this.isAuthorised)
				{
					return this.HandleMessage_UnAuthed(type, msg);
				}
				if (type == RCon.SERVERDATA_SWITCH_UTF8)
				{
					this.utf8Mode = true;
					return true;
				}
				if (type == RCon.SERVERDATA_EXECCOMMAND)
				{
					Debug.Log("[RCON][" + this.connectionName + "] " + msg);
					this.runningConsoleCommand = true;
					ConsoleSystem.Run(ConsoleSystem.Option.Server, msg, Array.Empty<object>());
					this.runningConsoleCommand = false;
					this.Reply(-1, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				if (type == RCon.SERVERDATA_RESPONSE_VALUE)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				Debug.Log(string.Concat(new object[]
				{
					"[RCON][",
					this.connectionName,
					"] Unhandled: ",
					this.lastMessageID,
					" -> ",
					type,
					" -> ",
					msg
				}));
				return false;
			}

			// Token: 0x06002F97 RID: 12183 RVA: 0x000EA9B8 File Offset: 0x000E8BB8
			internal bool HandleMessage_UnAuthed(int type, string msg)
			{
				if (type != RCon.SERVERDATA_AUTH)
				{
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Command - Not Authed");
					return false;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
				this.isAuthorised = (RCon.Password == msg);
				if (!this.isAuthorised)
				{
					this.Reply(-1, RCon.SERVERDATA_AUTH_RESPONSE, "");
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Password");
					return true;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_AUTH_RESPONSE, "");
				Debug.Log("[RCON] Auth: " + this.connectionName);
				Output.OnMessage += new Action<string, string, UnityEngine.LogType>(this.Output_OnMessage);
				return true;
			}

			// Token: 0x06002F98 RID: 12184 RVA: 0x000EAAA4 File Offset: 0x000E8CA4
			private void Output_OnMessage(string message, string stacktrace, UnityEngine.LogType type)
			{
				if (!this.isAuthorised)
				{
					return;
				}
				if (!this.IsValid())
				{
					return;
				}
				if (this.lastMessageID != -1 && this.runningConsoleCommand)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, message);
				}
				this.Reply(0, RCon.SERVERDATA_CONSOLE_LOG, message);
			}

			// Token: 0x06002F99 RID: 12185 RVA: 0x000EAAF4 File Offset: 0x000E8CF4
			internal void Reply(int id, int type, string msg)
			{
				MemoryStream memoryStream = new MemoryStream(1024);
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					if (this.utf8Mode)
					{
						byte[] bytes = Encoding.UTF8.GetBytes(msg);
						int num = 10 + bytes.Length;
						binaryWriter.Write(num);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						binaryWriter.Write(bytes);
					}
					else
					{
						int num2 = 10 + msg.Length;
						binaryWriter.Write(num2);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						for (int i = 0; i < msg.Length; i++)
						{
							char c = msg.get_Chars(i);
							binaryWriter.Write((sbyte)c);
						}
					}
					binaryWriter.Write(0);
					binaryWriter.Write(0);
					binaryWriter.Flush();
					try
					{
						this.socket.Send(memoryStream.GetBuffer(), (int)memoryStream.Position, 0);
					}
					catch (Exception ex)
					{
						Debug.LogWarning("Error sending rcon reply: " + ex);
						this.Close("Exception");
					}
				}
			}

			// Token: 0x06002F9A RID: 12186 RVA: 0x000EAC10 File Offset: 0x000E8E10
			internal void Close(string strReasn)
			{
				Output.OnMessage -= new Action<string, string, UnityEngine.LogType>(this.Output_OnMessage);
				if (this.socket == null)
				{
					return;
				}
				Debug.Log("[RCON][" + this.connectionName + "] Disconnected: " + strReasn);
				this.socket.Close();
				this.socket = null;
			}

			// Token: 0x06002F9B RID: 12187 RVA: 0x000EAC64 File Offset: 0x000E8E64
			internal string ReadNullTerminatedString(BinaryReader read)
			{
				string text = "";
				while (read.BaseStream.Position != read.BaseStream.Length)
				{
					char c = read.ReadChar();
					if (c == '\0')
					{
						return text;
					}
					text += c.ToString();
					if (text.Length > 8192)
					{
						return string.Empty;
					}
				}
				return "";
			}
		}

		// Token: 0x0200089A RID: 2202
		internal class RConListener
		{
			// Token: 0x04002A6F RID: 10863
			private TcpListener server;

			// Token: 0x04002A70 RID: 10864
			private List<RCon.RConClient> clients = new List<RCon.RConClient>();

			// Token: 0x06002F9C RID: 12188 RVA: 0x000EACC4 File Offset: 0x000E8EC4
			internal RConListener()
			{
				IPAddress any = IPAddress.Any;
				if (!IPAddress.TryParse(RCon.Ip, ref any))
				{
					any = IPAddress.Any;
				}
				this.server = new TcpListener(any, RCon.Port);
				try
				{
					this.server.Start();
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Couldn't start RCON Listener: " + ex.Message);
					this.server = null;
				}
			}

			// Token: 0x06002F9D RID: 12189 RVA: 0x00024896 File Offset: 0x00022A96
			internal void Shutdown()
			{
				if (this.server != null)
				{
					this.server.Stop();
					this.server = null;
				}
			}

			// Token: 0x06002F9E RID: 12190 RVA: 0x000248B2 File Offset: 0x00022AB2
			internal void Cycle()
			{
				if (this.server == null)
				{
					return;
				}
				this.ProcessConnections();
				this.RemoveDeadClients();
				this.UpdateClients();
			}

			// Token: 0x06002F9F RID: 12191 RVA: 0x000EAD4C File Offset: 0x000E8F4C
			private void ProcessConnections()
			{
				if (!this.server.Pending())
				{
					return;
				}
				Socket socket = this.server.AcceptSocket();
				if (socket == null)
				{
					return;
				}
				IPEndPoint ipendPoint = socket.RemoteEndPoint as IPEndPoint;
				if (RCon.IsBanned(ipendPoint.Address))
				{
					Debug.Log("[RCON] Ignoring connection - banned. " + ipendPoint.Address.ToString());
					socket.Close();
					return;
				}
				this.clients.Add(new RCon.RConClient(socket));
			}

			// Token: 0x06002FA0 RID: 12192 RVA: 0x000EADC4 File Offset: 0x000E8FC4
			private void UpdateClients()
			{
				foreach (RCon.RConClient rconClient in this.clients)
				{
					rconClient.Update();
				}
			}

			// Token: 0x06002FA1 RID: 12193 RVA: 0x000248CF File Offset: 0x00022ACF
			private void RemoveDeadClients()
			{
				this.clients.RemoveAll((RCon.RConClient x) => !x.IsValid());
			}
		}
	}
}
