using Kinetic;
using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheItem;
using Morinia.Content.TheEntity;
using Morinia.Logic;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;

namespace Morinia.Content.TheGui;

public class OverlayMain : ElementGui
{

	bool details;

	public override void Update(VaryingVector2 cursor)
	{
		base.Update(cursor);

		if(KeyBinds.SHOW_DETAILS.Pressed())
		{
			details = !details;
		}
	}

	public override void Draw(SpriteBatch batch)
	{
		base.Draw(batch);

		const float w = 256f;
		const float h = 20;
		const float s = 2f;
		const float lptr = 0;
		const float dptr = 0;
		const float lptri = 1;
		const float dptri = 1;
		const float st = 20;
		const float off = 20;
		const float lblank = 34;


		float bx = (Size.x - w * s) / 2;
		float by = 2 * s;

		EntityPlayer player = Game.GameLogic.Player;

		if(player.Pos.y < 3)
		{
			by = Size.y - h - 12 * s;
		}

		//ItemCaps Slots
		batch.Draw(GameTextures.Hotbar, bx, by, w * s, h * s, 0, 0, w, h);
		batch.Draw(GameTextures.Hotbar, bx + lblank * s + lptr * s + (st + 1) * player.InvCursor * s, by + dptr * s, st * s, st * s, 0, off, st, st);

		for(int i = 0; i < 9; i++)
		{
			ItemStack stack = player.Inv.Get(i);
			ItemRendering.GetTessellator(stack).Draw(batch, bx + lblank * s + (lptri + 1) * s + (st + 1) * i * s, by + (dptri + 1) * s, 16 * s, 16 * s, stack);
		}

		//
		//
		//
		//

		if(!details) return;

		EntityPlayer p = player;
		Level l = p.Level;

		float hy = 4;
		const float dy = 18;

		batch.Draw($"location: [x = {p.Pos.x}, y = {p.Pos.y}]", 6, hy);
		hy += dy;
		batch.Draw($"time: [c/t = {l.Ticks}, d/t = {l.TicksPerDay}]", 6, hy);
		hy += dy;
		batch.Draw($"level entities: {l.EntitiesById.Count}", 6, hy);
		hy += dy;
		batch.Draw($"hovering pos: {Game.GameLogic.HoverPos}", 6, hy);
		hy += dy;
		BlockState state = l.GetBlock(Game.GameLogic.HoverPos);
		batch.Draw($"hovering block: {state.Block.Uid.ToString()}, meta = {state.Meta}", 6, hy);
		hy += dy;

		batch.Draw($"tps = {Application.Tps}, expected = {Application.MaxTps}", 6, hy);
		hy += dy;
		batch.Draw($"fps = {Application.Fps}, expected = {Application.MaxFps}", 6, hy);
		hy += dy;
		batch.Draw("* debugger (press F1 to hide)", 6, hy);
		hy += dy;
	}

}
