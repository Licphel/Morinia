using Kinetic.App;
using Morinia.Content.TheItem;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;

namespace Morinia.Content;

public class Items
{

	public static readonly IDMap<Item> Registry = new IDMap<Item>();

	public static Item Empty = Registry.RegisterDefaultValue("empty", new Item());

	public static Item Coal = Registry.Register("coal", new Item());
	public static Item CopperIngot = Registry.Register("copper_ingot", new Item());
	public static Item IronIngot = Registry.Register("iron_ingot", new Item());
	public static Item IronPickaxe = Registry.Register("iron_pickaxe", new ItemPickaxe());
	public static Item IronAxe = Registry.Register("iron_axe", new ItemAxe());
	public static Item IronShovel = Registry.Register("iron_shovel", new ItemShovel());

}
