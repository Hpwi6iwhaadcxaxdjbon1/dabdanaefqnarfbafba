using System;

// Token: 0x02000487 RID: 1159
public static class ManagedNoise
{
	// Token: 0x040017DA RID: 6106
	private static readonly int[] hash = new int[]
	{
		151,
		160,
		137,
		91,
		90,
		15,
		131,
		13,
		201,
		95,
		96,
		53,
		194,
		233,
		7,
		225,
		140,
		36,
		103,
		30,
		69,
		142,
		8,
		99,
		37,
		240,
		21,
		10,
		23,
		190,
		6,
		148,
		247,
		120,
		234,
		75,
		0,
		26,
		197,
		62,
		94,
		252,
		219,
		203,
		117,
		35,
		11,
		32,
		57,
		177,
		33,
		88,
		237,
		149,
		56,
		87,
		174,
		20,
		125,
		136,
		171,
		168,
		68,
		175,
		74,
		165,
		71,
		134,
		139,
		48,
		27,
		166,
		77,
		146,
		158,
		231,
		83,
		111,
		229,
		122,
		60,
		211,
		133,
		230,
		220,
		105,
		92,
		41,
		55,
		46,
		245,
		40,
		244,
		102,
		143,
		54,
		65,
		25,
		63,
		161,
		1,
		216,
		80,
		73,
		209,
		76,
		132,
		187,
		208,
		89,
		18,
		169,
		200,
		196,
		135,
		130,
		116,
		188,
		159,
		86,
		164,
		100,
		109,
		198,
		173,
		186,
		3,
		64,
		52,
		217,
		226,
		250,
		124,
		123,
		5,
		202,
		38,
		147,
		118,
		126,
		255,
		82,
		85,
		212,
		207,
		206,
		59,
		227,
		47,
		16,
		58,
		17,
		182,
		189,
		28,
		42,
		223,
		183,
		170,
		213,
		119,
		248,
		152,
		2,
		44,
		154,
		163,
		70,
		221,
		153,
		101,
		155,
		167,
		43,
		172,
		9,
		129,
		22,
		39,
		253,
		19,
		98,
		108,
		110,
		79,
		113,
		224,
		232,
		178,
		185,
		112,
		104,
		218,
		246,
		97,
		228,
		251,
		34,
		242,
		193,
		238,
		210,
		144,
		12,
		191,
		179,
		162,
		241,
		81,
		51,
		145,
		235,
		249,
		14,
		239,
		107,
		49,
		192,
		214,
		31,
		181,
		199,
		106,
		157,
		184,
		84,
		204,
		176,
		115,
		121,
		50,
		45,
		127,
		4,
		150,
		254,
		138,
		236,
		205,
		93,
		222,
		114,
		67,
		29,
		24,
		72,
		243,
		141,
		128,
		195,
		78,
		66,
		215,
		61,
		156,
		180,
		151,
		160,
		137,
		91,
		90,
		15,
		131,
		13,
		201,
		95,
		96,
		53,
		194,
		233,
		7,
		225,
		140,
		36,
		103,
		30,
		69,
		142,
		8,
		99,
		37,
		240,
		21,
		10,
		23,
		190,
		6,
		148,
		247,
		120,
		234,
		75,
		0,
		26,
		197,
		62,
		94,
		252,
		219,
		203,
		117,
		35,
		11,
		32,
		57,
		177,
		33,
		88,
		237,
		149,
		56,
		87,
		174,
		20,
		125,
		136,
		171,
		168,
		68,
		175,
		74,
		165,
		71,
		134,
		139,
		48,
		27,
		166,
		77,
		146,
		158,
		231,
		83,
		111,
		229,
		122,
		60,
		211,
		133,
		230,
		220,
		105,
		92,
		41,
		55,
		46,
		245,
		40,
		244,
		102,
		143,
		54,
		65,
		25,
		63,
		161,
		1,
		216,
		80,
		73,
		209,
		76,
		132,
		187,
		208,
		89,
		18,
		169,
		200,
		196,
		135,
		130,
		116,
		188,
		159,
		86,
		164,
		100,
		109,
		198,
		173,
		186,
		3,
		64,
		52,
		217,
		226,
		250,
		124,
		123,
		5,
		202,
		38,
		147,
		118,
		126,
		255,
		82,
		85,
		212,
		207,
		206,
		59,
		227,
		47,
		16,
		58,
		17,
		182,
		189,
		28,
		42,
		223,
		183,
		170,
		213,
		119,
		248,
		152,
		2,
		44,
		154,
		163,
		70,
		221,
		153,
		101,
		155,
		167,
		43,
		172,
		9,
		129,
		22,
		39,
		253,
		19,
		98,
		108,
		110,
		79,
		113,
		224,
		232,
		178,
		185,
		112,
		104,
		218,
		246,
		97,
		228,
		251,
		34,
		242,
		193,
		238,
		210,
		144,
		12,
		191,
		179,
		162,
		241,
		81,
		51,
		145,
		235,
		249,
		14,
		239,
		107,
		49,
		192,
		214,
		31,
		181,
		199,
		106,
		157,
		184,
		84,
		204,
		176,
		115,
		121,
		50,
		45,
		127,
		4,
		150,
		254,
		138,
		236,
		205,
		93,
		222,
		114,
		67,
		29,
		24,
		72,
		243,
		141,
		128,
		195,
		78,
		66,
		215,
		61,
		156,
		180
	};

	// Token: 0x040017DB RID: 6107
	private const int hashMask = 255;

	// Token: 0x040017DC RID: 6108
	private const double sqrt2 = 1.4142135623730951;

	// Token: 0x040017DD RID: 6109
	private const double rsqrt2 = 0.7071067811865476;

	// Token: 0x040017DE RID: 6110
	private const double squaresToTriangles = 0.2113248654051871;

	// Token: 0x040017DF RID: 6111
	private const double trianglesToSquares = 0.36602540378443865;

	// Token: 0x040017E0 RID: 6112
	private const double simplexScale1D = 2.4074074074074074;

	// Token: 0x040017E1 RID: 6113
	private const double simplexScale2D = 32.99077398303956;

	// Token: 0x040017E2 RID: 6114
	private const double gradientScale2D = 4.0;

	// Token: 0x040017E3 RID: 6115
	private static double[] gradients1D = new double[]
	{
		1.0,
		-1.0
	};

	// Token: 0x040017E4 RID: 6116
	private const int gradientsMask1D = 1;

	// Token: 0x040017E5 RID: 6117
	private static double[] gradients2Dx = new double[]
	{
		1.0,
		-1.0,
		0.0,
		0.0,
		0.7071067811865476,
		-0.7071067811865476,
		0.7071067811865476,
		-0.7071067811865476
	};

	// Token: 0x040017E6 RID: 6118
	private static double[] gradients2Dy = new double[]
	{
		0.0,
		0.0,
		1.0,
		-1.0,
		0.7071067811865476,
		0.7071067811865476,
		-0.7071067811865476,
		-0.7071067811865476
	};

	// Token: 0x040017E7 RID: 6119
	private const int gradientsMask2D = 7;

	// Token: 0x06001AF6 RID: 6902 RVA: 0x00094CE0 File Offset: 0x00092EE0
	public static double Simplex1D(double x)
	{
		double num = 0.0;
		int num3;
		int num2 = num3 = ManagedNoise.Floor(x);
		double num4 = x - (double)num3;
		double num5 = 1.0 - num4 * num4;
		if (num5 > 0.0)
		{
			double num6 = num5 * num5;
			double num7 = num5 * num6;
			int num8 = ManagedNoise.hash[num3 & 255] & 1;
			double num9 = ManagedNoise.gradients1D[num8] * num4;
			num += num9 * num7;
		}
		int num10 = num2 + 1;
		double num11 = x - (double)num10;
		double num12 = 1.0 - num11 * num11;
		if (num12 > 0.0)
		{
			double num13 = num12 * num12;
			double num14 = num12 * num13;
			int num15 = ManagedNoise.hash[num10 & 255] & 1;
			double num16 = ManagedNoise.gradients1D[num15] * num11;
			num += num16 * num14;
		}
		return num * 2.4074074074074074;
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x00094DB8 File Offset: 0x00092FB8
	public static double Simplex1D(double x, out double dx)
	{
		double num = 0.0;
		dx = 0.0;
		int num3;
		int num2 = num3 = ManagedNoise.Floor(x);
		double num4 = x - (double)num3;
		double num5 = 1.0 - num4 * num4;
		if (num5 > 0.0)
		{
			double num6 = num5 * num5;
			double num7 = num5 * num6;
			int num8 = ManagedNoise.hash[num3 & 255] & 1;
			double num9 = ManagedNoise.gradients1D[num8];
			double num10 = num9 * num4;
			double num11 = num10 * 6.0 * num6;
			dx += num9 * num7 - num11 * num4;
			num += num10 * num7;
		}
		int num12 = num2 + 1;
		double num13 = x - (double)num12;
		double num14 = 1.0 - num13 * num13;
		if (num14 > 0.0)
		{
			double num15 = num14 * num14;
			double num16 = num14 * num15;
			int num17 = ManagedNoise.hash[num12 & 255] & 1;
			double num18 = ManagedNoise.gradients1D[num17];
			double num19 = num18 * num13;
			double num20 = num19 * 6.0 * num15;
			dx += num18 * num16 - num20 * num13;
			num += num19 * num16;
		}
		return num * 2.4074074074074074;
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x00094EE4 File Offset: 0x000930E4
	public static double Simplex2D(double x, double y)
	{
		double num = 0.0;
		double num2 = (x + y) * 0.36602540378443865;
		double num3 = x + num2;
		double num4 = y + num2;
		int num5 = ManagedNoise.Floor(num3);
		int num6 = ManagedNoise.Floor(num4);
		int num7 = num5;
		int num8 = num6;
		double num9 = (double)(num7 + num8) * 0.2113248654051871;
		double num10 = x - (double)num7 + num9;
		double num11 = y - (double)num8 + num9;
		double num12 = 0.5 - num10 * num10 - num11 * num11;
		if (num12 > 0.0)
		{
			double num13 = num12 * num12;
			double num14 = num12 * num13;
			int num15 = ManagedNoise.hash[ManagedNoise.hash[num7 & 255] + num8 & 255] & 7;
			double num16 = ManagedNoise.gradients2Dx[num15];
			double num17 = ManagedNoise.gradients2Dy[num15];
			double num18 = num16 * num10 + num17 * num11;
			num += num18 * num14;
		}
		int num19 = num5 + 1;
		int num20 = num6 + 1;
		double num21 = (double)(num19 + num20) * 0.2113248654051871;
		double num22 = x - (double)num19 + num21;
		double num23 = y - (double)num20 + num21;
		double num24 = 0.5 - num22 * num22 - num23 * num23;
		if (num24 > 0.0)
		{
			double num25 = num24 * num24;
			double num26 = num24 * num25;
			int num27 = ManagedNoise.hash[ManagedNoise.hash[num19 & 255] + num20 & 255] & 7;
			double num28 = ManagedNoise.gradients2Dx[num27];
			double num29 = ManagedNoise.gradients2Dy[num27];
			double num30 = num28 * num22 + num29 * num23;
			num += num30 * num26;
		}
		if (num3 - (double)num5 >= num4 - (double)num6)
		{
			int num31 = num5 + 1;
			int num32 = num6;
			double num33 = (double)(num31 + num32) * 0.2113248654051871;
			double num34 = x - (double)num31 + num33;
			double num35 = y - (double)num32 + num33;
			double num36 = 0.5 - num34 * num34 - num35 * num35;
			if (num36 > 0.0)
			{
				double num37 = num36 * num36;
				double num38 = num36 * num37;
				int num39 = ManagedNoise.hash[ManagedNoise.hash[num31 & 255] + num32 & 255] & 7;
				double num40 = ManagedNoise.gradients2Dx[num39];
				double num41 = ManagedNoise.gradients2Dy[num39];
				double num42 = num40 * num34 + num41 * num35;
				num += num42 * num38;
			}
		}
		else
		{
			int num43 = num5;
			int num44 = num6 + 1;
			double num45 = (double)(num43 + num44) * 0.2113248654051871;
			double num46 = x - (double)num43 + num45;
			double num47 = y - (double)num44 + num45;
			double num48 = 0.5 - num46 * num46 - num47 * num47;
			if (num48 > 0.0)
			{
				double num49 = num48 * num48;
				double num50 = num48 * num49;
				int num51 = ManagedNoise.hash[ManagedNoise.hash[num43 & 255] + num44 & 255] & 7;
				double num52 = ManagedNoise.gradients2Dx[num51];
				double num53 = ManagedNoise.gradients2Dy[num51];
				double num54 = num52 * num46 + num53 * num47;
				num += num54 * num50;
			}
		}
		return num * 32.99077398303956;
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x000951DC File Offset: 0x000933DC
	public static double Simplex2D(double x, double y, out double dx, out double dy)
	{
		double num = 0.0;
		dx = 0.0;
		dy = 0.0;
		double num2 = (x + y) * 0.36602540378443865;
		double num3 = x + num2;
		double num4 = y + num2;
		int num5 = ManagedNoise.Floor(num3);
		int num6 = ManagedNoise.Floor(num4);
		int num7 = num5;
		int num8 = num6;
		double num9 = (double)(num7 + num8) * 0.2113248654051871;
		double num10 = x - (double)num7 + num9;
		double num11 = y - (double)num8 + num9;
		double num12 = 0.5 - num10 * num10 - num11 * num11;
		if (num12 > 0.0)
		{
			double num13 = num12 * num12;
			double num14 = num12 * num13;
			int num15 = ManagedNoise.hash[ManagedNoise.hash[num7 & 255] + num8 & 255] & 7;
			double num16 = ManagedNoise.gradients2Dx[num15];
			double num17 = ManagedNoise.gradients2Dy[num15];
			double num18 = num16 * num10 + num17 * num11;
			double num19 = num18 * 6.0 * num13;
			dx += num16 * num14 - num19 * num10;
			dy += num17 * num14 - num19 * num11;
			num += num18 * num14;
		}
		int num20 = num5 + 1;
		int num21 = num6 + 1;
		double num22 = (double)(num20 + num21) * 0.2113248654051871;
		double num23 = x - (double)num20 + num22;
		double num24 = y - (double)num21 + num22;
		double num25 = 0.5 - num23 * num23 - num24 * num24;
		if (num25 > 0.0)
		{
			double num26 = num25 * num25;
			double num27 = num25 * num26;
			int num28 = ManagedNoise.hash[ManagedNoise.hash[num20 & 255] + num21 & 255] & 7;
			double num29 = ManagedNoise.gradients2Dx[num28];
			double num30 = ManagedNoise.gradients2Dy[num28];
			double num31 = num29 * num23 + num30 * num24;
			double num32 = num31 * 6.0 * num26;
			dx += num29 * num27 - num32 * num23;
			dy += num30 * num27 - num32 * num24;
			num += num31 * num27;
		}
		if (num3 - (double)num5 >= num4 - (double)num6)
		{
			int num33 = num5 + 1;
			int num34 = num6;
			double num35 = (double)(num33 + num34) * 0.2113248654051871;
			double num36 = x - (double)num33 + num35;
			double num37 = y - (double)num34 + num35;
			double num38 = 0.5 - num36 * num36 - num37 * num37;
			if (num38 > 0.0)
			{
				double num39 = num38 * num38;
				double num40 = num38 * num39;
				int num41 = ManagedNoise.hash[ManagedNoise.hash[num33 & 255] + num34 & 255] & 7;
				double num42 = ManagedNoise.gradients2Dx[num41];
				double num43 = ManagedNoise.gradients2Dy[num41];
				double num44 = num42 * num36 + num43 * num37;
				double num45 = num44 * 6.0 * num39;
				dx += num42 * num40 - num45 * num36;
				dy += num43 * num40 - num45 * num37;
				num += num44 * num40;
			}
		}
		else
		{
			int num46 = num5;
			int num47 = num6 + 1;
			double num48 = (double)(num46 + num47) * 0.2113248654051871;
			double num49 = x - (double)num46 + num48;
			double num50 = y - (double)num47 + num48;
			double num51 = 0.5 - num49 * num49 - num50 * num50;
			if (num51 > 0.0)
			{
				double num52 = num51 * num51;
				double num53 = num51 * num52;
				int num54 = ManagedNoise.hash[ManagedNoise.hash[num46 & 255] + num47 & 255] & 7;
				double num55 = ManagedNoise.gradients2Dx[num54];
				double num56 = ManagedNoise.gradients2Dy[num54];
				double num57 = num55 * num49 + num56 * num50;
				double num58 = num57 * 6.0 * num52;
				dx += num55 * num53 - num58 * num49;
				dy += num56 * num53 - num58 * num50;
				num += num57 * num53;
			}
		}
		dx *= 4.0;
		dy *= 4.0;
		return num * 32.99077398303956;
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x000955E4 File Offset: 0x000937E4
	public static double Turbulence(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		for (int i = 0; i < octaves; i++)
		{
			double num4 = ManagedNoise.Simplex2D(x * num2, y * num2);
			num += num3 * num4;
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x0009564C File Offset: 0x0009384C
	public static double Billow(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		for (int i = 0; i < octaves; i++)
		{
			double x2 = ManagedNoise.Simplex2D(x * num2, y * num2);
			num += num3 * ManagedNoise.Abs(x2);
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x000956B8 File Offset: 0x000938B8
	public static double Ridge(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		for (int i = 0; i < octaves; i++)
		{
			double x2 = ManagedNoise.Simplex2D(x * num2, y * num2);
			num += num3 * (1.0 - ManagedNoise.Abs(x2));
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x0009572C File Offset: 0x0009392C
	public static double Sharp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		for (int i = 0; i < octaves; i++)
		{
			double num4 = ManagedNoise.Simplex2D(x * num2, y * num2);
			num += num3 * (num4 * num4);
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x00095794 File Offset: 0x00093994
	public static double TurbulenceIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num7;
			double num8;
			double num6 = ManagedNoise.Simplex2D(x * num2, y * num2, out num7, out num8);
			num4 += num7;
			num5 += num8;
			num += num3 * num6 / (1.0 + (num4 * num4 + num5 * num5));
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x00095840 File Offset: 0x00093A40
	public static double BillowIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num6;
			double num7;
			double x2 = ManagedNoise.Simplex2D(x * num2, y * num2, out num6, out num7);
			num4 += num6;
			num5 += num7;
			num += num3 * ManagedNoise.Abs(x2) / (1.0 + (num4 * num4 + num5 * num5));
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x000958F0 File Offset: 0x00093AF0
	public static double RidgeIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num6;
			double num7;
			double x2 = ManagedNoise.Simplex2D(x * num2, y * num2, out num6, out num7);
			num4 += num6;
			num5 += num7;
			num += num3 * (1.0 - ManagedNoise.Abs(x2)) / (1.0 + (num4 * num4 + num5 * num5));
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x000959AC File Offset: 0x00093BAC
	public static double SharpIQ(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num7;
			double num8;
			double num6 = ManagedNoise.Simplex2D(x * num2, y * num2, out num7, out num8);
			num4 += num7;
			num5 += num8;
			num += num3 * (num6 * num6) / (1.0 + (num4 * num4 + num5 * num5));
			num2 *= lacunarity;
			num3 *= gain;
		}
		return num * amplitude;
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x00095A58 File Offset: 0x00093C58
	public static double TurbulenceWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num7;
			double num8;
			double num6 = ManagedNoise.Simplex2D((x + warp * num4) * num2, (y + warp * num5) * num2, out num7, out num8);
			num += num3 * num6;
			num4 += num3 * num7 * -num6;
			num5 += num3 * num8 * -num6;
			num2 *= lacunarity;
			num3 *= gain * ManagedNoise.Saturate(num);
		}
		return num * amplitude;
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x00095B0C File Offset: 0x00093D0C
	public static double BillowWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num7;
			double num8;
			double num6 = ManagedNoise.Simplex2D((x + warp * num4) * num2, (y + warp * num5) * num2, out num7, out num8);
			num += num3 * ManagedNoise.Abs(num6);
			num4 += num3 * num7 * -num6;
			num5 += num3 * num8 * -num6;
			num2 *= lacunarity;
			num3 *= gain * ManagedNoise.Saturate(num);
		}
		return num * amplitude;
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x00095BC4 File Offset: 0x00093DC4
	public static double RidgeWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num7;
			double num8;
			double num6 = ManagedNoise.Simplex2D((x + warp * num4) * num2, (y + warp * num5) * num2, out num7, out num8);
			num += num3 * (1.0 - ManagedNoise.Abs(num6));
			num4 += num3 * num7 * -num6;
			num5 += num3 * num8 * -num6;
			num2 *= lacunarity;
			num3 *= gain * ManagedNoise.Saturate(num);
		}
		return num * amplitude;
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x00095C88 File Offset: 0x00093E88
	public static double SharpWarp(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int i = 0; i < octaves; i++)
		{
			double num7;
			double num8;
			double num6 = ManagedNoise.Simplex2D((x + warp * num4) * num2, (y + warp * num5) * num2, out num7, out num8);
			num += num3 * (num6 * num6);
			num4 += num3 * num7 * -num6;
			num5 += num3 * num8 * -num6;
			num2 *= lacunarity;
			num3 *= gain * ManagedNoise.Saturate(num);
		}
		return num * amplitude;
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x00095D40 File Offset: 0x00093F40
	public static double Jordan(double x, double y, int octaves, double frequency, double amplitude, double lacunarity, double gain, double warp, double damp, double damp_scale)
	{
		x *= frequency;
		y *= frequency;
		double num = 0.0;
		double num2 = 1.0;
		double num3 = 1.0;
		double num4 = 0.0;
		double num5 = 0.0;
		double num6 = 0.0;
		double num7 = 0.0;
		double num8 = num2 * gain;
		for (int i = 0; i < octaves; i++)
		{
			double num10;
			double num11;
			double num9 = ManagedNoise.Simplex2D(x * num3 + num4, y * num3 + num5, out num10, out num11);
			double num12 = num9 * num9;
			double num13 = num10 * num9;
			double num14 = num11 * num9;
			num += num8 * num12;
			num4 += warp * num13;
			num5 += warp * num14;
			num6 += damp * num13;
			num7 += damp * num14;
			num3 *= lacunarity;
			num2 *= gain;
			num8 = num2 * (1.0 - damp_scale / (1.0 + (num6 * num6 + num7 * num7)));
		}
		return num * amplitude;
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x0001651C File Offset: 0x0001471C
	private static int Floor(double x)
	{
		if (x < 0.0)
		{
			return (int)x - 1;
		}
		return (int)x;
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x00016531 File Offset: 0x00014731
	private static double Abs(double x)
	{
		if (x < 0.0)
		{
			return -x;
		}
		return x;
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x00016543 File Offset: 0x00014743
	private static double Saturate(double x)
	{
		if (x > 1.0)
		{
			return 1.0;
		}
		if (x >= 0.0)
		{
			return x;
		}
		return 0.0;
	}
}
