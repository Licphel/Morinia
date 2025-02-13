using Kinetic.App;
using Kinetic.Visual;
using Morinia.World.TheItem;

namespace Morinia.Client.TheItem;

public class ItemRendering
{

	public static ItemTessellator GetTessellator(ItemStack stack)
	{
		return ItemTessellator.Normal;
	}

	public static Icon GetIcon(ItemStack stack)
	{
		return Loads.Get($"textures/items/{stack.Item.Uid.Key}.png");
	}

}
