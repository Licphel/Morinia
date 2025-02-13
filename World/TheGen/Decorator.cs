using Kinetic.Math;

namespace Morinia.World.TheGen;

public abstract class Decorator
{

	protected Seed Seed;

	public virtual DecoratorType Type => DecoratorType.WAITING;
	public virtual int SurfaceOffset => 0;

	public virtual void InitSeed(Level level, Chunk chunk)
	{
		Seed = chunk.GetUniqSeed();
	}

	public abstract void Decorate(Level level, Chunk chunk, int x, int y);

}

public enum DecoratorType
{

	FOLLOWING,//Run while filling basic blocks.
	WAITING,//Run after filling.
	SURFACE//Only run on surface after filling.

}
