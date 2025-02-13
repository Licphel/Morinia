using Kinetic.Math;

namespace Morinia.World.TheBlock;

public class VoxelBox
{

	public float h;
	public float w;

	public float x;
	public float y;

	public VoxelBox(float x, float y, float w, float h)
	{
		this.x = x;
		this.y = y;
		this.w = w;
		this.h = h;
	}

	public bool Interacts(Box aabb, float ox, float oy)
	{
		Box b1 = new Box();
		b1.Resize(w, h);
		b1.Locate(ox + x, oy + y);
		return b1.Interacts(aabb);
	}

	public bool Contains(float x0, float y0, float ox, float oy)
	{
		return x + ox <= x0 && y + oy <= y0 && x + ox + w >= x0 && y + oy + h >= y0;
	}

	public float ClipX(float dx, Box aabb, float ox, float oy)
	{
		float ax = ox + x;
		float ay = oy + y;
		if(aabb.yprom <= ay || aabb.y >= ay + h)
		{
			return dx;//No collide on y, impossible to collide.
		}

		if(dx > 0 && aabb.xprom <= ax)
		{
			dx = Math.Min(dx, ax - aabb.xprom);
		}
		if(dx < 0 && aabb.x >= ax + w)
		{
			dx = Math.Max(dx, ax + w - aabb.x);
		}

		return dx;
	}

	public float ClipY(float dy, Box aabb, float ox, float oy)
	{
		float ax = ox + x;
		float ay = oy + y;
		if(aabb.xprom <= ax || aabb.x >= ax + w)
		{
			return dy;//No collide on x, impossible to collide.
		}

		if(dy > 0 && aabb.yprom <= ay)
		{
			dy = Math.Min(dy, ay - aabb.yprom);
		}
		if(dy < 0 && aabb.y >= ay + h)
		{
			dy = Math.Max(dy, ay + h - aabb.y);
		}

		return dy;
	}

}
