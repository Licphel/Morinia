using Kinetic.Math;
using Morinia.Content;
using Morinia.World.TheBlock;

namespace Morinia.World.TheGen;

public class DecoratorPlants : Decorator
{

	readonly SeedLoot<BlockState> _loot = new SeedLoot<BlockState>()
	.Put(Blocks.Flower.Instantiate(), 5)
	.Put(Blocks.Grass.Instantiate(), 10)
	.Put(Blocks.Bush.Instantiate(), 3);

	public override DecoratorType Type => DecoratorType.SURFACE;
	public override int SurfaceOffset => 1;

	public override void Decorate(Level level, Chunk chunk, int x, int y)
	{
		if(Seed.NextFloat() < 0.75f
		   && chunk.GetBlock(x, y).GetShape() == BlockShape.Vacuum
		   && chunk.GetBlock(x, y - 1).Is(Tags.BlockSoil))
		{
			BlockState block = Seed.Select(_loot.Objects);
			chunk.SetBlock(block, x, y);
		}
	}

}
