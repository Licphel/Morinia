using Kinetic;
using Kinetic.App;
using Kinetic.Input;
using Kinetic.Math;
using Kinetic.Visual;

namespace Morinia.Client;

public class MatrixerAbsolute
{

	public PerspectiveCamera Camera = new PerspectiveCamera();

	public VaryingVector2 Cursor = new VaryingVector2();
	public Vector4 Viewport;

	public void DoTransform(SpriteBatch batch)
	{
		float screenWidth = GraphicsDevice.Global.Size.x;
		float screenHeight = GraphicsDevice.Global.Size.y;
		Cursor.Copy(RemoteKeyboard.Global.Cursor);

		Viewport = new Vector4(0, 0, screenWidth, screenHeight);
		batch.Viewport(Viewport);

		Camera.ScaleX = Camera.ScaleY = 1;
		Camera.Viewport.Resize(screenWidth, screenHeight);
		Camera.ToCenter();
		Camera.Push();
		Camera.Unproject(Cursor, Viewport);
	}

}
