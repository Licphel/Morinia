using Kinetic.Visual;
using Morinia.World;
using Morinia.World.TheBlock;

namespace Morinia.Client.TheBlock;

public class BlockTessellatorMaskedRandcoord : BlockTessellatorMasked
{

	//Only support 16 * n x 16 * n texture.
	protected override void DrawInternal(SpriteBatch batch, BlockState state, Direction overlapping, float x, float y, float w, float h)
	{
		Icon icon = BlockRendering.GetIcon(state);

		if(icon is TexturePart part)
		{
			batch.Draw(part.Texture, x, y, w, h, part.u, part.v + PosToRand(part, x, y), 16, 16);
		}
		else if(icon is Texture tex)
		{
			batch.Draw(tex, x, y, w, h, 0, PosToRand(tex, x, y), 16, 16);
		}

		return;

		int PosToRand(IconDimensional part, float x, float y)
		{
			long i = (long) (x * 31) + (long) (y * 17);
			return (int) (Math.Abs(i) % (part.Height >> 4)) << 4;
		}
	}

	protected override void DrawItemSymbolInternal(SpriteBatch batch, BlockState state, float x, float y, float w = 1, float h = 1)
	{
		Icon icon = BlockRendering.GetIcon(state);

		if(icon is TexturePart part)
		{
			batch.Draw(part.Texture, x, y, w, h, part.u, part.v, 16, 16);
		}
		else if(icon is Texture tex)
		{
			batch.Draw(tex, x, y, w, h, 0, 0, 16, 16);
		}
	}

}
