using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.World;

namespace Morinia.Client.TheCelesphere;

public abstract class Parallax
{

	public VaryingVector2 Delta = new VaryingVector2();
	public int layer;
	public Level Level;

	public float resist;

	public Parallax(int layer, float resist, VaryingVector2 delta, Level level)
	{
		this.layer = layer;
		this.resist = resist;
		Delta = delta;
		Level = level;
	}

	public abstract void Draw(SpriteBatch batch, float secs, float msecs, float spf, IPos pos);

	public virtual void Flush() { }

	public virtual void Tick() { }

}
