﻿using Morinia.World.TheBlock;

namespace Morinia.Content.TheBlock;

public class BlockRock : Block
{

	public override float GetHardness(BlockState state)
	{
		return 5;
	}

	public override BlockMaterial GetMaterial(BlockState state)
	{
		return BlockMaterial.Stone;
	}

}
