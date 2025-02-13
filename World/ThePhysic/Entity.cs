using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;
using Morinia.Client.TheEntity;
using Morinia.Content;
using Morinia.World.TheLight;

namespace Morinia.World.ThePhysic;

public abstract class Entity : Movable
{

	public bool AddedToChunk;
	public ChunkUnit ChunkUnit;
	public long LastTick;

	//Client Lighting

	public Vector3 Light;

	public long UniqueId;

	public VaryingVector2 VisualSize = new VaryingVector2();

	public virtual bool IsDead { get; set; }
	public abstract EntityBuilder Builder { get; }

	public Chunk Chunk => ChunkUnit.Chunk;

	public virtual Vector3 SmoothedLight => Light;

	public virtual void Tick()
	{
		RegrabLight();
	}

	public virtual void OnSpawned()
	{
	}

	public virtual void OnDespawned()
	{
	}

	//Builtin Functions

	public abstract EntityTessellator GetTessellator();

	public virtual float CastLight(byte pipe)
	{
		return 0;
	}

	//NOT USED NOW.
	public virtual float FilterLight(byte pipe, float v)
	{
		return v;
	}

	//Codec

	public virtual void Write(IBinaryCompound compound)
	{
		compound.Set("builder", Builder.Uid.Full);
		compound.Set("x", Pos.x);
		compound.Set("y", Pos.y);
		compound.Set("vx", Velocity.x);
		compound.Set("vy", Velocity.y);
		compound.Set("uid", UniqueId);
	}

	public virtual void Read(IBinaryCompound compound)
	{
		Velocity.x = compound.Get<float>("vx");
		Velocity.y = compound.Get<float>("vy");
		UniqueId = compound.Get<long>("uid");
	}

	public static Entity ReadSpawn(IBinaryCompound compound, Level level = null)
	{
		EntityBuilder builder = Entities.Registry.Get(compound.Get<string>("builder"));
		if(builder == null) return null;

		Entity entity = builder.Instantiate();
		entity.Read(compound);
		entity.Locate(compound.Get<float>("x"), compound.Get<float>("y"));
		if(level != null) level.AddEntity(entity);
		return entity;
	}

	public virtual void RegrabLight(bool force = false)
	{
		LightEngine engine = Level.LightEngine;

		if(engine.IsStableRoll || force)
		{
			Vector3 vec = engine.GetLinearLight(Pos.x, Pos.y);
			Light.x = Math.Max(vec.x, CastLight(LightPass.Red));
			Light.y = Math.Max(vec.y, CastLight(LightPass.Green));
			Light.z = Math.Max(vec.z, CastLight(LightPass.Blue));
		}
	}

}
