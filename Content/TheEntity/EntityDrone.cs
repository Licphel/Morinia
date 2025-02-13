using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheEntity;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheEntity;

public class EntityDrone : Creature
{

	public EntityDrone()
	{
		Mind.Activities.Add(new ActCreatureFlyWander());
		Box.Resize(12 / 16f, 12 / 16f);
		VisualSize.x = 16 / 16f;
		VisualSize.y = 16 / 16f;
		Health = MaxHealth = 10000;
	}

	public override EntityBuilder Builder => Entities.Drone;

	public override void Tick()
	{
		base.Tick();

		RestrictVelocity();
		Move();
		CheckVelocity();

		if(TimeSchedule.PeriodicTask(1))
		{
			Box b1 = Box;
			b1.Expand(20, 20);
			List<Entity> lst = Level.GetNearbyEntities(b1, e => e is Creature && e != this);
			lst.ForEach(e =>
			{
				Creature c = (Creature) e;
				EntityOrb eo = new EntityOrb();
				eo.Owner = this;
				float deg = FloatMath.AtanDeg(-Pos.y + e.Pos.y, -Pos.x + e.Pos.x);
				eo.Velocity.FromDeg(15, deg);
				Level.AddEntity(eo, Pos);
			});
		}
	}

	public override EntityTessellator GetTessellator()
	{
		return Tessellator.Value0;
	}

	class Tessellator : EntityTessellator
	{

		public static readonly Tessellator Value0 = new Tessellator();

		public void Draw(SpriteBatch batch, Entity entity, Box box)
		{
			EntityDrone drn = (EntityDrone) entity;

			batch.Color4(entity.SmoothedLight);
			if(drn.HurtCooldown > 0)
			{
				batch.Merge4(1, 0.3f, 0.3f);
			}
			batch.Draw(GameTextures.EntityDrone, box, 0, 0, 16, 16);
			batch.NormalizeColor();
			batch.Draw(GameTextures.EntityDrone, box, 0, 16, 16, 16);
		}

	}

}
