using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;
using Morinia.Content;
using Morinia.Logic;
using Morinia.World.TheBlock;
using Morinia.World.TheLight;
using Morinia.World.TheLiquid;

namespace Morinia.World;

public sealed class Chunk : ChunkBasic
{

	public const int Width = 16;
	public const int Height = 256;
	public const int Depth = 2;
	public const int Area = Width * Height;
	public const int Volume = Depth * Area;
	public const int MaxY = Height - 1;

	public const int Overland = 24;
	public const int SeaLevel = 102;
	public const int RockTransverse = 48;
	public const int SurfaceLevel = SeaLevel + Overland;
	public const int SpaceLevel = 164;

	public static int RefreshTimeNormal = 200;

	public static BlockMapScale ScaleBlocks = new BlockMapScale(Width, Height, Depth);
	public static LiquidMapScale ScaleLiquids = new LiquidMapScale(Width, Height);
	public int Coord;
	public bool Dirty;

	public Level Level;

	//[0] to storage block light and coords out for z = 0,
	//[1] to storage sky light and coords out for z = 1.
	LightWare[,,] lightStorages = new LightWare[Width, Height, Depth];
	LightWare[,,] lightStoragesBuffer = new LightWare[Width, Height, Depth];
	public int RefreshTime;
	public int[] Surface = new int[16];
	public int TicksSinceSaving;
	public Dictionary<BlockPos, BlockEntity> BlockEntities = new Dictionary<BlockPos, BlockEntity>();
	public BlockMap BlockMap = new BlockMap(BlockState.Empty, ScaleBlocks);
	public LiquidMap LiquidMap = new LiquidMap(Liquid.Empty, ScaleLiquids);

	public Chunk(Level level, int coord)
	{
		Level = level;
		Coord = coord;

		for(int i = 0; i < Units.Length; i++)
		{
			Units[i] = new ChunkUnit(this, new UnitPos(coord, i));
		}
	}

	public override ChunkUnit[] Units { get; } = new ChunkUnit[Height / Width];

	public bool IsLoadedUp { get; set; }

	public void OnLoaded()
	{
		IsLoadedUp = true;
	}

	public void OnUnloaded()
	{
		IsLoadedUp = false;
	}

	public Seed GetUniqSeed()
	{
		return Level.Seed.Copyx(Coord * 3);
	}

	public override BlockState SetBlock(BlockState state, int x, int y, int z = 1)
	{
		if(!Level.IsYPosAccessible(y))
		{
			return BlockState.Empty;
		}

		BlockPos pos = new BlockPos(x, y, z);
		BlockState i0 = BlockMap.Set(x, y, z, state);

		if(state.GetShape() == BlockShape.Solid)
			LiquidMap.Set(x, y, new LiquidStack(Liquid.Empty, 0));

		BlockEntity entity = state.CreateEntityBehavior(Level, pos);
		SetBlockEntity(entity, pos);
		Dirty = true;

		if(IsLoadedUp)
			NotifyChange(pos);

		return i0;
	}

	public void NotifyChange(BlockPos pos)
	{
		foreach(Direction d in Direction.Values)
		{
			BlockPos pos1 = d.Step(pos);
			BlockState state1 = Level.GetBlock(pos1);
			state1.OnNearbyChanged(Level, pos1, pos);
		}
	}

	public override BlockState GetBlock(int x, int y, int z = 1)
	{
		if(!Level.IsYPosAccessible(y))
		{
			return BlockState.Empty;
		}
		return BlockMap.Get(x, y, z);
	}

	public override BlockEntity GetBlockEntity(IPos pos)
	{
		return BlockEntities.GetValueOrDefault(pos is BlockPos tp ? tp : new BlockPos(pos), null);
	}

	public override BlockEntity SetBlockEntity(BlockEntity entity, IPos pos)
	{
		BlockPos tp = new BlockPos(pos);
		BlockEntity t0 = GetBlockEntity(tp);
		t0?.OnDespawned(false);
		if(entity == null)
		{
			BlockEntities.Remove(tp);
		}
		else
		{
			BlockEntities[tp] = entity;
			entity.OnSpawned();
		}
		t0?.OnDespawned(true);
		Dirty = true;
		return t0;
	}

	//LOGIC

	public void Tick()
	{
		if(!IsLoadedUp) return;

		foreach(ChunkUnit unit in Units)
			unit.Tick();

		foreach(BlockEntity te in BlockEntities.Values)
			te.Tick();

		for(int i = 0; i < 1; i++)
		{
			BlockPos pos = new BlockPos
                (Coord * 16 + Seed.Global.NextInt(16),
				Seed.Global.NextInt(Height),
				Seed.Global.NextInt(2));
			GetBlock(pos).OnRandomTick(Level, pos);
		}

		TicksSinceSaving++;
		RefreshTime--;

		if(RefreshTime <= 0)
		{
			Level.ChunkIO.WriteToBuffer(this, true);
			Level.RemoveChunk(Coord);
		}

		if(TimeSchedule.PeriodicTask(1 / 20f))
			Liquid.TickChunkLiquid(this);
	}

	//Codec

	public void Write(IBinaryCompound compound, bool removal = false)
	{
		compound.Set("surfaces", Surface);
		compound.Set("block_bytes", BlockMap.Bytes);
		compound.Set("liquid_bytes", LiquidMap.Bytes);

		foreach(ChunkUnit unit in Units)
		{
			IBinaryCompound compound1 = IBinaryCompound.New();
			unit.Write(compound1, removal);
			compound.Set("unit_" + unit.Pos.UnitY, compound1);
		}

		IBinaryList telist = IBinaryList.New();

		foreach(BlockEntity te in BlockEntities.Values)
		{
			IBinaryCompound c1 = IBinaryCompound.New();
			te.Write(c1);
			c1.Set("x", te.Pos.BlockX);
			c1.Set("y", te.Pos.BlockY);
			c1.Set("z", te.Pos.BlockZ);
			telist.Insert(c1);
		}

		compound.Set("block_entities", telist);

		compound.Set("chunk_version", Game.Version.Iteration);
	}

	public void Read(IBinaryCompound compound)
	{
		Surface = compound.Get("surfaces", new int[Width]);
		BlockMap.Bytes = compound.Get("block_bytes", new byte[ScaleBlocks.SizeInBytes]);
		LiquidMap.Bytes = compound.Get("liquid_bytes", new byte[ScaleLiquids.SizeInBytes]);

		foreach(ChunkUnit unit in Units)
		{
			unit.Read(compound.GetCompoundSafely("unit_" + unit.Pos.UnitY));
		}

		IBinaryList telist = compound.GetListSafely("block_entities");

		foreach(IBinaryCompound c1 in telist)
		{
			int x = c1.Get<int>("x");
			int y = c1.Get<int>("y");
			int z = c1.Get<int>("z");
			BlockEntity te = GetBlock(x, y, z).CreateEntityBehavior(Level, new BlockPos(x, y, z));
			te.Read(c1);
			BlockEntities[te.Pos] = te;
		}
	}

	public LightWare _LE_Surfpack(int x, int y, int type)
	{
		LightWare coords = lightStorages[x & 15, y & 255, type];
		if(coords == null)
		{
			coords = new LightWare(this, type);
			lightStorages[x & 15, y & 255, type] = coords;
		}
		return coords;
	}

	public LightWare _LE_Bufpack(int x, int y, int type)
	{
		LightWare coords = lightStoragesBuffer[x & 15, y & 255, type];
		if(coords == null)
		{
			coords = new LightWare(this, type);
			lightStoragesBuffer[x & 15, y & 255, type] = coords;
		}
		return coords;
	}

	public void _LE_SwapBuffer()
	{
		(lightStorages, lightStoragesBuffer) = (lightStoragesBuffer, lightStorages);
	}

}
