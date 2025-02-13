using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;
using Morinia.World.TheLiquid;

namespace Morinia.Content.TheFeature;

public class FeatureLake : Feature
{

	readonly Liquid liquid;
	readonly float spread;

	public FeatureLake(Liquid liquid, float spread, int clusters, Vector2 yrange)
	{
		IsSurfacePlaced = false;
		TryTimesPerChunk = clusters;
		Range = yrange;

		this.liquid = liquid;
		this.spread = spread;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		return level.GetBlock(x, y).Is(Tags.BlockCarvable);
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
				if((x1 - x) * (x1 - x) + (y1 - y) * (y1 - y) >= spread * spread)
					continue;
				if(level.GetBlock(x1, y1).Is(Tags.BlockStone))
				{
					level.SetBlock(BlockState.Empty, x1, y1);
					level.GetChunk(Posing.ToCoord(x)).LiquidMap
					.Set(x1, y1, new LiquidStack(liquid, (int) (Liquid.MaxAmount * 0.95f)));
				}
			}
		}
	}

}
