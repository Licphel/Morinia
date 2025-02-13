using Kinetic.App;
using Morinia.Content.TheRecipe;
using Morinia.World.TheRecipe;

namespace Morinia.Content;

public class Recipes
{

	public static readonly IDMap<RecipeCategory> Registry = new IDMap<RecipeCategory>();

	public static RecipeCategory Handcraft = Registry.Register("handcraft", new RecipeCategory(() => new RecipeHandcraft()));
	public static RecipeCategory Furnace = Registry.Register("furnace", new RecipeCategory(() => new RecipeFurnace()));

}
