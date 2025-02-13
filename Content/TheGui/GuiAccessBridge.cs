using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheItem;
using Morinia.Logic;
using Morinia.World.TheItem;

namespace Morinia.Content.TheGui;

public class GuiAccessBridge : ElementGui
{

	//[NOT PUBLIC]
	public static int sw = 18;

	public AccessBridge Bridge;
	public float i, j;
	public Texture Background;
	public float w, h;

	public GuiAccessBridge(AccessBridge br, Texture icon, Vector2 used)
	{
		Bridge = br;
		Background = icon;
		w = used.x;
		h = used.y;
	}

	public override float ScaleMul => 2;
	public override float FontScmul => 0.5f;

	public override void Reflush()
	{
		i = (int) (Size.x / 2 - w / 2);
		j = (int) (Size.y / 2 - h / 2);

		//pass i, j into component-adding
		base.Reflush();
	}

	public void DrawSlots(SpriteBatch batch)
	{
		Slot chosenSlot = Bridge.ChosenSlot;

		foreach(Slot slot in Bridge.Slots)
		{
			//batch.Draw(GameTextures.SLOT, i + slot.x, j + slot.y, sw, sw, 0, 0, sw, sw);
			//if(chosenSlot == slot)
			//	batch.Draw(GameTextures.SLOT, i + slot.x, j + slot.y, sw, sw, 0, sw, sw, sw);

			if(chosenSlot == slot)
			{
				batch.Color4(1, 1, 1, 0.25f);
				batch.Fill(i + slot.x, j + slot.y, sw, sw);
				batch.NormalizeColor();
			}

			int ofx = (sw - 16) / 2;

			ItemRendering.GetTessellator(slot.Stack)
			.Draw(batch, i + slot.x + ofx, j + slot.y + ofx, 16, 16, slot.Stack);
		}

		ItemRendering.GetTessellator(Bridge.Pickup)
		.Draw(batch, TempCursor.x - 8, TempCursor.y - 8, 16, 16, Bridge.Pickup);
	}

	public override void Draw(SpriteBatch batch)
	{
		batch.LinearCol4[0] = new Vector4(0, 0, 0, 0.75f);
		batch.LinearCol4[1] = new Vector4(0, 0, 0, 0.25f);
		batch.LinearCol4[2] = new Vector4(0, 0, 0, 0.25f);
		batch.LinearCol4[3] = new Vector4(0, 0, 0, 0.75f);
		batch.Fill(0, 0, Size.x, Size.y);
		batch.NormalizeColor();

		batch.Draw(Background, i, j, w, h, 0, 0, w, h);

		base.Draw(batch);

		DrawSlots(batch);
	}

	public override void Update(VaryingVector2 cursor)
	{
		float xc = cursor.x, yc = cursor.y;
		xc -= i;
		yc -= j;

		Bridge.ChosenSlot = null;

		foreach(Slot slot in Bridge.Slots)
		{
			int x = slot.x, y = slot.y;
			if(xc >= x && xc <= x + sw && yc >= y && yc <= y + sw)
			{
				Bridge.ChosenSlot = slot;
				break;
			}
		}

		AccessBridgeAction action = AccessBridgeAction.NONE;

		if(KeyBinds.BREAK.Holding() && KeyBinds.OPERATION.Holding())
		{
			action = AccessBridgeAction.FAST_TRANSFER;
		}

		if(KeyBinds.BREAK.Pressed() && !KeyBinds.OPERATION.Holding())
		{
			action = AccessBridgeAction.SWAP_PICKUP;
		}

		if(KeyBinds.PLACE.Pressed())
		{
			action = AccessBridgeAction.HALF_PICKUP;
		}

		Bridge.DoAction(action);

		if(KeyBinds.EXIT.Pressed() || KeyBinds.INVENTORY.Pressed())
		{
			if(OpenTicks >= 5)
			{
				Close();
				AccessBridge.CloseGracefully(Bridge.Player);
			}
		}

		base.Update(cursor);
	}

	public override void OnClosed()
	{
		base.OnClosed();
		Game.GameLogic.PausingItn = false;
	}

}
