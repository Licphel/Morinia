using Kinetic.App;
using Kinetic.IO;
using Morinia.World.TheItem;

namespace Morinia.World.TheRecipe;

public abstract class RecipeSimple : IRecipe
{

	public List<IRecipeMatcher> Matchers = new List<IRecipeMatcher>();
	public List<ItemStack> Outputs = new List<ItemStack>();

	public ID Path { get; private set; }

	public abstract RecipeCategory Category { get; }

	public virtual bool Matches(IRecipeSource source)
	{
		foreach(IRecipeMatcher matcher in Matchers)
		{
			if(!matcher.Test(source))
			{
				return false;
			}
		}
		return true;
	}

	public virtual void Read(IBinaryCompound compound, ID id)
	{
		Path = id;

		IBinaryList matchersComp = compound.GetListSafely("inputs");

		foreach(IBinaryCompound c in matchersComp)
		{
			if(c.Has("item"))
			{
				Matchers.Add(IRecipeMatcher.StaticItem.Read(c));
			}
			else if(c.Has("item_tag"))
			{
				Matchers.Add(IRecipeMatcher.TagItem.Read(c));
			}
		}

		IBinaryList opComp = compound.GetListSafely("outputs");

		foreach(IBinaryCompound c in opComp)
		{
			if(c.Has("item"))
			{
				Outputs.Add(RecipeUtils.StaticItem(c));
			}
		}
	}

	public abstract bool Assemble(IRecipeSource source);

	public virtual List<IRecipeMatcher> GetMatchers()
	{
		return Matchers;
	}

	public List<ItemStack> GetOutputs()
	{
		return Outputs;
	}

}
