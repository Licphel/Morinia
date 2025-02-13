using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content.TheBlock;

public class BlockSeedling : BlockPlant
{

	Func<Feature> FeatureFac;

	public BlockSeedling(Func<Feature> feature)
	{
		FeatureFac = feature;
	}

	public override void OnRandomTick(BlockState state, Level level, BlockPos pos)
	{
		base.OnRandomTick(state, level, pos);

		Seed seed = Seed.Global;
		Feature feature = FeatureFac();

		if(feature.IsPlacable(level, pos.BlockX, pos.BlockY - 1, seed))
		{
			feature.Place(level, pos.BlockX, pos.BlockY - 1, seed);
		}
	}

}
