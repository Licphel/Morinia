﻿using Kinetic.Math;

namespace Morinia.World;

public class Direction : IComparable<Direction>
{

	public static readonly Direction Left = new Direction(-1, 0, 180);
	public static readonly Direction Right = new Direction(1, 0, 0);
	public static readonly Direction Up = new Direction(0, 1, 90);
	public static readonly Direction Down = new Direction(0, -1, 270);

	public static readonly Direction[] Values = { Left, Right, Up, Down };
	public float Deg;

	public Vector2 Offset;
	public float Rad;

	Direction(int ox, int oy, int deg)
	{
		Offset = new Vector2(ox, oy);
		Deg = deg;
		Rad = FloatMath.Rad(deg);
	}

	public Direction Cw
	{
		get
		{
			if(this == Right)
			{
				return Down;
			}
			else if(this == Left)
			{
				return Up;
			}
			else if(this == Up)
			{
				return Right;
			}
			else if(this == Down)
			{
				return Left;
			}

			return null;
		}
	}

	public Direction Ccw
	{
		get
		{
			if(this == Right)
			{
				return Up;
			}
			else if(this == Left)
			{
				return Down;
			}
			else if(this == Up)
			{
				return Left;
			}
			else if(this == Down)
			{
				return Right;
			}

			return null;
		}
	}

	public Direction Opposite
	{
		get
		{
			if(this == Right)
			{
				return Left;
			}
			else if(this == Left)
			{
				return Right;
			}
			else if(this == Up)
			{
				return Down;
			}
			else if(this == Down)
			{
				return Up;
			}

			return null;
		}
	}

	public int CompareTo(Direction other)
	{
		return Deg.CompareTo(other.Deg);
	}

	public static Direction operator ++(Direction d)
	{
		return d.Cw;
	}

	public static Direction operator --(Direction d)
	{
		return d.Ccw;
	}

	public static Direction operator !(Direction d)
	{
		return d.Opposite;
	}

	public BlockPos Step(BlockPos pos)
	{
		return Posing.Offset(pos, Offset.xi, Offset.yi);
	}

	public PrecisePos Step(IPos pos)
	{
		return new PrecisePos(pos.x + Offset.xi, pos.y + Offset.yi);
	}

}
