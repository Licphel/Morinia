using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheBlock;

public class BlockPlantHang : Block
{

	public override float GetHardness(BlockState state)
	{
		return 0.01f;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Foliage;
	}

	public override VoxelOutline GetOutlineForPhysics(BlockState state, Movable bounder)
	{
		return VoxelOutline.Void;
	}

	public override BlockShape GetShape(BlockState state)
	{
		return BlockShape.Hollow;
	}

	public override void OnNearbyChanged(BlockState state, Level level, BlockPos pos, BlockPos changed)
	{
		BlockState state1 = level.GetBlock(Direction.Up.Step(pos));

		if(!state1.Is(Tags.BlockCarvable) && !state1.Is(this))
		{
			level.BreakBlock(pos);
		}
	}

}
