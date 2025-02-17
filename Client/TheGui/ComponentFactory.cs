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
		button.Bound.Resize(w, 14);
		if(click != null) button.OnLeftFired += click;
		button.Icons = [GameTextures.ButtonP1, GameTextures.ButtonP2, GameTextures.ButtonP3];
		button.TextOffset.x = 0;
		button.TextOffset.y = 1f;
		return button;
	}

	public static ElementTextField NewTextField(string text, float w, float h)
	{
		ElementTextField field = new ElementTextField();
		field.InputHint = text;
		field.Bound.Resize(w, h);
		field.Icons = [GameTextures.ButtonP1, GameTextures.ButtonP2, GameTextures.ButtonP3];
		return field;
	}

}
