using Morinia.World.TheItem;
using Morinia.World.TheRecipe;

namespace Morinia.Content.TheRecipe;

public class RecipeHandcraft : RecipeSimple
{

	public ItemStack Output0 => Outputs[0];

	public override RecipeCategory Category => Recipes.Handcraft;

	public override bool Assemble(IRecipeSource source)
	{
		if(!Matches(source))
		{
			return false;
		}
		if(source.IsResultDestinationAccessible(Output0, this))
		{
			source.CallAssemble(Output0.Copy(), this);
			return true;
		}
		return false;
	}

}
