using Kinetic.Math;
using Kinetic.Visual;
using Morinia.Client.TheBlock;
using Morinia.World.TheBlock;
using Morinia.World.TheItem;

namespace Morinia.Client.TheItem;

public class ItemTessellatorNormal : ItemTessellator
{

	public void Draw(SpriteBatch batch, float x, float y, float w, float h, ItemStack stack, bool overlay = true, bool forcecount = false)
	{
		if(!stack.IsEmpty)
		{
			Icon ico = ItemRendering.GetIcon(stack);
			if(ico == null && stack.Item is ItemBlockPlacer placer)
			{
				Block block = placer.GetBlockPlaced(stack);
				BlockState state = block.GetStoredState(0);
				BlockRendering.GetTessellator(state).DrawItemSymbol(batch, state, x, y, w, h);
			}
			else
			{
				batch.Draw(ico, x, y, w, h);
			}

			if(overlay)
			{
				/*
				int dr = stack.getBasicDurability();
				int scale = Mth.round(w / 16f);

				if(dr >= 0)
				{
					int lf = stack.getDurabilityLeft();
					float p = (float) lf / dr;
					if(p != 1)
					{
						Mth.hsvToRGB(p / 3, 0.8f, 0.9f, rgb);
						batch.color4f(0f, 0f, 0f, 1f);
						batch.colorFill(x + scale, y + scale, 14 * scale, scale);
						batch.color4f(rgb, 1f);
						batch.colorFill(x + scale, y + scale, 14 * p * scale, scale);
						batch.normalizeColor();
					}
				}
				*/

				int count = stack.Count;
				int scale = FloatMath.Round(w / 16f);

				if(count > 1 || forcecount)
				{
					string str = count >= 1000 ? "-" : count.ToString();

					batch.Draw(Lore.Literal(str).Style(new LoreStyle() {Bold = true, Darkoutline = true}), x + w + scale, y - scale * 1, Align.RIGHT);
				}
			}
		}
	}

}
