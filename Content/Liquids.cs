using Kinetic.App;
using Morinia.World.TheLiquid;

namespace Morinia.Content;

public class Liquids
{

	public static readonly IDMap<Liquid> Registry = new IDMap<Liquid>();

	public static Liquid Empty = Registry.RegisterDefaultValue("empty", new Liquid());
	public static Liquid Water = Registry.Register("water", new Liquid());

}
