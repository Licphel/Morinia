using Kinetic.Math;
using Morinia.World;

namespace Morinia.Client.TheCelesphere;

public class CelestComputation
{

	public static float BodyRadians(float secs, float msecs)
	{
		return (float) Math.PI * 2 / msecs * (secs % msecs) + 0.75f;
	}

	public static float Cos(float secs, float msecs)
	{
		float i = FloatMath.CosRad(BodyRadians(secs, msecs)) * 0.5f + 0.5f;
		return FloatMath.Clamp(i, 0, 1);
	}

	public static float Sin(float secs, float msecs)
	{
		float i = FloatMath.SinRad(BodyRadians(secs, msecs)) * 0.5f + 0.5f;
		return FloatMath.Clamp(i, 0, 1);
	}

	public static float SpaceFactor(IPos grid)
	{
		if(grid.y <= Chunk.SpaceLevel)
		{
			return 0;
		}
		float dy = (grid.y - Chunk.SpaceLevel) * 2.25f;
		return FloatMath.Clamp(dy / (Chunk.Height - Chunk.SpaceLevel), 0, 1);
	}

	public static Vector4 ColorOfSky(float secs, float mSecs, IPos pos)
	{
		float f = Cos(secs, mSecs);
		float f1 = Sin(secs, mSecs);

		const float tempNow = 0.34f;
		Vector3 rgb0 = Hsv.HsvToRgb(0.6f - tempNow * 0.05f - (f - 0.5f) * 0.1f, 0.15f + (1 - f) * 0.4f + tempNow * 0.1f, 0.98f);

		rgb0 *= f1;
		rgb0 *= 1.05f - SpaceFactor(pos);

		return new Vector4(rgb0.x + 0.03f, rgb0.y + 0.03f, rgb0.z + 0.05f, 1);
	}

	public static void RecolorDuskAndDawn(float day, Vector4[] colors, ref Vector4 color0)
	{
		float i = Math.Abs(day - 0.45f);
		const float len = 0.4f;
		if(i >= len)
		{
			i = 0;
		}
		else
		{
			i = len - i;
		}
		float per = i / len * 2;
		float rx = 1;
		float gx = 1 - per * 0.45f;
		float bx = 1 - per * 0.2f;

		colors[0] = colors[1] = colors[2] = colors[3] = color0;
		colors[0] *= new Vector4(rx, gx, bx, 1);
		colors[1] *= new Vector4(0.85f, 0.9f, 1f, 1);
		colors[2] *= new Vector4(0.85f, 0.9f, 1f, 1);
		colors[3] *= new Vector4(rx, gx, bx, 1);
	}

	public static Vector3 BgSunlight(float secs, float msecs)
	{
		float f1 = Sin(secs, msecs);
		f1 = FloatMath.Clamp(f1, 0.1f, 1f);
		return new Vector3(f1, f1, f1);
	}

	public static Vector3 LightingSunlight(Level level, float hard = 1)
	{
		float f1 = Sin(level.Ticks, level.TicksPerDay) + 0.25f;
		float i = -f1 + 0.8f;
		return new Vector3(
			Math.Clamp(f1 - i * 0.15f * hard, 0, 1.1f),
			Math.Clamp(f1 - i * 0.1f * hard, 0, 1.1f),
				Math.Clamp(f1 + i * 0.15f * hard, 0, 1.1f)
			);
	}

}
