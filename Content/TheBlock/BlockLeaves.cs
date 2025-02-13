using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheBlock;

public class BlockLeaves : Block
{

	public override VoxelOutline GetOutlineForPhysics(BlockState inst, Movable bounder)
	{
		return inst.Meta == 1 ? VoxelOutline.Void : VoxelOutline.Cube;
	}

	public override float GetHardness(BlockState state)
	{
		return 0.25f;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Foliage;
	}

	public override BlockShape GetShape(BlockState state)
	{
		return BlockShape.Parital;
	}

}
