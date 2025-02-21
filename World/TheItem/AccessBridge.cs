using Kinetic.Gui;
using Morinia.Client.TheGui;
using Morinia.Content.TheEntity;
using Morinia.Content.TheGui;

namespace Morinia.World.TheItem;

public class AccessBridge
{

	public Slot ChosenSlot;
	public List<Inventory> Invs = new List<Inventory>();

	public Level Level;
	public EntityPlayer Player;
	public BlockPos Pos;
	public List<Slot> Slots = new List<Slot>();

	public AccessBridge(BlockPos pos, EntityPlayer player)
	{
		Pos = pos;
		Player = player;
		Level = player.Level;
	}

	public ItemStack Pickup
	{
		get => Player.GuiHeldStack;
		set => Player.GuiHeldStack = value;
	}

	//UTILS

	//[NOT PUBLIC]
	public static int sw => GuiAccessBridge.sw;

	public void OnStoppedUsage()
	{
		DoAction(AccessBridgeAction.THROW);
		
		foreach(Slot slot in Slots)
		{
			if(!slot.Storage)
			{
				EntityItem entity = new EntityItem();
				entity.Stack = slot.Stack;
				Level.AddEntity(entity, Player.Pos);
			}
		}
	}

	public void Carve(Slot slot, int invi = 0)
	{
		Slots.Add(slot);

		int uuid = Slots.IndexOf(slot);

		if(slot.PlayerSide)
		{
			slot.Bind(Player.Inv, uuid);
		}
		else
		{
			slot.Bind(Invs[invi], uuid);
		}
	}

	public void DoAction(AccessBridgeAction action)
	{
		if(action == AccessBridgeAction.THROW)
		{
			if(!Pickup.IsEmpty)
			{
				EntityItem entity = new EntityItem();
				entity.Stack = Pickup;
				entity.Protection = 5;
				Level.AddEntity(entity, Player.Pos);
				Pickup = ItemStack.Empty;
			}
		}

		if(ChosenSlot == null)
		{
			return;
		}
		switch(action)
		{
			case AccessBridgeAction.SWAP_PICKUP:
				ItemStack pick0 = Pickup;

				ItemStack slot0 = ChosenSlot.Stack;
				if(pick0.CanMergePartly(slot0) && !slot0.IsEmpty && !pick0.IsEmpty)
				{
					Pickup = slot0.Merge(pick0);
				}
				else
				{
					ChosenSlot.Stack = pick0;
					Pickup = slot0;
				}
				break;
			case AccessBridgeAction.FAST_TRANSFER:
				Transport(ChosenSlot, ChosenSlot.Inv, Player.Inv);
				break;
			case AccessBridgeAction.HALF_PICKUP:
				pick0 = Pickup;
				slot0 = ChosenSlot.Stack;
				if(pick0.IsEmpty && !slot0.IsEmpty)
				{
					Pickup = ChosenSlot.Stack.SpiltHalf();
				}
				else if(!pick0.IsEmpty && slot0.IsEmpty)
				{
					ChosenSlot.Stack = pick0.Take(1);
				}
				else if(!pick0.IsEmpty && slot0.CanMergePartly(pick0))
				{
					slot0.Merge(pick0.Take(1));
				}
				break;
		}
	}

	public virtual void Transport(Slot clicked, ItemContainer clickedInv, Inventory invPlayer)
	{
	}

	public virtual ElementGui MakeScreen()
	{
		return null;
	}

	public static void OpenGracefully(EntityPlayer player, AccessBridge ci)
	{
		player.OpenContainer = ci;
		ci.MakeScreen().Display();
	}

	public static void CloseGracefully(EntityPlayer player)
	{
		player.OpenContainer?.OnStoppedUsage();
		player.OpenContainer = null;
	}

	public void Add3x9Side(int x0 = 0, int y0 = 0, int invi = 0, bool playerside = true)
	{
		int x = x0;
		int y = y0;

		for(int i = 0; i < 9; i++)
		{
			Carve(new Slot(i, i * sw + x, y, playerside, true), invi);
		}
		for(int i = 0; i < 9; i++)
		{
			Carve(new Slot(i + 9, i * sw + x, y + sw, playerside, true), invi);
		}
		for(int i = 0; i < 9; i++)
		{
			Carve(new Slot(i + 18, i * sw + x, y + sw * 2, playerside, true), invi);
		}
	}

	public void PlateCarve(int i0, int x0, int y0, int xn, int yn, bool playerside, bool store, bool oonly, int invi = 0)
	{
		int x = x0;
		int y = y0;

		for(int j = 0; j < yn; j++)
		for(int i = 0; i < xn; i++)
		{
			Carve(new Slot(i0 + i + j * xn, i * sw + x, j * (sw + 1) + y, playerside, store), invi);
		}
	}

}
