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

	public static Feature CoalOreCluster = Registry.Register("coal_ore", new FeatureOre(4, 5));
	public static Feature IronOreCluster = Registry.Register("iron_ore", new FeatureOre(3, 5));
	
	public static Feature WaterLake = Registry.Register("water_lake", new FeatureLake(Liquids.Water, 4, 0.75f));
	
}
