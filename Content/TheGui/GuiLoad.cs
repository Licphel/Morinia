using Kinetic;
using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client;

namespace Morinia.Content.TheGui;

public class GuiLoad : ElementGui
{

	readonly LoadingQueue Loader;

	public GuiLoad(LoadingQueue loader)
	{
		Loader = loader;
	}

	public override void Draw(SpriteBatch batch)
	{
		base.Draw(batch);

		batch.Draw(GamePreTextures.GuiSplashLogo, Size.x / 2 - 800, Size.y - 300, 1600, 320);
		batch.Fill(20, 20, (Size.x - 40) * Loader.Progress, 1);
	}

	public override void Update(VaryingVector2 cursor)
	{
		base.Update(cursor);

		if(!Loader.Done)
		{
			float time = Time.Millisecs;
			while(Time.Millisecs - time < 500)
				Loader.Next();
		}
		else
		{
			Loader.EndTask();

			Close();
			new GuiMenu().Display();
			GraphicsDevice.Global.Decorated = true;
			GraphicsDevice.Global.Maximize();
		}
	}

}
