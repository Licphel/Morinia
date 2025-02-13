using Kinetic;
using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;

namespace Morinia.World.ThePhysic;

public abstract class Creature : Entity
{

	public int Health, MaxHealth;
	public int HurtCooldown;

	public CreatureMind Mind = new CreatureMind();

	public override void Tick()
	{
		base.Tick();

		Mind.Update(this);

		HurtCooldown--;
		if(HurtCooldown < 0) HurtCooldown = 0;

		float avx = Math.Abs(Velocity.x);
		if(avx > 0.1f && TouchDown)
		{
			int mod = FloatMath.Ceiling(avx);
			mod = Math.Clamp(mod, 1, 10);
			if(TimeSchedule.PeriodicTask(12 - mod) && Seed.Global.NextFloat() < 0.25f)
			{
				BlockStepping.GetMaterial().SoundStep.PlaySound();
			}
		}
	}

	public int Hit(float value)
	{
		if(HurtCooldown <= 0)
		{
			Health -= (int) value;
			if(Health <= 0)
			{
				Health = 0;
				IsDead = true;
			}
			HurtCooldown = 5;
		}
		return Health;
	}

	public override void Read(IBinaryCompound compound)
	{
		base.Read(compound);

		Health = compound.Get<int>("health");
		MaxHealth = compound.Get<int>("max_health");
		HurtCooldown = compound.Get<int>("hcd");
	}

	public override void Write(IBinaryCompound compound)
	{
		base.Write(compound);

		compound.Set("health", Health);
		compound.Set("max_health", MaxHealth);
		compound.Set("hcd", HurtCooldown);
	}

}
