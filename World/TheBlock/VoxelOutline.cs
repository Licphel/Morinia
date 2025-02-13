using Kinetic.Math;

namespace Morinia.World.TheBlock;

public class VoxelOutline : List<VoxelBox>
{

	public static VoxelOutline Void = From(Array.Empty<VoxelBox>());
	public static VoxelOutline Cube = From(new VoxelBox(0, 0, 1, 1));

	VoxelOutline(VoxelBox[] boxes) : base(boxes) { }

	public static VoxelOutline From(params VoxelBox[] boxes)
	{
		return new VoxelOutline(boxes);
	}

	//END

	public bool Interacts(Box aabb, float ox, float oy)
	{
		foreach(VoxelBox box in this)
		{
			if(box.Interacts(aabb, ox, oy))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(float x, float y, float ox, float oy)
	{
		foreach(VoxelBox box in this)
		{
			if(box.Contains(x, y, ox, oy))
			{
				return true;
			}
		}
		return false;
	}

	public float ClipX(float dx, Box aabb, float ox, float oy)
	{
		foreach(VoxelBox box in this)
		{
			dx = box.ClipX(dx, aabb, ox, oy);
		}
		return dx;
	}

	public float ClipY(float dy, Box aabb, float ox, float oy)
	{
		foreach(VoxelBox box in this)
		{
			dy = box.ClipY(dy, aabb, ox, oy);
		}
		return dy;
	}

}
