using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008C8 RID: 2248
	[DefaultExecutionOrder(-103)]
	public class AiManager : SingletonComponent<AiManager>, IServerComponent
	{
		// Token: 0x04002B2A RID: 11050
		[Header("Cover System")]
		[SerializeField]
		public bool UseCover = true;

		// Token: 0x04002B2B RID: 11051
		public float CoverPointVolumeCellSize = 20f;

		// Token: 0x04002B2C RID: 11052
		public float CoverPointVolumeCellHeight = 8f;

		// Token: 0x04002B2D RID: 11053
		public float CoverPointRayLength = 1f;

		// Token: 0x04002B2E RID: 11054
		public CoverPointVolume cpvPrefab;

		// Token: 0x04002B2F RID: 11055
		[SerializeField]
		public LayerMask DynamicCoverPointVolumeLayerMask;

		// Token: 0x04002B30 RID: 11056
		private WorldSpaceGrid<CoverPointVolume> coverPointVolumeGrid;

		// Token: 0x06003057 RID: 12375 RVA: 0x000EC52C File Offset: 0x000EA72C
		internal void OnEnableCover()
		{
			if (this.coverPointVolumeGrid == null)
			{
				Vector3 size = TerrainMeta.Size;
				this.coverPointVolumeGrid = new WorldSpaceGrid<CoverPointVolume>(size.x, this.CoverPointVolumeCellSize);
			}
		}

		// Token: 0x06003058 RID: 12376 RVA: 0x000EC560 File Offset: 0x000EA760
		internal void OnDisableCover()
		{
			if (this.coverPointVolumeGrid == null || this.coverPointVolumeGrid.Cells == null)
			{
				return;
			}
			for (int i = 0; i < this.coverPointVolumeGrid.Cells.Length; i++)
			{
				Object.Destroy(this.coverPointVolumeGrid.Cells[i]);
			}
		}

		// Token: 0x06003059 RID: 12377 RVA: 0x000EC5B0 File Offset: 0x000EA7B0
		public CoverPointVolume GetCoverVolumeContaining(Vector3 point)
		{
			if (this.coverPointVolumeGrid == null)
			{
				return null;
			}
			Vector2i vector2i = this.coverPointVolumeGrid.WorldToGridCoords(point);
			return this.coverPointVolumeGrid[vector2i];
		}
	}
}
