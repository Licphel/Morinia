using Kinetic.App;
using Morinia.Content.TheBlock;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content;

public class Blocks
{

	public static readonly IDMap<Block> Registry = new IDMap<Block>();

	public static Block Empty = Registry.RegisterDefaultValue("empty", new Block());

	public static Block Border = Registry.Register("border", new BlockBorder());

	public static Block Flower = Registry.Register("flower", new BlockPlant());
	public static Block Grass = Registry.Register("grass", new BlockPlant());
	public static Block Bush = Registry.Register("bush", new BlockPlant());
	public static Block Vine = Registry.Register("vine", new BlockPlantHang());

	public static Block BirchSeedling = Registry.Register("birch_seedling", new BlockSeedling(() => Features.BirchTree));
	public static Block BirchLog = Registry.Register("birch_log", new BlockLog());
	public static Block BirchLeaves = Registry.Register("birch_leaves", new BlockLeaves());
	public static Block MapleSeedling = Registry.Register("maple_seedling", new BlockSeedling(() => Features.MapleTree));
	public static Block MapleLog = Registry.Register("maple_log", new BlockLog());
	public static Block MapleLeaves = Registry.Register("maple_leaves", new BlockLeaves());

	public static Block Limestone = Registry.Register("limestone", new BlockStone());
	public static Block LimestoneSoil = Registry.Register("limestone_soil", new BlockSoil());
	public static Block Phyllite = Registry.Register("phyllite", new BlockStone());
	public static Block PhylliteSoil = Registry.Register("phyllite_soil", new BlockSoil());
	public static Block Basalt = Registry.Register("basalt", new BlockStone());
	public static Block BasaltSoil = Registry.Register("basalt_soil", new BlockSoil());
	public static Block Granite = Registry.Register("granite", new BlockStone());
	public static Block GraniteSoil = Registry.Register("granite_soil", new BlockSoil());
	public static Block Diorite = Registry.Register("diorite", new BlockStone());
	public static Block DioriteSoil = Registry.Register("diorite_soil", new BlockSoil());

	public static Block CoalOre = Registry.Register("coal_ore", new BlockStone());
	public static Block CopperOre = Registry.Register("copper_ore", new BlockStone());
	public static Block IronOre = Registry.Register("iron_ore", new BlockStone());

	public static Block Ladder = Registry.Register("ladder", new BlockLadder());
	public static Block Chest = Registry.Register("chest", new BlockChest());
	public static Block Furnace = Registry.Register("furnace", new BlockFurnace());

	static Blocks()
	{
		Geolayer.Layers.Add(new(NativeStoneType.Sedimentary, new Geolayer(0.1f, [LimestoneSoil.Instantiate(1), LimestoneSoil.Instantiate(), Limestone.Instantiate(), Limestone.Instantiate()])));
		Geolayer.Layers.Add(new(NativeStoneType.Metamorphic, new Geolayer(0.2f, [PhylliteSoil.Instantiate(1), PhylliteSoil.Instantiate(), Phyllite.Instantiate(), Phyllite.Instantiate()])));
		Geolayer.Layers.Add(new(NativeStoneType.Effusive, new Geolayer(0.3f, [BasaltSoil.Instantiate(1), BasaltSoil.Instantiate(), Basalt.Instantiate(), Basalt.Instantiate()])));
		Geolayer.Layers.Add(new(NativeStoneType.Intruded, new Geolayer(0.4f, [GraniteSoil.Instantiate(1), GraniteSoil.Instantiate(), Granite.Instantiate(), Granite.Instantiate()])));
		Geolayer.Layers.Add(new(NativeStoneType.Intruded, new Geolayer(0.5f, [DioriteSoil.Instantiate(1), DioriteSoil.Instantiate(), Diorite.Instantiate(), Diorite.Instantiate()])));
	}

}
