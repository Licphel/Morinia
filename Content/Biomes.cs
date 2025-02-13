using Kinetic.App;
using Morinia.Content.TheBiome;
using Morinia.World;

namespace Morinia.Content;

public class Biomes
{

	public static readonly IDMap<Biome> Registry = new IDMap<Biome>();

	public static Biome Forest = Registry.Register("forest", new BiomeForest());

}
