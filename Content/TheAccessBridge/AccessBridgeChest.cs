using Kinetic.Gui;
using Kinetic.Math;
using Morinia.Client;
using Morinia.Client.TheGui;
using Morinia.Content.TheBlockEntity;
using Morinia.Content.TheEntity;
using Morinia.Content.TheGui;
using Morinia.World;
using Morinia.World.TheItem;

namespace Morinia.Content.TheAccessBridge;

public class AccessBridgeChest : AccessBridge
{

	public AccessBridgeChest(BlockPos pos, EntityPlayer player) : base(pos, player)
	{
		BlockEntityChest chest = (BlockEntityChest) Level.GetBlockEntity(pos);
		Invs.Add(chest.Inv);

		//#? player inv bind slots
		Add3x9Side(7, 7);
		//#0 cab inv bind slots
		Add3x9Side(7, 76, 0, false);
	}

	public override ElementGui MakeScreen()
	{
		return new GuiAccessBridge(this, GameTextures.GuiChest, new Vector2(176, 152));
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
			invPlayer[i] = Invs[0].Add(invPlayer[i]);
		}
	}

}
