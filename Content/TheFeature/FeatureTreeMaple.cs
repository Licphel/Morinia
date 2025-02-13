using Kinetic.Math;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;

namespace Morinia.Content.TheFeature;

public class FeatureTreeMaple : Feature
{

	static readonly BlockState logs = Blocks.MapleLog.Instantiate(1);
	static readonly BlockState leaves = Blocks.MapleLeaves.Instantiate(1);

	public FeatureTreeMaple()
	{
		IsSurfacePlaced = true;
		TryTimesPerChunk = 1;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		int times = 0;

		for(int i = -5; i < 5; i++)
		{
			for(int j = 1; j < 10; j++)
			{
				if(level.GetBlock(x + i, y + j).GetShape() == BlockShape.Solid)
				{
					times++;
				}
			}
		}

		return times < 10 && level.GetBlock(x, y).Is(Tags.BlockSoil);
	}

	public override void Place(Level level, int x, int y, Seed seed)
	{
		y++;
		int h = seed.NextInt(3, 15);
		for(int i = 0; i < h; i++)
			level.SetBlock(logs, x, y + i);

		for(int i = h; i < h + 2; i++)
			for(int j = -4; j < 4 + 1; j++)
				level.SetBlock(leaves, x + j, y + i);

		for(int i = h + 2; i < h + 4; i++)
			for(int j = -3; j < 3 + 1; j++)
				level.SetBlock(leaves, x + j, y + i);

		for(int i = h + 4; i < h + 6; i++)
			for(int j = -4; j < 4 + 1; j++)
				level.SetBlock(leaves, x + j, y + i);

		for(int i = h + 6; i < h + 8; i++)
			for(int j = -2; j < 2 + 1; j++)
				level.SetBlock(leaves, x + j, y + i);

		for(int i = h + 8; i < h + 9; i++)
			for(int j = -1; j < 1 + 1; j++)
				level.SetBlock(leaves, x + j, y + i);

		if(2 < h - 5)
		{
			if(seed.Next())
				placeBranch(level, x, y + seed.NextInt(2, h - 5), seed, true);
			if(seed.Next())
				placeBranch(level, x, y + seed.NextInt(2, h - 5), seed, false);

			for(int i = seed.NextInt(0, h / 4); i >= 0; i--)
			{
				//placeBranchTiny(level, x, y + seed.NextInt(1, h - 3), seed, seed.Next(), false);
			}
		}

		bool right = seed.Next();
		placeBranch(level, x, y + h - 2, seed, right);
		//placeBranchTiny(level, x, y + h - 1, seed, !right, true);
	}

	void placeBranch(Level level, int x, int y, Seed seed, bool right)
	{
		if(right)
		{
			level.SetBlock(logs, x + 1, y);
			level.SetBlock(logs, x + 2, y + 1);
			level.SetBlock(leaves, x + 1, y + 2);
			level.SetBlock(leaves, x + 2, y + 2);
			level.SetBlock(leaves, x + 3, y + 2);
			level.SetBlock(leaves, x + 2, y + 3);
		}
		else
		{
			level.SetBlock(logs, x - 1, y);
			level.SetBlock(logs, x - 2, y + 1);
			level.SetBlock(leaves, x - 1, y + 2);
			level.SetBlock(leaves, x - 2, y + 2);
			level.SetBlock(leaves, x - 3, y + 2);
			level.SetBlock(leaves, x - 2, y + 3);
		}
	}

	/*
	void placeBranchTiny(Level level, int x, int y, Rander seed, bool right, bool topBranch)
	{
		int state = 0;

		if(right && topBranch)
		{
			state = 0;
		}
		else if(right && !topBranch)
		{
			state = 1;
		}
		else if(!right && topBranch)
		{
			state = 2;
		}
		else if(!right && !topBranch)
		{
			state = 3;
		}

		//BlockInstance branch = Registry.LOG_BRANCH.getInstance().with(BlockProtoBranch.TYPE, state);

		if(right)
		{
			//	level.SetBlock(x + 1, y, 1, branch, true);
		}
		else
		{
			//	level.SetBlock(x - 1, y, 1, branch, true);
		}
	}
	*/

}
