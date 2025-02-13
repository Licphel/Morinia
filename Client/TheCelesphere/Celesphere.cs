using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Logic;
using Morinia.World;

namespace Morinia.Client.TheCelesphere;

public class Celesphere
{

	//STATIC
	protected static Celesphere _Celesphere = new Celesphere();
	protected static MatrixerAbsolute Transformation = new MatrixerAbsolute();
	public static float Secs = 30;
	public static float Msecs = 120;
	float dayTime;
	public VaryingVector2 Delta = new VaryingVector2();
	public float ExpectedStarSizeBase = 128;
	public bool JustFlushed;

	public Parallax[] Layers = new Parallax[6];
	public Level Level;//Nullable
	int lw, lh;
	public List<CelestStar> Stars = new List<CelestStar>();
	public Parallax[] Undergrounds = new Parallax[6];

	public Celesphere(Level level = null)
	{
		Layers[0] = new ParallaxSurface(0, 0.975f, Delta, level);
		Layers[1] = new ParallaxSurface(1, 0.925f, Delta, level);
		Layers[2] = new ParallaxSurface(2, 0.85f, Delta, level);

		Undergrounds[0] = new ParallaxCave(0, 0.9f, Delta, level);
		Undergrounds[1] = new ParallaxCave(1, 0.7f, Delta, level);

		Level = level;
	}

	public static void DrawCelesph(SpriteBatch batch)
	{
		Transformation.DoTransform(batch);
		batch.UseCamera(Transformation.Camera);
		_Celesphere.Draw(batch, Secs, Msecs, new PrecisePos(0, Chunk.SurfaceLevel));
		batch.UseCamera(Game.MatrixerFlow.Camera);
	}

	public static void TickCelesph()
	{
		Secs += Time.Delta;
		_Celesphere.CenterMoved(Time.Seconds / 3, 0);
		_Celesphere.Tick();
	}

	public void CenterMoved(float x, float y)
	{
		Delta.x = x;
		Delta.y = y;
	}

	public void Tick()
	{
		int w = GraphicsDevice.Global.Size.xi;
		int h = GraphicsDevice.Global.Size.yi;

		JustFlushed = false;

		if(lw != w || lh != h)
		{
			Flush();
			JustFlushed = true;
		}

		lw = w;
		lh = h;

		foreach(Parallax l in Layers)
		{
			l?.Tick();
		}
		foreach(Parallax l in Undergrounds)
		{
			l?.Tick();
		}

		int av = (w + h) / 2;

		for(int i = 0; Stars.Count < ExpectedStarSizeBase * (av / 300f); i++)
		{
			Stars.Add(new CelestStar(Seed.Global.NextFloat(0, w), Seed.Global.NextFloat(0, h)));
		}
	}

	public void Flush()
	{
		Stars.Clear();

		foreach(Parallax l in Layers)
		{
			l?.Flush();
		}
		foreach(Parallax l in Undergrounds)
		{
			l?.Flush();
		}
	}

	public void Draw(SpriteBatch batch, float secs, float msecs, IPos pos)
	{
		int w = GraphicsDevice.Global.Size.xi;
		int h = GraphicsDevice.Global.Size.yi;

		dayTime = CelestComputation.Sin(secs, msecs);

		Vector4 temp = CelestComputation.ColorOfSky(secs, msecs, pos);
		CelestComputation.RecolorDuskAndDawn(dayTime, batch.LinearCol4, ref temp);

		batch.Fill(0, 0, w, h);
		batch.NormalizeColor();

		float spf = CelestComputation.SpaceFactor(pos);//Space factor.

		for(int i = Stars.Count - 1; i >= 0; i--)
		{
			CelestStar star = Stars[i];
			star.Draw(batch, dayTime, spf);
		}

		float ctx = w / 2f, cty = h / 8f;
		float deg = FloatMath.Deg(CelestComputation.BodyRadians(secs, msecs));

		Vector2 vec = new Vector2();
		vec.FromDeg(h, deg);
		vec *= new Vector2(1.25f, 0.75f);
		float sx = ctx + vec.x, sy = cty + vec.y;

		const float scl = 1f;

		batch.Draw(GameTextures.CeBody1, sx - 32 * scl, sy - 32 * scl, 64 * scl, 64 * scl);

		sx = ctx - vec.x;
		sy = cty - vec.y;

		batch.Color4(1, 1, 1, 0.75f);
		batch.Draw(GameTextures.CeBody2, sx - 32 * scl, sy - 32 * scl, 64 * scl, 64 * scl);
		batch.NormalizeColor();

		foreach(Parallax l in Layers)
		{
			l?.Draw(batch, secs, msecs, spf, pos);
		}
		foreach(Parallax l in Undergrounds)
		{
			l?.Draw(batch, secs, msecs, spf, pos);
		}

		batch.NormalizeColor();
	}

}
