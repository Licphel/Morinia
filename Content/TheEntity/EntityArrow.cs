using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheEntity;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheEntity;

public class EntityArrow : Entity
{

	public Creature Owner;

	public EntityArrow()
	{
		Box.Resize(1 / 16f, 1 / 16f);
		VisualSize.x = 8 / 16f;
		VisualSize.y = 8 / 16f;
	}

	public override EntityBuilder Builder => Entities.Arrow;

	public override void Tick()
	{
		base.Tick();

		RestrictVelocity(0.05f, 0.05f);
		ApplyGravity(new Vector2(0.5f, 0.5f));
		Move();
		CheckVelocity();

		List<Entity> lst = Level.GetNearbyEntities(Box, e => e is Creature && e != Owner);
		lst.ForEach(e =>
		{
			Creature c = (Creature) e;
			c.Hit(20);
			IsDead = true;
		});

		if(Touch)
		{
			IsDead = true;
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
			batch.Color4(entity.SmoothedLight);
			batch.Matrices.Push();
			batch.Matrices.RotateRad(entity.VeloRad, 4 / 16f, 4 / 16f);
			batch.Draw(GameTextures.EntityArrow, box);
			batch.Matrices.Pop();
			batch.NormalizeColor();
		}

	}

}
