using Morinia.World.TheBlock;
using Morinia.World.TheLight;

namespace Morinia.Content.TheBlock;

public class BlockLantern : Block
{

	public override float GetHardness(BlockState state)
	{
		return 1;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Glass;
	}

	public override float CastLight(BlockState state, byte pipe, int x, int y)
	{
		return LightPass.Switch3(pipe, 1.05f, 0.95f, 0.83f);
	}

}
