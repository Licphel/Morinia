using Kinetic.Math;
using Morinia.World.TheBlock;

namespace Morinia.World.TheGen;

public class SerieOfSoil
{

	public static BlockState[] Poor;
	public static BlockState[] Norm;
	public static BlockState[] Loam;

	public static BlockState[] GetProperType(float rain, float temp)
	{
		if(rain > 0.75f || temp < 0.2f || temp > 0.8f)
			return Poor;
		if(rain > 0.35f && temp > 0.45f && temp < 0.65f)
			return Loam;
		return Norm;
	}

}