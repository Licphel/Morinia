using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheBlock;

public class BlockPlant : Block
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
		if(!level.GetBlock(Direction.Down.Step(pos)).Is(Tags.BlockSoil))
		{
			level.BreakBlock(pos);
		}
	}

}
