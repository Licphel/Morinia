using Kinetic.App;
using Kinetic.IO;
using Morinia.World.TheItem;
using Morinia.World.TheRecipe;

namespace Morinia.Content.TheRecipe;

public class RecipeFurnace : RecipeSimple
{

	public ItemStack Output0 => Outputs[0];

	public override RecipeCategory Category => Recipes.Furnace;

	public int Cooktime;

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

	public override void Read(IBinaryCompound compound, ID id)
	{
		base.Read(compound, id);
		Cooktime = compound.Get<int>("cooktime", 300);
	}

}
