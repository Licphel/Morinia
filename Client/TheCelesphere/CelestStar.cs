using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;

namespace Morinia.Client.TheCelesphere;

public class CelestStar
{

	static readonly Seed STAR_SEED = new SeedLCG();
	public float MaxOpacity;
	public float Opacity;
	public float OpacityOffs;
	public bool Removed;
	public float Size;

	public float x, y;

	public CelestStar(float x, float y)
	{
		this.x = x;
		this.y = y;
		Size = 2;
		Opacity = STAR_SEED.NextFloat();
		OpacityOffs = STAR_SEED.NextInt(128);
		MaxOpacity = STAR_SEED.NextFloat(0.75f, 1f) - STAR_SEED.NextFloat(0, 0.25f);
	}

	public void Draw(SpriteBatch batch, float daytime, float space)
	{
		float opa1 = Math.Clamp(Opacity - daytime + space, 0, 1)
		             * (FloatMath.SinRad(Time.Seconds + x) * 0.5f + 0.25f);
		batch.Color4(1, 1, 1, opa1);
		batch.Fill(x - Size / 2, y - Size / 2, Size, Size);
		batch.NormalizeColor();
	}

}
