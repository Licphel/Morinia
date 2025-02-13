﻿using Kinetic.IO;

namespace Morinia.World.TheItem;

public class Inventory : ItemContainer
{

	readonly ItemStack[] stacks;

	public Inventory(int size)
	{
		Count = size;
		stacks = new ItemStack[size];

		for(int i = 0; i < size; i++)
		{
			stacks[i] = ItemStack.Empty;
		}
	}

	public override int Count { get; }

	public override void Set(int i, ItemStack s)
	{
		if(i < 0 || i >= stacks.Length) return;
		stacks[i] = s.Copy();
	}

	public override ItemStack Add(ItemStack s, int startSlot, int endSlot, bool simulate = false)
	{
		s = s.Copy();

		if(Count == 0)
		{
			return s;
		}

		for(int i = startSlot; i <= endSlot; i++)
		{
			if(!IsAccessible(s, i))
			{
				continue;
			}
			ItemStack stack = Get(i);

			s = stack.Merge(s, simulate);

			if(s.IsEmpty)
			{
				return ItemStack.Empty;
			}
		}

		for(int i = startSlot; i <= endSlot; i++)
		{
			if(!IsAccessible(s, i))
			{
				continue;
			}
			ItemStack stack = Get(i);

			if(stack.IsEmpty)
			{
				if(!simulate)
				{
					Set(i, s);
				}
				return ItemStack.Empty;
			}
			s = stack.Merge(s, simulate);

			if(s.IsEmpty)
			{
				return ItemStack.Empty;
			}
		}

		return s;
	}

	public override List<ItemStack> Extract(int count, Predicate<ItemStack> filter, int startSlot, int endSlot, bool simulate = false)
	{
		List<ItemStack> satis = new List<ItemStack>();

		if(Count == 0)
		{
			return satis;
		}

		for(int i = startSlot; i <= endSlot; i++)
		{
			ItemStack stack = Get(i);
			if(stack.IsEmpty)
			{
				continue;
			}
			if(filter.Invoke(stack))
			{
				int c = Math.Min(stack.Count, count);
				ItemStack ext = stack.Copy(c);

				if(!simulate)
				{
					stack.Count -= c;
				}
				count -= c;

				foreach(ItemStack stack1 in satis)
				{
					if(stack1.IsCompatible(ext))
					{
						ext = stack1.Merge(ext);
						break;
					}
				}
				if(!ext.IsEmpty)
				{
					satis.Add(ext);
				}
			}
			if(count <= 0)
			{
				break;
			}
		}

		return satis;
	}

	public override bool IsAccessible(ItemStack s, int slot)
	{
		return true;
	}

	public override ItemStack Get(int i)
	{
		if(i < 0 || i >= stacks.Length) return ItemStack.Empty;
		return stacks[i] ?? ItemStack.Empty;
	}

	public override ItemStack[] Array => stacks;

	public override bool Contains(ItemStack test)
	{
		foreach(ItemStack stack in stacks)
		{
			if(stack.IsCompatible(test)
			   && stack.Count >= test.Count)
			{
				return true;
			}
		}
		return false;
	}

	//Codec

	public static Inventory Deserialize(IBinaryCompound compound)
	{
		int size = compound.Get<int>("size");

		Inventory inv = new Inventory(size);

		IBinaryList list = compound.GetListSafely("stacks");

		foreach(IBinaryCompound compound1 in list)
		{
			int slot = compound1.Get<int>("slot");
			ItemStack stack = ItemStack.Deserialize(compound1.GetCompoundSafely("stack"));
			inv[slot] = stack;
		}

		return inv;
	}

	public static IBinaryCompound Serialize(Inventory inventory)
	{
		IBinaryCompound c = IBinaryCompound.New();
		Serialize(inventory, c);
		return c;
	}

	public static void Serialize(Inventory inventory, IBinaryCompound compound)
	{
		compound.Set("size", inventory.Count);

		IBinaryList list = IBinaryList.New();

		for(int i = 0; i < inventory.Count; i++)
		{
			ItemStack stack = inventory[i];

			if(stack.IsEmpty)
			{
				continue;
			}

			IBinaryCompound compound1 = IBinaryCompound.New();
			compound1.Set("slot", i);
			compound1.Set("stack", ItemStack.Serialize(stack));
			list.Insert(compound1);
		}

		compound.Set("stacks", list);
	}

}
