using Kinetic.App;
using Kinetic.Math;

namespace Morinia.World.TheGen;

public abstract class Feature : IDHolder
{

	public bool IsSurfacePlaced = false;
	public int MaxCoverChunks = 1;
	public Vector2 Range = new Vector2(0, Chunk.SeaLevel);
	public bool RangedGuassian;
	public float TryChance = 1;
	public float TryTimesPerChunk = 0;

	public abstract bool IsPlacable(Level level,int x, int y, Seed seed);

	public abstract void Place(Level level, int x, int y, Seed seed);

}
