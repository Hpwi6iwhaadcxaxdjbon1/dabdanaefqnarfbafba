using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000856 RID: 2134
	[ConsoleSystem.Factory("entity")]
	public class Entity : ConsoleSystem
	{
		// Token: 0x06002E81 RID: 11905 RVA: 0x000E81AC File Offset: 0x000E63AC
		[ClientVar(AllowRunFromServer = true)]
		public static void debug_lookat(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			if (LocalPlayer.Entity.lookingAtEntity)
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Client, "entity.debug_toggle", new object[]
				{
					LocalPlayer.Entity.lookingAtEntity.net.ID
				});
			}
		}

		// Token: 0x06002E82 RID: 11906 RVA: 0x000E8218 File Offset: 0x000E6418
		private static TextTable GetEntityTable(Func<Entity.EntityInfo, bool> filter)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("entity");
			textTable.AddColumn("group");
			textTable.AddColumn("parent");
			textTable.AddColumn("name");
			textTable.AddColumn("position");
			textTable.AddColumn("local");
			textTable.AddColumn("rotation");
			textTable.AddColumn("local");
			textTable.AddColumn("status");
			textTable.AddColumn("invokes");
			foreach (BaseNetworkable baseNetworkable in BaseNetworkable.clientEntities)
			{
				if (!(baseNetworkable == null))
				{
					Entity.EntityInfo entityInfo = new Entity.EntityInfo(baseNetworkable);
					if (filter.Invoke(entityInfo))
					{
						textTable.AddRow(new string[]
						{
							"cl",
							entityInfo.entityID.ToString(),
							entityInfo.groupID.ToString(),
							entityInfo.parentID.ToString(),
							entityInfo.entity.ShortPrefabName,
							entityInfo.entity.transform.position.ToString(),
							entityInfo.entity.transform.localPosition.ToString(),
							entityInfo.entity.transform.rotation.eulerAngles.ToString(),
							entityInfo.entity.transform.localRotation.eulerAngles.ToString(),
							entityInfo.status,
							entityInfo.entity.InvokeString()
						});
					}
				}
			}
			return textTable;
		}

		// Token: 0x06002E83 RID: 11907 RVA: 0x000E8414 File Offset: 0x000E6614
		[ServerVar]
		[ClientVar]
		public static void find_entity(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			string filter = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => string.IsNullOrEmpty(filter) || info.entity.PrefabName.Contains(filter));
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x06002E84 RID: 11908 RVA: 0x000E8474 File Offset: 0x000E6674
		[ServerVar]
		[ClientVar]
		public static void find_id(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			uint filter = args.GetUInt(0, 0U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x06002E85 RID: 11909 RVA: 0x000E84D0 File Offset: 0x000E66D0
		[ServerVar]
		[ClientVar]
		public static void find_group(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			uint filter = args.GetUInt(0, 0U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.groupID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x06002E86 RID: 11910 RVA: 0x000E852C File Offset: 0x000E672C
		[ServerVar]
		[ClientVar]
		public static void find_parent(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			uint filter = args.GetUInt(0, 0U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.parentID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x06002E87 RID: 11911 RVA: 0x000E8588 File Offset: 0x000E6788
		[ClientVar]
		[ServerVar]
		public static void find_status(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			string filter = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => string.IsNullOrEmpty(filter) || info.status.Contains(filter));
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x000E85E8 File Offset: 0x000E67E8
		[ClientVar]
		[ServerVar]
		public static void find_radius(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			BasePlayer player = LocalPlayer.Entity;
			if (player == null)
			{
				return;
			}
			uint filter = args.GetUInt(0, 10U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => Vector3.Distance(info.entity.transform.position, player.transform.position) <= filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x000E8660 File Offset: 0x000E6860
		[ClientVar]
		[ServerVar]
		public static void find_self(ConsoleSystem.Arg args)
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (!LocalPlayer.Entity.IsAdmin)
			{
				return;
			}
			BasePlayer entity = LocalPlayer.Entity;
			if (entity == null)
			{
				return;
			}
			if (entity.net == null)
			{
				return;
			}
			uint filter = entity.net.ID;
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x06002E8A RID: 11914 RVA: 0x000E86D8 File Offset: 0x000E68D8
		[ClientVar(Name = "spawn")]
		public static void clspawn(string name)
		{
			RaycastHit raycast = MainCamera.Raycast;
			if (raycast.collider != null)
			{
				ConsoleNetwork.ClientRunOnServer(string.Format("spawn \"{0}\" \"{1}\"", name, raycast.point.ToString()));
			}
		}

		// Token: 0x06002E8B RID: 11915 RVA: 0x000E8720 File Offset: 0x000E6920
		[ClientVar(Name = "spawnitem")]
		public static void clspawnitem(string name)
		{
			RaycastHit raycast = MainCamera.Raycast;
			if (raycast.collider != null)
			{
				ConsoleNetwork.ClientRunOnServer(string.Format("spawnitem \"{0}\" \"{1}\"", name, raycast.point.ToString()));
			}
		}

		// Token: 0x02000857 RID: 2135
		private struct EntityInfo
		{
			// Token: 0x04002975 RID: 10613
			public BaseNetworkable entity;

			// Token: 0x04002976 RID: 10614
			public uint entityID;

			// Token: 0x04002977 RID: 10615
			public uint groupID;

			// Token: 0x04002978 RID: 10616
			public uint parentID;

			// Token: 0x04002979 RID: 10617
			public string status;

			// Token: 0x06002E8D RID: 11917 RVA: 0x000E8768 File Offset: 0x000E6968
			public EntityInfo(BaseNetworkable src)
			{
				this.entity = src;
				BaseEntity baseEntity = this.entity as BaseEntity;
				BaseEntity x = (baseEntity != null) ? baseEntity.GetParentEntity() : null;
				this.entityID = ((this.entity != null && this.entity.net != null) ? this.entity.net.ID : 0U);
				this.groupID = ((this.entity != null && this.entity.net != null && this.entity.net.group != null) ? this.entity.net.group.ID : 0U);
				this.parentID = ((baseEntity != null) ? baseEntity.parentEntity.uid : 0U);
				if (!(baseEntity != null) || baseEntity.parentEntity.uid == 0U)
				{
					this.status = string.Empty;
					return;
				}
				if (x == null)
				{
					this.status = "orphan";
					return;
				}
				this.status = "child";
			}
		}
	}
}
