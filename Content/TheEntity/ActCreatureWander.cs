using Kinetic.Math;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheEntity;

public class ActCreatureWander : Activity
{

	int mtime = 50;
	float vx;

	public void Act(Creature e)
	{
		mtime--;
		if(mtime <= 0)
		{
			mtime = 500;
			vx = Seed.Global.NextFloat(2, 3.5f) * (Seed.Global.Next() ? -1 : 1);
		}
		if(FloatMath.Abs(e.Velocity.x) < 1 && mtime <= 300)
		{
			e.Velocity.x = Easing.Linear(e.Velocity.x, vx, 0.5f);
		}
		if(e.TouchHorizontal && e.TouchDown)
		{
			e.Velocity.y += 6.75f;
		}
	}

}
