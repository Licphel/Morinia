using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Morinia.Content.TheEntity;
using Morinia.World.ThePhysic;

namespace Morinia.World;

public class ChunkUnit
{

	public Chunk Chunk;
	public List<Entity> Entities = new List<Entity>();

	public Level Level;
	public UnitPos Pos;

	public ChunkUnit(Chunk chunk, UnitPos pos)
	{
		Level = chunk.Level;
		Chunk = chunk;
		Pos = pos;
	}

	public void Tick()
	{
		_Tick();
	}

	public void _Tick()
	{
		for(int i = Entities.Count - 1; i >= 0; i--)
		{
			if(i >= Entities.Count)
			{
				break;
			}
			Entity e = Entities[i];
			if(e == null)
			{
				continue;
			}
			if(e.IsDead)
			{
				_RemoveEntity(e, true);
				continue;
			}
			if(Level.Ticks == e.LastTick)
			{
				continue;//Tick wrongly invoked. When transferring this happens.
			}
			e.Tick();
			e.LastTick = Level.Ticks;

			if(e.ChunkUnit == null)
			{
				e.ChunkUnit = this;
			}

			IPos oldpos = e.ChunkUnit.Pos;

			if(oldpos.UnitX != e.Pos.UnitX || oldpos.UnitY != e.Pos.UnitY)
			{
				ChunkUnit nunit = Level.GetUnit(e.Pos);

				if(nunit != null)
				{
					MoveEntityReference(e, nunit);
					e.ChunkUnit = nunit;
				}
			}
		}
	}

	public void _RemoveEntity(Entity e, bool inLevelDim)
	{
		Entities.Remove(e);

		if(inLevelDim)
		{
			Level._RemoveEntity(e);
		}

		Chunk.Dirty = true;
	}

	public void _AddEntity(Entity e)
	{
		Entities.Add(e);
		Chunk.Dirty = true;
	}

	public void MoveEntityReference(Entity e, ChunkUnit newUnit)
	{
		_RemoveEntity(e, false);
		newUnit._AddEntity(e);
		Chunk.Dirty = true;
	}

	//Codec

	public void Write(IBinaryCompound compound, bool removal = false)
	{
		IBinaryList list1 = IBinaryList.New();

		for(int i = 0; i < Entities.Count; i++)
		{
			Entity e = Entities[i];
			if(removal)
				Level.EntitiesById.Remove(e.UniqueId);
			if(e == null || e is EntityPlayer)
			{
				continue;
			}
			IBinaryCompound compound1 = IBinaryCompound.New();
			e.Write(compound1);
			list1.Insert(compound1);
		}

		compound.Set("entities", list1);
	}

	public void Read(IBinaryCompound compound)
	{
		IBinaryList list1 = compound.GetListSafely("entities");

		foreach(IBinaryCompound compound1 in list1)
		{
			Entity.ReadSpawn(compound1, Level);
		}
	}

}
