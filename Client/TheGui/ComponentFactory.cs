using Kinetic;
using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;

namespace Morinia.Client.TheGui;

public class ComponentFactory
{

	public static ElementButton NewTextedButton(Lore text, float w, Response click)
	{
		ElementButton button = new ElementButton();
		button.Text = text;
		button.Bound.Resize(w, 20);
		if(click != null) button.OnLeftFired += click;
		button.Icons = new[] { GameTextures.GUI_NP_B1, GameTextures.GUI_NP_B2, GameTextures.GUI_NP_B3 };
		button.TextOffset.x = 0;
		button.TextOffset.y = 3;
		return button;
	}

	public static ElementCheckbox NewCheckbox(Lore text, bool cross = false)
	{
		ElementCheckbox cb = new ElementCheckbox();
		cb.DisplayedLore = text;
		cb.TextOffset = new Vector2(4, 1);
		cb.Icons = new[] { GameTextures.GUI_CB1, GameTextures.GUI_CB2, GameTextures.GUI_CB3 };
		cb.ShouldShowCross = cross;
		cb.Bound.Resize(18, 18);
		return cb;
	}

	public static ElementTextField NewTextField(string text, float w, float h)
	{
		ElementTextField field = new ElementTextField();
		field.InputHint = text;
		field.Bound.Resize(w, h);
		field.Icons = new[] { GameTextures.GUI_NP_B1, GameTextures.GUI_NP_B2, GameTextures.GUI_NP_B3 };
		return field;
	}

	public static ElementPage _NewPage()
	{
		ElementPage elementPage = new ElementPage();
		elementPage.TitleOffset = new Vector2(10, 7);
		elementPage.CloserOffset = new Vector2(-19, -29);
		elementPage.LabelH = 30;
		return elementPage;
	}

	public static ElementPage NewPageEvent(Vector2 size)
	{
		ElementButton button = new ElementButton();
		button.Bound.Resize(18, 18);
		button.Texture3Line = GameTextures.GUI_WINDOW_CLOSER;
		ElementPage elementPage = _NewPage();
		elementPage.Icon = GameTextures.GUI_WINDOW_MID_EV;
		elementPage.Bound.Resize(450, 310);
		elementPage.Bound.LocateCentral(size.x / 2, size.y / 2);
		elementPage.Bound.Inted();
		elementPage.SetCloseButton(button);
		return elementPage;
	}

	public static ElementPage NewPageMid(Vector2 size)
	{
		ElementButton button = new ElementButton();
		button.Bound.Resize(18, 18);
		button.Texture3Line = GameTextures.GUI_WINDOW_CLOSER;
		ElementPage elementPage = _NewPage();
		elementPage.Icon = GameTextures.GUI_WINDOW_MID;
		elementPage.Bound.Resize(450, 310);
		elementPage.Bound.LocateCentral(size.x / 2, size.y / 2);
		elementPage.Bound.Inted();
		elementPage.SetCloseButton(button);
		return elementPage;
	}

	public static ElementPage NewPageLarge(Vector2 size)
	{
		ElementButton button = new ElementButton();
		button.Bound.Resize(18, 18);
		button.Texture3Line = GameTextures.GUI_WINDOW_CLOSER;
		ElementPage elementPage = _NewPage();
		elementPage.Icon = GameTextures.GUI_WINDOW_LARGE;
		elementPage.Bound.Resize(800, 610);
		elementPage.Bound.LocateCentral(size.x / 2, size.y / 2);
		elementPage.Bound.Inted();
		elementPage.SetCloseButton(button);
		return elementPage;
	}

}
