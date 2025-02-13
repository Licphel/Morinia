using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content.TheFeature;

public class FeatureOre : Feature
{

	readonly BlockState state;
	readonly float spread;

	public FeatureOre(BlockState state, float spread, int clusters, Vector2 yrange)
	{
		IsSurfacePlaced = false;
		TryTimesPerChunk = clusters;
		Range = yrange;

		this.state = state;
		this.spread = spread;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		return level.GetBlock(x, y).Is(Tags.BlockStone);
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
				if(seed.NextFloat() < c && level.GetBlock(x1, y1).Is(Tags.BlockStone))
				{
					level.SetBlock(state, x1, y1);
				}
			}
		}
	}

}
