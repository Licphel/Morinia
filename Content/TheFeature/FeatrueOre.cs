using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content.TheFeature;

public class FeatureOre : Feature
{
	
	readonly float spread;
	
	public FeatureOre(float spread, int clusters)
	{
		IsSurfacePlaced = false;
		TryTimesPerChunk = clusters;
		this.spread = spread;
	}

	BlockState GetStateFrom(BlockState target)
	{
		Block block = Blocks.Registry[Uid.Relocate(target.Block.Uid.Key + "_", "")];
		return block.GetStoredState();
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		BlockState state = level.GetBlock(x, y);
		return state.Is(Tags.BlockStone) && !GetStateFrom(state).IsEmpty;
	}

	public override void Place(Level level, int x, int y, Seed seed)
	{
		for(float i = -spread; i < spread; i++)
		{
			for(float j = -spread; j < spread; j++)
			{
				int x1 = FloatMath.Round(x + i);
				int y1 = FloatMath.Round(y + j);
				float d = FloatMath.Sqrt(i * i + j * j);
				float c = 1;
				if(d > spread / 2f) c = (spread - d) * 0.1f + 0.5f;
				if((c == 1 || seed.NextFloat() < c) && IsPlacable(level, x1, y1, seed))
				{
					BlockState state = GetStateFrom(level.GetBlock(x1, y1));
					if(!state.IsEmpty) level.SetBlock(state, x1, y1);
				}
			}
		}
	}

}
