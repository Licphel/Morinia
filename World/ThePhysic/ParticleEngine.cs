using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;

namespace Morinia.World.ThePhysic;

public class ParticleEngine
{

	readonly Level level;

	readonly List<Particle> particles = new List<Particle>();

	public ParticleEngine(Level level)
	{
		this.level = level;
	}

	public void Add(Particle particle, IPos pos)
	{
		particle.Level = level;
		particle.Locate(pos, false);
		particle.OnSpawned();
		particles.Add(particle);
	}

	public void AddSpreading(Particle particle, IPos pos, float r)
	{
		float rx = Seed.Global.NextFloat(-r, r);
		float ry = Seed.Global.NextFloat(-r, r);
		PrecisePos ppos = new PrecisePos(pos.x + rx, pos.y + ry);
		Add(particle, ppos);
	}

	public void TickParticles()
	{
		for(int i = particles.Count - 1; i >= 0; i--)
		{
			Particle p = particles[i];

			if(p == null)
			{
				continue;
			}

			p.Tick();
		}
	}

	public void DrawParticles(SpriteBatch batch)
	{
		for(int i = particles.Count - 1; i >= 0; i--)
		{
			Particle p = particles[i];

			if(p == null)
			{
				continue;
			}

			p.DrawInternal(batch);

			if(p.Removal)
			{
				particles.RemoveAt(i);
			}
		}
	}

}
