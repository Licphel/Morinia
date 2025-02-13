namespace Morinia.World.ThePhysic;

public class Navigation
{

	public static Queue<IPos> GetNearestNodes(Level level, Creature e, IPos src, IPos dest)
	{
		Queue<IPos> poses = new Queue<IPos>();
		int dirx = dest.x > src.x ? 1 : -1;
		int diry = dest.y > src.y ? 1 : -1;
		float x0 = src.x;
		float y0 = src.y;
		bool xd = false, yd = false;
		bool failed = false;

		while(!xd || !yd)
		{
			float nx = x0 + dirx;
			float ny = y0 + diry;

			if(Math.Abs(nx - dest.x) <= 1)
			{
				nx = dest.x;
				xd = true;
			}
			if(Math.Abs(ny - dest.y) <= 1)
			{
				ny = dest.y;
				yd = true;
			}

			IPos pos = new PrecisePos(nx, ny);
			//todo
		}

		return poses;
	}

}
