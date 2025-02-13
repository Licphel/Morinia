using Kinetic;
using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;
using Morinia.Client.TheCelesphere;
using Morinia.Client.TheGui;
using Morinia.Logic;

namespace Morinia.Content.TheGui;

public class GuiMenu : ElementGui
{

	readonly ElementButton[] Buttons = new ElementButton[5];

	public override void Draw(SpriteBatch batch)
	{
		Celesphere.DrawCelesph(batch);

		batch.Draw(GameTextures.GUI_MENU_TITLE_EN, Size.x / 2 - 800, Size.y - 300, 1600, 320);

		base.Draw(batch);
	}

	public override void InitComponents()
	{
		base.InitComponents();

		float x = Size.x / 2f;
		float y = Size.y - 280;
		const float bw = 300;

		Buttons[0] = ComponentFactory.NewTextedButton(Lore.Translate("button.menu_new_game"), bw, () =>
		{
			Close();
			new OverlayMain().Display();
			Game.GameLogic = new GameLogic();
		});
		Join(Buttons[0]);
		Buttons[1] = ComponentFactory.NewTextedButton(Lore.Translate("button.menu_load_game"), bw, () =>
		{
		});
		Join(Buttons[1]);
		Buttons[2] = ComponentFactory.NewTextedButton(Lore.Translate("button.menu_options"), bw, () =>
		{
		});
		Join(Buttons[2]);
		Buttons[3] = ComponentFactory.NewTextedButton(Lore.Translate("button.menu_mod_mngmt"), bw, () =>
		{
		});
		Join(Buttons[3]);
		Buttons[4] = ComponentFactory.NewTextedButton(Lore.Translate("button.menu_quit_game"), bw, () =>
		{
			Application.Stop();
		});
		Join(Buttons[4]);
	}

	public override void RelocateComponents()
	{
		for(int i = 0; i < Buttons.Length; i++)
		{
			Buttons[i].Bound.LocateCentral(Size.x / 2f, Size.y - 260 - i * 30);
		}
	}

	public override void Update(VaryingVector2 cursor)
	{
		base.Update(cursor);
		Celesphere.TickCelesph();
	}

}
