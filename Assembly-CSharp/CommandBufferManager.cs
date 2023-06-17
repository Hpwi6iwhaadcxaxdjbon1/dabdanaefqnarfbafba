using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200057D RID: 1405
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CommandBufferManager : MonoBehaviour
{
	// Token: 0x04001C1B RID: 7195
	private Camera _targetCamera;

	// Token: 0x04001C1C RID: 7196
	private Dictionary<int, CommandBufferManager.CommandBufferEntry> commandBuffers = new Dictionary<int, CommandBufferManager.CommandBufferEntry>();

	// Token: 0x04001C1D RID: 7197
	private bool changed = true;

	// Token: 0x04001C1E RID: 7198
	public Action OnPreRenderCall;

	// Token: 0x04001C1F RID: 7199
	public Action OnPreCullCall;

	// Token: 0x04001C20 RID: 7200
	private static Dictionary<Camera, CommandBufferManager> instances = new Dictionary<Camera, CommandBufferManager>();

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x0600201C RID: 8220 RVA: 0x000AF230 File Offset: 0x000AD430
	public Camera targetCamera
	{
		get
		{
			return this._targetCamera = ((this._targetCamera != null) ? this._targetCamera : base.GetComponent<Camera>());
		}
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600201D RID: 8221 RVA: 0x00019725 File Offset: 0x00017925
	public Camera Camera
	{
		get
		{
			return this.targetCamera;
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x0600201E RID: 8222 RVA: 0x0001972D File Offset: 0x0001792D
	public bool IsReady
	{
		get
		{
			return this.targetCamera != null;
		}
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000AF264 File Offset: 0x000AD464
	public static CommandBufferManager GetInstance(Camera camera)
	{
		CommandBufferManager result;
		CommandBufferManager.instances.TryGetValue(camera, ref result);
		return result;
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x0001973B File Offset: 0x0001793B
	private void Awake()
	{
		CommandBufferManager.instances[this.targetCamera] = this;
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x0001974E File Offset: 0x0001794E
	private void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		CommandBufferManager.instances.Remove(this.targetCamera);
	}

	// Token: 0x06002022 RID: 8226 RVA: 0x0001973B File Offset: 0x0001793B
	private void OnEnable()
	{
		CommandBufferManager.instances[this.targetCamera] = this;
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x000AF280 File Offset: 0x000AD480
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		foreach (KeyValuePair<int, CommandBufferManager.CommandBufferEntry> keyValuePair in this.commandBuffers)
		{
			this.targetCamera.RemoveCommandBuffer((CameraEvent)keyValuePair.Key, keyValuePair.Value.buffer);
		}
		this.commandBuffers.Clear();
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000AF300 File Offset: 0x000AD500
	public void AddCommands(CommandBufferDesc desc)
	{
		if (desc != null)
		{
			CommandBufferManager.CommandBufferEntry commandBufferEntry;
			bool flag = this.commandBuffers.TryGetValue((int)desc.CameraEvent, ref commandBufferEntry);
			if (!flag || !commandBufferEntry.Contains(desc.OrderId))
			{
				if (!flag)
				{
					commandBufferEntry = default(CommandBufferManager.CommandBufferEntry);
					commandBufferEntry.buffer = new CommandBuffer();
					commandBufferEntry.orderedFill = new List<KeyValuePair<int, Action<CommandBuffer>>>();
					commandBufferEntry.buffer.name = commandBufferEntry.buffer.GetHashCode().ToString("X8");
					this.targetCamera.AddCommandBuffer(desc.CameraEvent, commandBufferEntry.buffer);
					this.commandBuffers.Add((int)desc.CameraEvent, commandBufferEntry);
				}
				commandBufferEntry.Add(desc.OrderId, desc.FillDelegate);
			}
		}
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x000AF3C4 File Offset: 0x000AD5C4
	public void RemoveCommands(CommandBufferDesc desc)
	{
		CommandBufferManager.CommandBufferEntry commandBufferEntry;
		if (desc != null && (this.commandBuffers.TryGetValue((int)desc.CameraEvent, ref commandBufferEntry) && commandBufferEntry.Contains(desc.OrderId)))
		{
			commandBufferEntry.Remove(desc.OrderId);
			if (commandBufferEntry.orderedFill.Count == 0)
			{
				this.commandBuffers.Remove((int)desc.CameraEvent);
				this.targetCamera.RemoveCommandBuffer(desc.CameraEvent, commandBufferEntry.buffer);
			}
		}
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000AF440 File Offset: 0x000AD640
	private void CheckUpdateCameraEvents()
	{
		if (this.changed)
		{
			foreach (KeyValuePair<int, CommandBufferManager.CommandBufferEntry> keyValuePair in this.commandBuffers)
			{
				CommandBufferManager.CommandBufferEntry value = keyValuePair.Value;
				bool flag = false;
				CommandBuffer[] array = this.targetCamera.GetCommandBuffers((CameraEvent)keyValuePair.Key);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].name == value.buffer.name)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.targetCamera.AddCommandBuffer((CameraEvent)keyValuePair.Key, value.buffer);
				}
			}
			this.changed = false;
		}
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x000AF50C File Offset: 0x000AD70C
	private void OnPreCull()
	{
		foreach (KeyValuePair<int, CommandBufferManager.CommandBufferEntry> keyValuePair in this.commandBuffers)
		{
			CommandBufferManager.CommandBufferEntry value = keyValuePair.Value;
			value.buffer.Clear();
			this.CheckUpdateCameraEvents();
			for (int i = 0; i < value.orderedFill.Count; i++)
			{
				value.orderedFill[i].Value.Invoke(value.buffer);
			}
		}
		if (this.OnPreCullCall != null)
		{
			this.OnPreCullCall.Invoke();
		}
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x00019769 File Offset: 0x00017969
	private void OnPreRender()
	{
		if (this.OnPreRenderCall != null)
		{
			this.OnPreRenderCall.Invoke();
		}
	}

	// Token: 0x0200057E RID: 1406
	private struct CommandBufferEntry : IEquatable<CommandBufferManager.CommandBufferEntry>
	{
		// Token: 0x04001C21 RID: 7201
		public CommandBuffer buffer;

		// Token: 0x04001C22 RID: 7202
		public List<KeyValuePair<int, Action<CommandBuffer>>> orderedFill;

		// Token: 0x0600202B RID: 8235 RVA: 0x000AF59C File Offset: 0x000AD79C
		public void Add(int orderId, Action<CommandBuffer> value)
		{
			int num = 0;
			while (num < this.orderedFill.Count && this.orderedFill[num].Key < orderId)
			{
				num++;
			}
			this.orderedFill.Insert(num, new KeyValuePair<int, Action<CommandBuffer>>(orderId, value));
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000AF5EC File Offset: 0x000AD7EC
		public void Remove(int orderId)
		{
			this.orderedFill.RemoveAll((KeyValuePair<int, Action<CommandBuffer>> s) => s.Key == orderId);
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x000AF620 File Offset: 0x000AD820
		public bool Contains(int orderId)
		{
			for (int i = 0; i < this.orderedFill.Count; i++)
			{
				if (this.orderedFill[i].Key == orderId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000197A4 File Offset: 0x000179A4
		public bool Equals(CommandBufferManager.CommandBufferEntry other)
		{
			return this.buffer == other.buffer;
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000197B4 File Offset: 0x000179B4
		public override int GetHashCode()
		{
			return this.buffer.GetHashCode();
		}
	}
}
