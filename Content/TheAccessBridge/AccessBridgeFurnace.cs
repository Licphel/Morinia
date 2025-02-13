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

public class AccessBridgeFurnace : AccessBridge
{

	public BlockEntityFurnace Furnace;

	public AccessBridgeFurnace(BlockPos pos, EntityPlayer player) : base(pos, player)
	{
		Furnace = (BlockEntityFurnace) Level.GetBlockEntity(pos);
		Invs.Add(Furnace.Inv);

		//#? player inv bind slots
		Add3x9Side(7, 7);
		//#0 cab inv bind slots
		Carve(new Slot(0, 25, 126, false, true));
		Carve(new Slot(1, 43, 126, false, true));
		Carve(new Slot(2, 61, 126, false, true));
		Carve(new Slot(3, 43, 91, false, true));
		Carve(new Slot(4, 133, 126, false, true));
	}

	public override ElementGui MakeScreen()
	{
		return new GuiFurnace(this);
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
