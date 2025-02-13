using Kinetic.Math;
using Kinetic.Visual;
using Morinia.World;
using Morinia.World.TheBlock;
using Morinia.World.TheLight;
using Morinia.World.TheLiquid;

namespace Morinia.Client.TheLiquid;

public class LiquidTessellator
{

	public static void Draw(SpriteBatch batch, Level level, Chunk chunk, int x, int y)
	{
		LiquidStack stack = chunk.LiquidMap.Get(x, y);

		if(stack.Amount == 0 || stack.Liquid == Liquid.Empty)
			return;

		BlockState bl = level.GetBlock(x - 1, y);
		BlockState bu = level.GetBlock(x, y + 1);
		BlockState bd = level.GetBlock(x, y - 1);
		BlockState br = level.GetBlock(x + 1, y);

		LiquidStack stacku = chunk.LiquidMap.Get(x, y + 1);
		LiquidStack stackd = chunk.LiquidMap.Get(x, y - 1);

		float p = stacku.Amount > 0 ? 1 : stack.Percent;

		if(stacku.Amount == 0 && (p < 1 || bu.GetShape() != BlockShape.Solid))
			p += 0.025f * FloatMath.SinRad(x * 0.1f + y * 0.05f + level.Ticks * 0.1f);

		level.LightEngine.GetBlockLight(chunk._LE_Bufpack(x, y, 1), batch.LinearCol4, 1);
		batch.Merge4(0.3f, 0.8f, 0.95f, 0.25f);

		batch.Fill(x, y, 1, p);

		const float overhang = 1 / 16f;
		if(bl.GetShape() == BlockShape.Solid)
			batch.Fill(x - overhang, y, overhang, p);
		if(br.GetShape() == BlockShape.Solid)
			batch.Fill(x + 1, y, overhang, p);
		if(bu.GetShape() == BlockShape.Solid && p >= 1)
			batch.Fill(x, y + 1, 1, overhang);
		if(bd.GetShape() == BlockShape.Solid)
			batch.Fill(x, y - overhang, 1, overhang);

		batch.NormalizeColor();
	}

}
