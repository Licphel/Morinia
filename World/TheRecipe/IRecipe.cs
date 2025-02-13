using Kinetic.App;
using Kinetic.IO;
using Morinia.World.TheItem;

namespace Morinia.World.TheRecipe;

public interface IRecipe
{

	ID Path { get; }
	RecipeCategory Category { get; }

	bool Matches(IRecipeSource source);

	//The Assemble method should do:
	//1. Check if the recipe is matched, generally call #Matches.
	//2. Check if the destination is accessible.
	//3. If all fine, set the output to the source.
	bool Assemble(IRecipeSource source);

	List<IRecipeMatcher> GetMatchers();

	List<ItemStack> GetOutputs();

	void Read(IBinaryCompound compound, ID id);

}
