using System.Collections.Concurrent;
using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;
using Morinia.Api;
using Morinia.Content;
using Morinia.Content.TheEntity;
using Morinia.World.TheBlock;
using Morinia.World.TheGen;
using Morinia.World.TheLight;
using Morinia.World.ThePhysic;

namespace Morinia.World;

public sealed class Level : LevelBasic
{

	public readonly ConcurrentDictionary<int, Chunk> ChunkMap = new();
	public readonly ConcurrentDictionary<int, Chunk> ChunkMaskMap = new();

	public readonly List<Chunk> ChunkSafeList = new List<Chunk>();
	public Queue<Response> ChunkActionsQueue = new Queue<Response>();
	public ChunkIO ChunkIO;
	public Dictionary<long, Entity> EntitiesById = new Dictionary<long, Entity>();
	public Generator Generator;

	public VaryingVector2 Gravity = new VaryingVector2(0, -16);
	public FileHandle LevelSave = FileSystem.GetLocal("world/leveldat.bin");
	public long NextEntityId;
	public EntityPlayer SavedPlayer;

	public Seed Seed;

	public override LightEngine LightEngine { get; }
	public override ParticleEngine ParticleEngine { get; }

	public override IEnumerable<Chunk> ActiveChunks => ChunkSafeList;
	public override long Ticks { get; set; }
	public long TicksPerDay => 60 * 60 * 24;

	public Level()
	{
		LightEngine = new LightEngineImpl(this);
		ParticleEngine = new ParticleEngine(this);
		ChunkIO = new ChunkIO();
	}

	public override Chunk GetChunk(int coord)
	{
		return ChunkMap.GetValueOrDefault(coord, null);
	}

	public override void SetChunk(int coord, Chunk chunk)
	{
		ChunkMap[coord] = chunk;

		ChunkActionsQueue.Enqueue(() =>
		{
			ChunkSafeList.Add(chunk);
		});
	}

	public override void RemoveChunk(int coord)
	{
		Chunk chk = GetChunk(coord);
		ChunkMap.Remove(coord, out var _);

		if(chk != null)
		{
			ChunkActionsQueue.Enqueue(() =>
			{
				ChunkSafeList.Remove(chk);
				chk.OnUnloaded();
			});
		}
	}

	public Chunk ConsumeChunkMask(int coord)
	{
		Chunk msk = ChunkMaskMap.GetValueOrDefault(coord, null);
		if(msk != null)
			ChunkMaskMap.Remove(coord, out var _);
		return msk;
	}

	public override BlockState SetBlock(BlockState state, IPos pos)
	{
		if(!IsYPosAccessible(pos))
		{
			return BlockState.Empty;
		}
		Chunk chunk = GetChunk(pos.UnitX);
		if(chunk == null)
		{
			Chunk mask;

			if(ChunkMaskMap.TryGetValue(pos.UnitX, out Chunk mask0))
			{
				mask = mask0;
			}
			else
			{
				ChunkMaskMap[pos.UnitX] = mask = new Chunk(this, pos.UnitX);
			}

			return mask.SetBlock(state, pos);
		}
		return chunk.SetBlock(state, pos);
	}

	public override BlockState SetBlock(BlockState state, int x, int y, int z = 1)
	{
		if(!IsYPosAccessible(y))
		{
			return BlockState.Empty;
		}
		int cx = Posing.ToCoord(x);
		Chunk chunk = GetChunk(cx);
		if(chunk == null)
		{
			Chunk mask;

			if(ChunkMaskMap.TryGetValue(cx, out Chunk mask0))
			{
				mask = mask0;
			}
			else
			{
				ChunkMaskMap[cx] = mask = new Chunk(this, cx);
			}

			return mask.SetBlock(state, x, y, z);
		}
		return chunk.SetBlock(state, x, y, z);
	}

	public override BlockState BreakBlock(IPos pos, bool drop = false)
	{
		BlockState state = GetBlock(pos);
		if(drop)
			EntityItem.PopDrop((Level) this, state.GetDrop(this, new BlockPos(pos)), pos);
		PlayDestructParticles(state, pos);
		state.GetMaterial().SoundDestruct.PlaySound();
		SetBlock(BlockState.Empty, pos);
		return state;
	}

	public override BlockState GetBlock(int x, int y, int z = 1)
	{
		int coord = Posing.ToCoord(x);
		Chunk chunk = GetChunk(coord);
		if(chunk == null)
		{
			Chunk mask = ChunkMaskMap.GetValueOrDefault(coord, null);
			if(mask == null)
				return BlockState.Empty;
			return mask.GetBlock(x, y, z);
		}
		return chunk.GetBlock(x, y, z);
	}

	public override void AddEntity(Entity entity, IPos pos)
	{
		if(!IsAccessible(pos))
		{
			return;
		}

		ChunkUnit unit = GetUnit(pos);

		if(unit == null)
		{
			return;
		}

		entity.Locate(pos, false);

		entity.Level = this;
		entity.AddedToChunk = true;
		entity.ChunkUnit = unit;

		entity.UniqueId = NextEntityId;
		NextEntityId++;
		//It is expected not to reach long.MaxValue.

		unit._AddEntity(entity);
		entity.OnSpawned();
		EntitiesById[entity.UniqueId] = entity;

		entity.RegrabLight(true);
	}

	public void _RemoveEntity(Entity entity)
	{
		EntitiesById.Remove(entity.UniqueId);
		entity.OnDespawned();
	}

	//LOGIC

	public void Tick()
	{
		ParticleEngine.TickParticles();

		foreach(Chunk chunk in ActiveChunks)
		{
			chunk.Tick();

			if(chunk.Dirty && chunk.TicksSinceSaving > 60 * 5)
			{
				ChunkIO.WriteToBuffer(chunk, false);
				chunk.Dirty = false;
				chunk.TicksSinceSaving = 0;
			}
		}

		while(ChunkActionsQueue.Count > 0)
			ChunkActionsQueue.Dequeue()?.Invoke();

		Ticks++;
		ChunkIO._CheckOldBuffers(this);

		if(Ticks % (60 * 30) == 0)
			Save();
	}

	public void TryLoad()
	{
		if(LevelSave.Exists)
		{
			IBinaryCompound overall = BinaryIO.Read(LevelSave);

			Seed = new SeedLCG(overall.Get<long>("seed"));
			Generator = new GeneratorImpl(this);

			IBinaryList list = overall.GetListSafely("chunk_masks");

			foreach(IBinaryCompound compound in list)
			{
				int coord = compound.Get<int>("coord");
				Chunk mask = Generator.ProvideEmpty(coord);
				mask.Read(compound.GetCompoundSafely("data"));
				ChunkMaskMap[coord] = mask;
			}

			Ticks = overall.Get("time", 0);

			//it is supposed to fail to spawn.
			SavedPlayer = (EntityPlayer) Entity.ReadSpawn(overall.GetCompoundSafely("player"));
		}
		else
		{
			Seed = new SeedLCG();
			Generator = new GeneratorImpl(this);
		}
	}

	public void Save()
	{
		foreach(Chunk chunk in ActiveChunks)
		{
			ChunkIO.WriteToBuffer(chunk, false);
		}
		ChunkIO.WriteToDisk();

		IBinaryCompound overall = IBinaryCompound.New();

		overall.Set("seed", Seed.GetCSeed());

		//chunk masks
		IBinaryList list = IBinaryList.New();

		foreach(KeyValuePair<int, Chunk> kv in ChunkMaskMap)
		{
			IBinaryCompound compound = IBinaryCompound.New();
			compound.Set("coord", kv.Key);
			IBinaryCompound mskdata = IBinaryCompound.New();
			kv.Value.Write(mskdata, false);
			compound.Set("data", mskdata);
			list.Insert(compound);
		}

		overall.Set("time", Ticks);
		overall.Set("chunk_masks", list);

		IBinaryCompound compound1 = IBinaryCompound.New();

		SavedPlayer.Write(compound1);

		overall.Set("player", compound1);

		BinaryIO.Write(overall, LevelSave);
	}

}
