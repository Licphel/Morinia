using Kinetic.App;
using Morinia.World.TheBlock;
using Morinia.World.TheLiquid;

namespace Morinia.World;

public class Biome : IDHolder
{

	public float Activeness;
	public BlockState Air = BlockState.Empty;

	public float Depth;
	public float Rainfall;
	public float Continent;
	public float Temperature;

	public const int LayerFoliage = 0;
	public const int LayerSoil = 1;
	public const int LayerGravel = 2;
	public const int LayerStone = 3;

	public bool HasSpecialAir = false;

	public BlockState GetState(int layer, BlockState[] localStoneTypes)
	{
		return localStoneTypes[layer];
	}

}
