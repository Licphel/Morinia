using Kinetic.Gui;
using Morinia.Client.TheGui;
using Morinia.Content.TheEntity;
using Morinia.Content.TheGui;
using Morinia.World;
using Morinia.World.TheItem;

namespace Morinia.Content.TheAccessBridge;

public class AccessBridgeInventory : AccessBridge
{

	public AccessBridgeInventory(EntityPlayer player)
	: base(new BlockPos(player.Pos), player)
	{
		Add3x9Side(7, 7);
	}

	public override ElementGui MakeScreen()
	{
		return new GuiInventory(this);
	}

	public override void Transport(Slot clicked, ItemContainer clickedInv, Inventory invPlayer)
	{
		int i = clicked.InvIndex;

		if(!clicked.PlayerSide)
		{
			clickedInv[i] = invPlayer.Add(clickedInv[i]);
		}
		else
		{
			if(i <= 8)
			{
				invPlayer[i] = invPlayer.Add(invPlayer[i], 9, 26);
			}
			else
			{
				invPlayer[i] = invPlayer.Add(invPlayer[i], 0, 8);
			}
		}
	}

}
