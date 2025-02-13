using Morinia.Content.TheEntity;
using Morinia.World.TheItem;

namespace Morinia.World.TheRecipe;

public class RecipeSourcePlayer : IRecipeSource
{

	readonly EntityPlayer player;

	public RecipeSourcePlayer(EntityPlayer player)
	{
		this.player = player;
	}

	public ItemContainer ItemContainer => player.Inv;

	public void CallAssemble(ItemStack stack, IRecipe recipe)
	{
		if(player.OpenContainer.Pickup.IsEmpty)
		{
			player.OpenContainer.Pickup = stack;
		}
		else
		{
			player.OpenContainer.Pickup.Merge(stack);
		}

		foreach(IRecipeMatcher m in recipe.GetMatchers())
		{
			m.Consume(this);
		}
	}

	public bool IsResultDestinationAccessible(ItemStack stack, IRecipe recipe)
	{
		return player.OpenContainer.Pickup.CanMergeFully(stack);
	}

}
