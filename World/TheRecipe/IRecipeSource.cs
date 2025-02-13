using Morinia.World.TheItem;

namespace Morinia.World.TheRecipe;

public interface IRecipeSource
{

	ItemContainer ItemContainer { get; }

	bool IsResultDestinationAccessible(ItemStack stack, IRecipe recipe);

	void CallAssemble(ItemStack stack, IRecipe recipe);

}
