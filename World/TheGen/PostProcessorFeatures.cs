using Kinetic.Math;

namespace Morinia.World.TheGen;

public class PostProcessorFeatures : PostProcessor
{

	public override void Process(Level level, Chunk chunk)
	{
		Seed seed = chunk.GetUniqSeed();

		foreach(Feature f in Content.Features.Registry.List())
		{
			int times = f.TryTimesPerChunk;
			Vector2 r = f.Range;

			for(int i = 0; i < times; i++)
			{
				int x = seed.NextInt(chunk.Coord * 16, (chunk.Coord + 1) * 16 - 1);
				int y = f.IsSurfacePlaced
				? chunk.Surface[x & 15]
				: f.RangedGuassian
				? seed.NextGaussianInt((int) r.x, (int) r.y)
				: seed.NextInt((int) r.x, (int) r.y);

				if(!f.IsPlacable(level, x, y, seed.Copy())) continue;

				f.Place(level, x, y, seed);
			}
		}
	}

}
