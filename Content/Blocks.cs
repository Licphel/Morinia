using Kinetic.App;
using Kinetic.Math;
using Morinia.Content.TheBlock;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content;

public class Blocks
{

	public static readonly IDMap<Block> Registry = new IDMap<Block>();

	public static Block Empty = Registry.RegisterDefaultValue("empty", new Block());

	public static Block Border = Registry.Register("border", new BlockBorder()).DeriveItem();

	//PLANTS
	public static Block Flower = Registry.Register("flower", new BlockPlant());
	public static Block Grass = Registry.Register("grass", new BlockPlant());
	public static Block Bush = Registry.Register("bush", new BlockPlant());
	public static Block Vine = Registry.Register("vine", new BlockPlantHang());

	//TREES
	public static Block BirchSeedling = Registry.Register("birch_seedling", new BlockSeedling(() => Features.BirchTree)).DeriveItem();
	public static Block BirchLog = Registry.Register("birch_log", new BlockLog()).DeriveItem();
	public static Block BirchLeaves = Registry.Register("birch_leaves", new BlockLeaves()).DeriveItem();
	public static Block MapleSeedling = Registry.Register("maple_seedling", new BlockSeedling(() => Features.MapleTree)).DeriveItem();
	public static Block MapleLog = Registry.Register("maple_log", new BlockLog()).DeriveItem();
	public static Block MapleLeaves = Registry.Register("maple_leaves", new BlockLeaves()).DeriveItem();

	//STONES
	//-SEDIMENTARY
	public static Block Limestone = Registry.Register("limestone", new BlockRock()).DeriveItem();
	public static Block LimestoneSoil = Registry.Register("limestone_soil", new BlockSoil()).DeriveItem();
	public static Block LimestoneCoalOre = Registry.Register("limestone_coal_ore", new BlockRock()).DeriveItem();

	public static Block Hornstone = Registry.Register("hornstone", new BlockRock()).DeriveItem();
	public static Block HornstoneSoil = Registry.Register("hornstone_soil", new BlockSoil()).DeriveItem();
	public static Block HornstoneCoalOre = Registry.Register("hornstone_coal_ore", new BlockRock()).DeriveItem();

	//-METAMORPHIC
	public static Block Phyllite = Registry.Register("phyllite", new BlockRock()).DeriveItem();
	public static Block PhylliteSoil = Registry.Register("phyllite_soil", new BlockSoil()).DeriveItem();

	public static Block Marble = Registry.Register("marble", new BlockRock()).DeriveItem();
	public static Block MarbleSoil = Registry.Register("marble_soil", new BlockSoil()).DeriveItem();

	//-EFFUSIVE
	public static Block Basalt = Registry.Register("basalt", new BlockRock()).DeriveItem();
	public static Block BasaltSoil = Registry.Register("basalt_soil", new BlockSoil()).DeriveItem();
	public static Block BasaltIronOre = Registry.Register("basalt_iron_ore", new BlockRock()).DeriveItem();

	//-INTRUDED
	public static Block Granite = Registry.Register("granite", new BlockRock()).DeriveItem();
	public static Block GraniteSoil = Registry.Register("granite_soil", new BlockSoil()).DeriveItem();
	public static Block GraniteIronOre = Registry.Register("granite_iron_ore", new BlockRock()).DeriveItem();

	public static Block Diorite = Registry.Register("diorite", new BlockRock()).DeriveItem();
	public static Block DioriteSoil = Registry.Register("diorite_soil", new BlockSoil()).DeriveItem();
	public static Block DioriteIronOre = Registry.Register("diorite_iron_ore", new BlockRock()).DeriveItem();

	//FURNITURE
	public static Block Ladder = Registry.Register("ladder", new BlockLadder()).DeriveItem();
	public static Block Chest = Registry.Register("chest", new BlockChest()).DeriveItem();
	public static Block Furnace = Registry.Register("furnace", new BlockFurnace()).DeriveItem();

	static Blocks()
	{
		RegisterRockSerie(NativeStoneType.Sedimentary, "limestone");
		RegisterRockSerie(NativeStoneType.Sedimentary, "hornstone");
		RegisterRockSerie(NativeStoneType.Metamorphic, "phyllite");
		RegisterRockSerie(NativeStoneType.Metamorphic, "marble");
		RegisterRockSerie(NativeStoneType.Effusive, "basalt");
		RegisterRockSerie(NativeStoneType.Intruded, "granite");
		RegisterRockSerie(NativeStoneType.Intruded, "diorite");
	}

	static RockSerie RegisterRockSerie(NativeStoneType type, string name)
	{
		var b1 = Registry[name];
		var b2 = Registry[name + "_soil"];
		Seed seed = new SeedLCG(b1.Uid);
		RockSerie serie = new RockSerie(seed.NextFloat(), [b2.Instantiate(1), b2.Instantiate(0), b1.Instantiate(), b1.Instantiate()]);
		RockSerie.Layers.Add(new(type, serie));
		return serie;
	}

}
