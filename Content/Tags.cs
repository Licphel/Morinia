using Morinia.World.TheDict;

namespace Morinia.Content;

public class Tags
{

	//Blocks
	public static Tag BlockStone = Tag.Collect("blocks/stones");
	public static Tag BlockSoil = Tag.Collect("blocks/soils");
	public static Tag BlockCarvable = Tag.Combine("blocks/carvable", BlockStone, BlockSoil);

	//Items
	public static Tag ItemStone = Tag.Collect("items/stones");

	public static void Init()
	{
	}

}
