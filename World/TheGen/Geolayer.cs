using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.World.TheGen;

public class Geolayer
{

	public static List<KeyValuePair<NativeStoneType, Geolayer>> Layers = new();

	public static BlockState[] GetProperType(float a, int y)
	{
		if(y > Chunk.StoneTransverse)
			return GetProperStone(NativeStoneType.Sedimentary, NativeStoneType.Metamorphic, NativeStoneType.Effusive, a);
		return GetProperStone(NativeStoneType.Metamorphic, NativeStoneType.Intruded, NativeStoneType.Effusive, a);
	}

	static BlockState[] GetProperStone(NativeStoneType type1, NativeStoneType type2, NativeStoneType type3, float a)
	{
		BlockState[] proper = null;
		float delta = float.PositiveInfinity;

		foreach(var kv in Layers)
		{
			if(type1 != kv.Key && type2 != kv.Key && type3 != kv.Key)
				continue;
			float d1 = FloatMath.Pow(Math.Abs(a - kv.Value.Uniqueness) + 1, 2);
			if(delta > d1)
			{
				delta = d1;
				proper = kv.Value.States;
			}
		}

		return proper;
	}

	public float Uniqueness;
	public BlockState[] States;

	public Geolayer(float uniqueness, BlockState[] states)
	{
		Uniqueness = uniqueness;
		States = states;
	}

}

public enum NativeStoneType
{
	Sedimentary,
	Effusive,
	Intruded,
	Metamorphic
}
