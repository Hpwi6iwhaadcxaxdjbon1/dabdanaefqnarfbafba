using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Sqlite;
using Ionic.Crc;
using UnityEngine.Assertions;

// Token: 0x020005C6 RID: 1478
public class FileStorage : IDisposable
{
	// Token: 0x04001DBD RID: 7613
	private Database db;

	// Token: 0x04001DBE RID: 7614
	private CRC32 crc = new CRC32();

	// Token: 0x04001DBF RID: 7615
	private Dictionary<uint, FileStorage.CacheData> _cache = new Dictionary<uint, FileStorage.CacheData>();

	// Token: 0x04001DC0 RID: 7616
	public static FileStorage client = new FileStorage("cl.files." + 0, false);

	// Token: 0x060021D2 RID: 8658 RVA: 0x0001AE36 File Offset: 0x00019036
	protected FileStorage(string name, bool server)
	{
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x000B6B84 File Offset: 0x000B4D84
	~FileStorage()
	{
		this.Dispose();
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x0001AE54 File Offset: 0x00019054
	public void Dispose()
	{
		if (this.db != null)
		{
			this.db.Close();
			this.db = null;
		}
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x0001AE70 File Offset: 0x00019070
	private uint GetCRC(byte[] data, FileStorage.Type type)
	{
		this.crc.Reset();
		this.crc.SlurpBlock(data, 0, data.Length);
		this.crc.UpdateCRC((byte)type);
		return (uint)this.crc.Crc32Result;
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000B6BB0 File Offset: 0x000B4DB0
	public uint Store(byte[] data, FileStorage.Type type, uint entityID, uint numID = 0U)
	{
		uint result;
		using (TimeWarning.New("FileStorage.Store", 0.1f))
		{
			uint num = this.GetCRC(data, type);
			if (this.db != null)
			{
				this.db.Execute("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", new object[]
				{
					(int)num,
					data,
					(int)entityID,
					(int)type,
					(int)numID
				});
			}
			this._cache.Remove(num);
			this._cache.Add(num, new FileStorage.CacheData
			{
				data = data,
				entityID = entityID,
				numID = numID
			});
			result = num;
		}
		return result;
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000B6C70 File Offset: 0x000B4E70
	public byte[] Get(uint crc, FileStorage.Type type, uint entityID)
	{
		byte[] result;
		using (TimeWarning.New("FileStorage.Get", 0.1f))
		{
			FileStorage.CacheData cacheData;
			if (this._cache.TryGetValue(crc, ref cacheData))
			{
				Assert.IsTrue(cacheData.data != null, "FileStorage cache contains a null texture");
				result = cacheData.data;
			}
			else if (this.db == null)
			{
				result = null;
			}
			else
			{
				byte[] array = this.db.QueryBlob("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? LIMIT 1", new object[]
				{
					(int)crc,
					(int)type,
					(int)entityID
				});
				if (array == null)
				{
					result = null;
				}
				else
				{
					this._cache.Remove(crc);
					this._cache.Add(crc, new FileStorage.CacheData
					{
						data = array,
						entityID = entityID,
						numID = 0U
					});
					result = array;
				}
			}
		}
		return result;
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x000B6D54 File Offset: 0x000B4F54
	public void Remove(uint crc, FileStorage.Type type, uint entityID)
	{
		using (TimeWarning.New("FileStorage.Remove", 0.1f))
		{
			if (this.db != null)
			{
				this.db.Execute("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", new object[]
				{
					(int)crc,
					(int)type,
					(int)entityID
				});
			}
			if (this._cache.ContainsKey(crc))
			{
				this._cache.Remove(crc);
			}
		}
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x000B6DE4 File Offset: 0x000B4FE4
	public void RemoveEntityNum(uint entityid, uint numid)
	{
		using (TimeWarning.New("FileStorage.RemoveEntityNum", 0.1f))
		{
			if (this.db != null)
			{
				this.db.Execute("DELETE FROM data WHERE entid = ? AND part = ?", new object[]
				{
					(int)entityid,
					(int)numid
				});
			}
			IEnumerable<KeyValuePair<uint, FileStorage.CacheData>> cache = this._cache;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> <>9__0;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = ((KeyValuePair<uint, FileStorage.CacheData> x) => x.Value.entityID == entityid && x.Value.numID == numid));
			}
			foreach (uint num in Enumerable.ToArray<uint>(Enumerable.Select<KeyValuePair<uint, FileStorage.CacheData>, uint>(Enumerable.Where<KeyValuePair<uint, FileStorage.CacheData>>(cache, func), (KeyValuePair<uint, FileStorage.CacheData> x) => x.Key)))
			{
				this._cache.Remove(num);
			}
		}
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x000B6EE4 File Offset: 0x000B50E4
	internal void RemoveAllByEntity(uint entityid)
	{
		using (TimeWarning.New("FileStorage.RemoveAllByEntity", 0.1f))
		{
			if (this.db != null)
			{
				this.db.Execute("DELETE FROM data WHERE entid = ?", new object[]
				{
					(int)entityid
				});
			}
		}
	}

	// Token: 0x020005C7 RID: 1479
	private class CacheData
	{
		// Token: 0x04001DC1 RID: 7617
		public byte[] data;

		// Token: 0x04001DC2 RID: 7618
		public uint entityID;

		// Token: 0x04001DC3 RID: 7619
		public uint numID;
	}

	// Token: 0x020005C8 RID: 1480
	public enum Type
	{
		// Token: 0x04001DC5 RID: 7621
		png,
		// Token: 0x04001DC6 RID: 7622
		jpg
	}
}
