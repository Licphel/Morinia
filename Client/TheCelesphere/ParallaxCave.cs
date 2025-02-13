using Kinetic;
using Kinetic.App;
using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Logic;
using Morinia.World;
using Morinia.World.TheLight;

namespace Morinia.Client.TheCelesphere;

public class ParallaxCave : Parallax
{

	//cave blockd lighting
	const int nx = 24, ny = 16;
	readonly Vector3[,] cache = new Vector3[nx + 1, ny + 1];
	readonly Vector4[,,] cache1 = new Vector4[nx + 1, ny + 1, 4];
	float op2;

	public ParallaxCave(int layer, float resist, VaryingVector2 delta, Level level)
	: base(layer, resist, delta, level)
	{
	}

	public override void Tick()
	{
		if(Level == null || op2 <= 0)
		{
			return;
		}

		Vector2 size = GraphicsDevice.Global.Size;
		PerspectiveCamera cam = Game.GameLogic.Camera;
		Vector4 vp = Game.GameLogic.Transform.Viewport;
		LightEngine engine = Level.LightEngine;

		float blockx = size.x / nx, blocky = size.y / ny;

		for(int x = 0; x < nx; x++)
		{
			float px = blockx * x;
			float wx = cam.ToWldX(px, vp);

			for(int y = 0; y < ny; y++)
			{
				float py = blocky * y;
				float wy = cam.ToWldY(py, vp);

				if(engine.IsStableRoll)
				{
					Vector3 col = engine.GetLinearLight(wx, wy);
					ref Vector3 rcol = ref cache[x, y];
					rcol.x += (col.x - rcol.x) * 0.5f;
					rcol.y += (col.y - rcol.y) * 0.5f;
					rcol.z += (col.z - rcol.z) * 0.5f;
				}

				int xm1 = (x - 1 + nx) % nx;
				int xp1 = (x + 1 + nx) % nx;
				int ym1 = (y - 1 + ny) % ny;
				int yp1 = (y + 1 + ny) % ny;

				cache1[x, y, 0] = new Vector4(cache[x, y] / 4 + cache[xm1, ym1] / 4 + cache[xm1, y] / 4 + cache[x, ym1] / 4, op2);
				cache1[x, y, 1] = new Vector4(cache[x, y] / 4 + cache[xm1, yp1] / 4 + cache[x, yp1] / 4 + cache[xm1, y] / 4, op2);
				cache1[x, y, 2] = new Vector4(cache[x, y] / 4 + cache[xp1, yp1] / 4 + cache[xp1, y] / 4 + cache[x, yp1] / 4, op2);
				cache1[x, y, 3] = new Vector4(cache[x, y] / 4 + cache[xp1, ym1] / 4 + cache[xp1, y] / 4 + cache[x, ym1] / 4, op2);
			}
		}
	}

	public override void Draw(SpriteBatch batch, float secs, float msecs, float spf, IPos pos)
	{
		if(pos.y >= Chunk.SeaLevel)
		{
			op2 = FloatMath.Clamp(op2 - 0.01f, 0, 1);
		}
		else
		{
			op2 = FloatMath.Clamp(op2 + 0.01f, 0, 1);
		}

		const float exp = 16;

		Vector2 size = GraphicsDevice.Global.Size;
		float disX = Delta.x * (1 - resist) * exp;
		float disY = -Delta.y * (1 - resist) * exp;
		float ratio = (int) (size.x / Game.ScaledSize.x * 2) / 2f;
		float disX0 = disX / 1.5f / ratio;
		float disY0 = disY / 1.5f / ratio;

		if(Level != null && op2 > 0)
		{
			PerspectiveCamera cam = Game.GameLogic.Camera;
			Vector4 vp = Game.GameLogic.Transform.Viewport;
			LightEngine engine = Level.LightEngine;

			Texture texture = null;

			{
				if(layer == 0) texture = GameTextures.UCeBg1;
				if(layer == 1) texture = GameTextures.UCeBg2;
			}

			float blockx = size.x / nx, blocky = size.y / ny;
			float tx = blockx / ratio / 1.5f;
			float ty = blocky / ratio / 1.5f;

			for(int x = 0; x < nx; x++)
			{
				float px = blockx * x;
				float wx = cam.ToWldX(px, vp);
				float dx = disX0 + px / ratio / 1.5f;

				for(int y = 0; y < ny; y++)
				{
					float py = blocky * y;
					float wy = cam.ToWldY(py, vp);

					batch.LinearCol4[0] = cache1[x, y, 0];
					batch.LinearCol4[1] = cache1[x, y, 1];
					batch.LinearCol4[2] = cache1[x, y, 2];
					batch.LinearCol4[3] = cache1[x, y, 3];
					batch.Draw(texture, px, py, blockx, blocky,
						dx, disY0 - py / ratio / 1.5f, tx, ty);
				}
			}

			batch.NormalizeColor();
		}
	}

}
