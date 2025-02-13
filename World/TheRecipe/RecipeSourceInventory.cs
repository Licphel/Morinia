using Kinetic.Math;
using Morinia.Content.TheEntity;
using Morinia.World.TheItem;

namespace Morinia.World.TheRecipe;

public class RecipeSourceInventory : IRecipeSource
{

	readonly Inventory inv;
	readonly Vector2 range;

	public RecipeSourceInventory(Inventory inv, Vector2 optRange)
	{
		this.inv = inv;
		range = optRange;
	}

	public ItemContainer ItemContainer => inv;

	public void CallAssemble(ItemStack stack, IRecipe recipe)
	{
		inv.Add(stack, range.xi, range.yi);

		foreach(IRecipeMatcher m in recipe.GetMatchers())
		{
			m.Consume(this);
		}
	}

	public bool IsResultDestinationAccessible(ItemStack stack, IRecipe recipe)
	{
		return inv.Add(stack, range.xi, range.yi, true).IsEmpty;
	}

}
