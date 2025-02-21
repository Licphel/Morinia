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

public partial class OverlayMain : ElementGui
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

		EntityPlayer player = Game.GameLogic.Player;
		Level level = Game.GameLogic.Level;
		
		DrawHotBarAndHint(batch, player);
		DrawPStates(batch, player);
		
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
