using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content.TheFeature;

public class FeatureOre : Feature
{

	readonly BlockState state;
	readonly float spread;
	readonly HashSet<Block> Replacables;

	public FeatureOre(BlockState state, float spread, int clusters, params Block[] rep)
	{
		IsSurfacePlaced = false;
		TryTimesPerChunk = clusters;
		Replacables = new HashSet<Block>(rep);
		Range = new Vector2(0, Chunk.SeaLevel);

		this.state = state;
		this.spread = spread;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		return Replacables.Contains(level.GetBlock(x, y).Block);
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
				if(seed.NextFloat() < c && Replacables.Contains(level.GetBlock(x1, y1).Block))
				{
					level.SetBlock(state, x1, y1);
				}
			}
		}
	}

}
