using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.World;

public abstract class ChunkBasic
{

	public abstract ChunkUnit[] Units { get; }

	public virtual ChunkUnit GetUnitSafely(int ycoord)
	{
		ycoord = FloatMath.Clamp(ycoord, 0, Units.Length - 1);
		return Units[ycoord];
	}

	public virtual BlockState GetBlock(IPos pos)
	{
		return GetBlock(pos.BlockX, pos.BlockY, pos.BlockZ);
	}

	public abstract BlockState GetBlock(int x, int y, int z = 1);

	public abstract BlockState SetBlock(BlockState state, int x, int y, int z = 1);

	public virtual BlockState SetBlock(BlockState state, IPos pos)
	{
		return SetBlock(state, pos.BlockX, pos.BlockY, pos.BlockZ);
	}

	public abstract BlockEntity GetBlockEntity(IPos pos);

	public virtual BlockEntity GetBlockEntity(int x, int y, int z = 1)
	{
		return GetBlockEntity(new BlockPos(x, y, z));
	}

	public BlockEntity SetBlockEntity(BlockEntity entity, int x, int y)
	{
		return SetBlockEntity(entity, x, y, 1);
	}

	public virtual BlockEntity SetBlockEntity(BlockEntity entity, int x, int y, int z)
	{
		return SetBlockEntity(entity, new BlockPos(x, y, z));
	}

	public abstract BlockEntity SetBlockEntity(BlockEntity entity, IPos pos);

}
