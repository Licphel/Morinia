using Kinetic.App;

namespace Morinia.World.ThePhysic;

public delegate Entity SupplyEntity();

public class EntityBuilder : IDHolder
{

	public SupplyEntity Sup;

	public EntityBuilder(SupplyEntity sup)
	{
		Sup = sup;
	}

	public Entity Instantiate()
	{
		return Sup.Invoke();
	}

}
