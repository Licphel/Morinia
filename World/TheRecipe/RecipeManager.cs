using Kinetic.App;
using Kinetic.IO;
using Loh;
using Loh.Values;
using Morinia.Content;

namespace Morinia.World.TheRecipe;

public class RecipeManager
{

	static readonly Dictionary<ID, IRecipe> recipeMap = new Dictionary<ID, IRecipe>();
	static readonly Dictionary<RecipeCategory, List<IRecipe>> typeMap = new Dictionary<RecipeCategory, List<IRecipe>>();

	public static IRecipe GetRecipe(ID path)
	{
		return recipeMap[path];
	}

	public static List<IRecipe> GetRecipes(RecipeCategory type)
	{
		List<IRecipe> recipes = typeMap.GetValueOrDefault(type, null);
		return recipes == null ? new List<IRecipe>() : recipes;
	}

	public static void Decode(ID key, FileHandle file)
	{
		RecipeCategory category = Recipes.Registry[key.Space + ":" + file.Exit().Name];
		LohTable table = LohEngine.Exec(file);

		IRecipe recipe = category.Sup();
		recipe.Read(table, key);

		recipeMap[key] = recipe;

		if(!typeMap.ContainsKey(category))
		{
			typeMap[category] = new List<IRecipe>();
		}
		List<IRecipe> lst = typeMap[category];
		lst.Add(recipe);
	}

}
