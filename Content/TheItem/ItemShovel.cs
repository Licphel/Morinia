using Morinia.World.TheItem;

namespace Morinia.Content.TheItem;

public class ItemShovel : Item
{

	public override ItemToolType GetToolType(ItemStack stack)
	{
		return ItemToolType.Shovel;
	}

	public override float GetToolEfficiency(ItemStack stack)
	{
		return float.PositiveInfinity;
	}

}
