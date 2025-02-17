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
		const float lptr = 0;
		const float dptr = 0;
		const float lptri = 1;
		const float dptri = 1;
		const float st = 20;
		const float off = 20;
		const float lblank = 34;


		float bx = (Size.x - w) / 2;
		float by = 2;

		EntityPlayer player = Game.GameLogic.Player;
		Level level = player.Level;

		bool changelayout = /* player.Pos.y < 3 */ false;

		if(changelayout)
		{
			by = Size.y - h - 12;
		}

		//ItemCaps Slots
		batch.Draw(GameTextures.Hotbar, bx, by, w, h, 0, 0, w, h);
		batch.Draw(GameTextures.Hotbar, bx + lblank + lptr + (st + 1) * player.InvCursor, by + dptr, st, st, 0, off, st, st);

		for(int i = 0; i < 9; i++)
		{
			ItemStack stack = player.Inv.Get(i);
			ItemRendering.GetTessellator(stack).Draw(batch, bx + lblank + (lptri + 1) + (st + 1) * i, by + (dptri + 1), 16, 16, stack);
		}
		
		Block block = level.GetBlock(Game.GameLogic.HoverPos).Block;
		
		if(block != Block.Empty)
		{
			string name = I18N.GetText($"{block.Uid.Space}:blocks.{block.Uid.Key}");
			string fromw = block.Uid.Space;
			GlyphBounds gb = batch.Font.GetBounds(name);
			GlyphBounds gb1 = batch.Font.GetBounds(fromw);
			float frw = Math.Max(gb.Width, gb1.Width) + 8;
			float frh = gb.Height + gb1.Height + 6;

			float yd = changelayout ? -18 : 0;
			
			batch.Draw(ElementGui.DefaultTooltipPatches, (Size.x - frw) / 2, Size.y - frh - 6 + yd, frw, frh);
			
			batch.Draw(name, Size.x / 2, Size.y - frh + 5 + yd, Align.CENTER);
			batch.Color4(1, 1, 1, 0.5f);
			batch.Draw(fromw, Size.x / 2, Size.y - frh - 3 + yd, Align.CENTER);
			batch.NormalizeColor();
		}

		//
		//
		//
		//

		if(!details) return;

		EntityPlayer p = player;

		float hy = 4;
		const float dy = 9;

		batch.Draw($"location: [x = {p.Pos.x}, y = {p.Pos.y}, c = {p.Pos.UnitX}]", 6, hy);
		hy += dy;
		batch.Draw($"time: [c/t = {level.Ticks}, d/t = {level.TicksPerDay}]", 6, hy);
		hy += dy;
		batch.Draw($"level entities: {level.EntitiesById.Count}", 6, hy);
		hy += dy;
		batch.Draw($"hovering pos: {Game.GameLogic.HoverPos}", 6, hy);
		hy += dy;
		BlockState state = level.GetBlock(Game.GameLogic.HoverPos);
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
