using System;
using ConVar;
using Facepunch.Extend;
using GameTips;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class ConsoleGen
{
	// Token: 0x040000C2 RID: 194
	public static ConsoleSystem.Command[] All = new ConsoleSystem.Command[]
	{
		new ConsoleSystem.Command
		{
			Name = "lootpaneloverride",
			Parent = "baseplayer",
			FullName = "baseplayer.lootpaneloverride",
			ClientAdmin = true,
			Client = true,
			Variable = true,
			GetOveride = (() => BasePlayer.lootPanelOverride.ToString()),
			SetOveride = delegate(string str)
			{
				BasePlayer.lootPanelOverride = str;
			}
		},
		new ConsoleSystem.Command
		{
			Name = "altlook",
			Parent = "buttons",
			FullName = "buttons.altlook",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.AltLook.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "attack",
			Parent = "buttons",
			FullName = "buttons.attack",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Attack.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "attack2",
			Parent = "buttons",
			FullName = "buttons.attack2",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Attack2.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "attack3",
			Parent = "buttons",
			FullName = "buttons.attack3",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Attack3.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "backward",
			Parent = "buttons",
			FullName = "buttons.backward",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Backward.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "chat",
			Parent = "buttons",
			FullName = "buttons.chat",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Chat.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "compass",
			Parent = "buttons",
			FullName = "buttons.compass",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Compass.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "console",
			Parent = "buttons",
			FullName = "buttons.console",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Console.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "duck",
			Parent = "buttons",
			FullName = "buttons.duck",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Duck.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "forward",
			Parent = "buttons",
			FullName = "buttons.forward",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Forward.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "inventory",
			Parent = "buttons",
			FullName = "buttons.inventory",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Inventory.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "invnext",
			Parent = "buttons",
			FullName = "buttons.invnext",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.InvNext.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "invprev",
			Parent = "buttons",
			FullName = "buttons.invprev",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.InvPrev.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "jump",
			Parent = "buttons",
			FullName = "buttons.jump",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Jump.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "left",
			Parent = "buttons",
			FullName = "buttons.left",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Left.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "map",
			Parent = "buttons",
			FullName = "buttons.map",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Map.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "reload",
			Parent = "buttons",
			FullName = "buttons.reload",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Reload.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "right",
			Parent = "buttons",
			FullName = "buttons.right",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Right.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot1",
			Parent = "buttons",
			FullName = "buttons.slot1",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot1.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot2",
			Parent = "buttons",
			FullName = "buttons.slot2",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot2.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot3",
			Parent = "buttons",
			FullName = "buttons.slot3",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot3.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot4",
			Parent = "buttons",
			FullName = "buttons.slot4",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot4.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot5",
			Parent = "buttons",
			FullName = "buttons.slot5",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot5.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot6",
			Parent = "buttons",
			FullName = "buttons.slot6",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot6.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot7",
			Parent = "buttons",
			FullName = "buttons.slot7",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot7.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "slot8",
			Parent = "buttons",
			FullName = "buttons.slot8",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Slot8.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "sprint",
			Parent = "buttons",
			FullName = "buttons.sprint",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Sprint.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "use",
			Parent = "buttons",
			FullName = "buttons.use",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Use.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "voice",
			Parent = "buttons",
			FullName = "buttons.voice",
			Client = true,
			Call = new Action<ConsoleSystem.Arg>(Buttons.Voice.Call)
		},
		new ConsoleSystem.Command
		{
			Name = "echo",
			Parent = "commands",
			FullName = "commands.echo",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Commands.Echo(arg.FullString);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find",
			Parent = "commands",
			FullName = "commands.find",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Commands.Find(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ent",
			Parent = "global",
			FullName = "global.ent",
			Client = true,
			Description = "Sends a command to the server followed by the ID of the entity we're looking at",
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = Admin.ent(arg.GetString(0, ""));
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "aidebug_loadbalanceoverduereport",
			Parent = "ai",
			FullName = "ai.aidebug_loadbalanceoverduereport",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				AI.aiDebug_LoadBalanceOverdueReport(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "aidebug_lookat",
			Parent = "ai",
			FullName = "ai.aidebug_lookat",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				AI.aiDebug_lookat(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debugvis",
			Parent = "ai",
			FullName = "ai.debugvis",
			Client = true,
			Variable = true,
			GetOveride = (() => AI.debugVis.ToString()),
			SetOveride = delegate(string str)
			{
				AI.debugVis = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "groundalign",
			Parent = "ai",
			FullName = "ai.groundalign",
			Client = true,
			Variable = true,
			GetOveride = (() => AI.groundAlign.ToString()),
			SetOveride = delegate(string str)
			{
				AI.groundAlign = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxgroundaligndist",
			Parent = "ai",
			FullName = "ai.maxgroundaligndist",
			Client = true,
			Variable = true,
			GetOveride = (() => AI.maxGroundAlignDist.ToString()),
			SetOveride = delegate(string str)
			{
				AI.maxGroundAlignDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "npc_no_foot_ik",
			Parent = "ai",
			FullName = "ai.npc_no_foot_ik",
			Client = true,
			Variable = true,
			GetOveride = (() => AI.npc_no_foot_ik.ToString()),
			SetOveride = delegate(string str)
			{
				AI.npc_no_foot_ik = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "selectnpclookat",
			Parent = "ai",
			FullName = "ai.selectnpclookat",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				AI.selectNPCLookat(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "advancedocclusion",
			Parent = "audio",
			FullName = "audio.advancedocclusion",
			Client = true,
			Saved = true,
			Description = "Use more advanced sound occlusion",
			Variable = true,
			GetOveride = (() => Audio.advancedocclusion.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.advancedocclusion = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ambience",
			Parent = "audio",
			FullName = "audio.ambience",
			Client = true,
			Description = "Ambience System",
			Variable = true,
			GetOveride = (() => Audio.ambience.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.ambience = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "framebudget",
			Parent = "audio",
			FullName = "audio.framebudget",
			Client = true,
			Description = "Max ms per frame to spend updating sounds",
			Variable = true,
			GetOveride = (() => Audio.framebudget.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.framebudget = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "game",
			Parent = "audio",
			FullName = "audio.game",
			Client = true,
			Saved = true,
			Description = "Volume",
			Variable = true,
			GetOveride = (() => Audio.game.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.game = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "master",
			Parent = "audio",
			FullName = "audio.master",
			Client = true,
			Saved = true,
			Description = "Volume",
			Variable = true,
			GetOveride = (() => Audio.master.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.master = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "musicvolume",
			Parent = "audio",
			FullName = "audio.musicvolume",
			Client = true,
			Saved = true,
			Description = "Volume",
			Variable = true,
			GetOveride = (() => Audio.musicvolume.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.musicvolume = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "musicvolumemenu",
			Parent = "audio",
			FullName = "audio.musicvolumemenu",
			Client = true,
			Saved = true,
			Description = "Volume",
			Variable = true,
			GetOveride = (() => Audio.musicvolumemenu.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.musicvolumemenu = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "speakers",
			Parent = "audio",
			FullName = "audio.speakers",
			Client = true,
			Saved = true,
			Description = "Volume",
			Variable = true,
			GetOveride = (() => Audio.speakers.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.speakers = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "voices",
			Parent = "audio",
			FullName = "audio.voices",
			Client = true,
			Saved = true,
			Description = "Volume",
			Variable = true,
			GetOveride = (() => Audio.voices.ToString()),
			SetOveride = delegate(string str)
			{
				Audio.voices = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "collider_capacity",
			Parent = "batching",
			FullName = "batching.collider_capacity",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.collider_capacity.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.collider_capacity = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "collider_submeshes",
			Parent = "batching",
			FullName = "batching.collider_submeshes",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.collider_submeshes.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.collider_submeshes = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "collider_threading",
			Parent = "batching",
			FullName = "batching.collider_threading",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.collider_threading.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.collider_threading = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "collider_vertices",
			Parent = "batching",
			FullName = "batching.collider_vertices",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.collider_vertices.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.collider_vertices = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "colliders",
			Parent = "batching",
			FullName = "batching.colliders",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.colliders.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.colliders = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "print_renderers",
			Parent = "batching",
			FullName = "batching.print_renderers",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Batching.print_renderers(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "refresh_renderers",
			Parent = "batching",
			FullName = "batching.refresh_renderers",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Batching.refresh_renderers(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "renderer_capacity",
			Parent = "batching",
			FullName = "batching.renderer_capacity",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.renderer_capacity.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.renderer_capacity = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "renderer_submeshes",
			Parent = "batching",
			FullName = "batching.renderer_submeshes",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.renderer_submeshes.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.renderer_submeshes = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "renderer_threading",
			Parent = "batching",
			FullName = "batching.renderer_threading",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.renderer_threading.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.renderer_threading = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "renderer_vertices",
			Parent = "batching",
			FullName = "batching.renderer_vertices",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.renderer_vertices.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.renderer_vertices = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "renderers",
			Parent = "batching",
			FullName = "batching.renderers",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.renderers.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.renderers = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "verbose",
			Parent = "batching",
			FullName = "batching.verbose",
			Client = true,
			Variable = true,
			GetOveride = (() => Batching.verbose.ToString()),
			SetOveride = delegate(string str)
			{
				Batching.verbose = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "add",
			Parent = "chat",
			FullName = "chat.add",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Chat.add(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "add2",
			Parent = "chat",
			FullName = "chat.add2",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Chat.add2(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "chat",
			FullName = "chat.enabled",
			Client = true,
			Variable = true,
			GetOveride = (() => Chat.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				Chat.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "open",
			Parent = "chat",
			FullName = "chat.open",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Chat.open();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "benchmark",
			Parent = "client",
			FullName = "client.benchmark",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.Client.benchmark();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "cambone",
			Parent = "client",
			FullName = "client.cambone",
			Client = true,
			Saved = true,
			AllowRunFromServer = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.cambone.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.cambone = str;
			}
		},
		new ConsoleSystem.Command
		{
			Name = "camdist",
			Parent = "client",
			FullName = "client.camdist",
			Client = true,
			Saved = true,
			AllowRunFromServer = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.camdist.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.camdist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "camfov",
			Parent = "client",
			FullName = "client.camfov",
			Client = true,
			Saved = true,
			AllowRunFromServer = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.camfov.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.camfov = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "camlerp",
			Parent = "client",
			FullName = "client.camlerp",
			Client = true,
			AllowRunFromServer = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.camlerp.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.camlerp = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "camoffset",
			Parent = "client",
			FullName = "client.camoffset",
			Client = true,
			Saved = true,
			AllowRunFromServer = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.camoffset.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.camoffset = StringExtensions.ToVector3(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "camoffset_relative",
			Parent = "client",
			FullName = "client.camoffset_relative",
			Client = true,
			Saved = true,
			AllowRunFromServer = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.camoffset_relative.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.camoffset_relative = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "camspeed",
			Parent = "client",
			FullName = "client.camspeed",
			Client = true,
			AllowRunFromServer = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.camspeed.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.camspeed = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "connect",
			Parent = "client",
			FullName = "client.connect",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.connect(arg.GetString(0, "127.0.0.1:28015"));
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "consoletoggle",
			Parent = "client",
			FullName = "client.consoletoggle",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.Client.consoletoggle();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debugdragdrop",
			Parent = "client",
			FullName = "client.debugdragdrop",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.debugdragdrop.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.debugdragdrop = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "disconnect",
			Parent = "client",
			FullName = "client.disconnect",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.disconnect();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "fps",
			Parent = "client",
			FullName = "client.fps",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.fps();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "lookatradius",
			Parent = "client",
			FullName = "client.lookatradius",
			Client = true,
			Saved = true,
			Description = "The radius of the sphere trace used to determine what you're looking at",
			Variable = true,
			GetOveride = (() => ConVar.Client.lookatradius.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.lookatradius = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxpeerspersecond",
			Parent = "client",
			FullName = "client.maxpeerspersecond",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.maxpeerspersecond.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.maxpeerspersecond = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxreceivetime",
			Parent = "client",
			FullName = "client.maxreceivetime",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.maxreceivetime.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.maxreceivetime = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxunack",
			Parent = "client",
			FullName = "client.maxunack",
			Client = true,
			Description = "Max amount of unacknowledged messages before we assume we're congested",
			Variable = true,
			GetOveride = (() => ConVar.Client.maxunack.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.maxunack = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ping",
			Parent = "client",
			FullName = "client.ping",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				int num = ConVar.Client.ping();
				arg.ReplyWithObject(num);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "prediction",
			Parent = "client",
			FullName = "client.prediction",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.prediction.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.prediction = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "printeyes",
			Parent = "client",
			FullName = "client.printeyes",
			Client = true,
			Description = "Print the current player eyes.",
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.printeyes();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "printhead",
			Parent = "client",
			FullName = "client.printhead",
			Client = true,
			Description = "Print the current player head.",
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.printhead();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "printinput",
			Parent = "client",
			FullName = "client.printinput",
			Client = true,
			Description = "Print the current player input.",
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.printinput();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "printpos",
			Parent = "client",
			FullName = "client.printpos",
			Client = true,
			Description = "Print the current player position.",
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.printpos();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "printrot",
			Parent = "client",
			FullName = "client.printrot",
			Client = true,
			Description = "Print the current player rotation.",
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Client.printrot();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "pushtotalk",
			Parent = "client",
			FullName = "client.pushtotalk",
			Client = true,
			Saved = true,
			Description = "When enabled voice-chat will be in push-to-talk mode instead of always on",
			Variable = true,
			GetOveride = (() => ConVar.Client.pushtotalk.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.pushtotalk = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "report",
			Parent = "client",
			FullName = "client.report",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.Client.report();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "rockskin",
			Parent = "client",
			FullName = "client.rockskin",
			Client = true,
			Saved = true,
			ClientInfo = true,
			Variable = true,
			GetOveride = (() => ConVar.Client.RockSkin.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Client.RockSkin = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "sv",
			Parent = "client",
			FullName = "client.sv",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.Client.sv(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "clear",
			Parent = "console",
			FullName = "console.clear",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Console.clear(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "copy",
			Parent = "console",
			FullName = "console.copy",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Console.copy(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debug",
			Parent = "culling",
			FullName = "culling.debug",
			ClientAdmin = true,
			Client = true,
			Description = "Debug occludees 0=off, 1=dynamic, 2=static, 4=grid, 7=all (green:visible, red:culled)",
			Variable = true,
			GetOveride = (() => Culling.debug.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.debug = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "entitymaxdist",
			Parent = "culling",
			FullName = "culling.entitymaxdist",
			Client = true,
			Saved = true,
			Description = "Maximum distance to show any players in meters",
			Variable = true,
			GetOveride = (() => Culling.entityMaxDist.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.entityMaxDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "entityminanimatorculldist",
			Parent = "culling",
			FullName = "culling.entityminanimatorculldist",
			Client = true,
			Saved = true,
			Description = "Minimum distance at which we start disabling animators for entities",
			Variable = true,
			GetOveride = (() => Culling.entityMinAnimatorCullDist.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.entityMinAnimatorCullDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "entityminculldist",
			Parent = "culling",
			FullName = "culling.entityminculldist",
			Client = true,
			Saved = true,
			Description = "Entity of any kind will always be visible closer than this",
			Variable = true,
			GetOveride = (() => Culling.entityMinCullDist.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.entityMinCullDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "entityminshadowculldist",
			Parent = "culling",
			FullName = "culling.entityminshadowculldist",
			Client = true,
			Saved = true,
			Description = "Minimum distance at which we start disabling shadows for entities",
			Variable = true,
			GetOveride = (() => Culling.entityMinShadowCullDist.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.entityMinShadowCullDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "entityupdaterate",
			Parent = "culling",
			FullName = "culling.entityupdaterate",
			Client = true,
			Saved = true,
			Description = "How many times per second to check for visibility",
			Variable = true,
			GetOveride = (() => Culling.entityUpdateRate.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.entityUpdateRate = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "env",
			Parent = "culling",
			FullName = "culling.env",
			Client = true,
			Saved = true,
			Description = "Enable environment culling",
			Variable = true,
			GetOveride = (() => Culling.env.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.env = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "envmindist",
			Parent = "culling",
			FullName = "culling.envmindist",
			Client = true,
			Saved = true,
			Description = "Minimum distance in meters to start culling environment",
			Variable = true,
			GetOveride = (() => Culling.envMinDist.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.envMinDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "nodatavisible",
			Parent = "culling",
			FullName = "culling.nodatavisible",
			Client = true,
			Description = "Default visibility for unknown data during reprojection; e.g. rotating camera fast.",
			Variable = true,
			GetOveride = (() => Culling.noDataVisible.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.noDataVisible = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "safemode",
			Parent = "culling",
			FullName = "culling.safemode",
			Client = true,
			Saved = true,
			Description = "Culling safe mode; VERY SLOW and LEAKY... for debugging purposes only",
			Variable = true,
			GetOveride = (() => Culling.safeMode.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.safeMode = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "toggle",
			Parent = "culling",
			FullName = "culling.toggle",
			Client = true,
			Description = "Enable/Disable occlusion culling",
			Variable = true,
			GetOveride = (() => Culling.toggle.ToString()),
			SetOveride = delegate(string str)
			{
				Culling.toggle = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "export",
			Parent = "data",
			FullName = "data.export",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Data.export(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "arrow",
			Parent = "ddraw",
			FullName = "ddraw.arrow",
			ClientAdmin = true,
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.DDraw.arrow(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "box",
			Parent = "ddraw",
			FullName = "ddraw.box",
			ClientAdmin = true,
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.DDraw.box(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "line",
			Parent = "ddraw",
			FullName = "ddraw.line",
			ClientAdmin = true,
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.DDraw.line(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "sphere",
			Parent = "ddraw",
			FullName = "ddraw.sphere",
			ClientAdmin = true,
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.DDraw.sphere(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "text",
			Parent = "ddraw",
			FullName = "ddraw.text",
			ClientAdmin = true,
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.DDraw.text(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ambientvolumes",
			Parent = "debug",
			FullName = "debug.ambientvolumes",
			Client = true,
			Description = "Whether or not to update ambient light from environment volumes",
			Variable = true,
			GetOveride = (() => Debugging.ambientvolumes.ToString()),
			SetOveride = delegate(string str)
			{
				Debugging.ambientvolumes = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "callbacks",
			Parent = "debug",
			FullName = "debug.callbacks",
			Client = true,
			Variable = true,
			GetOveride = (() => Debugging.callbacks.ToString()),
			SetOveride = delegate(string str)
			{
				Debugging.callbacks = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "checktriggers",
			Parent = "debug",
			FullName = "debug.checktriggers",
			Client = true,
			Variable = true,
			GetOveride = (() => Debugging.checktriggers.ToString()),
			SetOveride = delegate(string str)
			{
				Debugging.checktriggers = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debugcamera",
			Parent = "debug",
			FullName = "debug.debugcamera",
			Client = true,
			Description = "Enable debug camera mode",
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.debugcamera(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debugcamera_unfreeze",
			Parent = "debug",
			FullName = "debug.debugcamera_unfreeze",
			Client = true,
			Description = "Unfreeze the player when in debug camera mode",
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.debugcamera_unfreeze(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "drawcolliders",
			Parent = "debug",
			FullName = "debug.drawcolliders",
			Client = true,
			Description = "Draw colliders",
			Variable = true,
			GetOveride = (() => Debugging.drawcolliders.ToString()),
			SetOveride = delegate(string str)
			{
				Debugging.drawcolliders = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "log",
			Parent = "debug",
			FullName = "debug.log",
			Client = true,
			Variable = true,
			GetOveride = (() => Debugging.log.ToString()),
			SetOveride = delegate(string str)
			{
				Debugging.log = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "lookingat",
			Parent = "debug",
			FullName = "debug.lookingat",
			Client = true,
			Description = "Print info about what the player is currently looking at",
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.lookingat(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "lookingat_debug",
			Parent = "debug",
			FullName = "debug.lookingat_debug",
			Client = true,
			Description = "Enable debug mode on the entity we're currently looking at",
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.lookingat_debug(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "morphcachemem",
			Parent = "debug",
			FullName = "debug.morphcachemem",
			Client = true,
			Description = "Print the current morph cache memory footprint.",
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.morphCacheMem(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "noclip",
			Parent = "debug",
			FullName = "debug.noclip",
			Client = true,
			Description = "Toggle admin no clipping",
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.noclip(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "oninventorychanged",
			Parent = "debug",
			FullName = "debug.oninventorychanged",
			Client = true,
			Variable = true,
			GetOveride = (() => Debugging.oninventorychanged.ToString()),
			SetOveride = delegate(string str)
			{
				Debugging.oninventorychanged = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "renderinfo",
			Parent = "debug",
			FullName = "debug.renderinfo",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.renderinfo(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "skyreflection",
			Parent = "debug",
			FullName = "debug.skyreflection",
			Client = true,
			Description = "Whether or not to update the sky reflection probe",
			Variable = true,
			GetOveride = (() => Debugging.skyreflection.ToString()),
			SetOveride = delegate(string str)
			{
				Debugging.skyreflection = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "stall",
			Parent = "debug",
			FullName = "debug.stall",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Debugging.stall(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "cache",
			Parent = "decal",
			FullName = "decal.cache",
			Client = true,
			Variable = true,
			GetOveride = (() => Decal.cache.ToString()),
			SetOveride = delegate(string str)
			{
				Decal.cache = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "capacity",
			Parent = "decal",
			FullName = "decal.capacity",
			Client = true,
			Variable = true,
			GetOveride = (() => Decal.capacity.ToString()),
			SetOveride = delegate(string str)
			{
				Decal.capacity = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "clear",
			Parent = "decal",
			FullName = "decal.clear",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Decal.clear(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "instancing",
			Parent = "decal",
			FullName = "decal.instancing",
			Client = true,
			Variable = true,
			GetOveride = (() => Decal.instancing.ToString()),
			SetOveride = delegate(string str)
			{
				Decal.instancing = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "limit",
			Parent = "decal",
			FullName = "decal.limit",
			Client = true,
			Variable = true,
			GetOveride = (() => Decal.limit.ToString()),
			SetOveride = delegate(string str)
			{
				Decal.limit = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "decor",
			FullName = "decor.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Decor.quality.ToString()),
			SetOveride = delegate(string str)
			{
				Decor.quality = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "play",
			Parent = "demo",
			FullName = "demo.play",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = Demo.play(arg.GetString(0, ""));
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "record",
			Parent = "demo",
			FullName = "demo.record",
			ClientAdmin = true,
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = Demo.record(arg.GetString(0, ""));
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "stop",
			Parent = "demo",
			FullName = "demo.stop",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = Demo.stop();
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "timedemo",
			Parent = "demo",
			FullName = "demo.timedemo",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = Demo.timedemo(arg.GetString(0, ""));
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "timescale",
			Parent = "demo",
			FullName = "demo.timescale",
			Client = true,
			Variable = true,
			GetOveride = (() => Demo.timescale.ToString()),
			SetOveride = delegate(string str)
			{
				Demo.timescale = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "antialiasing",
			Parent = "effects",
			FullName = "effects.antialiasing",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.antialiasing.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.antialiasing = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ao",
			Parent = "effects",
			FullName = "effects.ao",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.ao.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.ao = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "bloom",
			Parent = "effects",
			FullName = "effects.bloom",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.bloom.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.bloom = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "footsteps",
			Parent = "effects",
			FullName = "effects.footsteps",
			Client = true,
			Variable = true,
			GetOveride = (() => Effects.footsteps.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.footsteps = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "lensdirt",
			Parent = "effects",
			FullName = "effects.lensdirt",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.lensdirt.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.lensdirt = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxgibs",
			Parent = "effects",
			FullName = "effects.maxgibs",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.maxgibs.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.maxgibs = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "motionblur",
			Parent = "effects",
			FullName = "effects.motionblur",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.motionblur.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.motionblur = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "otherplayerslightflares",
			Parent = "effects",
			FullName = "effects.otherplayerslightflares",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.otherplayerslightflares.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.otherplayerslightflares = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "shafts",
			Parent = "effects",
			FullName = "effects.shafts",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.shafts.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.shafts = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "sharpen",
			Parent = "effects",
			FullName = "effects.sharpen",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.sharpen.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.sharpen = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "showoutlines",
			Parent = "effects",
			FullName = "effects.showoutlines",
			Client = true,
			Saved = true,
			Description = "Show outlines of objects when applicable i.e. dropped items",
			Variable = true,
			GetOveride = (() => Effects.showoutlines.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.showoutlines = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "vignet",
			Parent = "effects",
			FullName = "effects.vignet",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Effects.vignet.ToString()),
			SetOveride = delegate(string str)
			{
				Effects.vignet = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "spawn",
			Parent = "entity",
			FullName = "entity.spawn",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.clspawn(arg.GetString(0, ""));
			}
		},
		new ConsoleSystem.Command
		{
			Name = "spawnitem",
			Parent = "entity",
			FullName = "entity.spawnitem",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.clspawnitem(arg.GetString(0, ""));
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debug_lookat",
			Parent = "entity",
			FullName = "entity.debug_lookat",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.debug_lookat(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find_entity",
			Parent = "entity",
			FullName = "entity.find_entity",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.find_entity(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find_group",
			Parent = "entity",
			FullName = "entity.find_group",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.find_group(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find_id",
			Parent = "entity",
			FullName = "entity.find_id",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.find_id(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find_parent",
			Parent = "entity",
			FullName = "entity.find_parent",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.find_parent(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find_radius",
			Parent = "entity",
			FullName = "entity.find_radius",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.find_radius(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find_self",
			Parent = "entity",
			FullName = "entity.find_self",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.find_self(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "find_status",
			Parent = "entity",
			FullName = "entity.find_status",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Entity.find_status(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debug",
			Parent = "file",
			FullName = "file.debug",
			Client = true,
			Variable = true,
			GetOveride = (() => FileConVar.debug.ToString()),
			SetOveride = delegate(string str)
			{
				FileConVar.debug = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "time",
			Parent = "file",
			FullName = "file.time",
			Client = true,
			Variable = true,
			GetOveride = (() => FileConVar.time.ToString()),
			SetOveride = delegate(string str)
			{
				FileConVar.time = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "graph",
			Parent = "fps",
			FullName = "fps.graph",
			Client = true,
			Variable = true,
			GetOveride = (() => FPS.graph.ToString()),
			SetOveride = delegate(string str)
			{
				FPS.graph = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "limit",
			Parent = "fps",
			FullName = "fps.limit",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => FPS.limit.ToString()),
			SetOveride = delegate(string str)
			{
				FPS.limit = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "buffer",
			Parent = "gc",
			FullName = "gc.buffer",
			Client = true,
			Variable = true,
			GetOveride = (() => GC.buffer.ToString()),
			SetOveride = delegate(string str)
			{
				GC.buffer = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "collect",
			Parent = "gc",
			FullName = "gc.collect",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				GC.collect();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "unload",
			Parent = "gc",
			FullName = "gc.unload",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				GC.unload();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "censornudity",
			Parent = "global",
			FullName = "global.censornudity",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Global.censornudity.ToString()),
			SetOveride = delegate(string str)
			{
				Global.censornudity = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "censorsigns",
			Parent = "global",
			FullName = "global.censorsigns",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Global.censorsigns.ToString()),
			SetOveride = delegate(string str)
			{
				Global.censorsigns = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "colliders",
			Parent = "global",
			FullName = "global.colliders",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.colliders(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "developer",
			Parent = "global",
			FullName = "global.developer",
			Client = true,
			Variable = true,
			GetOveride = (() => Global.developer.ToString()),
			SetOveride = delegate(string str)
			{
				Global.developer = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "error",
			Parent = "global",
			FullName = "global.error",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.error(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "exec",
			Parent = "global",
			FullName = "global.exec",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.exec(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "free",
			Parent = "global",
			FullName = "global.free",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.free(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "god",
			Parent = "global",
			FullName = "global.god",
			Client = true,
			Saved = true,
			Description = "If you're an admin this will enable god mode",
			ClientInfo = true,
			Variable = true,
			GetOveride = (() => Global.god.ToString()),
			SetOveride = delegate(string str)
			{
				Global.god = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "language",
			Parent = "global",
			FullName = "global.language",
			Client = true,
			Saved = true,
			ClientInfo = true,
			Variable = true,
			GetOveride = (() => Global.language.ToString()),
			SetOveride = delegate(string str)
			{
				Global.language = str;
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxthreads",
			Parent = "global",
			FullName = "global.maxthreads",
			Client = true,
			Variable = true,
			GetOveride = (() => Global.maxthreads.ToString()),
			SetOveride = delegate(string str)
			{
				Global.maxthreads = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "objects",
			Parent = "global",
			FullName = "global.objects",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.objects(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "perf",
			Parent = "global",
			FullName = "global.perf",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Global.perf.ToString()),
			SetOveride = delegate(string str)
			{
				Global.perf = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "queue",
			Parent = "global",
			FullName = "global.queue",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.queue(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quit",
			Parent = "global",
			FullName = "global.quit",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.quit(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "readcfg",
			Parent = "global",
			FullName = "global.readcfg",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.readcfg(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "specnet",
			Parent = "global",
			FullName = "global.specnet",
			Client = true,
			Saved = true,
			Description = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.",
			ClientInfo = true,
			Variable = true,
			GetOveride = (() => Global.specnet.ToString()),
			SetOveride = delegate(string str)
			{
				Global.specnet = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "status_cl",
			Parent = "global",
			FullName = "global.status_cl",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.status_cl(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "streamermode",
			Parent = "global",
			FullName = "global.streamermode",
			Client = true,
			Saved = true,
			ClientInfo = true,
			Variable = true,
			GetOveride = (() => Global.streamermode.ToString()),
			SetOveride = delegate(string str)
			{
				Global.streamermode = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "subscriptions",
			Parent = "global",
			FullName = "global.subscriptions",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.subscriptions(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "sysinfo",
			Parent = "global",
			FullName = "global.sysinfo",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.sysinfo(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "sysuid",
			Parent = "global",
			FullName = "global.sysuid",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.sysuid(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "textures",
			Parent = "global",
			FullName = "global.textures",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.textures(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "timewarning",
			Parent = "global",
			FullName = "global.timewarning",
			Client = true,
			Variable = true,
			GetOveride = (() => Global.timewarning.ToString()),
			SetOveride = delegate(string str)
			{
				Global.timewarning = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "version",
			Parent = "global",
			FullName = "global.version",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.version(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "writecfg",
			Parent = "global",
			FullName = "global.writecfg",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Global.writecfg();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "af",
			Parent = "graphics",
			FullName = "graphics.af",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.af.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.af = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "branding",
			Parent = "graphics",
			FullName = "graphics.branding",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.branding.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.branding = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "chat",
			Parent = "graphics",
			FullName = "graphics.chat",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.chat.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.chat = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "compass",
			Parent = "graphics",
			FullName = "graphics.compass",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.compass.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.compass = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "contactshadows",
			Parent = "graphics",
			FullName = "graphics.contactshadows",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.contactshadows.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.contactshadows = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "dof",
			Parent = "graphics",
			FullName = "graphics.dof",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.dof.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.dof = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "dof_aper",
			Parent = "graphics",
			FullName = "graphics.dof_aper",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.dof_aper.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.dof_aper = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "dof_blur",
			Parent = "graphics",
			FullName = "graphics.dof_blur",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.dof_blur.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.dof_blur = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "drawdistance",
			Parent = "graphics",
			FullName = "graphics.drawdistance",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.drawdistance.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.drawdistance = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "fov",
			Parent = "graphics",
			FullName = "graphics.fov",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.fov.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.fov = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "grassshadows",
			Parent = "graphics",
			FullName = "graphics.grassshadows",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.grassshadows.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.grassshadows = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "hud",
			Parent = "graphics",
			FullName = "graphics.hud",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.hud.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.hud = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "impostorshadows",
			Parent = "graphics",
			FullName = "graphics.impostorshadows",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.impostorshadows.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.impostorshadows = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "itemskins",
			Parent = "graphics",
			FullName = "graphics.itemskins",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.itemskins.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.itemskins = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "itemskintimeout",
			Parent = "graphics",
			FullName = "graphics.itemskintimeout",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.itemskintimeout.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.itemskintimeout = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "lodbias",
			Parent = "graphics",
			FullName = "graphics.lodbias",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.lodbias.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.lodbias = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "parallax",
			Parent = "graphics",
			FullName = "graphics.parallax",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.parallax.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.parallax = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "graphics",
			FullName = "graphics.quality",
			Client = true,
			Description = "The currently selected quality level",
			Variable = true,
			GetOveride = (() => ConVar.Graphics.quality.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.quality = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "shaderlod",
			Parent = "graphics",
			FullName = "graphics.shaderlod",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.shaderlod.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.shaderlod = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "shadowcascades",
			Parent = "graphics",
			FullName = "graphics.shadowcascades",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.shadowcascades.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.shadowcascades = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "shadowdistance",
			Parent = "graphics",
			FullName = "graphics.shadowdistance",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.shadowdistance.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.shadowdistance = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "shadowlights",
			Parent = "graphics",
			FullName = "graphics.shadowlights",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.shadowlights.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.shadowlights = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "shadowmode",
			Parent = "graphics",
			FullName = "graphics.shadowmode",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.shadowmode.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.shadowmode = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "shadowquality",
			Parent = "graphics",
			FullName = "graphics.shadowquality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.shadowquality.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.shadowquality = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "showtexeldensity",
			Parent = "graphics",
			FullName = "graphics.showtexeldensity",
			ClientAdmin = true,
			Client = true,
			Description = "Texture density visualization tool: 0=disabled, 1=256, 2=512, 3=1024 texel/meter",
			Variable = true,
			GetOveride = (() => ConVar.Graphics.showtexeldensity.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.showtexeldensity = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "uiscale",
			Parent = "graphics",
			FullName = "graphics.uiscale",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Graphics.uiscale.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Graphics.uiscale = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "displace",
			Parent = "grass",
			FullName = "grass.displace",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Grass.displace.ToString()),
			SetOveride = delegate(string str)
			{
				Grass.displace = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "grass",
			FullName = "grass.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Grass.quality.ToString()),
			SetOveride = delegate(string str)
			{
				Grass.quality = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "notification_level",
			Parent = "hitnotify",
			FullName = "hitnotify.notification_level",
			Client = true,
			Variable = true,
			GetOveride = (() => hitnotify.notification_level.ToString()),
			SetOveride = delegate(string str)
			{
				hitnotify.notification_level = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "autocrouch",
			Parent = "input",
			FullName = "input.autocrouch",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Input.autocrouch.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Input.autocrouch = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "bind",
			Parent = "input",
			FullName = "input.bind",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				string text = ConVar.Input.bind(arg);
				arg.ReplyWithObject(text);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "clearbinds",
			Parent = "input",
			FullName = "input.clearbinds",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.Input.clearbinds(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "flipy",
			Parent = "input",
			FullName = "input.flipy",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Input.flipy.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Input.flipy = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "holdtime",
			Parent = "input",
			FullName = "input.holdtime",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Input.holdtime.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Input.holdtime = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "sensitivity",
			Parent = "input",
			FullName = "input.sensitivity",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Input.sensitivity.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Input.sensitivity = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "vehicle_flipy",
			Parent = "input",
			FullName = "input.vehicle_flipy",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Input.vehicle_flipy.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Input.vehicle_flipy = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "vehicle_sensitivity",
			Parent = "input",
			FullName = "input.vehicle_sensitivity",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Input.vehicle_sensitivity.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Input.vehicle_sensitivity = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "examineheld",
			Parent = "inventory",
			FullName = "inventory.examineheld",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Inventory.examineheld();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "listcraftcounts",
			Parent = "inventory",
			FullName = "inventory.listcraftcounts",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Inventory.ListCraftCounts();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "resetcraftcounts",
			Parent = "inventory",
			FullName = "inventory.resetcraftcounts",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Inventory.ResetCraftCounts();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "toggle",
			Parent = "inventory",
			FullName = "inventory.toggle",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Inventory.toggle();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "togglecrafting",
			Parent = "inventory",
			FullName = "inventory.togglecrafting",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Inventory.togglecrafting();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "culling",
			Parent = "layer",
			FullName = "layer.culling",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Layer.culling(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "hide",
			Parent = "layer",
			FullName = "layer.hide",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Layer.hide(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "show",
			Parent = "layer",
			FullName = "layer.show",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Layer.show(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "toggle",
			Parent = "layer",
			FullName = "layer.toggle",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Layer.toggle(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debug",
			Parent = "lerp",
			FullName = "lerp.debug",
			Client = true,
			Variable = true,
			GetOveride = (() => Lerp.debug.ToString()),
			SetOveride = delegate(string str)
			{
				Lerp.debug = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "lerp",
			FullName = "lerp.enabled",
			Client = true,
			Description = "Enables interpolation and extrapolation of network positions",
			Variable = true,
			GetOveride = (() => Lerp.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				Lerp.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "extrapolation",
			Parent = "lerp",
			FullName = "lerp.extrapolation",
			Client = true,
			Description = "How many seconds ahead to lerp",
			Variable = true,
			GetOveride = (() => Lerp.extrapolation.ToString()),
			SetOveride = delegate(string str)
			{
				Lerp.extrapolation = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "interpolation",
			Parent = "lerp",
			FullName = "lerp.interpolation",
			Client = true,
			Description = "How many seconds behind to lerp",
			Variable = true,
			GetOveride = (() => Lerp.interpolation.ToString()),
			SetOveride = delegate(string str)
			{
				Lerp.interpolation = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "smoothing",
			Parent = "lerp",
			FullName = "lerp.smoothing",
			Client = true,
			Description = "How many seconds to smoothen velocity",
			Variable = true,
			GetOveride = (() => Lerp.smoothing.ToString()),
			SetOveride = delegate(string str)
			{
				Lerp.smoothing = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "printmanifest",
			Parent = "manifest",
			FullName = "manifest.printmanifest",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				object obj = Manifest.PrintManifest();
				arg.ReplyWithObject(obj);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "printmanifestraw",
			Parent = "manifest",
			FullName = "manifest.printmanifestraw",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				object obj = Manifest.PrintManifestRaw();
				arg.ReplyWithObject(obj);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "mesh",
			FullName = "mesh.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => ConVar.Mesh.quality.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Mesh.quality = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "music",
			FullName = "music.enabled",
			Client = true,
			Variable = true,
			GetOveride = (() => Music.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				Music.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "info",
			Parent = "music",
			FullName = "music.info",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Music.info(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "songgapmax",
			Parent = "music",
			FullName = "music.songgapmax",
			Client = true,
			Variable = true,
			GetOveride = (() => Music.songGapMax.ToString()),
			SetOveride = delegate(string str)
			{
				Music.songGapMax = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "songgapmin",
			Parent = "music",
			FullName = "music.songgapmin",
			Client = true,
			Variable = true,
			GetOveride = (() => Music.songGapMin.ToString()),
			SetOveride = delegate(string str)
			{
				Music.songGapMin = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debug",
			Parent = "net",
			FullName = "net.debug",
			Client = true,
			Variable = true,
			GetOveride = (() => Net.debug.ToString()),
			SetOveride = delegate(string str)
			{
				Net.debug = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "netgraph",
			FullName = "netgraph.enabled",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Netgraph.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				Netgraph.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "entityfilter",
			Parent = "netgraph",
			FullName = "netgraph.entityfilter",
			Client = true,
			Variable = true,
			GetOveride = (() => Netgraph.entityfilter.ToString()),
			SetOveride = delegate(string str)
			{
				Netgraph.entityfilter = str;
			}
		},
		new ConsoleSystem.Command
		{
			Name = "typefilter",
			Parent = "netgraph",
			FullName = "netgraph.typefilter",
			Client = true,
			Variable = true,
			GetOveride = (() => Netgraph.typefilter.ToString()),
			SetOveride = delegate(string str)
			{
				Netgraph.typefilter = str;
			}
		},
		new ConsoleSystem.Command
		{
			Name = "updatespeed",
			Parent = "netgraph",
			FullName = "netgraph.updatespeed",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Netgraph.updatespeed.ToString()),
			SetOveride = delegate(string str)
			{
				Netgraph.updatespeed = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "craft_add",
			Parent = "note",
			FullName = "note.craft_add",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Note.craft_add(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "craft_done",
			Parent = "note",
			FullName = "note.craft_done",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Note.craft_done(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "craft_start",
			Parent = "note",
			FullName = "note.craft_start",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Note.craft_start(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "inv",
			Parent = "note",
			FullName = "note.inv",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Note.inv(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "particle",
			FullName = "particle.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Particle.quality.ToString()),
			SetOveride = delegate(string str)
			{
				Particle.quality = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "minsteps",
			Parent = "physics",
			FullName = "physics.minsteps",
			Client = true,
			Variable = true,
			GetOveride = (() => Physics.minsteps.ToString()),
			SetOveride = delegate(string str)
			{
				Physics.minsteps = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ragdollmode",
			Parent = "physics",
			FullName = "physics.ragdollmode",
			Client = true,
			Description = "The collision detection mode that ragdolls should use",
			Variable = true,
			GetOveride = (() => Physics.ragdollmode.ToString()),
			SetOveride = delegate(string str)
			{
				Physics.ragdollmode = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "steps",
			Parent = "physics",
			FullName = "physics.steps",
			Client = true,
			Variable = true,
			GetOveride = (() => Physics.steps.ToString()),
			SetOveride = delegate(string str)
			{
				Physics.steps = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "footik",
			Parent = "player",
			FullName = "player.footik",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Player.footik.ToString()),
			SetOveride = delegate(string str)
			{
				Player.footik = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "footikdistance",
			Parent = "player",
			FullName = "player.footikdistance",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Player.footikdistance.ToString()),
			SetOveride = delegate(string str)
			{
				Player.footikdistance = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "recoilcomp",
			Parent = "player",
			FullName = "player.recoilcomp",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Player.recoilcomp.ToString()),
			SetOveride = delegate(string str)
			{
				Player.recoilcomp = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debug",
			Parent = "playercull",
			FullName = "playercull.debug",
			ClientAdmin = true,
			Client = true,
			Variable = true,
			GetOveride = (() => PlayerCull.debug.ToString()),
			SetOveride = delegate(string str)
			{
				PlayerCull.debug = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "playercull",
			FullName = "playercull.enabled",
			Client = true,
			Saved = true,
			Description = "Enable/Disable player culling entirely (always render even when hidden)",
			Variable = true,
			GetOveride = (() => PlayerCull.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				PlayerCull.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxplayerdist",
			Parent = "playercull",
			FullName = "playercull.maxplayerdist",
			Client = true,
			Saved = true,
			Description = "Maximum distance to show any players in meters",
			Variable = true,
			GetOveride = (() => PlayerCull.maxPlayerDist.ToString()),
			SetOveride = delegate(string str)
			{
				PlayerCull.maxPlayerDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "maxsleeperdist",
			Parent = "playercull",
			FullName = "playercull.maxsleeperdist",
			Client = true,
			Saved = true,
			Description = "Maximum distance to show sleepers in meters",
			Variable = true,
			GetOveride = (() => PlayerCull.maxSleeperDist.ToString()),
			SetOveride = delegate(string str)
			{
				PlayerCull.maxSleeperDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "minculldist",
			Parent = "playercull",
			FullName = "playercull.minculldist",
			Client = true,
			Saved = true,
			Description = "Players of any kind will always be visible closer than this",
			Variable = true,
			GetOveride = (() => PlayerCull.minCullDist.ToString()),
			SetOveride = delegate(string str)
			{
				PlayerCull.minCullDist = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "updaterate",
			Parent = "playercull",
			FullName = "playercull.updaterate",
			Client = true,
			Saved = true,
			Description = "How many times per second to check for visibility",
			Variable = true,
			GetOveride = (() => PlayerCull.updateRate.ToString()),
			SetOveride = delegate(string str)
			{
				PlayerCull.updateRate = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "visquality",
			Parent = "playercull",
			FullName = "playercull.visquality",
			Client = true,
			Saved = true,
			Description = "Quality of Vis : 0 = Chest check, 1 = Chest+Head, 2 = Chest+Head+Arms, 3 = Chest+Head+Arms+Feet",
			Variable = true,
			GetOveride = (() => PlayerCull.visQuality.ToString()),
			SetOveride = delegate(string str)
			{
				PlayerCull.visQuality = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "clear_assets",
			Parent = "pool",
			FullName = "pool.clear_assets",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.clear_assets(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "clear_memory",
			Parent = "pool",
			FullName = "pool.clear_memory",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.clear_memory(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "clear_prefabs",
			Parent = "pool",
			FullName = "pool.clear_prefabs",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.clear_prefabs(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "debug",
			Parent = "pool",
			FullName = "pool.debug",
			Client = true,
			Variable = true,
			GetOveride = (() => Pool.debug.ToString()),
			SetOveride = delegate(string str)
			{
				Pool.debug = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "pool",
			FullName = "pool.enabled",
			Client = true,
			Variable = true,
			GetOveride = (() => Pool.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				Pool.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "export_prefabs",
			Parent = "pool",
			FullName = "pool.export_prefabs",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.export_prefabs(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "mode",
			Parent = "pool",
			FullName = "pool.mode",
			Client = true,
			Variable = true,
			GetOveride = (() => Pool.mode.ToString()),
			SetOveride = delegate(string str)
			{
				Pool.mode = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "print_assets",
			Parent = "pool",
			FullName = "pool.print_assets",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.print_assets(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "print_memory",
			Parent = "pool",
			FullName = "pool.print_memory",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.print_memory(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "print_prefabs",
			Parent = "pool",
			FullName = "pool.print_prefabs",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.print_prefabs(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "print_rigcache",
			Parent = "pool",
			FullName = "pool.print_rigcache",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Pool.print_rigcache(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "start",
			Parent = "profile",
			FullName = "profile.start",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.Profile.start(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "stop",
			Parent = "profile",
			FullName = "profile.stop",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.Profile.stop(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "reflection",
			FullName = "reflection.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Reflection.quality.ToString()),
			SetOveride = delegate(string str)
			{
				Reflection.quality = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "sss",
			FullName = "sss.enabled",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => SSS.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				SSS.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "halfres",
			Parent = "sss",
			FullName = "sss.halfres",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => SSS.halfres.ToString()),
			SetOveride = delegate(string str)
			{
				SSS.halfres = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "sss",
			FullName = "sss.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => SSS.quality.ToString()),
			SetOveride = delegate(string str)
			{
				SSS.quality = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "scale",
			Parent = "sss",
			FullName = "sss.scale",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => SSS.scale.ToString()),
			SetOveride = delegate(string str)
			{
				SSS.scale = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "achievements",
			Parent = "steam",
			FullName = "steam.achievements",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				object obj = Steam.achievements();
				arg.ReplyWithObject(obj);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "resetstats",
			Parent = "steam",
			FullName = "steam.resetstats",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Steam.ResetStats();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "stats",
			Parent = "steam",
			FullName = "steam.stats",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				object obj = Steam.stats();
				arg.ReplyWithObject(obj);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "terrain",
			FullName = "terrain.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Terrain.quality.ToString()),
			SetOveride = delegate(string str)
			{
				Terrain.quality = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "recenter",
			Parent = "trackir",
			FullName = "trackir.recenter",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				TrackIR.recenter();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "refresh",
			Parent = "trackir",
			FullName = "trackir.refresh",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				TrackIR.refresh();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "meshes",
			Parent = "tree",
			FullName = "tree.meshes",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Tree.meshes.ToString()),
			SetOveride = delegate(string str)
			{
				Tree.meshes = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "tree",
			FullName = "tree.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Tree.quality.ToString()),
			SetOveride = delegate(string str)
			{
				Tree.quality = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "attack",
			Parent = "vis",
			FullName = "vis.attack",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Vis.attack.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Vis.attack = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "lerp",
			Parent = "vis",
			FullName = "vis.lerp",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Vis.lerp.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Vis.lerp = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "protection",
			Parent = "vis",
			FullName = "vis.protection",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.Vis.protection.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.Vis.protection = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "loopback",
			Parent = "voice",
			FullName = "voice.loopback",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Voice.loopback.ToString()),
			SetOveride = delegate(string str)
			{
				Voice.loopback = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ui_cut",
			Parent = "voice",
			FullName = "voice.ui_cut",
			Client = true,
			Variable = true,
			GetOveride = (() => Voice.ui_cut.ToString()),
			SetOveride = delegate(string str)
			{
				Voice.ui_cut = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ui_lerp",
			Parent = "voice",
			FullName = "voice.ui_lerp",
			Client = true,
			Variable = true,
			GetOveride = (() => Voice.ui_lerp.ToString()),
			SetOveride = delegate(string str)
			{
				Voice.ui_lerp = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ui_samples",
			Parent = "voice",
			FullName = "voice.ui_samples",
			Client = true,
			Variable = true,
			GetOveride = (() => Voice.ui_samples.ToString()),
			SetOveride = delegate(string str)
			{
				Voice.ui_samples = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "ui_scale",
			Parent = "voice",
			FullName = "voice.ui_scale",
			Client = true,
			Variable = true,
			GetOveride = (() => Voice.ui_scale.ToString()),
			SetOveride = delegate(string str)
			{
				Voice.ui_scale = StringExtensions.ToFloat(str, 0f);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "quality",
			Parent = "water",
			FullName = "water.quality",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Water.quality.ToString()),
			SetOveride = delegate(string str)
			{
				Water.quality = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "reflections",
			Parent = "water",
			FullName = "water.reflections",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => Water.reflections.ToString()),
			SetOveride = delegate(string str)
			{
				Water.reflections = StringExtensions.ToInt(str, 0);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "report",
			Parent = "weather",
			FullName = "weather.report",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Weather.report(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "print_loaded_skins",
			Parent = "workshop",
			FullName = "workshop.print_loaded_skins",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				Workshop.print_loaded_skins(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "cache",
			Parent = "world",
			FullName = "world.cache",
			Client = true,
			Variable = true,
			GetOveride = (() => ConVar.World.cache.ToString()),
			SetOveride = delegate(string str)
			{
				ConVar.World.cache = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "monuments",
			Parent = "world",
			FullName = "world.monuments",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				ConVar.World.monuments(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "dump",
			Parent = "global",
			FullName = "global.dump",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				DiagnosticsConSys.dump(arg);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "hidegametip",
			Parent = "gametip",
			FullName = "gametip.hidegametip",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				GameTip.HideGameTip();
			}
		},
		new ConsoleSystem.Command
		{
			Name = "listgametips",
			Parent = "gametip",
			FullName = "gametip.listgametips",
			Client = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				BaseTip[] array = GameTip.ListGameTips();
				arg.ReplyWithObject(array);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "showgametip",
			Parent = "gametip",
			FullName = "gametip.showgametip",
			Client = true,
			AllowRunFromServer = true,
			Variable = false,
			Call = delegate(ConsoleSystem.Arg arg)
			{
				GameTip.ShowGameTip(arg.GetString(0, ""));
			}
		},
		new ConsoleSystem.Command
		{
			Name = "showgametips",
			Parent = "gametip",
			FullName = "gametip.showgametips",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => GameTip.showgametips.ToString()),
			SetOveride = delegate(string str)
			{
				GameTip.showgametips = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "enabled",
			Parent = "nametags",
			FullName = "nametags.enabled",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => nametags.enabled.ToString()),
			SetOveride = delegate(string str)
			{
				nametags.enabled = StringExtensions.ToBool(str);
			}
		},
		new ConsoleSystem.Command
		{
			Name = "forceoff",
			Parent = "strobelight",
			FullName = "strobelight.forceoff",
			Client = true,
			Saved = true,
			Variable = true,
			GetOveride = (() => StrobeLight.forceoff.ToString()),
			SetOveride = delegate(string str)
			{
				StrobeLight.forceoff = StringExtensions.ToBool(str);
			}
		}
	};
}
