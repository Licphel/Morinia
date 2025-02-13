using Kinetic.App;
using Morinia.Content.TheItem;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;

namespace Morinia.Content;

public class Items
{

	public static readonly IDMap<Item> Registry = new IDMap<Item>();

	public static Item Empty = Registry.RegisterDefaultValue("empty", new Item());

	public static Item Limestone = Registry.Register("limestone", new ItemBlockPlacer(Blocks.Limestone));
	public static Item LimestoneSoil = Registry.Register("limestone_soil", new ItemBlockPlacer(Blocks.LimestoneSoil));
	public static Item Phyllite = Registry.Register("phyllite", new ItemBlockPlacer(Blocks.Phyllite));
	public static Item PhylliteSoil = Registry.Register("phyllite_soil", new ItemBlockPlacer(Blocks.PhylliteSoil));
	public static Item Basalt = Registry.Register("basalt", new ItemBlockPlacer(Blocks.Basalt));
	public static Item BasaltSoil = Registry.Register("basalt_soil", new ItemBlockPlacer(Blocks.BasaltSoil));
	public static Item Granite = Registry.Register("granite", new ItemBlockPlacer(Blocks.Granite));
	public static Item GraniteSoil = Registry.Register("granite_soil", new ItemBlockPlacer(Blocks.GraniteSoil));
	public static Item Diorite = Registry.Register("diorite", new ItemBlockPlacer(Blocks.Diorite));
	public static Item DioriteSoil = Registry.Register("diorite_soil", new ItemBlockPlacer(Blocks.DioriteSoil));

	public static Item CoalOre = Registry.Register("coal_ore", new ItemBlockPlacer(Blocks.CoalOre));
	public static Item CopperOre = Registry.Register("copper_ore", new ItemBlockPlacer(Blocks.CopperOre));
	public static Item IronOre = Registry.Register("iron_ore", new ItemBlockPlacer(Blocks.IronOre));

	public static Item BirchSeedling = Registry.Register("birch_seedling", new ItemBlockPlacer(Blocks.BirchSeedling));
	public static Item BirchLog = Registry.Register("birch_log", new ItemBlockPlacer(Blocks.BirchLog));
	public static Item BirchLeaves = Registry.Register("birch_leaves", new ItemBlockPlacer(Blocks.BirchLeaves));
	public static Item MapleSeedling = Registry.Register("maple_seedling", new ItemBlockPlacer(Blocks.MapleSeedling));
	public static Item MapleLog = Registry.Register("maple_log", new ItemBlockPlacer(Blocks.MapleLog));
	public static Item MapleLeaves = Registry.Register("maple_leaves", new ItemBlockPlacer(Blocks.MapleLeaves));

	public static Item Ladder = Registry.Register("ladder", new ItemBlockPlacer(Blocks.Ladder));
	public static Item Chest = Registry.Register("chest", new ItemBlockPlacer(Blocks.Chest));
	public static Item Furnace = Registry.Register("furnace", new ItemBlockPlacer(Blocks.Furnace));

	public static Item Coal = Registry.Register("coal", new Item());
	public static Item CopperIngot = Registry.Register("copper_ingot", new Item());
	public static Item IronIngot = Registry.Register("iron_ingot", new Item());
	public static Item IronPickaxe = Registry.Register("iron_pickaxe", new ItemPickaxe());
	public static Item IronAxe = Registry.Register("iron_axe", new ItemAxe());
	public static Item IronShovel = Registry.Register("iron_shovel", new ItemShovel());

}
