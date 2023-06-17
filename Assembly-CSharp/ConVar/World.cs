using System;

namespace ConVar
{
	// Token: 0x0200088F RID: 2191
	[ConsoleSystem.Factory("world")]
	public class World : ConsoleSystem
	{
		// Token: 0x04002A3D RID: 10813
		[ClientVar]
		[ServerVar]
		public static bool cache = true;

		// Token: 0x06002F7E RID: 12158 RVA: 0x000E9F88 File Offset: 0x000E8188
		[ClientVar]
		public static void monuments(ConsoleSystem.Arg arg)
		{
			if (!TerrainMeta.Path)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("name");
			textTable.AddColumn("pos");
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				textTable.AddRow(new string[]
				{
					monumentInfo.Type.ToString(),
					monumentInfo.name,
					monumentInfo.transform.position.ToString()
				});
			}
			arg.ReplyWith(textTable.ToString());
		}
	}
}
