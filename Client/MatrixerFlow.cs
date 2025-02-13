using Kinetic;
using Kinetic.App;
using Kinetic.Gui;
using Kinetic.Input;
using Kinetic.Math;
using Kinetic.Visual;

namespace Morinia.Client;

public class MatrixerFlow
{

	public PerspectiveCamera Camera = new PerspectiveCamera();

	public VaryingVector2 Cursor = new VaryingVector2();
	public Vector4 Viewport;

	static MatrixerFlow()
	{
		Application.Resize += () =>
		{
			for(int i = 0; i < ElementGui.Viewings.Count; i++)
			{
				ElementGui gui = ElementGui.Viewings[i];
				new Resolution(gui);
			}
		};
	}

	public void DoTransform(ElementGui gui, SpriteBatch batch)
	{
		float screenWidth = GraphicsDevice.Global.Size.x;
		float screenHeight = GraphicsDevice.Global.Size.y;
		Cursor.Copy(RemoteKeyboard.Global.Cursor);

		Viewport = new Vector4(0, 0, screenWidth, screenHeight);
		batch.Viewport(Viewport);

		Camera.ScaleX = Camera.ScaleY = 1;
		Camera.Viewport.Resize(gui.Resolution.Xsize, gui.Resolution.Ysize);
		Camera.ToCenter();
		Camera.Push();
		Camera.Unproject(Cursor, Viewport);
	}

}
