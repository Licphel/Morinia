using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.World.TheGen;

public class EnvironRock
{

	public static List<KeyValuePair<NativeStoneType, EnvironRock>> Layers = new();

	public static BlockState[] GetProperType(float a, int dist)
	{
		if(dist < Chunk.RockTransverse)
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
			float d1 = Math.Abs(a - kv.Value.Uniqueness);

			if(delta > d1)
			{
				delta = d1;
				proper = kv.Value.States;
			}
		}

		return proper;
	}

	public float Uniqueness => FloatMath.SafeDiv(uniq_id, uniq_id_counter - 1);
	private int uniq_id;
	private static int uniq_id_counter;
	public BlockState[] States;

	public EnvironRock(BlockState[] states)
	{
		uniq_id = uniq_id_counter++;
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
