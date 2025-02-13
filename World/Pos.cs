using Kinetic.Math;

namespace Morinia.World;

public class Posing
{

	public static BlockPos Offset(BlockPos pos, int dx, int dy, int dz = 0)
	{
		return new BlockPos(pos.BlockX + dx, pos.BlockY + dy, pos.BlockZ + dz);
	}

	public static float Distance(IPos pos1, IPos pos2)
	{
		return FloatMath.Sqrt(Distance2(pos1, pos2));
	}

	public static float Distance2(IPos pos1, IPos pos2)
	{
		return FloatMath.Pow(pos1.x - pos2.x, 2) + FloatMath.Pow(pos1.y - pos2.y, 2);
	}

	public static float PointRad(IPos pos1, IPos pos2)
	{
		return FloatMath.AtanRad(pos2.y - pos1.y, pos2.x - pos1.x);
	}

	public static float PointDeg(IPos pos1, IPos pos2)
	{
		return FloatMath.AtanDeg(pos2.y - pos1.y, pos2.x - pos1.x);
	}

	public static int ToCoord(int block)
	{
		return FloatMath.Floor(block / 16f);
	}

}

//1 Chunk = 16 TheBlock;
//1 Grid = 16 Pixel;
//1 Pixel => N Precise;
public interface IPos
{

	public float x { get; }
	public float y { get; }
	public int BlockX { get; }
	public int BlockY { get; }
	public int UnitX { get; }
	public int UnitY { get; }

	//Default impl at z = 1.
	public int BlockZ => 1;

}

public struct PrecisePos : IPos
{

	public PrecisePos(IPos grid)
	{
		x = grid.x;
		y = grid.y;
	}

	public PrecisePos(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public float x { get; }
	public float y { get; }
	public int BlockX => FloatMath.Floor(x);
	public int BlockY => FloatMath.Floor(y);
	public int UnitX => FloatMath.Floor(BlockX / 16f);
	public int UnitY => FloatMath.Floor(BlockY / 16f);

	public override bool Equals(object obj)
	{
		return obj is PrecisePos other && Equals(other);
	}

	public bool Equals(PrecisePos other)
	{
		return x.Equals(other.x) && y.Equals(other.y);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(x, y);
	}

	public static PrecisePos operator +(PrecisePos p1, PrecisePos p2)
	{
		return new PrecisePos(p1.x + p2.x, p1.y + p2.y);
	}

	public static PrecisePos operator -(PrecisePos p1, PrecisePos p2)
	{
		return new PrecisePos(p1.x - p2.x, p1.y - p2.y);
	}

	public static PrecisePos operator -(PrecisePos p)
	{
		return new PrecisePos(-p.x, -p.y);
	}

	public static bool operator ==(PrecisePos p1, PrecisePos p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(PrecisePos p1, PrecisePos p2)
	{
		return !p1.Equals(p2);
	}

	public override string ToString()
	{
		return $"precise[{x} {y}]";
	}

}

public struct BlockPos : IPos
{

	public static int DefaultZDep = 1;

	public BlockPos(IPos grid, int z)
	{
		BlockX = grid.BlockX;
		BlockY = grid.BlockY;
		BlockZ = z;
	}

	public BlockPos(IPos grid)
	{
		BlockX = grid.BlockX;
		BlockY = grid.BlockY;
		BlockZ = grid is BlockPos g2 ? g2.BlockZ : DefaultZDep;
	}

	public BlockPos(int x, int y, int z)
	{
		BlockX = x;
		BlockY = y;
		BlockZ = z;
	}

	public BlockPos(int x, int y)
	{
		BlockX = x;
		BlockY = y;
		BlockZ = DefaultZDep;
	}

	public float x => BlockX + 0.5f;
	public float y => BlockY + 0.5f;
	public int BlockX { get; }
	public int BlockY { get; }
	public int BlockZ { get; }
	public int UnitX => FloatMath.Floor(BlockX / 16f);
	public int UnitY => FloatMath.Floor(BlockY / 16f);

	public override bool Equals(object obj)
	{
		return obj is BlockPos other && Equals(other);
	}

	public bool Equals(BlockPos other)
	{
		return BlockX == other.BlockX && BlockY == other.BlockY && BlockZ == other.BlockZ;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(BlockX, BlockY, BlockZ);
	}

	public static BlockPos operator +(BlockPos p1, BlockPos p2)
	{
		return new BlockPos(p1.BlockX + p2.BlockX, p1.BlockY + p2.BlockY, p1.BlockZ + p2.BlockZ);
	}

	public static BlockPos operator -(BlockPos p1, BlockPos p2)
	{
		return new BlockPos(p1.BlockX - p2.BlockX, p1.BlockY - p2.BlockY, p1.BlockZ - p2.BlockZ);
	}

	public static BlockPos operator -(BlockPos p)
	{
		return new BlockPos(-p.BlockX, -p.BlockY, -p.BlockZ);
	}

	public static bool operator ==(BlockPos p1, BlockPos p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(BlockPos p1, BlockPos p2)
	{
		return !p1.Equals(p2);
	}

	public override string ToString()
	{
		return $"block[{BlockX} {BlockY} {BlockZ}]";
	}

}

public struct UnitPos : IPos
{

	public UnitPos(IPos grid)
	{
		UnitX = grid.UnitX;
		UnitY = grid.UnitY;
	}

	public UnitPos(int x, int y)
	{
		UnitX = x;
		UnitY = y;
	}

	public float x => BlockX;
	public float y => BlockY;
	public int BlockX => UnitX * 16;
	public int BlockY => UnitY * 16;
	public int UnitX { get; }
	public int UnitY { get; }

	public override bool Equals(object obj)
	{
		return obj is UnitPos other && Equals(other);
	}

	public bool Equals(UnitPos other)
	{
		return UnitX == other.UnitX && UnitY == other.UnitY;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(UnitX, UnitY);
	}

	public static UnitPos operator +(UnitPos p1, UnitPos p2)
	{
		return new UnitPos(p1.UnitX + p2.UnitX, p1.UnitY + p2.UnitY);
	}

	public static UnitPos operator -(UnitPos p1, UnitPos p2)
	{
		return new UnitPos(p1.UnitX - p2.UnitX, p1.UnitY - p2.UnitY);
	}

	public static UnitPos operator -(UnitPos p)
	{
		return new UnitPos(-p.UnitX, -p.UnitY);
	}

	public static bool operator ==(UnitPos p1, UnitPos p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(UnitPos p1, UnitPos p2)
	{
		return !p1.Equals(p2);
	}

	public override string ToString()
	{
		return $"unit[{UnitX} {UnitY}]";
	}

}
