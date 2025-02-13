using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client.TheItem;
using Morinia.World.TheDict;
using Morinia.World.TheItem;

namespace Morinia.World.TheRecipe;

public interface IRecipeMatcher
{

	int Count { get; }

	bool Test(IRecipeSource source);

	void Consume(IRecipeSource source);

	void DrawSymbol(SpriteBatch batch, float x, float y, float w, float h);

	void TooltipSymbol(List<Lore> tooltips);

	class StaticItem : IRecipeMatcher
	{

		readonly ItemStack stack;

		public StaticItem(ItemStack stack)
		{
			this.stack = stack;
		}

		public StaticItem(Item type, int count)
		{
			stack = type.Instantiate(count);
		}

		public StaticItem(Item type)
		{
			stack = type.Instantiate();
		}

		public int Count => stack.Count;

		public bool Test(IRecipeSource source)
		{
			int i = 0;
			foreach(ItemStack stk in source.ItemContainer)
			{
				if(stk.IsCompatible(stack) && !stk.IsEmpty) i += stk.Count;
			}
			return i >= Count;
		}

		public void Consume(IRecipeSource source)
		{
			source.ItemContainer.Extract(stack);
		}

		public void DrawSymbol(SpriteBatch batch, float x, float y, float w, float h)
		{
			ItemRendering.GetTessellator(stack).Draw(batch, x, y, w, h, stack, forcecount: true);
		}

		public void TooltipSymbol(List<Lore> tooltips)
		{
			stack.GetTooltips(tooltips);
		}

		public static StaticItem Read(IBinaryCompound compound)
		{
			return new StaticItem(RecipeUtils.StaticItem(compound));
		}

	}

	class TagItem : IRecipeMatcher
	{

		private Item itemSymbol;
		private dynamic[] array;
		private int counter;
		private TimeSchedule schedule;

		readonly int count;
		readonly Tag _tag;

		public TagItem(Tag tag, int count = 1)
		{
			this._tag = tag;
			this.count = count;

			array = tag.GetContents().ToArray();
			schedule = new TimeSchedule();
		}

		public int Count => count;

		public bool Test(IRecipeSource source)
		{
			foreach(ItemStack s in source.ItemContainer)
			{
				if(s.Is(_tag) && s.Count >= count)
				{
					return true;
				}
			}
			return false;
		}

		public void Consume(IRecipeSource source)
		{
			source.ItemContainer.Extract(1, s => s.Is(_tag));
		}

		public void DrawSymbol(SpriteBatch batch, float x, float y, float w, float h)
		{
			if(schedule.PeriodicTaskChecked(0.5f) || itemSymbol == null)
			{
				itemSymbol = array[counter];

				counter++;
				if(counter >= array.Length)
					counter = 0;
			}

			ItemStack stack = itemSymbol.Instantiate(count);
			ItemRendering.GetTessellator(stack).Draw(batch, x, y, w, h, stack, forcecount: true);
		}

		public void TooltipSymbol(List<Lore> tooltips)
		{
			tooltips.Add(Lore.Translate("utexts.recipe_tag_acceptance", _tag.GetAbbreviatedID()));
		}

		public static TagItem Read(IBinaryCompound compound)
		{
			var tuple = RecipeUtils.TagItem(compound);
			return new TagItem(tuple.Item1, tuple.Item2);
		}

	}

}
