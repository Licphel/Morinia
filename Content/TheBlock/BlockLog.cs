using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheBlock;

public class BlockLog : Block
{

	public override VoxelOutline GetOutlineForPhysics(BlockState inst, Movable bounder)
	{
		return inst.Meta == 1 ? VoxelOutline.Void : VoxelOutline.Cube;
	}

	public override float GetHardness(BlockState state)
	{
		return 5;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Wooden;
	}

}
