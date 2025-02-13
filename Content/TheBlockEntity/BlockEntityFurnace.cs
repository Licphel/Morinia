using Kinetic.App;
using Kinetic.IO;
using Kinetic.Math;
using Morinia.Content.TheRecipe;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;
using Morinia.World.TheRecipe;

namespace Morinia.Content.TheBlockEntity;

public class BlockEntityFurnace : BlockEntity
{

	public Inventory Inv = new Inventory(5);
	public int Cooktime, MaxTime = 1;
	public int Fuel, MaxFuel = 1;
	public RecipeFurnace Recipe;

	public BlockEntityFurnace(BlockState state, Level level, BlockPos pos) : base(state, level, pos)
	{
	}

	public override void OverrideBlockDrops(List<ItemStack> stacks)
	{
		base.OverrideBlockDrops(stacks);

		foreach(ItemStack stk in Inv)
		{
			if(!stk.IsEmpty) stacks.Add(stk);
		}
	}

	public override bool IsTickable => true;

	public override void Tick()
	{
		IRecipeSource src = new RecipeSourceInventory(Inv, new Vector2(4, 4));

		Recipe = null;
		List<IRecipe> recipes = RecipeManager.GetRecipes(Recipes.Furnace);

		foreach(IRecipe r in recipes)
		{
			if(r.Matches(src))
			{
				Recipe = (RecipeFurnace) r;
				break;
			}
		}

		if(Recipe != null)
		{
			MaxTime = Recipe.Cooktime;

			bool ac = src.IsResultDestinationAccessible(Recipe.Output0, Recipe);

			if(Fuel <= 0)
			{
				ItemStack stack = Inv[3];
				if(Recipe != null && stack.Is(Items.Coal) && ac)
				{
					Fuel = MaxFuel = 2700;
					Inv[3].Grow(-1);
				}
			}

			if(Fuel > 0 && ac)
				Cooktime++;
			else
				Cooktime--;

			if(Cooktime >= Recipe.Cooktime)
			{
				Cooktime = 0;
				Recipe.Assemble(src);
				Recipe = null;
			}
		}
		else
		{
			Cooktime = Math.Clamp(Cooktime - 1, 0, MaxTime);
		}

		Fuel = Math.Clamp(Fuel - 1, 0, MaxFuel);
	}

	public override void Read(IBinaryCompound compound)
	{
		base.Read(compound);
		Inv = Inventory.Deserialize(compound.GetCompoundSafely("cinv"));
		Cooktime = compound.Get<int>("cooktime");
		Fuel = compound.Get<int>("fuel");
		MaxFuel = compound.Get<int>("maxfuel");
	}

	public override void Write(IBinaryCompound compound)
	{
		base.Write(compound);
		compound.Set("cinv", Inventory.Serialize(Inv));
		compound.Set("cooktime", Cooktime);
		compound.Set("fuel", Fuel);
		compound.Set("maxfuel", MaxFuel);
	}

}
