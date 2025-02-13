using Kinetic.IO;
using Kinetic.Visual;
using Morinia.World.TheDict;
using Morinia.World.ThePhysic;
using Morinia.World.TheRecipe;

namespace Morinia.World.TheItem;

public sealed class ItemStack : IMetable
{

	public static readonly ItemStack Empty = Content.Items.Empty.Instantiate(mod: false);

	public IBinaryCompound compound;
	public int Count;
	public int Meta { get; private set; }
	public bool Modifiable;
	Item _item;
	public Item Item => Count <= 0 ? Item.Empty : _item;

	public ItemStack(Item item, bool mod = true, int count = 1)
	{
		_item = item;
		Modifiable = mod;
		Count = count;
	}

	public bool HasData => compound != null;
	public bool IsEmpty => Count <= 0 || Item == Item.Empty || Item == null || this == Empty;

	public IBinaryCompound Data
	{
		get => compound == null ? compound = IBinaryCompound.New() : compound;
		set => compound = value;
	}

	public ItemStack SetMeta(int meta)
	{
		Meta = meta;
		return this;
	}

	//Paletting Requirements

	public override bool Equals(object obj)
	{
		return obj is ItemStack state && state.Item == Item && state.Meta == Meta;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Item, Meta);
	}

	//Instance Relative

	public int MaxStackSize => Item.GetStackSize(this);

	//Cpy

	public ItemStack Copy(int count = -1)
	{
		ItemStack stack = new ItemStack(Item, Modifiable);
		stack.Copy(this, count);
		return stack;
	}

	public void Copy(ItemStack stack, int count = -1)
	{
		_item = stack.Item;
		Modifiable = stack.Modifiable;
		if(stack.HasData)
		{
			compound = stack.compound.Copy();
		}
		else
		{
			compound = null;
		}

		Count = count == -1 ? stack.Count : count;
		Meta = stack.Meta;
	}

	//Operations

	public ItemStack Merge(ItemStack s, bool simulate = false)
	{
		int ss = this.MaxStackSize;

		if(!Modifiable)
		{
			return s;
		}

		s = s.Copy();

		if(IsEmpty)
		{
			if(!simulate)
			{
				Copy(s);
			}

			return Empty;
		}

		if(!IsCompatible(s))
		{
			return s;
		}

		int c = s.Count;
		if(c + Count <= ss)
		{
			if(!simulate)
			{
				Count += c;
			}

			s.Count = 0;
		}
		else if(c + Count > ss)
		{
			int add = ss - Count;
			if(!simulate)
			{
				Count = ss;
			}

			s.Count -= add;
		}

		return s;
	}

	public ItemStack Take(int c)
	{
		if(Count < c)
		{
			Count = 0;
			return Copy(Count);
		}

		Count -= c;
		return Copy(c);
	}

	public void Grow(int c)
	{
		Count -= c;
	}

	public ItemStack SpiltHalf()
	{
		int count0 = Count % 2 == 0 ? Count / 2 : (Count + 1) / 2;
		return Take(count0);
	}

	//Checks

	public bool IsDataEqual(ItemStack stack)
	{
		if(stack.compound == null && compound == null)
		{
			return true;
		}

		if(stack.compound != null && compound != null)
		{
			return stack.compound.Compare(compound);
		}

		return false;
	}

	public bool IsCompatible(ItemStack stack)
	{
		return IsEmpty || stack.Item == Item && IsDataEqual(stack);
	}

	public bool CanMergeFully(ItemStack stack)
	{
		return Count + stack.Count <= MaxStackSize && IsCompatible(stack);
	}

	public bool CanMergePartly(ItemStack stack)
	{
		return Count <= MaxStackSize && IsCompatible(stack);
	}

	public bool Is(ItemStack stack)
	{
		return Count == stack.Count && IsCompatible(stack) && Item == stack.Item;
	}

	public bool Is(Tag tag)
	{
		return tag.Contains(Item);
	}

	// Instance Relative

	public ItemToolType GetToolType()
	{
		return  Item.GetToolType(this);
	}

	public float GetToolEfficiency()
	{
		return Item.GetToolEfficiency(this);
	}

	public void GetTooltips(List<Lore> tooltips)
	{
		Item.GetTooltips(this, tooltips);
	}

	public InterResult OnClickBlock(Entity entity, Level level, IPos pos, bool sim = false)
	{
		return Item.OnClickBlock(this, entity, level, pos, sim);
	}

	public InterResult OnUseItem(Entity entity, Level level, IPos pos, bool sim = false)
	{
		return Item.OnUseItem(this, entity, level, pos, sim);
	}

	//Codec

	public static ItemStack Deserialize(IBinaryCompound compound)
	{
		string type = compound.Get<string>("type");
		int count = compound.Get<int>("count");
		int meta = compound.Get<int>("meta");

		Item item = Content.Items.Registry.Get(type);

		if(item == null || item == Content.Items.Empty) return Empty;

		ItemStack stack = item.Instantiate(count, meta);
		if(compound.Has("data"))
		{
			stack.Data = compound.GetCompoundSafely("compound");
		}

		return stack;
	}

	public static IBinaryCompound Serialize(ItemStack stack)
	{
		IBinaryCompound compound = IBinaryCompound.New();
		Serialize(stack, compound);
		return compound;
	}

	public static void Serialize(ItemStack stack, IBinaryCompound compound)
	{
		compound.Set("type", stack.Item.Uid.Full);
		compound.Set("count", stack.Count);
		compound.Set("meta", stack.Meta);

		if(stack.HasData)
		{
			compound.Set("compound", stack.compound);
		}
	}

}
