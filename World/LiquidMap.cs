using Morinia.Content;
using Morinia.World.TheBlock;
using Morinia.World.TheLiquid;

namespace Morinia.World;

public unsafe class LiquidMap
{

	readonly int SX;
	readonly int SY;
	public byte[] Bytes;
	public Liquid Default;

	public LiquidMapScale Scale;

	public LiquidMap(Liquid defval, LiquidMapScale scl)
	{
		Default = defval;
		Scale = scl;
		Bytes = new byte[Scale.SizeInBytes];

		SX = scl.SizeX;
		SY = scl.SizeY;
	}

	public int Index(int x, int y)
	{
		x &= SX - 1;
		y &= SY - 1;
		return (x * SY + y) * (sizeof(int) + 1);
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

	public LiquidStack Set(int x, int y, LiquidStack obj)
	{
		int idx = Index(x, y);
		int oldid = ReadBytes(idx);
		int olda = ReadByte(idx + sizeof(int));
		WriteBytes(idx, obj.Liquid.Uid);
		WriteByte(idx + sizeof(int), (byte) obj.Amount);
		Liquid old = Liquids.Registry[oldid];
		LiquidStack oldstack = new LiquidStack(old, olda);
		return oldstack;
	}

	public LiquidStack Get(int x, int y)
	{
		int idx = Index(x, y);
		int id = ReadBytes(idx);
		int a = ReadByte(idx + sizeof(int));
		Liquid lq = Liquids.Registry[id];
		LiquidStack stack = new LiquidStack(lq, a);
		return stack;
	}

	public void Cover(LiquidMap map)
	{
		Foreach((x, y) =>
		{
			int idx = Index(x, y);
			int id1 = ReadBytes(idx);

			if(id1 != 0)// 0 is default id.
			{
				int meta = ReadByte(idx + sizeof(int));
				map.WriteBytes(idx, id1);
				map.WriteByte(idx + sizeof(int), (byte) meta);
			}
		});
	}

	public void Foreach(XyForeach r)
	{
		for(int x = 0; x < Scale.SizeX; x++)
		for(int y = 0; y < Scale.SizeY; y++)
		{
			r.Invoke(x, y);
		}
	}

	public delegate void XyForeach(int x, int y);

}

public class LiquidMapScale
{

	public int Size;
	public int SizeInBytes;

	public int SizeX;
	public int SizeY;

	public LiquidMapScale(int x, int y)
	{
		SizeX = x;
		SizeY = y;
		Size = SizeX * SizeY;
		SizeInBytes = Size * (sizeof(int) + 1);
	}

}
