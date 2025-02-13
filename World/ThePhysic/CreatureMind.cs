using Kinetic;
using Kinetic.App;

namespace Morinia.World.ThePhysic;

public class CreatureMind
{

	public List<Activity> Activities = new List<Activity>();

	public void Update(Creature creature)
	{
		foreach(Activity act in Activities)
		{
			act.Act(creature);
		}
	}

}
