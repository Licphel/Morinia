using Kinetic.App;
using Kinetic.IO;
using Morinia.World.TheDict;
using Morinia.World.TheItem;

namespace Morinia.World.TheRecipe;

public class RecipeUtils
{

	public static ItemStack StaticItem(IBinaryCompound compound)
	{
		string key = compound.Get<string>("item");

		Item type = Content.Items.Registry.Get(key);
		if(type == null)
		{
			Logger.Warn($"Parsing recipe error! Item: {key} was not found.");
			return ItemStack.Empty;
		}

		return type.Instantiate(compound.Get("count", 1));
	}

	public static (Tag, int) TagItem(IBinaryCompound compound)
	{
		string key = compound.Get<string>("item_tag");

		Tag tag = TagManager.Get(key);
		if(tag == null)
		{
			Logger.Warn($"Parsing recipe error! Item tag: {key} was not found.");
			return (null, 0);
		}

		return (tag, compound.Get("count", 1));
	}

}
