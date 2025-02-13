using Kinetic.IO;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;

namespace Morinia.Content.TheBlockEntity;

public class BlockEntityChest : BlockEntity
{

	public Inventory Inv = new Inventory(27);

	public BlockEntityChest(BlockState state, Level level, BlockPos pos) : base(state, level, pos)
	{
	}

	public override void OverrideBlockDrops(List<ItemStack> stacks)
	{
		base.OverrideBlockDrops(stacks);

		foreach(ItemStack stk in Inv)
		{
			if(!stk.IsEmpty) stacks.Add(stk);
		}
	}

	public override void Read(IBinaryCompound compound)
	{
		base.Read(compound);
		Inv = Inventory.Deserialize(compound.GetCompoundSafely("cinv"));
	}

	public override void Write(IBinaryCompound compound)
	{
		base.Write(compound);
		compound.Set("cinv", Inventory.Serialize(Inv));
	}

}
