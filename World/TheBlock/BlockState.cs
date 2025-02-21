using Morinia.Content;
using Morinia.Content.TheEntity;
using Morinia.Util;
using Morinia.World.TheDict;
using Morinia.World.TheItem;
using Morinia.World.ThePhysic;
using Morinia.World.TheRecipe;

namespace Morinia.World.TheBlock;

public sealed class BlockState : IMetable
{

	public static readonly BlockState Empty = Blocks.Empty.Instantiate();

	public Block Block;
	public int Meta { get; private set; }
	public bool IsEmpty => Block == Blocks.Empty || Block == null || this == Empty;
	public IntPair StateUid;

	public BlockState(Block block)
	{
		Block = block;
		SetMeta(0);
	}

	public BlockState SetMeta(int meta)
	{
		Meta = meta;
		StateUid = IntPair.Create(Block.Uid, Meta);
		return this;
	}

	//Paletting Requirements

	public override bool Equals(object obj)
	{
		return obj is BlockState state && state.Block == Block && state.Meta == Meta;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Block, Meta);
	}

	//Comparison

	public bool Is(Tag tag)
	{
		return tag.Contains(Block);
	}

	public bool Is(BlockState state)
	{
		return state.Is(Block);
	}

	public bool IsStrictly(BlockState state)
	{
		return state.Block == Block && state.Meta == Meta;
	}

	//Instance Relative

	public BlockMaterial GetMaterial()
	{
		return Block.GetMaterial(this);
	}

	public float GetHardness()
	{
		return Block.GetHardness(this);
	}

	public VoxelOutline GetOutlineForHover()
	{
		return Block.GetOutlineForHover(this);
	}

	public VoxelOutline GetOutlineForPhysics(Movable bounder)
	{
		return Block.GetOutlineForPhysics(this, bounder);
	}

	public float GetFloatingForce(Movable bounder)
	{
		return Block.GetFloatingForce(this, bounder);
	}

	public float GetFricitonForce(Movable bounder)
	{
		return Block.GetFrictionForce(this, bounder);
	}

	public float GetAntiForce(Movable bounder)
	{
		return Block.GetAntiForce(this, bounder);
	}

	public BlockShape GetShape()
	{
		return Block.GetShape(this);
	}

	public float CastLight(byte pipe, int x, int y)
	{
		return Block.CastLight(this, pipe, x, y);
	}

	public float FilterLight(byte pipe, float v, int x, int y)
	{
		return Block.FilterLight(this, pipe, v, x, y);
	}

	public BlockEntity CreateEntityBehavior(Level level, BlockPos pos)
	{
		return Block.CreateEntityBehavior(this, level, pos);
	}

	public AccessBridge CreateCABridge(BlockPos pos, EntityPlayer player)
	{
		return Block.CreateAccessBridge(pos, player);
	}

	public bool HasEntityBehavior(Level level, BlockPos pos)
	{
		return Block.HasEntityBehavior(this, level, pos);
	}

	public bool HasCABridge(BlockPos pos, EntityPlayer player)
	{
		return Block.HasAccessBridge(pos, player);
	}

	public List<ItemStack> GetDrop(Level level, BlockPos pos)
	{
		return Block.GetDrop(this, level, pos);
	}

	public void OnRandomTick(Level level, BlockPos pos)
	{
		Block.OnRandomTick(this, level, pos);
	}

	public void OnNearbyChanged(Level level, BlockPos pos, BlockPos changed)
	{
		Block.OnNearbyChanged(this, level, pos, changed);
	}
	
}
