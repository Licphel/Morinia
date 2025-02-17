using Kinetic.Visual;
using Morinia.World;
using Morinia.World.TheBlock;

namespace Morinia.Client.TheBlock;

public class BlockTessellatorMaskedRepeat : BlockTessellatorMasked
{

	//Only support 16 * n x 16 * n texture.
	protected override void DrawInternal(SpriteBatch batch, BlockState state, Direction overlapping, float x, float y, float w, float h)
	{
		IconDimensional icon = (IconDimensional) BlockRendering.GetIcon(state);

		int fw = icon.Width / 16;
		int fh = icon.Height / 16;
		float u = Math.Abs(x) % fw * 16;
		float v = Math.Abs(y) % fh * 16;

		if(overlapping == Direction.Down)
			v += 14;
		else if(overlapping == Direction.Up)
			v -= 14;
		else if(overlapping == Direction.Right)
			u += 14;
		else if(overlapping == Direction.Left)
			u -= 14;

		if(icon is TexturePart part)
		{
			u += part.u;
			v += part.v;
			batch.Draw(part.Src, x, y, w, h, u, v, 16, 16);
		}
		else if(icon is Texture tex)
		{
			batch.Draw(tex, x, y, w, h, u, v, 16, 16);
		}
	}

	protected override void DrawItemSymbolInternal(SpriteBatch batch, BlockState state, float x, float y, float w = 1, float h = 1)
	{
		IconDimensional icon = (IconDimensional) BlockRendering.GetIcon(state);

		int fw = icon.Width / 16;
		int fh = icon.Height / 16;

		if(icon is TexturePart part)
		{
			batch.Draw(part.Src, x, y, w, h, part.u, part.v, 16, 16);
		}
		else if(icon is Texture tex)
		{
			batch.Draw(tex, x, y, w, h, 0, 0, 16, 16);
		}
	}

}
