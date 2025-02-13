using Kinetic;
using Kinetic.App;
using Kinetic.Input;
using Kinetic.Math;
using Kinetic.Visual;

namespace Morinia.Client;

public class MatrixerFloat
{

	public VaryingVector2 Cursor = new VaryingVector2();
	public Vector4 Viewport;

	public void DoTransform(SpriteBatch batch, PerspectiveCamera camera)
	{
		float screenWidth = GraphicsDevice.Global.Size.x;
		float screenHeight = GraphicsDevice.Global.Size.y;
		float screenRatio = screenHeight / screenWidth;
		Cursor.Copy(RemoteKeyboard.Global.Cursor);
		Viewport = new Vector4(0, 0, screenWidth, screenHeight);
		batch.Viewport(Viewport);
		camera.Unproject(Cursor, Viewport);
	}

}
