using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client.TheBlock;
using Morinia.Logic;
using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheParticle;

public class ParticleBlockcrack : Particle
{

	readonly float degree;

	readonly TexturePart blockicon;
	readonly int u;
	readonly int v;
	float opacity = 1;
	float prevOpacity;

	public ParticleBlockcrack(BlockState state)
	{
		degree = Seed.Global.NextFloat(0, 360);

		if(BlockRendering.GetIcon(state) is TexturePart p)
		{
			blockicon = p;

			u = (int) (Seed.Global.NextFloat(0, 1) * p.Width);
			v = (int) (Seed.Global.NextFloat(0, 1) * p.Height);
			if(u > p.Width)
			{
				u = 0;
			}
			if(v > p.Height)
			{
				v = 0;
			}
			u += (int) p.u;
			v += (int) p.v;
		}
		else
		{
			Removal = true;
		}

		Box.Resize(1 / 16f, 1 / 16f);
		Velocity.x = 2.5f * FloatMath.CosDeg(degree);
		MaxAliveTicks = Seed.Global.NextInt(30, 120);
	}

	public override int MaxAliveTicks { get; }

	public override void Draw(SpriteBatch batch, Box box, Vector4 color)
	{
		base.Draw(batch, box, color);

		if(Removal) return;

		float npoc = 1 - (float) AliveTicks / MaxAliveTicks;
		opacity = Time.Lerp(prevOpacity, npoc);
		prevOpacity = npoc;

		color.w = opacity;

		MatrixStack matrixStack = batch.Matrices;

		matrixStack.Push();
		matrixStack.RotateDeg(degree, 0.05f, 0.05f);
		batch.Color4(color);
		batch.Draw(blockicon.Src, box.x - 0.05f, box.y - 0.05f, 0.1f, 0.1f, u, v, 1, 1);
		batch.NormalizeColor();
		matrixStack.Pop();
	}

	public override void Tick()
	{
		base.Tick();

		ApplyGravity(Vector2.One);
		RestrictVelocity();
		if(TouchHorizontal) Velocity.x *= 0.95f;
		if(TouchVertical) Velocity.y *= 0.95f;
		Move();
		CheckVelocity();
	}

}
