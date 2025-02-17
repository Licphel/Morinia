using Kinetic.App;
using Kinetic.Math;
using Morinia.Content;
using Morinia.World.TheBlock;
using Morinia.World.TheLiquid;

namespace Morinia.World.TheGen;

public class GeneratorImpl : Generator
{

	readonly float[] borderPossibilities = { 1, 0.9f, 0.75f, 0.25f };

	readonly List<Decorator> decorators = new List<Decorator>();
	readonly List<Decorator> decoratorsFollowing = new List<Decorator>();
	readonly List<Decorator> decoratorsSurface = new List<Decorator>();
	readonly List<Decorator> decoratorsWaiting = new List<Decorator>();
	readonly BlockState stateBorder = Blocks.Border.Instantiate();

	readonly List<PostProcessor> processors = new List<PostProcessor>();
	readonly Noise noise;
	readonly Noise activeness;
	readonly Noise rainfall;
	readonly Noise temperature;
	readonly Noise continent;
	readonly Noise stoneuniq;

	public Level Level;

	public GeneratorImpl(Level level)
	{
		Level = level;

		//pay attention to this noise, it shouldn't have sth to do with the section coord!
		noise = new NoiseOctave(level.Seed.Copyx(183), 2);//some magic numbers...

		rainfall = new NoiseOctave(level.Seed.Copyx(923), 2);
		temperature = new NoiseOctave(level.Seed.Copyx(1256), 2);
		activeness = new NoiseOctave(level.Seed.Copyx(1847), 1);
		continent = new NoiseOctave(level.Seed.Copyx(2508), 1);
		stoneuniq = new NoiseVoronoi(level.Seed.Copyx(6254));

		AddDecorator(new DecoratorCave());//add it first and caves should be decorated more then!
		AddDecorator(new DecoratorPlants());
		AddProcessor(new PostProcessorFeatures());
	}

	public void Provide(int coord)
	{
		Chunk chunk = Level.GetChunk(coord);
		if(chunk != null)
			return;

		chunk = ProvideEmpty(coord);

		if(Level.ChunkIO.IsChunkArchived(chunk))
		{
			Level.ChunkIO.Read(chunk);
			submitToLevel(chunk, coord);
			return;
		}

		_Provide(chunk, coord);
	}

	public Chunk ProvideEmpty(int coord)
	{
		Chunk chunk = new Chunk(Level, coord);
		Level.SetChunk(coord, chunk);
		return chunk;
	}

	public bool ProvideAsync(int coord)
	{
		Chunk chunk = Level.GetChunk(coord);
		if(chunk != null)
			return true;

		chunk = ProvideEmpty(coord);

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
		Level.ConsumeChunkMask(coord);
		chunk.OnLoaded();
		Level.SetChunk(coord, chunk);
	}

	void _Provide(Chunk chunk, int coord)
	{
		foreach(Decorator decorator in decorators)
		{
			decorator.InitSeed(Level, chunk);
		}

		Seed seed = Level.Seed.Copyx(87 * chunk.Coord);
		GenerateContext ctx = new GenerateContext();

		for(int s = 0; s < 16; s++)
		{
			int x = s + coord * 16;

			float hd = 1;
			float sc = Chunk.Overland;
			float surface = noise.Generate(x * hd / 64f, 1, 1) * sc + Chunk.SeaLevel;
			chunk.Surface[s] = (int) surface;
			
			for(int y = Chunk.MaxY; y >= 0; y--)
			{
				GetLocationData(x, y, (int) surface, ctx);
				Biome biome = ctx.Biome;
				BlockState[] rockType = ctx.RockSerie;
				BlockState[] soilType = ctx.SoilSerie;
				
				int soildep = ctx.Rain switch
				{
					<= 0.25f => 0,
					<= 0.35f => 1,
					<= 0.5f => 2,
					<= 0.75f => 3,
					_ => 4
				};
				
				if(y <= surface)
				{
					int dist = (int) surface - y;
					if(dist == 0)
					{
						chunk.SetBlock(soilType[0], x, y);
						chunk.SetBlock(soilType[1], x, y, 0);
					}
					else if(dist <= soildep)
					{
						BlockState state = soilType[1];
						chunk.SetBlock(state, x, y);
						chunk.SetBlock(state, x, y, 0);
					}
					else if(dist <= soildep + 4)
					{
						BlockState state = rockType[0];
						chunk.SetBlock(state, x, y);
						chunk.SetBlock(state, x, y, 0);
					}
					else
					{
						BlockState state = rockType[1];
						chunk.SetBlock(state, x, y);
						chunk.SetBlock(state, x, y, 0);

						if(y <= 3)
						{
							if(seed.NextFloat() < borderPossibilities[y])
							{
								chunk.SetBlock(stateBorder, x, y);
							}
							if(seed.NextFloat() < borderPossibilities[y])
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

	public void GetLocationData(int x, int y, int surface, GenerateContext ctx)
	{
		float temp = ctx.Temp = temperature.Generate(x / 32f / 16f, y / 256f, 1);
		float rain = ctx.Rain = rainfall.Generate(x / 32f / 16f, y / 256f, 1);
		float act = ctx.Act = activeness.Generate(x / 32f / 16f, y / 128f, 1);
		float dep;
		float cont = ctx.Cont = continent.Generate(x / 32f / 16f, y / 1024f, 1);

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
			s += _IncS(biome.Depth, dep) * 3;//Ensure they generate at correct depth.
			s += _IncS(biome.Continent, cont) * 3;

			if(sm > s)
			{
				sm = s;
				bm = biome;
			}
		}

		ctx.Biome = bm;

		float uniq = stoneuniq.Generate(x / 128f, y / 128f, 1);
		if(surface - y > Chunk.RockTransverse)
			uniq = 1 - uniq;
		ctx.RockSerie = SerieOfRock.GetProperType(uniq, surface - y);
		ctx.SoilSerie = SerieOfSoil.GetProperType(rain, temp) ?? ctx.RockSerie;
	}

	static float _IncS(float m, float v)
	{
		if(m == -1)
			return 1.125f;
		return (float) Math.Pow(Math.Abs(m - v) + 1, 2);
	}

}
