using Morinia.Content.TheAccessBridge;
using Morinia.Content.TheBlockEntity;
using Morinia.Content.TheEntity;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheBlock;

public class BlockChest : Block
{

	public override float GetHardness(BlockState state)
	{
		return 3;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Wooden;
	}

	public override VoxelOutline GetOutlineForPhysics(BlockState state, Movable bounder)
	{
		return VoxelOutline.Void;
	}

	public override BlockShape GetShape(BlockState state)
	{
		return BlockShape.Hollow;
	}

	public override BlockEntity CreateEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return new BlockEntityChest(state, level, pos);
	}

	public override AccessBridge CreateAccessBridge(BlockPos pos, EntityPlayer player)
	{
		return new AccessBridgeChest(pos, player);
	}

	public override bool HasAccessBridge(BlockPos pos, EntityPlayer player)
	{
		return true;
	}

	public override bool HasEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return true;
	}

}
