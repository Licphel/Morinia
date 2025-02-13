using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.Content.TheBlock;

public class BlockBorder : Block
{

	public override float GetHardness(BlockState state)
	{
		return float.PositiveInfinity;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Glass;
	}

	public override float CastLight(BlockState state, byte pipe, int x, int y)
	{
		return FloatMath.SinRad(Time.Seconds + pipe + y * x) * 0.25f;
	}

}
