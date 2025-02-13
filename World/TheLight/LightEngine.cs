using Kinetic.Math;

namespace Morinia.World.TheLight;

public abstract class LightEngine
{

	public const float Amplifier = 1.25f;

	public static float MaxValue = 1;//To use brighter light, see #maxValue.
	public static float Unit = MaxValue / 16f;
	public float[] Max = { 10, 10, 10 };
	public float[] Min = { 0, 0, 0 };

	public abstract bool IsStableRoll { get; }

	public abstract void DrawEnqueue(float x, float y, float v1, float v2, float v3);
	public abstract void DrawDirect(int x, int y, float v1, float v2, float v3);

	public abstract void Glow(int x, int y, float v1, float v2, float v3);
	public abstract Chunk GetBufferedChunk(int xb);

	public abstract Vector3 GetLinearLight(float x, float y);
	public abstract void GetBlockLight(LightWare coords, Vector4[] color, float a);
	public abstract void CalculateByViewdim(Box cam);

}
