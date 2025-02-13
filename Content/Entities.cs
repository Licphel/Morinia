using Kinetic.App;
using Morinia.Content.TheEntity;
using Morinia.World.ThePhysic;

namespace Morinia.Content;

public class Entities
{

	public static readonly IDMap<EntityBuilder> Registry = new IDMap<EntityBuilder>();

	public static EntityBuilder Player = Registry.Register("player", new EntityBuilder(() => new EntityPlayer()));
	public static EntityBuilder Item = Registry.Register("item", new EntityBuilder(() => new EntityItem()));
	public static EntityBuilder MagicOrb = Registry.Register("magic_orb", new EntityBuilder(() => new EntityOrb()));
	public static EntityBuilder Arrow = Registry.Register("arrow", new EntityBuilder(() => new EntityArrow()));
	public static EntityBuilder Drone = Registry.Register("drone", new EntityBuilder(() => new EntityDrone()));

}
