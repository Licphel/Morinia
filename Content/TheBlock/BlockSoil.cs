using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;

namespace Morinia.Content.TheBlock;

public class BlockSoil : Block
{

	public override float GetHardness(BlockState state)
	{
		return 1.25f;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Powder;
	}

	public override void OnRandomTick(BlockState state, Level level, BlockPos pos)
	{
		if(state.Meta == 0)
		{
			return;
		}
		bool upq = level.GetBlock(Direction.Up.Step(pos)).GetShape() == BlockShape.Solid;
		if(upq)
		{
			level.SetBlock(Instantiate(0), pos);
			return;
		}
		BlockPos pos2 = Posing.Offset(pos,
			Seed.Global.NextInt(-3, 3),
			Seed.Global.NextInt(-2, 2));

		BlockState t2 = level.GetBlock(pos2);
		upq = level.GetBlock(Direction.Up.Step(pos2)).GetShape() == BlockShape.Solid;

		if(t2.Is(state) && t2.Meta == 0 && !upq)
		{
			level.SetBlock(Instantiate(1), pos2);
		}
	}

}
