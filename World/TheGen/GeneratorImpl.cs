using Kinetic.App;
using Kinetic.Math;
using Morinia.Content;
using Morinia.World.TheBlock;
using Morinia.World.TheLiquid;

namespace Morinia.World.TheGen;

public class GeneratorImpl : IGenerator
{

	readonly float[] bdchancearr = { 1, 0.9f, 0.75f, 0.25f };

	readonly List<Decorator> decorators = new List<Decorator>();
	readonly List<Decorator> decoratorsFollowing = new List<Decorator>();
	readonly List<Decorator> decoratorsSurface = new List<Decorator>();
	readonly List<Decorator> decoratorsWaiting = new List<Decorator>();
	readonly BlockState stateBorder = Blocks.Border.Instantiate();

	readonly List<PostProcessor> processors = new List<PostProcessor>();
	readonly INoise noise;
	readonly INoise activeness;
	readonly INoise rainfall;
	readonly INoise temperature;
	readonly INoise continent;
	readonly INoise stoneuniq;

	public Level Level;

	public GeneratorImpl(Level level)
	{
		Level = level;

		//pay attention to this noise, it shouldn't have sth to do with the section coord!
		noise = new ComplexNoise(level.Seed.Copyx(183), 2);//some magic numbers...

		rainfall = new ComplexNoise(level.Seed.Copyx(923), 2);
		temperature = new ComplexNoise(level.Seed.Copyx(1256), 2);
		activeness = new ComplexNoise(level.Seed.Copyx(1847), 1);
		continent = new ComplexNoise(level.Seed.Copyx(2508), 1);
		stoneuniq = new ComplexNoise(level.Seed.Copyx(6254), 1);

		AddDecorator(new DecoratorCave());//add it first and caves should be decorated more then!
		AddDecorator(new DecoratorPlants());
		AddProcessor(new PostProcessorFeatures());
	}

	public void Provide(int coord)
	{
		Chunk chunk = Level.GetChunk(coord);
		if(chunk != null)
			return;

		chunk = MakeEmptyChunk(coord);

		if(Level.ChunkIO.IsChunkArchived(chunk))
		{
			Level.ChunkIO.Read(chunk);
			submitToLevel(chunk, coord);
			return;
		}

		_Provide(chunk, coord);
	}

	public Chunk MakeEmptyChunk(int coord)
	{
		Chunk chunk = new Chunk(Level, coord);
		Level.SetChunk(coord, chunk);
		return chunk;
	}

	public bool GenerateAsync(int coord)
	{
		Chunk chunk = Level.GetChunk(coord);
		if(chunk != null)
			return true;

		chunk = MakeEmptyChunk(coord);

		if(Level.ChunkIO.IsChunkArchived(chunk))
		{
			Level.ChunkIO.Read(chunk);
			submitToLevel(chunk, coord);
			return true;
		}

		new Coroutine(() =>
		{
			_Provide(chunk, coord);
		}).Start();
		return false;
	}

	public void AddDecorator(Decorator dec)
	{
		switch(dec.Type)
		{
			case DecoratorType.SURFACE:
				decoratorsSurface.Add(dec);
				break;
			case DecoratorType.WAITING:
				decoratorsWaiting.Add(dec);
				break;
			case DecoratorType.FOLLOWING:
				decoratorsFollowing.Add(dec);
				break;
		}
		decorators.Add(dec);
	}

	public void AddProcessor(PostProcessor proc)
	{
		processors.Add(proc);
	}

	void submitToLevel(Chunk chunk, int coord)
	{
		Chunk mask = Level.ConsumeChunkMask(coord);
		if(mask != null)
		{
			mask.BlockMap.Cover(chunk.BlockMap);
			mask.LiquidMap.Cover(chunk.LiquidMap);
		}
		chunk.OnLoaded();
		chunk.Dirty = true;
		Level.ChunkIO.WriteToBuffer(chunk, false);
	}

	void _Provide(Chunk chunk, int coord)
	{
		foreach(Decorator decorator in decorators)
		{
			decorator.InitSeed(Level, chunk);
		}

		Seed seed = Level.Seed.Copyx(87 * chunk.Coord);

		for(int s = 0; s < 16; s++)
		{
			int x = s + coord * 16;

			float hd = 1;
			float sc = Chunk.Overland;
			float surface = noise.Generate(x * hd / 64f, 1, 1) * sc + Chunk.SeaLevel;
			chunk.Surface[s] = (int) surface;
			int soildep = (int) noise.Generate(x / 16f, 1, 1) * 4 + 3;

			for(int y = Chunk.MaxY; y >= 0; y--)
			{
				Biome biome = getLocationBiome(x, y, coord, (int) surface);
				int yclo = (int) (y + (surface - Chunk.SeaLevel) * 0.25f + soildep * 0.1f);
				BlockState[] stoneType = Geolayer.GetProperType(stoneuniq.Generate(x / 256f, y / 256f, 1), yclo);

				if(y <= surface)
				{
					int dist = (int) surface - y;
					if(dist == 0)
					{
						chunk.SetBlock(biome.GetState(Biome.LayerFoliage, stoneType), x, y);
						if(dist >= 1) chunk.SetBlock(biome.GetState(Biome.LayerSoil, stoneType), x, y, 0);
					}
					else if(dist <= soildep && seed.NextFloat() < (soildep + 2 - dist) / 3f)
					{
						BlockState state = biome.GetState(Biome.LayerSoil, stoneType);
						chunk.SetBlock(state, x, y);
						chunk.SetBlock(state, x, y, 0);
					}
					else if(dist <= soildep + 4 && seed.NextFloat() < (soildep + 8 - dist) / 6f)
					{
						BlockState state = biome.GetState(Biome.LayerStone, stoneType);
						chunk.SetBlock(state, x, y);
						chunk.SetBlock(state, x, y, 0);
					}
					else
					{
						BlockState state = biome.GetState(Biome.LayerStone, stoneType);
						chunk.SetBlock(state, x, y);
						chunk.SetBlock(state, x, y, 0);

						if(y <= 3)
						{
							if(seed.NextFloat() < bdchancearr[y])
							{
								chunk.SetBlock(stateBorder, x, y);
							}
							if(seed.NextFloat() < bdchancearr[y])
							{
								chunk.SetBlock(stateBorder, x, y, 0);
							}
						}
					}
				}
				else if(biome.HasSpecialAir)
				{
					chunk.SetBlock(biome.Air, x, y);
					chunk.SetBlock(biome.Air, x, y, 0);
				}

				for(int p = 0; p < decoratorsFollowing.Count; p++)
				{
					decoratorsFollowing[p].Decorate(Level, chunk, x, y);
				}
			}
		}

		if(decoratorsWaiting.Count != 0)
		{
			for(int s = 0; s < 16; s++)
			{
				int x = s + coord * 16;

				for(int y = Chunk.MaxY; y >= 0; y--)
				{
					for(int p = 0; p < decoratorsWaiting.Count; p++)
					{
						decoratorsWaiting[p].Decorate(Level, chunk, x, y);
					}
				}
			}
		}

		if(decoratorsSurface.Count != 0)
		{
			for(int s = 0; s < 16; s++)
			{
				int x = s + coord * 16;
				int y = chunk.Surface[s & 15];

				for(int p = 0; p < decoratorsSurface.Count; p++)
				{
					Decorator dec = decoratorsSurface[p];
					dec.Decorate(Level, chunk, x, y + dec.SurfaceOffset);
				}
			}
		}

		foreach(PostProcessor processor in processors)
		{
			processor.Process(Level, chunk);
		}

		submitToLevel(chunk, coord);
	}

	Biome getLocationBiome(int x, int y, int coord, int surface)
	{
		float temp = temperature.Generate(x / 32f / 16f, y / 256f, 1);
		float rain = rainfall.Generate(x / 32f / 16f, y / 256f, 1);
		float act = activeness.Generate(x / 32f / 16f, y / 128f, 1);
		float dep;
		float cont = continent.Generate(x / 32f / 16f, y / 1024f, 1);

		if(y >= Chunk.SeaLevel + Chunk.Overland || y <= Chunk.SeaLevel - Chunk.Overland)
			cont = 1;

		if(y >= surface)
		{
			dep = (float) (y - surface) / (Chunk.Height - surface);
		}
		else
		{
			dep = -(float) (surface - y) / surface;
		}

		Biome bm = null;
		float sm = float.PositiveInfinity;

		Biome biome;
		List<Biome> list = Biomes.Registry.IdList;

		for(int i = 0; i < list.Count; i++)
		{
			biome = list[i];

			float s = 0;

			s += _IncS(biome.Temperature, temp);
			s += _IncS(biome.Rainfall, rain);
			s += _IncS(biome.Activeness, act);
			s += _IncS(biome.Depth, dep) * 256;//Ensure they generate at correct depth.
			s += _IncS(biome.Continent, cont) * 32;

			if(sm > s)
			{
				sm = s;
				bm = biome;
			}
		}

		return bm;
	}

	static float _IncS(float m, float v)
	{
		if(m == -1)
			return 1.125f;
		return (float) Math.Pow(Math.Abs(m - v) + 1, 2);
	}

}
