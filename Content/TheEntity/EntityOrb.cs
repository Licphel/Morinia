using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheEntity;
using Morinia.World.TheLight;
using Morinia.World.ThePhysic;

namespace Morinia.Content.TheEntity;

public class EntityOrb : Entity
{

	public Creature Owner;

	public EntityOrb()
	{
		Box.Resize(2 / 16f, 2 / 16f);
		VisualSize.x = 8 / 16f;
		VisualSize.y = 8 / 16f;
	}

	public override EntityBuilder Builder => Entities.MagicOrb;

	public override float ReboundFactor => 1f;

	public override void Tick()
	{
		base.Tick();

		RestrictVelocity(0.05f, 0.05f);
		Move();
		CheckVelocity();

		List<Entity> lst = Level.GetNearbyEntities(Box, e => e is Creature && e != Owner);
		lst.ForEach(e =>
		{
			Creature c = (Creature) e;
			c.Hit(50);
			IsDead = true;
		});

		if(Touch && Seed.Global.NextFloat() < 0.25f)
		{
			IsDead = true;
		}
	}

	public override float CastLight(byte pipe)
	{
		return LightPass.Switch3(pipe, 0.75f, 0.25f, 0.25f);
	}

	public override EntityTessellator GetTessellator()
	{
		return Tessellator.Value0;
	}

	class Tessellator : EntityTessellator
	{

		public static readonly Tessellator Value0 = new Tessellator();
		static readonly IconAnim anim = new IconAnim(GameTextures.EntityOrb, 4, 16, 16, 0, 0).Seconds(0.25f);

		public void Draw(SpriteBatch batch, Entity entity, Box box)
		{
			batch.Color4(entity.SmoothedLight);
			batch.Draw(anim, box);
			batch.NormalizeColor();
		}

	}

}
