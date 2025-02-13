using Morinia.Content;
using Morinia.World.TheBlock;

namespace Morinia.World;

public unsafe class BlockMap
{

	readonly int SX;
	readonly int SY;
	readonly int SZ;
	public byte[] Bytes;
	public BlockState Default;

	public BlockMapScale Scale;

	public BlockMap(BlockState defval, BlockMapScale scl)
	{
		Default = defval;
		Scale = scl;
		Bytes = new byte[Scale.SizeInBytes];

		SX = scl.SizeX;
		SY = scl.SizeY;
		SZ = scl.SizeZ;
	}

	public int Index(int x, int y, int z)
	{
		x &= SX - 1;
		y &= SY - 1;
		return ((x * SY + y) * SZ + z) * (sizeof(int) + 1);
	}

	protected void WriteBytes(int idx, int v)
	{
		fixed(byte* p = Bytes)
		{
			*(p + idx + 0) = (byte) (v >> (0 << 3) & 0xff);
			*(p + idx + 1) = (byte) (v >> (1 << 3) & 0xff);
			*(p + idx + 2) = (byte) (v >> (2 << 3) & 0xff);
			*(p + idx + 3) = (byte) (v >> (3 << 3) & 0xff);
		}
	}

	protected int ReadBytes(int idx)
	{
		int ri = 0;

		fixed(byte* p = Bytes)
		{
			ri <<= 8;
			ri |= *(p + idx + 3);
			ri <<= 8;
			ri |= *(p + idx + 2);
			ri <<= 8;
			ri |= *(p + idx + 1);
			ri <<= 8;
			ri |= *(p + idx + 0);
		}

		return ri;
	}

	protected byte ReadByte(int idx)
	{
		return Bytes[idx];
	}

	protected void WriteByte(int idx, byte v)
	{
		Bytes[idx] = v;
	}

	public BlockState Set(int x, int y, int z, BlockState obj)
	{
		int idx = Index(x, y, z);
		int oldid = ReadBytes(idx);
		int oldmeta = ReadByte(idx + sizeof(int));
		WriteBytes(idx, obj.Block.Uid);
		WriteByte(idx + sizeof(int), (byte) obj.Meta);
		Block old = Blocks.Registry[oldid];
		BlockState oldstate = old.GetOrCreatePalette(oldmeta);
		return oldstate;
	}

	public BlockState Get(int x, int y, int z)
	{
		int idx = Index(x, y, z);
		int id = ReadBytes(idx);
		int meta = ReadByte(idx + sizeof(int));
		Block block = Blocks.Registry[id];
		BlockState state = block.GetOrCreatePalette(meta);
		return state;
	}

	public void Cover(BlockMap map)
	{
		Foreach((x, y, z) =>
		{
			int idx = Index(x, y, z);
			int id1 = ReadBytes(idx);

			if(id1 != 0)// 0 is default id.
			{
				int meta = ReadByte(idx + sizeof(int));
				map.WriteBytes(idx, id1);
				map.WriteByte(idx + sizeof(int), (byte) meta);
			}
		});
	}

	public void Foreach(XyzForeach r)
	{
		for(int x = 0; x < Scale.SizeX; x++)
		for(int y = 0; y < Scale.SizeY; y++)
		for(int z = 0; z < Scale.SizeZ; z++)
		{
			r.Invoke(x, y, z);
		}
	}

	public delegate void XyzForeach(int x, int y, int z);

}

public class BlockMapScale
{

	public int Size;
	public int SizeInBytes;

	public int SizeX;
	public int SizeY;
	public int SizeZ;

	public BlockMapScale(int x, int y, int z)
	{
		SizeX = x;
		SizeY = y;
		SizeZ = z;
		Size = SizeX * SizeY * SizeZ;
		SizeInBytes = Size * (sizeof(int) + 1);
	}

}
