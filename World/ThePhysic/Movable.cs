using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.World.ThePhysic;

public class Movable
{

	static readonly List<BlockPos> _PosCache = new List<BlockPos>();

	public Box Box;
	public VaryingVector2 Face = new VaryingVector2();
	public bool IsReachingChunkEdge;

	public float _DeltaPath;
	public bool JustMoved;

	public Level Level;
	public PrecisePos Pos;
	public PrecisePos PosLastTick;
	public float Rotation, VeloRad;
	public BlockState BlockStepping = BlockState.Empty, BlockStaying = BlockState.Empty;

	public bool TouchLeft, TouchRight, TouchUp, TouchDown, TouchHorizontal, TouchVertical, Touch;

	public VaryingVector2 Velocity = new VaryingVector2();
	public VaryingVector2 WannaPos = new VaryingVector2();

	public virtual bool IsCollidable => true;
	public virtual bool IsSlidable => true;
	public virtual float ReboundFactor => 0;

	public Box Move()
	{
		Box origin = Box;//value copy.
		Box destination = Box;

		float dx = Velocity.x * Time.Delta, dy = Velocity.y * Time.Delta;
		float dx0 = dx, dy0 = dy;

		WannaPos.x = origin.xcentral + dx;
		WannaPos.y = origin.ycentral + dy;
		JustMoved = true;

		IsReachingChunkEdge = false;

		if(IsCollidable && dx != 0)
		{
			Level.GetNearbyBlocks(_PosCache, destination);

			for(int i = 0; i < _PosCache.Count; i++)
			{
				BlockPos pos = _PosCache[i];
				VoxelOutline otl = Level.GetBlock(pos).GetOutlineForPhysics(this);
				float ox = pos.BlockX;
				float oy = pos.BlockY;
				dx = otl.ClipX(dx, destination, ox, oy);
			}
		}

		if(Math.Abs(dx) < 10E-5)
		{
			dx = 0;
		}

		destination.Translate(dx, 0);

		if(!IsSlidable && dx != dx0)
		{
			dx = dy = 0;
			JustMoved = false;
		}

		if(IsCollidable && dy != 0)
		{
			Level.GetNearbyBlocks(_PosCache, destination);

			for(int i = 0; i < _PosCache.Count; i++)
			{
				BlockPos pos = _PosCache[i];
				VoxelOutline otl = Level.GetBlock(pos).GetOutlineForPhysics(this);
				float ox = pos.BlockX;
				float oy = pos.BlockY;
				dy = otl.ClipY(dy, destination, ox, oy);
			}
		}

		if(Math.Abs(dy) < 10E-5)
		{
			dy = 0;
		}

		//clamp it in section
		if(origin.yprom + dy >= Chunk.MaxY)
		{
			dy = Chunk.MaxY - origin.yprom;
		}
		if(origin.y + dy <= 0)
		{
			dy = -origin.y;
		}

		destination.Translate(0, dy);

		if(!IsSlidable && dy != dy0)
		{
			dx = dy = 0;
			JustMoved = false;
		}

		if(dx == 0 && dy == 0)
		{
			JustMoved = false;
		}

		bool xClip = Math.Abs(dx - dx0) > 10E-5;
		bool yClip = Math.Abs(dy - dy0) > 10E-5;

		TouchLeft = dx0 < 0 && xClip;
		TouchRight = dx0 > 0 && xClip;
		TouchDown = dy0 < 0 && yClip;
		TouchUp = dy0 > 0 && yClip;
		TouchHorizontal = TouchLeft || TouchRight;
		TouchVertical = TouchUp || TouchDown;
		Touch = TouchHorizontal || TouchVertical;
		_DeltaPath = FloatMath.Sqrt(dx * dx + dy * dy);

		if(JustMoved)
		{
			VeloRad = FloatMath.AtanRad(dy, dx);
			Face.x = dx > 0 ? 1 : -1;
			Face.y = dy > 0 ? 1 : -1;
			//if not moved, keep the previous face still.
		}

		Locate(destination.xcentral, destination.ycentral);

		BlockStaying = Level.GetBlock(Pos);
		BlockStepping = Level.GetBlock(Direction.Down.Step(new BlockPos(Pos)));

		return destination;
	}

	public void ApplyGravity(Vector2 multipler)
	{
		Velocity.x += Level.Gravity.x * Time.Delta * multipler.x;
		float ff = BlockStepping.GetFloatingForce(this);
		Velocity.y += Level.Gravity.y * Time.Delta * multipler.y * (1 - ff);
	}

	public void CheckVelocity()
	{
		float reboundFactor = ReboundFactor;

		if(TouchUp && Velocity.y > 0)
		{
			Velocity.y *= -reboundFactor;
		}
		if(TouchDown && Velocity.y < 0)
		{
			Velocity.y *= -reboundFactor;
		}
		if(TouchLeft && Velocity.x < 0)
		{
			Velocity.x *= -reboundFactor;
		}
		if(TouchRight && Velocity.x > 0)
		{
			Velocity.x *= -reboundFactor;
		}
	}

	public void RestrictVelocity()
	{
		RestrictVelocity(
			FloatMath.Abs(Velocity.x) * 0.005f,
			FloatMath.Abs(Velocity.y) * 0.005f
		);
		Velocity.x *= BlockStepping.GetFricitonForce(this);//Friction
		float af = BlockStaying.GetAntiForce(this);
		Velocity.x *= af;
		Velocity.y *= af;
	}

	public void RestrictVelocity(float fax, float fay)
	{
		if(Velocity.x > 0)
		{
			Velocity.x = FloatMath.Clamp(Velocity.x - fax, 0, 9999);
		}
		if(Velocity.x < 0)
		{
			Velocity.x = FloatMath.Clamp(Velocity.x + fax, -9999, 0);
		}
		if(Velocity.y > 0)
		{
			Velocity.y = FloatMath.Clamp(Velocity.y - fay, 0, 9999);
		}
		if(Velocity.y < 0)
		{
			Velocity.y = FloatMath.Clamp(Velocity.y + fay, -9999, 0);
		}
	}

	public void Locate(IPos pos, bool keepLastPos = true)
	{
		Locate(pos.x, pos.y, keepLastPos);
	}

	public void Locate(float x, float y, bool keepLastPos = true)
	{
		PosLastTick = Pos;
		Box.LocateCentral(x, y);
		Pos = new PrecisePos(x, y);
		if(!keepLastPos)
		{
			PosLastTick = Pos;
		}
		UpdatePosition();
	}

	protected virtual void UpdatePosition()
	{
	}

}
