using Kinetic.Math;
using Kinetic.Visual;
using Morinia.World.TheItem;

namespace Morinia.Client.TheItem;

public interface ItemTessellator
{

	public static ItemTessellatorNormal Normal = new ItemTessellatorNormal();

	public void Draw(SpriteBatch batch, float x, float y, float w, float h, ItemStack stack, bool overlay = true, bool forcecount = false);

	public void Draw(SpriteBatch batch, Box box, ItemStack stack, bool overlay = true)
	{
		Draw(batch, box.x, box.y, box.w, box.h, stack, overlay);
	}

}
