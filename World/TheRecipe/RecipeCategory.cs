using Kinetic;
using Kinetic.App;

namespace Morinia.World.TheRecipe;

public class RecipeCategory : IDHolder
{

	public Factory<IRecipe> Sup;

	public RecipeCategory(Factory<IRecipe> sup)
	{
		Sup = sup;
	}

}
