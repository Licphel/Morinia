using Morinia.World.TheItem;

namespace Morinia.Content.TheItem;

public class ItemPickaxe : Item
{

	public override ItemToolType GetToolType(ItemStack stack)
	{
		return ItemToolType.Pickaxe;
	}

	public override float GetToolEfficiency(ItemStack stack)
	{
		return float.PositiveInfinity;
	}

}
