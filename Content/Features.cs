using Kinetic.App;
using Kinetic.Math;
using Morinia.Content.TheFeature;
using Morinia.World;
using Morinia.World.TheGen;

namespace Morinia.Content;

public class Features
{

	public static readonly IDMap<Feature> Registry = new IDMap<Feature>();

	public static Feature BirchTree = Registry.Register("birch_tree", new FeatureTreeBirch());
	public static Feature MapleTree = Registry.Register("maple_tree", new FeatureTreeMaple());

	public static Feature Vine = Registry.Register("vine", new FeatureVine());

	public static Feature CoalOreCluster = Registry.Register("coal_ore_cluster", new FeatureOre(Blocks.CoalOre.Instantiate(), 3, 7, new Vector2(0, Chunk.SeaLevel)));
	public static Feature CopperOreCluster = Registry.Register("copper_ore_cluster", new FeatureOre(Blocks.CopperOre.Instantiate(), 2, 4, new Vector2(0, Chunk.SeaLevel)));
	public static Feature IronOreCluster = Registry.Register("iron_ore_cluster", new FeatureOre(Blocks.IronOre.Instantiate(), 2, 6, new Vector2(0, Chunk.SeaLevel)));

	public static Feature WaterLake = Registry.Register("water_lake", new FeatureLake(Liquids.Water, 4, 4, new Vector2(0, Chunk.Height)));

}
