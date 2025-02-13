using Kinetic.App;
using Kinetic.Visual;
using Morinia.Content;
using Morinia.World.TheDict;
using Morinia.World.ThePhysic;
using Morinia.World.TheRecipe;

namespace Morinia.World.TheItem;

public class Item : Tag
{

	//INJECT
	public static Item Empty => Items.Empty;

	public override bool Contains<T>(T o)
	{
		return o is Item item && item == this;
	}

	public virtual ItemStack Instantiate(int count = 1, int meta = 0, bool mod = true)
	{
		return new ItemStack(this, mod, count).SetMeta(meta);
	}

	//END

	//Instance Relative

	public virtual int GetStackSize(ItemStack stack)
	{
		return GetToolType(stack) == ItemToolType.None ? 100 : 1;
	}

	public virtual ItemToolType GetToolType(ItemStack stack)
	{
		return ItemToolType.None;
	}

	public virtual float GetToolEfficiency(ItemStack stack)
	{
		return 1;// Hand is 1.
	}

	public virtual void GetTooltips(ItemStack stack, List<Lore> tooltips)
	{
		string name = I18N.GetText($"{Uid.Space}:items.{Uid.Key}");
		tooltips.Add(Lore.Literal(name));
	}

	public virtual InterResult OnClickBlock(ItemStack stack, Entity entity, Level level, IPos pos, bool sim = false)
	{
		return InterResult.Pass;
	}

	public virtual InterResult OnUseItem(ItemStack stack, Entity entity, Level level, IPos pos, bool sim = false)
	{
		return InterResult.Pass;
	}

}
