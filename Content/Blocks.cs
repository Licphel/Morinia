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

	public static Block SoilPoor = Registry.Register("soil_poor", new BlockSoil()).DeriveItem();
	public static Block SoilOrdinary = Registry.Register("soil_ordinary", new BlockSoil()).DeriveItem();
	public static Block SoilLoam = Registry.Register("soil_loam", new BlockSoil()).DeriveItem();
	//STONES
	//-SEDIMENTARY
	public static Block Limestone = Registry.Register("limestone", new BlockRock()).DeriveItem();
	public static Block LimestoneCoalOre = Registry.Register("limestone_coal_ore", new BlockRock()).DeriveItem();

	public static Block Dolomite = Registry.Register("dolomite", new BlockRock()).DeriveItem();
	public static Block DolomiteCoalOre = Registry.Register("dolomite_coal_ore", new BlockRock()).DeriveItem();

	//-METAMORPHIC
	public static Block Phyllite = Registry.Register("phyllite", new BlockRock()).DeriveItem();

	public static Block Marble = Registry.Register("marble", new BlockRock()).DeriveItem();

	//-EFFUSIVE
	public static Block Basalt = Registry.Register("basalt", new BlockRock()).DeriveItem();
	public static Block BasaltIronOre = Registry.Register("basalt_iron_ore", new BlockRock()).DeriveItem();

	public static Block Dacite = Registry.Register("dacite", new BlockRock()).DeriveItem();
	public static Block DaciteIronOre = Registry.Register("dacite_iron_ore", new BlockRock()).DeriveItem();

	//-INTRUDED
	public static Block Gabbro = Registry.Register("gabbro", new BlockRock()).DeriveItem();
	public static Block GabbroIronOre = Registry.Register("gabbro_iron_ore", new BlockRock()).DeriveItem();

	public static Block Diorite = Registry.Register("diorite", new BlockRock()).DeriveItem();
	public static Block DioriteIronOre = Registry.Register("diorite_iron_ore", new BlockRock()).DeriveItem();

	//FURNITURE
	public static Block Ladder = Registry.Register("ladder", new BlockLadder()).DeriveItem();
	public static Block Chest = Registry.Register("chest", new BlockChest()).DeriveItem();
	public static Block Furnace = Registry.Register("furnace", new BlockFurnace()).DeriveItem();

	static Blocks()
	{
		RegisterRockSerie(NativeStoneType.Sedimentary, "limestone");
		RegisterRockSerie(NativeStoneType.Sedimentary, "dolomite");
		RegisterRockSerie(NativeStoneType.Metamorphic, "phyllite");
		RegisterRockSerie(NativeStoneType.Metamorphic, "marble");
		RegisterRockSerie(NativeStoneType.Effusive, "basalt");
		RegisterRockSerie(NativeStoneType.Effusive, "dacite");
		RegisterRockSerie(NativeStoneType.Intruded, "gabbro");
		RegisterRockSerie(NativeStoneType.Intruded, "diorite");

		SerieOfSoil.Poor = [SoilPoor.Instantiate(1), SoilPoor.Instantiate(0)];
		SerieOfSoil.Ordinary = [SoilOrdinary.Instantiate(1), SoilOrdinary.Instantiate(0)];
		SerieOfSoil.Loam = [SoilLoam.Instantiate(1), SoilLoam.Instantiate(0)];
	}

	static SerieOfRock RegisterRockSerie(NativeStoneType type, string name)
	{
		Block block = Registry[name];
		SerieOfRock serie = new SerieOfRock([block.Instantiate(), block.Instantiate()]);
		SerieOfRock.Layers.Add(new(type, serie));
		return serie;
	}

}
