using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020006F6 RID: 1782
public class BaseCommandBuffer : MonoBehaviour
{
	// Token: 0x04002334 RID: 9012
	private Dictionary<Camera, Dictionary<int, CommandBuffer>> cameras = new Dictionary<Camera, Dictionary<int, CommandBuffer>>();

	// Token: 0x06002741 RID: 10049 RVA: 0x000CC4B8 File Offset: 0x000CA6B8
	protected CommandBuffer GetCommandBuffer(string name, Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, ref dictionary))
		{
			dictionary = new Dictionary<int, CommandBuffer>();
			this.cameras.Add(camera, dictionary);
		}
		CommandBuffer commandBuffer;
		if (dictionary.TryGetValue((int)cameraEvent, ref commandBuffer))
		{
			commandBuffer.Clear();
		}
		else
		{
			commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			dictionary.Add((int)cameraEvent, commandBuffer);
			this.CleanupCamera(name, camera, cameraEvent);
			camera.AddCommandBuffer(cameraEvent, commandBuffer);
		}
		return commandBuffer;
	}

	// Token: 0x06002742 RID: 10050 RVA: 0x000CC524 File Offset: 0x000CA724
	protected void CleanupCamera(string name, Camera camera, CameraEvent cameraEvent)
	{
		foreach (CommandBuffer commandBuffer in camera.GetCommandBuffers(cameraEvent))
		{
			if (commandBuffer.name == name)
			{
				camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
			}
		}
	}

	// Token: 0x06002743 RID: 10051 RVA: 0x000CC564 File Offset: 0x000CA764
	protected void CleanupCommandBuffer(Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, ref dictionary))
		{
			return;
		}
		CommandBuffer buffer;
		if (!dictionary.TryGetValue((int)cameraEvent, ref buffer))
		{
			return;
		}
		camera.RemoveCommandBuffer(cameraEvent, buffer);
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000CC598 File Offset: 0x000CA798
	protected void Cleanup()
	{
		foreach (KeyValuePair<Camera, Dictionary<int, CommandBuffer>> keyValuePair in this.cameras)
		{
			Camera key = keyValuePair.Key;
			Dictionary<int, CommandBuffer> value = keyValuePair.Value;
			if (key)
			{
				foreach (KeyValuePair<int, CommandBuffer> keyValuePair2 in value)
				{
					int key2 = keyValuePair2.Key;
					CommandBuffer value2 = keyValuePair2.Value;
					key.RemoveCommandBuffer((CameraEvent)key2, value2);
				}
			}
		}
	}
}
