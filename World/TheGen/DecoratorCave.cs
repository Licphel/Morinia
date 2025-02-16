using Morinia.Content;
using Morinia.World.TheBlock;

namespace Morinia.World.TheGen;

public class DecoratorCave : Decorator
{

	bool init;
	Noise noise1;
	Noise noise2;
	Noise noise3;

	public override DecoratorType Type => DecoratorType.FOLLOWING;

	public override void InitSeed(Level level, Chunk chunk)
	{
		base.InitSeed(level, chunk);

		if(init)
		{
			return;
		}

		init = true;

		//use level seed. make caves connected.
		noise1 = new NoiseOctave(level.Seed.Copyx(3), 1);
		noise2 = new NoiseOctave(level.Seed.Copyx(6), 1);
		noise3 = new NoiseOctave(level.Seed.Copyx(9), 1);
	}

	public override void Decorate(Level level, Chunk chunk, int x, int y)
	{
		if(y > chunk.Surface[x & 15]) return;

		BlockState state = chunk.GetBlock(x, y);

		if(!state.Is(Tags.BlockCarvable))
			return;

		float n1 = noise1.Generate(x / 16f, y / 12f, 1);
		float n2 = noise2.Generate(x / 16f, y / 16f, 1);
		float n3 = noise3.Generate(x / 24f, y / 16f, 1);

		if(y > Chunk.RockTransverse)
			n2 -= (y - Chunk.RockTransverse) / (Chunk.SeaLevel - Chunk.RockTransverse);

		if((n1 < 0.35f || n1 > 0.8f || n3 > 0.5f && n3 < 0.6f) && n2 > 0.2f)
			chunk.SetBlock(BlockState.Empty, x, y);
	}

}
