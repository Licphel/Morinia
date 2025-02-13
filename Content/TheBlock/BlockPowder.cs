using Morinia.World.TheBlock;

namespace Morinia.Content.TheBlock;

public class BlockPowder : Block
{

	public override float GetHardness(BlockState state)
	{
		return 2.5f;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Powder;
	}

}
