using Kinetic.Math;
using Morinia.Content.TheParticle;
using Morinia.World.TheBlock;
using Morinia.World.TheLight;
using Morinia.World.ThePhysic;

namespace Morinia.World;

public abstract class LevelBasic
{

	static readonly List<Entity> LevelETemp = new List<Entity>();

	public abstract LightEngine LightEngine { get; }
	public abstract ParticleEngine ParticleEngine { get; }
	public abstract IEnumerable<Chunk> ActiveChunks { get; }
	public abstract long Ticks { get; set; }

	public Chunk GetChunk(IPos pos)
	{
		return GetChunk(pos.UnitX);
	}

	public Chunk GetChunkByBlock(int blockx)
	{
		return GetChunk(Posing.ToCoord(blockx));
	}

	public abstract Chunk GetChunk(int coord);

	public void GetNearbyBlocks(List<BlockPos> list, IBox dim)
	{
		list.Clear();
		Chunk chunk = null;

		for(float i = dim.x - dim.w / 2f - 2; i <= dim.xprom + dim.w / 2f + 2; i++)
		{
			int x = FloatMath.Floor(i);
			int coord = Posing.ToCoord(x);

			if(chunk == null || chunk.Coord != coord)
			{
				chunk = GetChunk(coord);
			}

			for(float j = dim.y - dim.h / 2f - 2; j <= dim.yprom + dim.h / 2f + 2; j++)
			{
				int y = FloatMath.Floor(j);

				BlockState state = chunk == null ? BlockState.Empty : chunk.GetBlock(x, y);
				if(!state.IsEmpty) list.Add(new BlockPos(x, y));
			}
		}
	}

	public BlockState GetBlock(IPos pos)
	{
		return GetBlock(pos.BlockX, pos.BlockY, pos.BlockZ);
	}

	public abstract BlockState GetBlock(int x, int y, int z = 1);

	public BlockEntity GetBlockEntity(IPos pos)
	{
		Chunk chunk = GetChunk(pos);
		return chunk == null ? null : chunk.GetBlockEntity(pos);
	}

	public BlockEntity GetBlockEntity(int x, int y, int z = 1)
	{
		return GetBlockEntity(new BlockPos(x, y, z));
	}

	public ChunkUnit GetUnit(IPos pos)
	{
		Chunk chunk = GetChunk(pos);
		return chunk == null ? null : chunk.GetUnitSafely(pos.UnitY);
	}

	public void SetChunk(IPos pos, Chunk chunk)
	{
		SetChunk(pos.UnitX, chunk);
	}

	public abstract void SetChunk(int coord, Chunk chunk);

	public abstract void RemoveChunk(int coord);

	public abstract BlockState SetBlock(BlockState state, IPos grid);

	public abstract BlockState SetBlock(BlockState state, int x, int y, int z = 1);

	public void PlayDestructParticles(BlockState state, IPos pos)
	{
		for(int i = 0; i < 24; i++)
		{
			ParticleBlockcrack p = new ParticleBlockcrack(state);
			ParticleEngine.AddSpreading(p, pos, 0.5f);
		}
	}

	public abstract BlockState BreakBlock(IPos pos, bool drop = false);

	public BlockState BreakBlock(int x, int y, int z = 1)
	{
		return BreakBlock(new BlockPos(x, y, z));
	}

	public BlockEntity SetBlockEntity(BlockEntity e, IPos pos)
	{
		Chunk chunk = GetChunk(pos);
		if(chunk == null) return null;
		return chunk.SetBlockEntity(e, pos);
	}

	public BlockEntity SetBlockEntity(BlockEntity e, int x, int y, int z = 1)
	{
		return SetBlockEntity(e, new BlockPos(x, y, z));
	}

	public void AddEntity(Entity entity, float x, float y)
	{
		AddEntity(entity, new PrecisePos(x, y));
	}

	public void AddEntity(Entity entity)
	{
		AddEntity(entity, entity.Pos);
	}

	public abstract void AddEntity(Entity entity, IPos pos);

	public List<Entity> GetNearbyEntities(IBox aabb, Predicate<Entity> predicate = null, int offset = 2)
	{
		return GetNearbyEntities(LevelETemp, aabb, predicate, offset);
	}

	public List<Entity> GetNearbyEntities(List<Entity> buffer, IBox aabb, Predicate<Entity> predicate = null, int offset = 2)
	{
		if(predicate == null) predicate = e => true;

		Chunk chunk = null;

		buffer.Clear();
		int i1 = FloatMath.Floor((aabb.x - offset) / 16f);
		int i2 = FloatMath.Floor((aabb.xprom + offset) / 16f);
		int j1 = FloatMath.Floor((aabb.y - offset) / 16f);
		int j2 = FloatMath.Floor((aabb.yprom + offset) / 16f);

		for(int i = i1; i <= i2; i++)
		{
			if(chunk == null || chunk.Coord != i)
			{
				chunk = GetChunk(i);
			}

			if(chunk == null) continue;

			for(int j = j1; j <= j2; j++)
			{
				List<Entity> entityList = chunk.GetUnitSafely(j).Entities;

				for(int k = 0; k < entityList.Count; k++)
				{
					Entity e = entityList[k];

					if(e == null || e.IsDead)
					{
						continue;
					}
					if(e.Box.Interacts(aabb) && predicate.Invoke(e))
					{
						buffer.Add(e);
					}
				}
			}
		}

		return buffer;
	}

	//Utilities

	public bool IsAccessible(IPos pos)
	{
		if(pos.y < 0 || pos.y > Chunk.MaxY)
		{
			return false;
		}
		Chunk chk = GetChunk(pos);
		return chk != null;//do not check whether it is loaded. entity serial will collapse.
	}

	public bool IsYPosAccessible(IPos pos)
	{
		return IsYPosAccessible(pos.y);
	}

	public bool IsYPosAccessible(float y)
	{
		if(y < 0 || y > Chunk.MaxY)
		{
			return false;
		}
		return true;
	}

	//STATIC

	public static bool __IsYPosAccessible(IPos pos)
	{
		return __IsYPosAccessible(pos.y);
	}

	public static bool __IsYPosAccessible(float y)
	{
		if(y < 0 || y > Chunk.MaxY)
		{
			return false;
		}
		return true;
	}

}
