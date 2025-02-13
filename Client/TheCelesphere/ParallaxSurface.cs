using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Logic;
using Morinia.World;

namespace Morinia.Client.TheCelesphere;

public class ParallaxSurface : Parallax
{

	float op1;

	public ParallaxSurface(int layer, float resist, VaryingVector2 delta, Level level)
	: base(layer, resist, delta, level)
	{
	}

	public override void Draw(SpriteBatch batch, float secs, float msecs, float spf, IPos pos)
	{
		if(pos.y >= Chunk.SeaLevel)
		{
			op1 = FloatMath.Clamp(op1 + 0.01f, 0, 1);
		}
		else
		{
			op1 = FloatMath.Clamp(op1 - 0.01f, 0, 1);
		}

		if(op1 <= 0) return;

		Vector3 sunlight = CelestComputation.BgSunlight(secs, msecs);
		batch.Color4(sunlight);
		spf = 1 - spf;
		batch.Merge4(spf, spf, spf, spf);

		const float exp = 16;

		Vector2 size = GraphicsDevice.Global.Size;
		float disX = Delta.x * (1 - resist) * exp;
		float disY = -Delta.y * (1 - resist) * exp;
		float ratio = (float) Math.Round(size.x / Game.ScaledSize.x * 2 / 2f);
		float w = 512 * ratio;
		float h = 256 * ratio;
		float disX0 = disX / 1.5f / ratio;
		float disY0 = disY / 1.5f / ratio;
		float w0 = size.x / ratio;
		float h0 = size.y / ratio;

		Texture texture = null;

		{
			if(layer == 0) texture = GameTextures.CeBg3;
			if(layer == 1) texture = GameTextures.CeBg1;
			if(layer == 2) texture = GameTextures.CeBg2;
		}

		batch.Draw(texture, 0, disY, size.x, h, disX0, 0, w0, 256);
		batch.NormalizeColor();
	}

}
