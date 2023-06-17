using System;
using System.IO;
using Network;

// Token: 0x0200047B RID: 1147
public class NetworkCryptographyClient : NetworkCryptography
{
	// Token: 0x06001AC4 RID: 6852 RVA: 0x0001626F File Offset: 0x0001446F
	protected override void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		if (connection.encryptionLevel <= 1U)
		{
			Craptography.XOR(2153U, src, srcOffset, dst, dstOffset);
			return;
		}
		EAC.Encrypt(connection, src, srcOffset, dst, dstOffset);
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x00016297 File Offset: 0x00014497
	protected override void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		if (connection.encryptionLevel <= 1U)
		{
			Craptography.XOR(2153U, src, srcOffset, dst, dstOffset);
			return;
		}
		EAC.Decrypt(connection, src, srcOffset, dst, dstOffset);
	}
}
