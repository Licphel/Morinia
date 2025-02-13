using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheBlock;

public class BlockLadder : Block
{

	public override float GetHardness(BlockState state)
	{
		return 0.5f;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Wooden;
	}

	public override float GetFloatingForce(BlockState state, Movable bounder)
	{
		return 1;
	}

	public override float GetAntiForce(BlockState state, Movable bounder)
	{
		return 0.85f;
	}

	public override VoxelOutline GetOutlineForPhysics(BlockState state, Movable bounder)
	{
		return VoxelOutline.Void;
	}

	public override BlockShape GetShape(BlockState state)
	{
		return BlockShape.Hollow;
	}

}
