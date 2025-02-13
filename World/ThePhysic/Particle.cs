using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Logic;
using Morinia.World.TheLight;

namespace Morinia.World.ThePhysic;

public class Particle : Movable
{

	public int AliveTicks;

	public Vector3 Light;
	public bool Removal;
	public Vector4 RgbaOverride = new Vector4(1, 1, 1, 1);

	public VaryingVector2 VisualSize = new VaryingVector2();
	public virtual int MaxAliveTicks => int.MaxValue;

	public void DrawInternal(SpriteBatch batch)
	{
		Box smoothbox = new Box();
		smoothbox.w = VisualSize.x;
		smoothbox.h = VisualSize.y;
		smoothbox.xcentral = Time.Lerp(PosLastTick.x, Pos.x);
		smoothbox.ycentral = Time.Lerp(PosLastTick.y, Pos.y);

		Vector4 outv = new Vector4(Light.x, Light.y, Light.z, 1) * RgbaOverride;

		Draw(batch, smoothbox, outv);

		if(AliveTicks > MaxAliveTicks)
		{
			Removal = true;
		}
	}

	public virtual void Draw(SpriteBatch batch, Box box, Vector4 color)
	{
	}

	public virtual void Tick()
	{
		RegrabLight();
		AliveTicks++;
	}

	public virtual void OnSpawned()
	{
	}

	public virtual void OnDespawned()
	{
	}

	public virtual void RegrabLight(bool force = false)
	{
		LightEngine engine = Level.LightEngine;

		if(engine.IsStableRoll || force)
		{
			Light = engine.GetLinearLight(Pos.x, Pos.y);
		}
	}

}
