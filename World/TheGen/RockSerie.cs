using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.World.TheGen;

public class RockSerie
{

	public static List<KeyValuePair<NativeStoneType, RockSerie>> Layers = new();

	public static BlockState[] GetProperType(float a, int y)
	{
		if(y > Chunk.RockTransverse)
			return GetProperStone(NativeStoneType.Sedimentary, NativeStoneType.Metamorphic, NativeStoneType.Effusive, NativeStoneType.Intruded, a);
		return GetProperStone(NativeStoneType.Intruded, NativeStoneType.Effusive, NativeStoneType.Metamorphic, NativeStoneType.Sedimentary, a);
	}

	static BlockState[] GetProperStone(NativeStoneType type1, NativeStoneType type2, NativeStoneType type3, NativeStoneType notgen, float a)
	{
		BlockState[] proper = null;
		float delta = float.PositiveInfinity;

		foreach(var kv in Layers)
		{
			if(notgen == kv.Key)
				continue;
			float d1 = FloatMath.Pow(Math.Abs(a - kv.Value.Uniqueness) + 1, 2);
			if(kv.Key == type1)
				d1 *= 0.5f;
			else if(kv.Key == type2)
				d1 *= 0.75f;

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

	public RockSerie(float uniqueness, BlockState[] states)
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
