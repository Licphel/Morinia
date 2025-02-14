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

	public static Feature LimestoneCoalOreCluster = Registry.Register("limestone_coal_ore", new FeatureOre(Blocks.LimestoneCoalOre.Instantiate(), 4, 5, Blocks.Limestone));
	public static Feature BasaltIronOreCluster = Registry.Register("basalt_iron_ore", new FeatureOre(Blocks.BasaltIronOre.Instantiate(), 3, 5, Blocks.Basalt));
	public static Feature GraniteIronOreCluster = Registry.Register("granite_iron_ore", new FeatureOre(Blocks.GraniteIronOre.Instantiate(), 3, 5, Blocks.Granite));
	public static Feature DioriteIronOreCluster = Registry.Register("diorite_iron_ore", new FeatureOre(Blocks.DioriteIronOre.Instantiate(), 3, 5, Blocks.Diorite));

	public static Feature WaterLake = Registry.Register("water_lake", new FeatureLake(Liquids.Water, 4, 2, new Vector2(0, Chunk.Height)));

}
