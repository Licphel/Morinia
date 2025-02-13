using Kinetic.App;
using Kinetic.Math;
using Kinetic.OpenGL;
using Kinetic.Visual;
using Morinia.Client.TheBlock;
using Morinia.Client.TheLiquid;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.ThePhysic;

namespace Morinia.Client;

public class WorldTessellator
{

	static readonly List<Entity> cache = new List<Entity>();

	public static void Draw(SpriteBatch batch, Level level, IBox camera)
	{
		DrawLayer(batch, level, camera, 0);

		level.GetNearbyEntities(cache, camera, null, 8);

		foreach(Entity e in cache)
		{
			e.GetTessellator()?.Draw(batch, e);
		}
		DrawLiquidLayer(batch, level, camera);
		DrawLayer(batch, level, camera, 1);

		level.ParticleEngine.DrawParticles(batch);
	}

	static void DrawLayer(SpriteBatch batch, Level level, IBox camera, int z)
	{
		int cy0 = FloatMath.Round(camera.y - 1);
		int cy1 = FloatMath.Round(camera.yprom + 1);
		int cx0 = FloatMath.Round(camera.x - 1);
		int cx1 = FloatMath.Round(camera.xprom + 1);

		cy0 = FloatMath.Clamp(cy0, 0, Chunk.MaxY);
		cy1 = FloatMath.Clamp(cy1, 0, Chunk.MaxY);

		BlockTessellator lastp = null;

		for(int x = cx0; x <= cx1; x++)
		{
			Chunk c = level.GetChunkByBlock(x);

			if(c == null) continue;

			for(int y = cy0; y < cy1; y++)
			{
				BlockState state = c.GetBlock(x, y, z);

				if(state.IsEmpty) continue;

				BlockTessellator painter = BlockRendering.GetTessellator(state);

				if(painter == null) continue;

				if(!painter.IsInSameState(lastp))
				{
					lastp?.ResetState(batch);
					painter.SetState(batch);
				}

				lastp = painter;

				painter.Draw(batch, level, c, state, x, y, z);
			}
		}

		lastp?.ResetState(batch);
	}

	static void DrawLiquidLayer(SpriteBatch batch, Level level, IBox camera)
	{
		int cy0 = FloatMath.Round(camera.y - 1);
		int cy1 = FloatMath.Round(camera.yprom + 1);
		int cx0 = FloatMath.Round(camera.x - 1);
		int cx1 = FloatMath.Round(camera.xprom + 1);

		cy0 = FloatMath.Clamp(cy0, 0, Chunk.MaxY);
		cy1 = FloatMath.Clamp(cy1, 0, Chunk.MaxY);

		for(int x = cx0; x <= cx1; x++)
		{
			Chunk c = level.GetChunkByBlock(x);

			if(c == null) continue;

			for(int y = cy0; y < cy1; y++)
			{
				LiquidTessellator.Draw(batch, level, c, x, y);
			}
		}
	}

}
