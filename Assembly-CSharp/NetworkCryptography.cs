using System;
using System.IO;
using Network;

// Token: 0x0200047A RID: 1146
public abstract class NetworkCryptography : INetworkCryptocraphy
{
	// Token: 0x040017A6 RID: 6054
	private MemoryStream buffer = new MemoryStream();

	// Token: 0x06001ABB RID: 6843 RVA: 0x00093954 File Offset: 0x00091B54
	public MemoryStream EncryptCopy(Connection connection, MemoryStream stream, int offset)
	{
		this.buffer.Position = 0L;
		this.buffer.SetLength(0L);
		this.buffer.Write(stream.GetBuffer(), 0, offset);
		this.EncryptionHandler(connection, stream, offset, this.buffer, offset);
		return this.buffer;
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x000939A4 File Offset: 0x00091BA4
	public MemoryStream DecryptCopy(Connection connection, MemoryStream stream, int offset)
	{
		this.buffer.Position = 0L;
		this.buffer.SetLength(0L);
		this.buffer.Write(stream.GetBuffer(), 0, offset);
		this.DecryptionHandler(connection, stream, offset, this.buffer, offset);
		return this.buffer;
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x00016216 File Offset: 0x00014416
	public void Encrypt(Connection connection, MemoryStream stream, int offset)
	{
		this.EncryptionHandler(connection, stream, offset, stream, offset);
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x00016223 File Offset: 0x00014423
	public void Decrypt(Connection connection, MemoryStream stream, int offset)
	{
		this.DecryptionHandler(connection, stream, offset, stream, offset);
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x00016230 File Offset: 0x00014430
	public bool IsEnabledIncoming(Connection connection)
	{
		return connection != null && connection.encryptionLevel > 0U && connection.decryptIncoming;
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x00016246 File Offset: 0x00014446
	public bool IsEnabledOutgoing(Connection connection)
	{
		return connection != null && connection.encryptionLevel > 0U && connection.encryptOutgoing;
	}

	// Token: 0x06001AC1 RID: 6849
	protected abstract void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);

	// Token: 0x06001AC2 RID: 6850
	protected abstract void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);
}
