using Kinetic.Math;
using Morinia.Content;
using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.World.TheItem;

public class ItemBlockPlacer : Item
{

	public static Dictionary<Item, Block> PlacingMap = new Dictionary<Item, Block>();
	public static Dictionary<Block, ItemBlockPlacer> PickingMap = new Dictionary<Block, ItemBlockPlacer>();

	Block Placement;

	public ItemBlockPlacer(Block block)
	{
		Placement = block;
		PlacingMap[this] = block;
		PickingMap[block] = this;
	}

	public static ItemStack MakeFrom(BlockState state, int count = 1)
	{
		if(!PickingMap.ContainsKey(state.Block))
		{
			return ItemStack.Empty;
		}
		return PickingMap[state.Block].Instantiate(count);
	}

	public override InterResult OnClickBlock(ItemStack stack, Entity entity, Level level, IPos pos, bool sim = false)
	{
		if(!level.IsAccessible(pos))
		{
			return InterResult.Fail;
		}

		BlockState state = GetStateForPlacing(stack, entity, level, pos);
		BlockState prev = level.GetBlock(pos);

		if(!stack.IsEmpty)
		{
			if(prev.GetShape() == BlockShape.Vacuum && !prev.Equals(state)
			                                       && (!CheckEntitiesAnyInteractWithBlock(level, state, pos) || pos.BlockZ == 0))
			{
				if(!sim)
				{
					level.SetBlock(state, pos);
					stack.Count--;
				}
				return InterResult.Success;
			}
			return InterResult.Fail;
		}

		return InterResult.Pass;
	}

	public virtual BlockState GetStateForPlacing(ItemStack stack, Entity entity, Level level, IPos pos)
	{
		Block block = GetBlockPlaced(stack);
		return block.GetStateForPlacing(level, entity, pos, stack);
	}

	public virtual Block GetBlockPlaced(ItemStack stack)
	{
		return PlacingMap[stack.Item];
	}

	public static bool CheckEntitiesAnyInteractWithBlock(Level level, BlockState state, IPos pos)
	{
		List<Entity> lst = new List<Entity>();
		Box aabb = new Box();
		aabb.LocateCentral(pos.x, pos.y);
		aabb.Resize(16, 16);
		level.GetNearbyEntities(lst, aabb);

		foreach(Entity e in lst)
		{
			if(state.GetOutlineForPhysics(e).Interacts(e.Box, pos.BlockX, pos.BlockY))
			{
				return true;
			}
		}

		return false;
	}

}
