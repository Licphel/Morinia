using Kinetic.App;
using Kinetic.Math;
using Morinia.Content;
using Morinia.Content.TheEntity;
using Morinia.World.TheDict;
using Morinia.World.TheItem;
using Morinia.World.TheLoot;
using Morinia.World.ThePhysic;
using Morinia.World.TheRecipe;

namespace Morinia.World.TheBlock;

public class Block : Tag
{

	//INJECT
	public static Block Empty => Blocks.Empty;

	// Paletting.
	public Dictionary<int, BlockState> StatePaletteCache = new();

	public BlockState GetOrCreatePalette(int meta)
	{
		if(StatePaletteCache.TryGetValue(meta, out BlockState state))
			return state;
		return StatePaletteCache[meta] = Instantiate(meta);
	}
	// End

	public override bool Contains<T>(T o)
	{
		return o is Block block && block == this;
	}

	public virtual BlockState Instantiate(int meta = 0)
	{
		return new BlockState(this).SetMeta(meta);
	}

	//Instance Relative

	public virtual BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Powder;
	}

	// 1 means 1 second to break it with bare hands.
	public virtual float GetHardness(BlockState state)
	{
		return 1;
	}

	public virtual VoxelOutline GetOutlineForHover(BlockState state)
	{
		return VoxelOutline.Cube;
	}

	public virtual VoxelOutline GetOutlineForPhysics(BlockState state, Movable bounder)
	{
		return VoxelOutline.Cube;
	}

	//1 is keeping still, > 1 is pushing upwards, (0, 1) is not able to hold an entity, 0 is no effect.
	public virtual float GetFloatingForce(BlockState state, Movable bounder)
	{
		return 0;
	}

	//velocity will times this value.
	//so, 1 is no friction, 0 is infinite friciton.
	//(tips: theres's still air friciton)
	public virtual float GetFrictionForce(BlockState state, Movable bounder)
	{
		return 0.9f;
	}

	//when entity is inside the block, this force make effects.
	//it resembles friction force.
	public virtual float GetAntiForce(BlockState state, Movable bounder)
	{
		return 1;
	}

	public virtual BlockShape GetShape(BlockState state)
	{
		return state.IsEmpty ? BlockShape.Vacuum : BlockShape.Solid;
	}

	public virtual float CastLight(BlockState state, byte pipe, int x, int y)
	{
		return 0;
	}

	public virtual float FilterLight(BlockState state, byte pipe, float v, int x, int y)
	{
		return GetShape(state).FilterLight(state, pipe, v, x, y);
	}

	public virtual BlockEntity CreateEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return null;
	}

	public virtual AccessBridge CreateAccessBridge(BlockPos pos, EntityPlayer player)
	{
		return null;
	}

	public virtual bool HasEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return false;
	}

	public virtual bool HasAccessBridge(BlockPos pos, EntityPlayer player)
	{
		return false;
	}

	public virtual List<ItemStack> GetDrop(BlockState state, Level level, BlockPos pos)
	{
		List<ItemStack> list = new List<ItemStack>();

		Loot loot = LootManager.Get(new ID(Uid.Space, "blocks/" + Uid.Key));

		if(loot == null)
		{
			if(!state.IsEmpty)
			{
				ItemStack stack = ItemBlockPlacer.MakeFrom(state);
				if(!stack.IsEmpty) list.Add(stack);
			}
		}
		else
		{
			loot.Generate(list, Seed.Global);
		}

		BlockEntity te = level.GetBlockEntity(pos);
		if(te != null) te.OverrideBlockDrops(list);

		return list;
	}

	public virtual BlockState GetStateForPlacing(Level level, Entity entity, IPos pos, ItemStack placerItem)
	{
		return Instantiate();
	}

	public virtual void OnRandomTick(BlockState state, Level level, BlockPos pos)
	{
	}

	public virtual void OnNearbyChanged(BlockState state, Level level, BlockPos pos, BlockPos changed)
	{
	}

}
