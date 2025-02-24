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

		float hy = 42;
		const float dy = 7;
		const float dx = 3;
		batch.Font.Scale /= 1.5f;

		batch.Draw($"location: [x = {p.Pos.x}, y = {p.Pos.y}, c = {p.Pos.UnitX}]", dx, hy);
		hy += dy;
		batch.Draw($"time: [c/t = {level.Ticks} %= {level.Ticks % level.TicksPerDay}, d/t = {level.TicksPerDay}]", dx, hy);
		hy += dy;
		batch.Draw($"level entities: {level.EntitiesById.Count}", dx, hy);
		hy += dy;
		batch.Draw($"hovering pos: {Game.GameLogic.HoverPos}", dx, hy);
		hy += dy;
		BlockState state = level.GetBlock(Game.GameLogic.HoverPos);
		batch.Draw($"hovering block: {state.Block.Uid.ToString()}, meta = {state.Meta}", dx, hy);
		hy += dy;

		batch.Draw($"tps = {Application.Tps}, expected = {Application.MaxTps}", dx, hy);
		hy += dy;
		batch.Draw($"fps = {Application.Fps}, expected = {Application.MaxFps}", dx, hy);
		hy += dy;
		batch.Draw("Debugger (press F1 to hide)", dx, hy);
		hy += dy;
		
		batch.Font.Scale *= 1.5f;
	}

}
