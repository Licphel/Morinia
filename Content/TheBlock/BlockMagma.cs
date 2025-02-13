using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Morinia.World.TheBlock;
using Morinia.World.TheLight;

namespace Morinia.Content.TheBlock;

public class BlockMagma : Block
{

	public override float GetHardness(BlockState state)
	{
		return 6;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Stone;
	}

	public override float CastLight(BlockState state, byte pipe, int x, int y)
	{
		float d = FloatMath.SinRad(Time.Seconds / 2 + y * x);
		return LightPass.Switch3(pipe, d * 0.25f + 0.5f, d * 0.35f, d * 0.23f);
	}

}
