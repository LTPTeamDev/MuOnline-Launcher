using System;
using System.Security.Cryptography;

public class Crc32 : HashAlgorithm
{
	public const uint DefaultPolynomial = 3988292384u;

	public const uint DefaultSeed = 4294967295u;

	private uint hash;

	private uint seed;

	private uint[] table;

	private static uint[] defaultTable;

	public override int HashSize
	{
		get
		{
			return 32;
		}
	}

	public Crc32()
	{
		this.table = Crc32.InitializeTable(3988292384u);
		this.seed = 4294967295u;
		this.Initialize();
	}

	public Crc32(uint polynomial, uint seed)
	{
		this.table = Crc32.InitializeTable(polynomial);
		this.seed = seed;
		this.Initialize();
	}

	public override void Initialize()
	{
		this.hash = this.seed;
	}

	protected override void HashCore(byte[] buffer, int start, int length)
	{
		this.hash = Crc32.CalculateHash(this.table, this.hash, buffer, start, length);
	}

	protected override byte[] HashFinal()
	{
		byte[] array = this.UInt32ToBigEndianBytes(~this.hash);
		this.HashValue = array;
		return array;
	}

	public static uint Compute(byte[] buffer)
	{
		return ~Crc32.CalculateHash(Crc32.InitializeTable(3988292384u), 4294967295u, buffer, 0, buffer.Length);
	}

	public static uint Compute(uint seed, byte[] buffer)
	{
		return ~Crc32.CalculateHash(Crc32.InitializeTable(3988292384u), seed, buffer, 0, buffer.Length);
	}

	public static uint Compute(uint polynomial, uint seed, byte[] buffer)
	{
		return ~Crc32.CalculateHash(Crc32.InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
	}

	private static uint[] InitializeTable(uint polynomial)
	{
		if (polynomial == 3988292384u && Crc32.defaultTable != null)
		{
			return Crc32.defaultTable;
		}
		uint[] array = new uint[256];
		for (int i = 0; i < 256; i++)
		{
			uint num = (uint)i;
			for (int j = 0; j < 8; j++)
			{
				if ((num & 1u) == 1u)
				{
					num = (num >> 1 ^ polynomial);
				}
				else
				{
					num >>= 1;
				}
			}
			array[i] = num;
		}
		if (polynomial == 3988292384u)
		{
			Crc32.defaultTable = array;
		}
		return array;
	}

	private static uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
	{
		uint num = seed;
		for (int i = start; i < size; i++)
		{
			num = (num >> 8 ^ table[(int)((UIntPtr)((uint)buffer[i] ^ (num & 255u)))]);
		}
		return num;
	}

	private byte[] UInt32ToBigEndianBytes(uint x)
	{
		return new byte[]
		{
			(byte)(x >> 24 & 255u),
			(byte)(x >> 16 & 255u),
			(byte)(x >> 8 & 255u),
			(byte)(x & 255u)
		};
	}
}
