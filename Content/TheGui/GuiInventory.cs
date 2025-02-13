using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheItem;
using Morinia.World.TheItem;
using Morinia.World.TheRecipe;

namespace Morinia.Content.TheGui;

public class GuiInventory : GuiAccessBridge
{

	ElementSelection selection;

	public GuiInventory(AccessBridge container)
	: base(container, GameTextures.GuiInventory, new Vector2(176, 186))
	{
	}

	public override void Draw(SpriteBatch batch)
	{
		base.Draw(batch);

		foreach(ElementSelection.Entry entry0 in selection.Entries)
		{
			GuiPlayerInvEntry entry = entry0 as GuiPlayerInvEntry;

			if(entry.IsHovering()) GuiPlayerInvEntry.selected = entry;
			if(entry != GuiPlayerInvEntry.selected) continue;

			float x1 = selection.Bound.xprom + 8;
			float y1 = selection.Bound.yprom - 6 - 8;

			batch.Draw(I18N.GetText("utexts.recipe_ingredients"), x1, y1);

			int i = 1;
			int j = 0;

			foreach(IRecipeMatcher matcher in entry.recipe.GetMatchers())
			{
				matcher.DrawSymbol(batch, x1 + j * 18, y1 - i * 18, 16, 16);
				float x = TempCursor.x;
				float y = TempCursor.x;

				i++;

				if(i > 3)
				{
					j++;
					i = 1;
				}
			}
		}
	}

	public override List<Lore> CollectTooltips()
	{
		List<Lore> lst = base.CollectTooltips();

		foreach(ElementSelection.Entry entry0 in selection.Entries)
		{
			GuiPlayerInvEntry entry = entry0 as GuiPlayerInvEntry;

			if(entry != GuiPlayerInvEntry.selected) continue;

			float x1 = selection.Bound.xprom + 8;
			float y1 = selection.Bound.yprom - 6 - 8;

			int i = 1;
			int j = 0;

			foreach(IRecipeMatcher matcher in entry.recipe.GetMatchers())
			{
				float x = TempCursor.x;
				float y = TempCursor.y;

				if(x >= x1 + j * 18 && x <= x1 + j * 18 + 16 && y >= y1 - i * 18 && y <= y1 - i * 18 + 16)
					matcher.TooltipSymbol(lst);

				i++;

				if(i > 3)
				{
					j++;
					i = 1;
				}
			}
		}

		return lst;
	}

	public override void InitComponents()
	{
		base.InitComponents();

		selection = new ElementSelection();
		selection.Bound.Resize(30, 86);
		selection.Bound.Locate(i + 8, j + 77);
		selection.EntryH = 22;

		foreach(IRecipe recipe in RecipeManager.GetRecipes(Recipes.Handcraft))
		{
			selection.Add(new GuiPlayerInvEntry(Bridge, recipe));
		}

		Join(selection);
	}

	public class GuiPlayerInvEntry : ElementSelection.Entry
	{

		public static ElementSelection.Entry selected;

		public AccessBridge menu;
		public IRecipe recipe;

		public GuiPlayerInvEntry(AccessBridge menu, IRecipe recipe)
		{
			this.menu = menu;
			this.recipe = recipe;
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(GameTextures.RecipeEntry, x, y, 22, 22, 0, 0, 22, 22);
			ItemStack stack = recipe.GetOutputs()[0];
			ItemRendering.GetTessellator(stack).Draw(batch, x + 3, y + 3, 16, 16, stack);

			if(IsHovering())
			{
				IRecipeSource src = new RecipeSourcePlayer(menu.Player);
				if(recipe.Matches(src))
				{
					batch.Color4(0.3f, 1f, 0.4f);
				}
				else
				{
					batch.Color4(1f, 0.3f, 0.3f);
				}
				batch.Draw(GameTextures.RecipeEntry, x, y, 22, 22, 0, 22, 22, 22);
				batch.NormalizeColor();
			}
		}

		public override bool IsHovering()
		{
			return base.IsHovering() && Cursor.x < x + 22;
		}

		public override void Pressed()
		{
			IRecipeSource src = new RecipeSourcePlayer(menu.Player);
			recipe.Assemble(src);
		}

	}

}
