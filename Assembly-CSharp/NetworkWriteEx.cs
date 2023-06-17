﻿using System;
using Network;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;

// Token: 0x02000728 RID: 1832
public static class NetworkWriteEx
{
	// Token: 0x0600280E RID: 10254 RVA: 0x000CE960 File Offset: 0x000CCB60
	public static void WriteObject<T>(this Write write, T obj)
	{
		if (typeof(T) == typeof(Vector3))
		{
			write.Vector3(GenericsUtil.Cast<T, Vector3>(obj));
			return;
		}
		if (typeof(T) == typeof(Ray))
		{
			write.Ray(GenericsUtil.Cast<T, Ray>(obj));
			return;
		}
		if (typeof(T) == typeof(float))
		{
			write.Float(GenericsUtil.Cast<T, float>(obj));
			return;
		}
		if (typeof(T) == typeof(short))
		{
			write.Int16(GenericsUtil.Cast<T, short>(obj));
			return;
		}
		if (typeof(T) == typeof(ushort))
		{
			write.UInt16(GenericsUtil.Cast<T, ushort>(obj));
			return;
		}
		if (typeof(T) == typeof(int))
		{
			write.Int32(GenericsUtil.Cast<T, int>(obj));
			return;
		}
		if (typeof(T) == typeof(uint))
		{
			write.UInt32(GenericsUtil.Cast<T, uint>(obj));
			return;
		}
		if (typeof(T) == typeof(byte[]))
		{
			write.Bytes(GenericsUtil.Cast<T, byte[]>(obj));
			return;
		}
		if (typeof(T) == typeof(long))
		{
			write.Int64(GenericsUtil.Cast<T, long>(obj));
			return;
		}
		if (typeof(T) == typeof(ulong))
		{
			write.UInt64(GenericsUtil.Cast<T, ulong>(obj));
			return;
		}
		if (typeof(T) == typeof(string))
		{
			write.String(GenericsUtil.Cast<T, string>(obj));
			return;
		}
		if (typeof(T) == typeof(sbyte))
		{
			write.Int8(GenericsUtil.Cast<T, sbyte>(obj));
			return;
		}
		if (typeof(T) == typeof(byte))
		{
			write.UInt8(GenericsUtil.Cast<T, byte>(obj));
			return;
		}
		if (typeof(T) == typeof(bool))
		{
			write.Bool(GenericsUtil.Cast<T, bool>(obj));
			return;
		}
		if (obj is IProto)
		{
			((IProto)((object)obj)).WriteToStream(write);
			return;
		}
		Debug.LogError(string.Concat(new object[]
		{
			"NetworkData.Write - no handler to write ",
			obj,
			" -> ",
			obj.GetType()
		}));
	}
}
