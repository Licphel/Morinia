﻿using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content.TheFeature;

public class FeatureVine : Feature
{

	public FeatureVine()
	{
		IsSurfacePlaced = false;
		TryTimesPerChunk = 16;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		return level.GetBlock(x, y + 1).GetShape() == BlockShape.Solid;
	}

	public override void Place(Level level, int x, int y, Seed seed)
	{
		BlockState state = Blocks.Vine.Instantiate();

		for(int k = -3; k < 3; k++)
		{
			int o = -99;

			if(level.GetBlock(x + k, y + 1).GetShape() == BlockShape.Solid) o = 1;
			if(level.GetBlock(x + k, y).GetShape() == BlockShape.Solid) o = 0;
			if(level.GetBlock(x + k, y - 1).GetShape() == BlockShape.Solid) o = -1;

			if(o == -99) continue;

			for(int i = 0, j = seed.NextInt(1, 4); i <= j; i++)
			{
				if(!level.GetBlock(x + k, y - i + o - 1).IsEmpty || y - i + o < 0) break;
				level.SetBlock(state, x + k, y - i + o);
			}
		}
	}

}
