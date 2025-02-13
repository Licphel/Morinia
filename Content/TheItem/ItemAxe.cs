using Morinia.World.TheItem;

namespace Morinia.Content.TheItem;

public class ItemAxe : Item
{

	public override ItemToolType GetToolType(ItemStack stack)
	{
		return ItemToolType.Axe;
	}

	public override float GetToolEfficiency(ItemStack stack)
	{
		return float.PositiveInfinity;
	}

}
